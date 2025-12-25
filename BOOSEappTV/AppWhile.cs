using BOOSE;
using System;
using System.Data;

namespace BOOSEappTV
{
    /// <summary>
    /// Represents a <c>while</c> loop command.
    /// </summary>
    /// <remarks>
    /// This command evaluates a boolean condition at runtime and controls
    /// loop execution. If the condition evaluates to <c>false</c>, execution
    /// jumps to the instruction immediately following the matching
    /// <c>end while</c> command.
    /// Structural linking between <c>while</c> and <c>end while</c> is handled
    /// by the parser.
    /// </remarks>
    public class AppWhile : Command
    {
        /// <summary>
        /// The loop condition expression.
        /// </summary>
        private string condition;

        /// <summary>
        /// Gets or sets the program counter index of this <c>while</c> command.
        /// </summary>
        public int WhileLine { get; set; }

        /// <summary>
        /// Gets or sets the program counter index of the matching
        /// <c>end while</c> command.
        /// </summary>
        public int EndWhileLine { get; set; }

        /// <summary>
        /// Parses and stores the condition expression for the <c>while</c> loop.
        /// </summary>
        /// <param name="program">The active <see cref="StoredProgram"/> instance.</param>
        /// <param name="parameters">The loop condition expression.</param>
        /// <exception cref="ParserException">
        /// Thrown when the condition expression is missing.
        /// </exception>
        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;
            condition = parameters?.Trim()
                ?? throw new ParserException("while missing condition");
        }

        /// <summary>
        /// Performs compile-time processing for the <c>while</c> command.
        /// </summary>
        /// <remarks>
        /// The loop start line is recorded when the command is added
        /// to the program. Structural linking is completed by the parser.
        /// </remarks>
        public override void Compile()
        {
            // set when added to program
            WhileLine = Program.Count;
        }

        /// <summary>
        /// Executes the <c>while</c> command at runtime.
        /// </summary>
        /// <remarks>
        /// If the condition evaluates to <c>false</c>, execution jumps
        /// to the instruction immediately after the matching
        /// <c>end while</c> command.
        /// </remarks>
        public override void Execute()
        {
            bool result = EvaluateCondition(condition);

            if (!result)
            {
                // jump past END WHILE
                Program.PC = EndWhileLine + 1;
            }
        }

        /// <summary>
        /// Evaluates the loop condition expression.
        /// </summary>
        /// <param name="expr">The condition expression.</param>
        /// <returns>
        /// <c>true</c> if the condition evaluates to true; otherwise <c>false</c>.
        /// </returns>
        private bool EvaluateCondition(string expr)
        {
            string replaced = ReplaceVariables(expr);
            var table = new DataTable();
            object result = table.Compute(replaced, "");
            return Convert.ToBoolean(result);
        }

        /// <summary>
        /// Replaces variable names in a condition expression with their current values.
        /// </summary>
        /// <param name="expr">The expression containing variables.</param>
        /// <returns>An evaluable expression string.</returns>
        private string ReplaceVariables(string expr)
        {
            var tokens = expr.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tokens.Length; i++)
            {
                if (double.TryParse(tokens[i], out _)) continue;
                if ("+-*/()<>=!".Contains(tokens[i])) continue;

                if (Program.VariableExists(tokens[i]))
                {
                    tokens[i] = Program.GetVarValue(tokens[i]);
                }
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
        public override void CheckParameters(string[] parameter) { }
    }
}
