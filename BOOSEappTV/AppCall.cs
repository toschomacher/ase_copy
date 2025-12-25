using BOOSE;
using System.Collections.Generic;

namespace BOOSEappTV
{
    /// <summary>
    /// Represents a method call command within the BOOSE interpreter.
    /// </summary>
    /// <remarks>
    /// This command resolves and invokes a previously declared method
    /// at runtime, passing any supplied arguments to the method context.
    /// </remarks>
    public class AppCall : Command
    {
        /// <summary>
        /// The name of the method to be invoked.
        /// </summary>
        private string methodName = "";

        /// <summary>
        /// The list of arguments supplied to the method call.
        /// </summary>
        private readonly List<string> args = new();

        /// <summary>
        /// Parses and stores the method call parameters.
        /// </summary>
        /// <param name="program">
        /// The current <see cref="StoredProgram"/> instance.
        /// </param>
        /// <param name="parameters">
        /// A parameter string containing the method name followed by any arguments.
        /// </param>
        /// <exception cref="StoredProgramException">
        /// Thrown when no method name is provided.
        /// </exception>
        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;

            var parts = parameters.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 1)
                throw new StoredProgramException("call requires a method name.");

            methodName = parts[0];

            for (int i = 1; i < parts.Length; i++)
                args.Add(parts[i]);
        }

        /// <summary>
        /// Executes the method call at runtime.
        /// </summary>
        /// <remarks>
        /// Method resolution and invocation are delegated to
        /// <see cref="AppStoredProgram"/>, which manages method scope
        /// and execution context.
        /// </remarks>
        /// <exception cref="StoredProgramException">
        /// Thrown when the program type is invalid or the method cannot be found.
        /// </exception>
        public override void Execute()
        {
            if (Program is not AppStoredProgram asp)
                throw new StoredProgramException("Invalid program type.");

            if (!asp.TryGetMethod(methodName, out var def))
                throw new StoredProgramException($"Unknown method '{methodName}'.");

            asp.EnterMethod(def, args);
        }

        /// <summary>
        /// Performs compile-time processing for the method call.
        /// </summary>
        /// <remarks>
        /// No compile-time action is required for method calls.
        /// </remarks>
        public override void Compile() { }

        /// <summary>
        /// Performs parameter validation.
        /// </summary>
        /// <param name="parameter">The parameter array.</param>
        /// <remarks>
        /// Parameter validation is handled during parsing.
        /// </remarks>
        public override void CheckParameters(string[] parameter) { }
    }
}
