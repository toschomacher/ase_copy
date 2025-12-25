using BOOSE;

namespace BOOSEappTV
{
    /// <summary>
    /// Represents a boolean variable declaration within the BOOSE interpreter.
    /// </summary>
    /// <remarks>
    /// This implementation follows BOOSE semantics:
    /// boolean variables are declared at compile time and do not perform any
    /// action at runtime. Runtime assignments are delegated to
    /// <see cref="AppAssign"/> commands.
    /// </remarks>
    public class AppBoolean : Evaluation, ICommand
    {
        /// <summary>
        /// Gets or sets the current boolean value of the variable.
        /// </summary>
        /// <remarks>
        /// The value is updated only at runtime by assignment commands.
        /// </remarks>
        public bool Value { get; set; }

        /// <summary>
        /// Initialises a new instance of the <see cref="AppBoolean"/> class.
        /// </summary>
        /// <remarks>
        /// Boolean variables are not numeric expressions and therefore
        /// do not participate in numeric evaluation.
        /// </remarks>
        public AppBoolean()
        {
            IsDouble = false;
        }

        /// <summary>
        /// Parses and stores the parameters required to declare the boolean variable.
        /// </summary>
        /// <param name="program">
        /// The current <see cref="StoredProgram"/> instance.
        /// </param>
        /// <param name="parameters">
        /// The parameter string containing the variable name and optional initialiser.
        /// </param>
        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;

            if (string.IsNullOrWhiteSpace(parameters))
                return;

            var parts = parameters.Split('=', 2, StringSplitOptions.TrimEntries);

            VarName = parts[0];
            Expression = parts.Length > 1 ? parts[1] : "";
        }

        /// <summary>
        /// Declares the boolean variable and queues any initial assignment.
        /// </summary>
        /// <remarks>
        /// Declaration occurs at compile time, while any initialiser expression
        /// is deferred to runtime via an <see cref="AppAssign"/> command.
        /// </remarks>
        /// <exception cref="ParserException">
        /// Thrown when the variable name is missing.
        /// </exception>
        public override void Compile()
        {
            if (string.IsNullOrWhiteSpace(VarName))
                throw new ParserException("Variable name missing");

            // Declaration only
            if (!Program.VariableExists(VarName))
            {
                Value = false;
                Program.AddVariable(this);

                AppConsole.WriteLine(
                    $"[DEBUG] Boolean '{VarName}' declared (initial value = false)"
                );
            }

            // Initialiser - runtime assignment
            if (!string.IsNullOrWhiteSpace(Expression))
            {
                var assign = new AppAssign(VarName, Expression);
                assign.Set(Program, null);
                Program.Add(assign);

                AppConsole.WriteLine(
                    $"[DEBUG] Assignment command queued: {VarName} = {Expression}"
                );
            }
        }

        /// <summary>
        /// Executes the boolean declaration at runtime.
        /// </summary>
        /// <remarks>
        /// Boolean declarations perform no action during execution.
        /// All value changes are handled by runtime assignment commands.
        /// </remarks>
        public override void Execute()
        {
            // Boolean declarations do nothing at runtime
        }
    }
}
