using BOOSE;
using System;

namespace BOOSEappTV
{
    /// <summary>
    /// Represents an <c>end for</c> command that concludes a <c>for</c> loop.
    /// </summary>
    /// <remarks>
    /// This command is responsible for advancing the loop control variable
    /// and redirecting execution back to the corresponding <c>for</c> statement.
    /// All structural linking between <c>for</c> and <c>end for</c> commands
    /// is performed by the parser.
    /// </remarks>
    public class AppEndFor : Command
    {
        /// <summary>
        /// Gets or sets the <see cref="AppFor"/> command associated with this
        /// <c>end for</c> statement.
        /// </summary>
        /// <remarks>
        /// This reference must be established by the parser during compilation.
        /// </remarks>
        public AppFor MatchingFor;

        /// <summary>
        /// Associates this command with the current stored program.
        /// </summary>
        /// <param name="program">The active <see cref="StoredProgram"/> instance.</param>
        /// <param name="parameters">Unused parameter string.</param>
        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;
        }

        /// <summary>
        /// Performs compile-time processing for the <c>end for</c> command.
        /// </summary>
        /// <remarks>
        /// All linking and validation of loop structure is handled by the parser.
        /// </remarks>
        public override void Compile()
        {
            // linking handled by parser
        }

        /// <summary>
        /// Executes the <c>end for</c> command at runtime.
        /// </summary>
        /// <remarks>
        /// This method updates the loop control variable using the step expression
        /// and redirects execution back to the start of the associated <c>for</c> loop.
        /// </remarks>
        /// <exception cref="StoredProgramException">
        /// Thrown when loop control values cannot be evaluated correctly.
        /// </exception>
        public override void Execute()
        {
            int current = int.Parse(Program.GetVarValue(MatchingFor.VarName));
            int step = MatchingFor.EvalInt(MatchingFor.StepExpr);

            Program.UpdateVariable(MatchingFor.VarName, current + step);

            // jump back to for
            Program.PC = MatchingFor.ForLine;
        }

        /// <summary>
        /// Performs parameter validation.
        /// </summary>
        /// <param name="parameter">The parameter array.</param>
        /// <remarks>
        /// This command does not accept parameters.
        /// </remarks>
        public override void CheckParameters(string[] parameter) { }
    }
}
