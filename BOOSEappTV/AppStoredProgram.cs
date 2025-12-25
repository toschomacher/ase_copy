using BOOSE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace BOOSEappTV
{
    /// <summary>
    /// Custom stored program implementation extending BOOSE.StoredProgram.
    /// </summary>
    /// <remarks>
    /// This class is the core runtime container for the BOOSEappTV interpreter.
    /// It extends <see cref="StoredProgram"/> to support:
    /// <list type="bullet">
    /// <item><description>User-defined methods with parameters and return values</description></item>
    /// <item><description>Call stack management</description></item>
    /// <item><description>Custom variable types (int, real, boolean)</description></item>
    /// <item><description>Robust expression tidying and evaluation</description></item>
    /// </list>
    /// The class preserves BOOSE’s two-pass execution model by ensuring that
    /// variable declaration occurs during parsing/compilation and that all
    /// evaluation and assignment occurs at runtime.
    /// </remarks>
    public class AppStoredProgram : StoredProgram
    {
        // Method support

        /// <summary>
        /// Represents a method definition stored in the program.
        /// </summary>
        /// <remarks>
        /// This structure is populated by <see cref="AppParser"/> and is used
        /// at runtime by <see cref="AppCall"/> to manage method invocation.
        /// </remarks>
        public class MethodDef
        {
            /// <summary>
            /// The method name.
            /// </summary>
            public string Name = "";

            /// <summary>
            /// The return type of the method ("int", "real", or "boolean").
            /// </summary>
            public string ReturnType = "";

            /// <summary>
            /// The program counter index of the corresponding <see cref="AppMethod"/> command.
            /// </summary>
            public int MethodLine;

            /// <summary>
            /// The program counter index of the matching <see cref="AppEndMethod"/> command.
            /// </summary>
            public int EndMethodLine;

            /// <summary>
            /// The list of method parameters as (Type, Name) tuples.
            /// </summary>
            public List<(string Type, string Name)> Params = new();
        }

        /// <summary>
        /// Represents a call stack frame for a method invocation.
        /// </summary>
        /// <remarks>
        /// Each frame stores the return address, the method definition,
        /// and a snapshot of parameter variable values so they can be
        /// restored when the method exits.
        /// </remarks>
        private class CallFrame
        {
            /// <summary>
            /// The program counter to return to after method completion.
            /// </summary>
            public int ReturnPC;

            /// <summary>
            /// The method being executed.
            /// </summary>
            public MethodDef Method;

            /// <summary>
            /// Saved variable values for parameters.
            /// A value of <c>null</c> indicates the variable did not previously exist.
            /// </summary>
            public Dictionary<string, string?> SavedValues = new();

            public CallFrame(int ReturnPC, MethodDef method)
            {
                this.ReturnPC = ReturnPC;
                Method = method;
            }

            public CallFrame(int returnPc, MethodDef method, int ReturnPC)
            {
                ReturnPC = returnPc;
                Method = method;
                this.ReturnPC = ReturnPC;
            }
        }

        private readonly Dictionary<string, MethodDef> _methods =
            new(StringComparer.OrdinalIgnoreCase);

        private readonly Stack<CallFrame> _callStack = new();

        /// <summary>
        /// Initialises a new instance of the <see cref="AppStoredProgram"/> class.
        /// </summary>
        /// <param name="canvas">The canvas used for drawing commands.</param>
        public AppStoredProgram(ICanvas canvas) : base(canvas) { }

        /// <summary>
        /// Registers a method definition with the program.
        /// </summary>
        /// <param name="def">The method definition to register.</param>
        /// <exception cref="StoredProgramException">
        /// Thrown when the method name is missing.
        /// </exception>
        public void RegisterMethod(MethodDef def)
        {
            if (string.IsNullOrWhiteSpace(def.Name))
                throw new StoredProgramException("Method name missing.");

            _methods[def.Name] = def;
        }

        /// <summary>
        /// Attempts to retrieve a method definition by name.
        /// </summary>
        /// <param name="name">The method name.</param>
        /// <param name="def">The resulting method definition.</param>
        /// <returns>
        /// <c>true</c> if the method exists; otherwise <c>false</c>.
        /// </returns>
        public bool TryGetMethod(string name, out MethodDef def) =>
            _methods.TryGetValue(name, out def!);

        // Method execution

        /// <summary>
        /// Enters a method call.
        /// </summary>
        /// <remarks>
        /// This method:
        /// <list type="bullet">
        /// <item><description>Saves the current execution context</description></item>
        /// <item><description>Evaluates argument expressions</description></item>
        /// <item><description>Initialises parameter variables</description></item>
        /// <item><description>Jumps execution to the method body</description></item>
        /// </list>
        /// </remarks>
        /// <param name="def">The method definition.</param>
        /// <param name="argExprs">The argument expressions.</param>
        public void EnterMethod(MethodDef def, List<string> argExprs)
        {
            var frame = new CallFrame(PC + 1, def);

            // Save only parameters (not return variable)
            foreach (var p in def.Params)
                SaveVar(frame, p.Name);

            if (argExprs.Count != def.Params.Count)
                throw new StoredProgramException(
                    $"Method '{def.Name}' expects {def.Params.Count} argument(s) but got {argExprs.Count}."
                );

            for (int i = 0; i < def.Params.Count; i++)
            {
                var (ptype, pname) = def.Params[i];
                string evaluated = EvaluateToString(argExprs[i]);

                EnsureVariableExistsForType(ptype, pname);
                AssignByType(ptype, pname, evaluated);
            }

            // Ensure return variable exists (do not save it)
            EnsureVariableExistsForType(def.ReturnType, def.Name);

            _callStack.Push(frame);
            PC = def.MethodLine + 1;
        }

        /// <summary>
        /// Exits the currently executing method.
        /// </summary>
        /// <remarks>
        /// Restores saved parameter variables and resumes execution
        /// at the instruction following the original call.
        /// </remarks>
        public void ExitMethod()
        {
            if (_callStack.Count == 0)
                throw new StoredProgramException("end method with empty call stack.");

            var frame = _callStack.Pop();

            // Restore only parameters
            foreach (var kv in frame.SavedValues)
                RestoreVar(kv.Key, kv.Value);

            PC = frame.ReturnPC - 1;
        }

        // Expression helpers

        /// <summary>
        /// Normalises an expression by inserting spacing around operators.
        /// </summary>
        /// <param name="expr">The expression to tidy.</param>
        /// <returns>A normalised expression string.</returns>
        public string TidyExpression(string expr)
        {
            if (expr == null) return "";

            expr = Regex.Replace(expr, @"([+\-*/()])", " $1 ");
            expr = expr.Replace("&&", " && ").Replace("||", " || ");
            expr = Regex.Replace(expr, @"(!=|==|<=|>=|<|>)", " $1 ");
            expr = Regex.Replace(expr, @"\s+", " ").Trim();
            return expr;
        }

        /// <summary>
        /// Evaluates an expression and returns the result as a string.
        /// </summary>
        /// <remarks>
        /// BOOSE’s evaluator is attempted first. If that fails, a numeric-only
        /// fallback using <see cref="DataTable.Compute"/> is used.
        /// </remarks>
        private string EvaluateToString(string expr)
        {
            string tidy = TidyExpression(expr);
            try
            {
                return EvaluateExpression(tidy);
            }
            catch
            {
                try
                {
                    var table = new DataTable();
                    object result = table.Compute(ReplaceVarsForCompute(tidy), "");
                    return Convert.ToString(result) ?? "0";
                }
                catch
                {
                    throw new StoredProgramException(
                        $"Invalid expression, can't evaluate {expr}"
                    );
                }
            }
        }

        private string ReplaceVarsForCompute(string exp)
        {
            var tokens = exp.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tokens.Length; i++)
            {
                string t = tokens[i];

                if (double.TryParse(t, out _)) continue;
                if (t is "+" or "-" or "*" or "/" or "(" or ")") continue;

                if (!VariableExists(t))
                    throw new StoredProgramException(
                        $"Unknown variable '{t}' in expression."
                    );

                tokens[i] = GetVarValue(t);
            }
            return string.Join(" ", tokens);
        }

        // Variable save / restore
        private void SaveVar(CallFrame frame, string name)
        {
            if (VariableExists(name))
                frame.SavedValues[name] = GetVarValue(name);
            else
                frame.SavedValues[name] = null;
        }

        private void RestoreVar(string name, string? oldValueOrNull)
        {
            if (oldValueOrNull == null)
            {
                if (VariableExists(name))
                {
                    var v = GetVariable(name);
                    if (v is AppInt ai) ai.Value = 0;
                    else if (v is AppReal ar) ar.Value = 0.0;
                    else if (v is AppBoolean ab) ab.Value = false;
                }
                return;
            }

            var varObj = GetVariable(name);
            if (varObj is AppInt)
                UpdateVariable(name, Convert.ToInt32(oldValueOrNull));
            else if (varObj is AppReal)
                UpdateVariable(name, Convert.ToDouble(oldValueOrNull));
            else if (varObj is AppBoolean ab)
                ab.Value =
                    oldValueOrNull.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                    oldValueOrNull == "1";
            else
                UpdateVariable(name, Convert.ToInt32(oldValueOrNull));
        }

        /// <summary>
        /// Ensures a variable of the specified type exists in the program.
        /// </summary>
        /// <param name="type">The variable type.</param>
        /// <param name="name">The variable name.</param>
        public void EnsureVariableExistsForType(string type, string name)
        {
            if (VariableExists(name)) return;

            type = type.ToLower().Trim();

            Evaluation v = type switch
            {
                "int" => new AppInt(),
                "real" => new AppReal(),
                "boolean" => new AppBoolean(),
                _ => throw new StoredProgramException(
                    $"Unknown type '{type}' for '{name}'."
                )
            };

            v.VarName = name;
            AddVariable(v);
        }

        private void AssignByType(string type, string varName, string value)
        {
            type = type.ToLower().Trim();

            if (type == "boolean")
            {
                var v = GetVariable(varName);
                bool b =
                    value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                    value == "1";

                if (v is AppBoolean ab) ab.Value = b;
                else
                    throw new StoredProgramException(
                        $"Type mismatch assigning boolean to '{varName}'."
                    );
                return;
            }

            if (type == "int")
            {
                int iv = Convert.ToInt32(Convert.ToDouble(value));
                UpdateVariable(varName, iv);
                return;
            }

            if (type == "real")
            {
                double dv = Convert.ToDouble(value);
                UpdateVariable(varName, dv);
                return;
            }

            throw new StoredProgramException($"Unknown type '{type}'.");
        }

        /// <inheritdoc />
        public override void UpdateVariable(string varName, int value)
        {
            Evaluation v = GetVariable(varName);

            if (v is AppInt ai)
            {
                ai.Value = value;
                return;
            }

            base.UpdateVariable(varName, value);
        }

        /// <inheritdoc />
        public override void UpdateVariable(string varName, double value)
        {
            Evaluation v = GetVariable(varName);

            if (v is AppReal ar)
            {
                ar.Value = value;
                return;
            }

            base.UpdateVariable(varName, value);
        }

        /// <inheritdoc />
        public override string GetVarValue(string varName)
        {
            Evaluation v = GetVariable(varName);

            if (v is AppInt ai) return ai.Value.ToString();
            if (v is AppReal ar) return ar.Value.ToString();
            if (v is AppBoolean ab) return ab.Value ? "true" : "false";

            return base.GetVarValue(varName);
        }
    }
}
