using BOOSE;
using System;
using System.Data;

namespace BOOSEappTV
{
    /// <summary>
    /// Represents a runtime array read operation using the <c>peek</c> command.
    /// </summary>
    /// <remarks>
    /// This command reads a value from an array at runtime and assigns it to
    /// a target variable. The command follows the syntax:
    /// <c>peek varName = arrayName index</c>.
    /// Evaluation of the index expression and assignment occur strictly
    /// at runtime in accordance with BOOSE semantics.
    /// </remarks>
    public class AppPeek : Command
    {
        /// <summary>
        /// The name of the target variable that will receive the array value.
        /// </summary>
        private string targetVar;

        /// <summary>
        /// The name of the source array.
        /// </summary>
        private string arrayName;

        /// <summary>
        /// The expression used to compute the array index.
        /// </summary>
        private string indexExpr;

        /// <summary>
        /// Parses and stores the parameters for the <c>peek</c> command.
        /// </summary>
        /// <param name="program">The active <see cref="StoredProgram"/> instance.</param>
        /// <param name="parameters">
        /// The parameter string specifying the target variable, array name,
        /// and index expression.
        /// </param>
        /// <exception cref="ParserException">
        /// Thrown when the syntax of the <c>peek</c> command is invalid.
        /// </exception>
        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;

            if (string.IsNullOrWhiteSpace(parameters))
                throw new ParserException("peek missing parameters");

            // Expected: x = nums 5
            var parts = parameters.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 4 || parts[1] != "=")
                throw new ParserException("Invalid peek syntax");

            targetVar = parts[0];
            arrayName = parts[2];
            indexExpr = parts[3];
        }

        /// <summary>
        /// Performs compile-time processing for the <c>peek</c> command.
        /// </summary>
        /// <remarks>
        /// As this is a runtime command, it must be explicitly added
        /// to the program command list.
        /// </remarks>
        public override void Compile()
        {
            // Runtime command → MUST be added to program
            Program.Add(this);
        }

        /// <summary>
        /// Executes the <c>peek</c> command at runtime.
        /// </summary>
        /// <remarks>
        /// This method evaluates the index expression, retrieves the value
        /// from the specified array, and assigns it to the target variable.
        /// </remarks>
        /// <exception cref="StoredProgramException">
        /// Thrown when variables are undeclared, types mismatch, or
        /// the array type is unsupported.
        /// </exception>
        public override void Execute()
        {
            if (!Program.VariableExists(targetVar))
                throw new StoredProgramException(
                    $"Variable '{targetVar}' not declared"
                );

            if (!Program.VariableExists(arrayName))
                throw new StoredProgramException(
                    $"Array '{arrayName}' not declared"
                );

            if (!(Program.GetVariable(arrayName) is AppArray array))
                throw new StoredProgramException(
                    $"'{arrayName}' is not an array"
                );

            int index = EvaluateInt(indexExpr);

            AppConsole.WriteLine(
                $"[DEBUG] Peek '{arrayName}[{index}]' → '{targetVar}'"
            );

            var variable = Program.GetVariable(targetVar);

            if (array.ElementType == "int")
            {
                if (variable is not AppInt)
                    throw new StoredProgramException(
                        "Type mismatch, expected integer variable"
                    );

                int value = array.GetIntValue(index);
                Program.UpdateVariable(targetVar, value);

                AppConsole.WriteLine(
                    $"[DEBUG] Assigned '{targetVar}' = {value}"
                );
            }
            else if (array.ElementType == "real")
            {
                if (variable is not AppReal)
                    throw new StoredProgramException(
                        "Type mismatch, expected real variable"
                    );

                double value = array.GetRealValue(index);
                Program.UpdateVariable(targetVar, value);

                AppConsole.WriteLine(
                    $"[DEBUG] Assigned '{targetVar}' = {value}"
                );
            }
            else
            {
                throw new StoredProgramException("Unsupported array type");
            }
        }

        // Helpers

        /// <summary>
        /// Evaluates an integer expression using the current program state.
        /// </summary>
        /// <param name="expr">The expression to evaluate.</param>
        /// <returns>The evaluated integer result.</returns>
        private int EvaluateInt(string expr)
        {
            string replaced = ReplaceVariables(expr);
            var table = new DataTable();
            object result = table.Compute(replaced, "");
            return Convert.ToInt32(result);
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
