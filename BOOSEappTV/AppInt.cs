using BOOSE;
using System;

namespace BOOSEappTV
{
    /// <summary>
    /// Represents an integer variable declaration command.
    /// </summary>
    /// <remarks>
    /// This class implements BOOSE-correct integer variable semantics.
    /// <list type="bullet">
    /// <item><description><see cref="Compile"/> declares the variable in the program's variable table.</description></item>
    /// <item><description>If an initialiser is present, an <see cref="AppAssign"/> command is queued for runtime execution.</description></item>
    /// <item><description><see cref="Execute"/> intentionally performs no action.</description></item>
    /// </list>
    /// Evaluation of expressions and value updates occur strictly at runtime,
    /// preserving BOOSE’s two-pass execution model.
    /// </remarks>
    public class AppInt : Evaluation, ICommand
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="AppInt"/> class.
        /// </summary>
        /// <remarks>
        /// Integer variables are not treated as double-precision values.
        /// </remarks>
        public AppInt()
        {
            IsDouble = false;
        }

        /// <summary>
        /// Parses and stores the parameters for the integer declaration.
        /// </summary>
        /// <param name="program">The active <see cref="StoredProgram"/> instance.</param>
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

            if (parts.Length > 1)
                Expression = parts[1];
            else
                Expression = "";
        }

        /// <summary>
        /// Declares the integer variable and queues any initial assignment.
        /// </summary>
        /// <remarks>
        /// Variable declaration occurs at compile time.
        /// If an initialiser expression is present, it is deferred to runtime
        /// via an <see cref="AppAssign"/> command.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the program reference has not been set.
        /// </exception>
        /// <exception cref="ParserException">
        /// Thrown when the variable name is missing.
        /// </exception>
        public override void Compile()
        {
            if (Program == null)
                throw new InvalidOperationException("Program not set");

            if (string.IsNullOrWhiteSpace(VarName))
                throw new ParserException("Variable name missing");

            // 1) Declare variable once
            if (!Program.VariableExists(VarName))
            {
                Value = 0;
                Program.AddVariable(this);

                AppConsole.WriteLine(
                    $"[DEBUG] Variable '{VarName}' declared (initial value = 0)"
                );
            }

            // 2) If initialiser exists, queue runtime assignment
            if (!string.IsNullOrWhiteSpace(Expression))
            {
                // IMPORTANT: tidy so "2*radius" becomes "2 * radius"
                string rhs = ExpressionUtil.Tidy(Expression);

                var assign = new AppAssign(VarName, rhs);
                assign.Set(Program, null); // just to set Program ref
                Program.Add(assign);

                AppConsole.WriteLine(
                    $"[DEBUG] Assignment command queued: {VarName} = {rhs}"
                );
            }

            // IMPORTANT: DO NOT Program.Add(this)
            // We only declare here; assignments happen through AppAssign.
        }

        /// <summary>
        /// Executes the integer declaration at runtime.
        /// </summary>
        /// <remarks>
        /// This method intentionally performs no action.
        /// Overriding this prevents <see cref="Evaluation.Execute"/> from running,
        /// which would otherwise violate BOOSE execution semantics.
        /// </remarks>
        public override void Execute()
        {
            // Int declarations do nothing at runtime.
            // (If we didn't override, Evaluation.Execute() would run and cause issues.)
        }
    }
}
