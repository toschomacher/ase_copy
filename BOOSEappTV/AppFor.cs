using BOOSE;
using System;
using System.Data;

namespace BOOSEappTV
{
    /// <summary>
    /// Represents a <c>for</c> loop command following BOOSE syntax:
    /// <c>for var = start to end step n</c>.
    /// </summary>
    /// <remarks>
    /// This command controls loop initialisation, condition checking,
    /// and execution flow. Loop termination and iteration control
    /// are completed in conjunction with the corresponding
    /// <see cref="AppEndFor"/> command.
    /// </remarks>
    public class AppFor : Command
    {
        /// <summary>
        /// The name of the loop control variable.
        /// </summary>
        public string VarName;

        /// <summary>
        /// The expression defining the initial value of the loop variable.
        /// </summary>
        public string StartExpr;

        /// <summary>
        /// The expression defining the terminating value of the loop.
        /// </summary>
        public string EndExpr;

        /// <summary>
        /// The expression defining the loop step value.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>1</c> if not explicitly specified.
        /// </remarks>
        public string StepExpr = "1";

        /// <summary>
        /// The program counter position of this <c>for</c> command.
        /// </summary>
        public int ForLine;

        /// <summary>
        /// The program counter position of the matching <c>end for</c> command.
        /// </summary>
        public int EndForLine;

        /// <summary>
        /// Indicates whether the loop has been initialised.
        /// </summary>
        /// <remarks>
        /// Initialisation occurs only once, during the first execution
        /// of the loop.
        /// </remarks>
        private bool initialized = false;

        /// <summary>
        /// Parses and stores the parameters defining the <c>for</c> loop.
        /// </summary>
        /// <param name="program">The active <see cref="StoredProgram"/> instance.</param>
        /// <param name="parameters">
        /// The parameter string defining loop variable, bounds, and step.
        /// </param>
        /// <exception cref="ParserException">
        /// Thrown when the syntax of the <c>for</c> statement is invalid.
        /// </exception>
        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;

            if (string.IsNullOrWhiteSpace(parameters))
                throw new ParserException("for missing parameters");

            var parts = parameters.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 5 || parts[1] != "=" ||
                !parts[3].Equals("to", StringComparison.OrdinalIgnoreCase))
                throw new ParserException("Invalid for syntax");

            VarName = parts[0];
            StartExpr = parts[2];
            EndExpr = parts[4];

            if (parts.Length >= 7 && parts[5].Equals("step", StringComparison.OrdinalIgnoreCase))
                StepExpr = parts[6];

            // BOOSE-correct variable declaration
            if (!Program.VariableExists(VarName))
            {
                var loopVar = new AppInt();
                loopVar.Set(Program, VarName);
                Program.AddVariable(loopVar);
            }
        }

        /// <summary>
        /// Performs compile-time processing for the <c>for</c> command.
        /// </summary>
        /// <remarks>
        /// All structural linking between <c>for</c> and <c>end for</c>
        /// commands is handled by the parser.
        /// </remarks>
        public override void Compile()
        {
            // linking handled by parser
        }

        /// <summary>
        /// Executes the <c>for</c> command at runtime.
        /// </summary>
        /// <remarks>
        /// This method initialises the loop variable on first execution,
        /// evaluates the loop condition, and controls whether execution
        /// continues into the loop body or skips to the matching
        /// <c>end for</c> command.
        /// </remarks>
        public override void Execute()
        {
            // initialise once
            if (!initialized)
            {
                int start = EvalInt(StartExpr);
                Program.UpdateVariable(VarName, start);
                initialized = true;
            }

            int current = int.Parse(Program.GetVarValue(VarName));
            int end = EvalInt(EndExpr);
            int step = EvalInt(StepExpr);

            bool condition =
                step >= 0 ? current <= end
                          : current >= end;

            if (!condition)
            {
                // skip loop body
                Program.PC = EndForLine + 1;
            }
        }

        // Helpers
        /// <summary>
        /// Evaluates an integer expression using the current program state.
        /// </summary>
        /// <param name="expr">The expression to evaluate.</param>
        /// <returns>The evaluated integer result.</returns>
        public int EvalInt(string expr)
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

                if (int.TryParse(t, out _))
                    continue;

                if (t is "+" or "-" or "*" or "/" or "(" or ")")
                    continue;

                if (Program.VariableExists(t))
                {
                    tokens[i] = Program.GetVarValue(t);
                    continue;
                }

                throw new StoredProgramException($"Invalid expression '{exp}'");
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
