using BOOSE;
using System;
using System.Data;

namespace BOOSEappTV
{
    /// <summary>
    /// Represents a runtime array assignment using the <c>poke</c> command.
    /// </summary>
    /// <remarks>
    /// This command writes a value into an array element at runtime.
    /// The expected syntax is:
    /// <c>poke arrayName index = value</c>.
    /// Both the index and the value expressions are evaluated at runtime
    /// using the current program state.
    /// </remarks>
    public class AppPoke : Command
    {
        /// <summary>
        /// The name of the target array.
        /// </summary>
        private string arrayName;

        /// <summary>
        /// The expression used to compute the array index.
        /// </summary>
        private string indexExpr;

        /// <summary>
        /// The expression used to compute the value to be assigned.
        /// </summary>
        private string valueExpr;

        /// <summary>
        /// Parses and stores the parameters for the <c>poke</c> command.
        /// </summary>
        /// <param name="program">The active <see cref="StoredProgram"/> instance.</param>
        /// <param name="parameters">
        /// The parameter string specifying the array name, index expression,
        /// and value expression.
        /// </param>
        /// <exception cref="ParserException">
        /// Thrown when the syntax of the <c>poke</c> command is invalid
        /// or required parameters are missing.
        /// </exception>
        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;

            if (string.IsNullOrWhiteSpace(parameters))
                throw new ParserException("poke missing parameters");

            var parts = parameters.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 4 || parts[2] != "=")
                throw new ParserException("Invalid poke syntax");

            arrayName = parts[0];
            indexExpr = parts[1];
            valueExpr = string.Join(' ', parts, 3, parts.Length - 3);
        }

        /// <summary>
        /// Performs compile-time processing for the <c>poke</c> command.
        /// </summary>
        /// <remarks>
        /// As this is a runtime command, it must be explicitly added
        /// to the program command list.
        /// </remarks>
        public override void Compile()
        {
            // Runtime command - must be added to program
            Program.Add(this);
        }

        /// <summary>
        /// Executes the <c>poke</c> command at runtime.
        /// </summary>
        /// <remarks>
        /// This method evaluates the index and value expressions, validates
        /// the array type, and writes the computed value into the array.
        /// </remarks>
        /// <exception cref="StoredProgramException">
        /// Thrown when the array is undeclared, the index is invalid,
        /// a type mismatch occurs, or the array element type is unsupported.
        /// </exception>
        public override void Execute()
        {
            if (!Program.VariableExists(arrayName))
                throw new StoredProgramException($"Array '{arrayName}' not declared");

            if (!(Program.GetVariable(arrayName) is AppArray array))
                throw new StoredProgramException($"'{arrayName}' is not an array");

            // Evaluate index
            int index;
            try
            {
                index = EvaluateInt(indexExpr);
                AppConsole.WriteLine(
                    $"[DEBUG] Poke '{arrayName}[{index}]' = {valueExpr}"
                );
            }
            catch
            {
                throw new StoredProgramException("Array index must be an integer");
            }

            // Evaluate value
            if (array.ElementType == "int")
            {
                int value;
                try
                {
                    value = EvaluateInt(valueExpr);
                }
                catch
                {
                    throw new StoredProgramException(
                        "Type mismatch, expected integer value"
                    );
                }

                array.SetValue(index, value);
                AppConsole.WriteLine(
                    $"[DEBUG] Array '{arrayName}[{index}]' now = {value}"
                );
            }
            else if (array.ElementType == "real")
            {
                double value;
                try
                {
                    value = EvaluateDouble(valueExpr);
                }
                catch
                {
                    throw new StoredProgramException(
                        "Type mismatch, expected real value"
                    );
                }

                array.SetValue(index, value);
                AppConsole.WriteLine(
                    $"[DEBUG] Array '{arrayName}[{index}]' now = {value}"
                );
            }
            else
            {
                throw new StoredProgramException(
                    "Unsupported array element type"
                );
            }
        }

        // Helpers

        /// <summary>
        /// Evaluates an integer expression using the current program state.
        /// </summary>
        /// <param name="expr">The expression to evaluate.</param>
        /// <returns>The evaluated integer value.</returns>
        private int EvaluateInt(string expr)
        {
            string replaced = ReplaceVariables(expr);
            var table = new DataTable();
            object result = table.Compute(replaced, "");
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Evaluates a floating-point expression using the current program state.
        /// </summary>
        /// <param name="expr">The expression to evaluate.</param>
        /// <returns>The evaluated double-precision value.</returns>
        private double EvaluateDouble(string expr)
        {
            string replaced = ReplaceVariables(expr);
            var table = new DataTable();
            object result = table.Compute(replaced, "");
            return Convert.ToDouble(result);
        }

        /// <summary>
        /// Replaces variable names in an expression with their current values.
        /// </summary>
        /// <param name="exp">The expression containing variables.</param>
        /// <returns>An evaluable expression string.</returns>
        /// <exception cref="StoredProgramException">
        /// Thrown when an invalid variable or token is encountered.
        /// </exception>
        private string ReplaceVariables(string exp)
        {
            var tokens = exp.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tokens.Length; i++)
            {
                string t = tokens[i];

                if (double.TryParse(t, out _))
                    continue;

                if (t is "+" or "-" or "*" or "/" or "(" or ")")
                    continue;

                if (Program.VariableExists(t))
                {
                    tokens[i] = Program.GetVarValue(t);
                    continue;
                }

                throw new StoredProgramException("Invalid variable or expressions.");
            }

            return string.Join(" ", tokens);
        }

        /// <summary>
        /// Performs parameter validation.
        /// </summary>
        /// <param name="parameter">The parameter array.</param>
        /// <remarks>
        /// Parameter validation is handled during parsing in <see cref="Set"/>.
        /// </remarks>
        public override void CheckParameters(string[] parameter)
        {
            // handled by Set()
        }
    }
}
