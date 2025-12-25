using BOOSE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace BOOSEappTV
{
    public class AppStoredProgram : StoredProgram
    {
        // Method support
        public class MethodDef
        {
            public string Name = "";
            public string ReturnType = "";                 // "int" / "real" / "boolean"
            public int MethodLine;                         // index of AppMethod command
            public int EndMethodLine;                      // index of AppEndMethod command
            public List<(string Type, string Name)> Params = new();
        }

        private class CallFrame
        {
            public int ReturnPC;
            public MethodDef Method;
            public Dictionary<string, string?> SavedValues = new(); // null = didn't exist

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

        private readonly Dictionary<string, MethodDef> _methods = new(StringComparer.OrdinalIgnoreCase);
        private readonly Stack<CallFrame> _callStack = new();

        public AppStoredProgram(ICanvas canvas) : base(canvas) { }

        public void RegisterMethod(MethodDef def)
        {
            if (string.IsNullOrWhiteSpace(def.Name))
                throw new StoredProgramException("Method name missing.");

            _methods[def.Name] = def;
        }

        public bool TryGetMethod(string name, out MethodDef def) => _methods.TryGetValue(name, out def!);

        // Called by AppCall
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

        // Called by AppEndMethod
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
        // fixes count*10, g+h, count2*20, etc.
        public string TidyExpression(string expr)
        {
            if (expr == null) return "";

            // space around arithmetic + parentheses
            expr = Regex.Replace(expr, @"([+\-*/()])", " $1 ");

            // space around boolean operators && || !
            expr = expr.Replace("&&", " && ").Replace("||", " || ");
            expr = Regex.Replace(expr, @"(!=|==|<=|>=|<|>)", " $1 ");

            // collapse whitespace
            expr = Regex.Replace(expr, @"\s+", " ").Trim();
            return expr;
        }

        private string EvaluateToString(string expr)
        {
            // Try BOOSE's evaluator first (handles booleans, etc.)
            // but normalise spacing so it doesn't choke on count*10.
            string tidy = TidyExpression(expr);
            try
            {
                return EvaluateExpression(tidy); // BOOSE.StoredProgram.EvaluateExpression(string)
            }
            catch
            {
                // fallback: numeric-only DataTable.Compute
                try
                {
                    var table = new DataTable();
                    object result = table.Compute(ReplaceVarsForCompute(tidy), "");
                    return Convert.ToString(result) ?? "0";
                }
                catch
                {
                    throw new StoredProgramException($"Invalid expression, can't evaluate {expr}");
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
                    throw new StoredProgramException($"Unknown variable '{t}' in expression.");

                tokens[i] = GetVarValue(t);
            }
            return string.Join(" ", tokens);
        }

        // Variable save/restore + typed assignment
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
                // We cannot truly remove variables via BOOSE API easily,
                // so we reset to a sensible default if it exists now.
                if (VariableExists(name))
                {
                    // try reset by type
                    var v = GetVariable(name);
                    if (v is AppInt ai) ai.Value = 0;
                    else if (v is AppReal ar) ar.Value = 0.0;
                    else if (v is AppBoolean ab) ab.Value = false;
                }
                return;
            }

            // restore value by type
            var varObj = GetVariable(name);
            if (varObj is AppInt)
                UpdateVariable(name, Convert.ToInt32(oldValueOrNull));
            else if (varObj is AppReal)
                UpdateVariable(name, Convert.ToDouble(oldValueOrNull));
            else if (varObj is AppBoolean ab)
                ab.Value = oldValueOrNull.Equals("true", StringComparison.OrdinalIgnoreCase) || oldValueOrNull == "1";
            else
                UpdateVariable(name, Convert.ToInt32(oldValueOrNull));
        }

        public void EnsureVariableExistsForType(string type, string name)
        {
            if (VariableExists(name)) return;

            // BOOSE's StoredProgram.AddVariable(...) takes ONE argument: an Evaluation.
            // The custom types are Evaluations (AppInt/AppReal/AppBoolean).
            type = type.ToLower().Trim();

            Evaluation v = type switch
            {
                "int" => new AppInt(),
                "real" => new AppReal(),
                "boolean" => new AppBoolean(),
                _ => throw new StoredProgramException($"Unknown type '{type}' for '{name}'.")
            };

            // Most BOOSE Evaluation types expose VarName
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
                else throw new StoredProgramException($"Type mismatch assigning boolean to '{varName}'.");
                return;
            }

            // numeric: allow real/int conversion, BOOSE rule is to truncate real into an int
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
