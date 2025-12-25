using BOOSE;

namespace BOOSEappTV
{
    /// <summary>
    /// Represents an <c>end if</c> command that marks the termination
    /// of an <c>if</c> or <c>if-else</c> block.
    /// </summary>
    /// <remarks>
    /// This command serves as a structural marker within the program.
    /// It does not perform any action at runtime; instead, it is used
    /// by the parser to establish correct control-flow links between
    /// <c>if</c>, <c>else</c>, and <c>end if</c> commands.
    /// </remarks>
    public class AppEndIf : Command
    {
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
        /// Performs compile-time processing for the <c>end if</c> command.
        /// </summary>
        /// <remarks>
        /// No compile-time action is required, as all control-flow linking
        /// is handled by the parser.
        /// </remarks>
        public override void Compile() { }

        /// <summary>
        /// Executes the <c>end if</c> command at runtime.
        /// </summary>
        /// <remarks>
        /// This command performs no runtime action and exists solely
        /// as a control-flow boundary.
        /// </remarks>
        public override void Execute() { }

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
