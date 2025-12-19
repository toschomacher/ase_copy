using BOOSE;
using System;
using System.Data;

namespace BOOSEappTV
{
    /// <summary>
    /// Runtime assignment command: var = expression
    /// Expression MUST already be tidied by AppParser.
    /// </summary>
    public class AppAssign : Command
    {
        private readonly string varName;
        private readonly string expression;

        public AppAssign(string varName, string expression)
        {
            this.varName = varName;
            this.expression = expression;
        }

        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;
        }

        /// <summary>
        /// Replace variable names in expression with their current values.
        /// Expression is assumed to be space-separated.
        /// </summary>
        private string ReplaceVariables(string exp)
        {
            var tokens = exp.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tokens.Length; i++)
            {
                // Numeric literal → leave unchanged
                if (int.TryParse(tokens[i], out _))
                    continue;

                // Variable → replace with current value
                if (Program.VariableExists(tokens[i]))
                {
                    tokens[i] = Program.GetVarValue(tokens[i]);
                }
            }

            return string.Join(" ", tokens);
        }

        public override void Execute()
        {
            // 1️⃣ Replace variables with their runtime values
            string evaluable = ReplaceVariables(expression);

            int result;
            try
            {
                var table = new DataTable();
                result = Convert.ToInt32(table.Compute(evaluable, ""));
            }
            catch
            {
                throw new StoredProgramException(
                    $"Invalid expression, can't evaluate {expression}"
                );
            }

            // 2️⃣ Store result
            Program.UpdateVariable(varName, result);

            AppConsole.WriteLine(
                $"[DEBUG] Assigned '{varName}' = {result}"
            );
        }

        public override void CheckParameters(string[] parameter)
        {
            // Parameters handled entirely by AppParser
        }
    }
}
