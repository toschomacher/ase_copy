using BOOSE;

namespace BOOSEappTV
{
    /// <summary>
    /// Represents an <c>end while</c> command that terminates a <c>while</c> loop.
    /// </summary>
    /// <remarks>
    /// This command redirects execution back to the corresponding
    /// <c>while</c> statement, allowing the loop condition to be
    /// re-evaluated. Structural linking between <c>while</c> and
    /// <c>end while</c> commands is performed by the parser.
    /// </remarks>
    public class AppEndWhile : Command
    {
        /// <summary>
        /// Gets or sets the <see cref="AppWhile"/> command associated with this
        /// <c>end while</c> statement.
        /// </summary>
        /// <remarks>
        /// This reference must be set by the parser during compilation.
        /// </remarks>
        public AppWhile MatchingWhile { get; set; }

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
        /// Performs compile-time processing for the <c>end while</c> command.
        /// </summary>
        /// <remarks>
        /// No compile-time action is required, as loop linking
        /// is handled by the parser.
        /// </remarks>
        public override void Compile()
        {
            // nothing needed
        }

        /// <summary>
        /// Executes the <c>end while</c> command at runtime.
        /// </summary>
        /// <remarks>
        /// Execution is redirected back to the matching <c>while</c> command
        /// so that the loop condition can be evaluated again.
        /// </remarks>
        public override void Execute()
        {
            // jump back to WHILE
            Program.PC = MatchingWhile.WhileLine;
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
