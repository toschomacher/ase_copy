using BOOSE;

namespace BOOSEappTV
{
    /// <summary>
    /// Represents an <c>end method</c> command that terminates the execution
    /// of a user-defined method.
    /// </summary>
    /// <remarks>
    /// This command is executed at runtime to exit the current method context
    /// and return control to the caller. Method scope and execution flow
    /// are managed by <see cref="AppStoredProgram"/>.
    /// </remarks>
    public class AppEndMethod : Command
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
        /// Executes the <c>end method</c> command at runtime.
        /// </summary>
        /// <remarks>
        /// This method exits the current method scope and restores
        /// the previous execution context.
        /// </remarks>
        /// <exception cref="StoredProgramException">
        /// Thrown when the stored program is not of the expected type.
        /// </exception>
        public override void Execute()
        {
            if (Program is not AppStoredProgram asp)
                throw new StoredProgramException("Invalid program type.");

            asp.ExitMethod();
        }

        /// <summary>
        /// Performs compile-time processing for the <c>end method</c> command.
        /// </summary>
        /// <remarks>
        /// No compile-time action is required for this command.
        /// </remarks>
        public override void Compile() { }

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
