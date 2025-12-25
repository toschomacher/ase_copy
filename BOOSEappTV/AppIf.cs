using BOOSE;
using System;
using System.Data;

namespace BOOSEappTV
{
    /// <summary>
    /// Represents an <c>if</c> conditional command that controls
    /// program flow based on a boolean expression.
    /// </summary>
    /// <remarks>
    /// This command evaluates its condition at runtime and determines
    /// whether execution continues into the <c>if</c> body, jumps to an
    /// <c>else</c> block, or skips directly to the matching <c>end if</c>.
    /// All structural linking between <c>if</c>, <c>else</c>, and
    /// <c>end if</c> commands is performed by the parser.
    /// </remarks>
    public class AppIf : Command
    {
        /// <summary>
        /// The conditional expression to be evaluated.
        /// </summary>
        private string condition;

        /// <summary>
        /// Gets or sets the program counter position of this <c>if</c> command.
        /// </summary>
        public int IfLine { get; set; } = -1;

        /// <summary>
        /// Gets or sets the program counter position of the matching
        /// <c>else</c> command.
        /// </summary>
        public int ElseLine { get; set; } = -1;

        /// <summary>
        /// Gets or sets the program counter position of the matching
        /// <c>end if</c> command.
        /// </summary>
        public int EndIfLine { get; set; } = -1;

        /// <summary>
        /// Gets a value indicating whether the condition evaluated to
        /// <c>true</c> during the last execution.
        /// </summary>
        /// <remarks>
        /// This value is used by the associated <see cref="AppElse"/>
        /// command to determine execution flow.
        /// </remarks>
        public bool LastConditionTrue { get; private set; }

        /// <summary>
        /// Parses and stores the condition expression for the <c>if</c> statement.
        /// </summary>
        /// <param name="program">The active <see cref="StoredProgram"/> instance.</param>
        /// <param name="parameters">The conditional expression.</param>
        /// <exception cref="ParserException">
        /// Thrown when the condition is missing or empty.
        /// </exception>
        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;
            condition = parameters?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(condition))
                throw new ParserException("if missing condition");
        }

        /// <summary>
        /// Performs compile-time processing for the <c>if</c> command.
        /// </summary>
        /// <remarks>
        /// All control-flow linking is handled by the parser.
        /// </remarks>
        public override void Compile()
        {
            // linking happens in parser
        }

        /// <summary>
        /// Executes the <c>if</c> command at runtime.
        /// </summary>
        /// <remarks>
        /// The condition is evaluated and execution flow is redirected
        /// depending on the result and the presence of an <c>else</c> block.
        /// </remarks>
        public override void Execute()
        {
            bool result = EvaluateCondition(condition);
            LastConditionTrue = result;

            if (!result)
            {
                if (ElseLine >= 0)
                {
                    // Jump to FIRST executable line inside ELSE block
                    Program.PC = ElseLine;
                }
                else
                {
                    // No ELSE → jump past END IF
                    Program.PC = EndIfLine;
                }
            }
        }

        /// <summary>
        /// Evaluates the conditional expression.
        /// </summary>
        /// <param name="expr">The condition expression.</param>
        /// <returns>
        /// <c>true</c> if the condition evaluates to true; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="StoredProgramException">
        /// Thrown when the condition cannot be evaluated.
        /// </exception>
        private bool EvaluateCondition(string expr)
        {
            // replace variable names with values
            string replaced = ReplaceVariables(expr);

            try
            {
                var table = new DataTable();
                object result = table.Compute(replaced, "");

                // DataTable.Compute comparisons return bool (usually)
                if (result is bool b) return b;

                // fallback: numeric truthiness
                int n = Convert.ToInt32(result);
                return n != 0;
            }
            catch (Exception ex)
            {
                throw new StoredProgramException(
                    $"Invalid if condition '{expr}': {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Replaces variable names in the condition with their current values.
        /// </summary>
        /// <param name="exp">The condition expression.</param>
        /// <returns>An evaluable expression string.</returns>
        /// <exception cref="StoredProgramException">
        /// Thrown when an unknown variable is encountered.
        /// </exception>
        private string ReplaceVariables(string exp)
        {
            // Tokenise by space (parser tidies expressions)
            var tokens = exp.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tokens.Length; i++)
            {
                string t = tokens[i];

                // operators/comparators
                if (t is "<" or ">" or "<=" or ">=" or "==" or "!=" or "(" or ")")
                    continue;

                // numeric literal
                if (double.TryParse(t, out _))
                    continue;

                // boolean literal
                if (t.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                    t.Equals("false", StringComparison.OrdinalIgnoreCase))
                    continue;

                // variable
                if (!Program.VariableExists(t))
                    throw new StoredProgramException(
                        $"Unknown variable '{t}' in if condition"
                    );

                tokens[i] = Program.GetVarValue(t);
            }

            return string.Join(" ", tokens);
        }

        /// <summary>
        /// Performs parameter validation.
        /// </summary>
        /// <param name="parameter">The parameter array.</param>
        /// <remarks>
        /// This command does not require additional parameter validation.
        /// </remarks>
        public override void CheckParameters(string[] parameter) { }
    }
}
