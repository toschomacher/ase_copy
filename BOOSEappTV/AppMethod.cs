using BOOSE;
using System;

namespace BOOSEappTV
{
    /// <summary>
    /// Represents a method declaration command.
    /// </summary>
    /// <remarks>
    /// This command defines a user-defined method within the program.
    /// Method metadata and structure are collected and registered by the parser.
    /// During normal program execution, method bodies are skipped unless
    /// explicitly invoked via a <c>call</c> command.
    /// </remarks>
    public class AppMethod : Command
    {
        /// <summary>
        /// Gets the method definition associated with this method declaration.
        /// </summary>
        /// <remarks>
        /// This structure is populated and registered by the parser and contains
        /// information such as parameter definitions and method boundaries.
        /// </remarks>
        public AppStoredProgram.MethodDef Def = new();

        /// <summary>
        /// Parses and validates the method declaration parameters.
        /// </summary>
        /// <param name="program">The active <see cref="StoredProgram"/> instance.</param>
        /// <param name="parameters">
        /// The parameter string defining the method signature.
        /// </param>
        /// <exception cref="StoredProgramException">
        /// Thrown when the stored program is not an <see cref="AppStoredProgram"/>.
        /// </exception>
        /// <exception cref="ParserException">
        /// Thrown when the method declaration parameters are missing.
        /// </exception>
        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;

            if (program is not AppStoredProgram)
                throw new StoredProgramException("AppMethod requires AppStoredProgram.");

            if (string.IsNullOrWhiteSpace(parameters))
                throw new ParserException("method missing parameters");
        }

        /// <summary>
        /// Performs compile-time processing for the method declaration.
        /// </summary>
        /// <remarks>
        /// The parser is responsible for populating the method definition
        /// and registering it with the stored program.
        /// </remarks>
        public override void Compile()
        {
            // Parser fills Def and registers it
        }

        /// <summary>
        /// Executes the method declaration at runtime.
        /// </summary>
        /// <remarks>
        /// When encountered during normal execution flow, the method body
        /// is skipped by advancing the program counter to the line
        /// immediately following the matching <c>end method</c> command.
        /// </remarks>
        /// <exception cref="StoredProgramException">
        /// Thrown when execution occurs without an <see cref="AppStoredProgram"/>.
        /// </exception>
        public override void Execute()
        {
            // When program reaches a method definition during normal run:
            // skip over the body.
            if (Program is AppStoredProgram asp)
            {
                Program.PC = Def.EndMethodLine + 1;
                return;
            }

            throw new StoredProgramException("Method executed without AppStoredProgram.");
        }

        /// <summary>
        /// Performs parameter validation.
        /// </summary>
        /// <param name="parameter">The parameter array.</param>
        /// <remarks>
        /// Parameter validation is handled by the parser.
        /// </remarks>
        public override void CheckParameters(string[] parameter) { }
    }
}
