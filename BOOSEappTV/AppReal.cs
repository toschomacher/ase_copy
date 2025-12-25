using BOOSE;
using System;
using System.Text.RegularExpressions;

namespace BOOSEappTV
{
    /// <summary>
    /// Represents a real (floating-point) variable declaration command.
    /// </summary>
    /// <remarks>
    /// This class is a replacement for <c>BOOSE.Real</c>, which is sealed.
    /// It follows BOOSE-correct semantics:
    /// <list type="bullet">
    /// <item><description>Variable declaration occurs at compile time.</description></item>
    /// <item><description>If an initialiser is provided, an <see cref="AppAssign"/> command is queued for runtime.</description></item>
    /// <item><description>No evaluation is performed during compilation.</description></item>
    /// </list>
    /// All expression evaluation and value updates are deferred to runtime.
    /// </remarks>
    public class AppReal : Evaluation, ICommand
    {
        /// <summary>
        /// Gets or sets the current value of the real variable.
        /// </summary>
        /// <remarks>
        /// This property must be writable so that
        /// <see cref="AppStoredProgram.UpdateVariable"/> can update the value
        /// at runtime.
        /// </remarks>
        public double Value { get; set; }

        /// <summary>
        /// Initialises a new instance of the <see cref="AppReal"/> class.
        /// </summary>
        /// <remarks>
        /// Real variables are treated as double-precision values.
        /// </remarks>
        public AppReal()
        {
            IsDouble = true;
        }

        /// <summary>
        /// Parses and stores the parameters for the real variable declaration.
        /// </summary>
        /// <param name="program">The active <see cref="StoredProgram"/> instance.</param>
        /// <param name="parameters">
        /// The parameter string containing the variable name and optional initialiser.
        /// </param>
        public override void Set(StoredProgram program, string parameters)
        {
            // Always set Program reference
            Program = program;

            if (string.IsNullOrWhiteSpace(parameters))
                return;

            // Parameters example: "num = 55.5" or "circ = 2*pi*radius"
            var parts = parameters.Split('=', 2, StringSplitOptions.TrimEntries);

            VarName = parts[0];
            Expression = parts.Length > 1 ? parts[1] : "";
        }

        /// <summary>
        /// Declares the real variable and queues any initial assignment.
        /// </summary>
        /// <remarks>
        /// Variable declaration occurs once at compile time.
        /// If an initialiser expression is present, a runtime assignment
        /// command is created and queued.
        /// </remarks>
        /// <exception cref="ParserException">
        /// Thrown when the variable name is missing.
        /// </exception>
        public override void Compile()
        {
            if (string.IsNullOrWhiteSpace(VarName))
                throw new ParserException("Variable name missing");

            // Declaration: add to variable table once
            if (!Program.VariableExists(VarName))
            {
                Value = 0.0;
                Program.AddVariable(this);

                AppConsole.WriteLine(
                    $"[DEBUG] Real '{VarName}' declared (initial value = 0.0)"
                );
            }

            // Initialiser: queue runtime assignment (must be tidied)
            if (!string.IsNullOrWhiteSpace(Expression))
            {
                string rhs = TidyExpression(Expression);

                var assign = new AppAssign(VarName, rhs);
                assign.Set(Program, null);
                Program.Add(assign);

                AppConsole.WriteLine(
                    $"[DEBUG] Assignment command queued: {VarName} = {rhs}"
                );
            }
        }

        /// <summary>
        /// Executes the real variable declaration at runtime.
        /// </summary>
        /// <remarks>
        /// This method intentionally performs no action.
        /// All value changes are handled by queued <see cref="AppAssign"/> commands.
        /// </remarks>
        public override void Execute()
        {
            // Declarations do nothing at runtime (assignments handled via AppAssign).
        }

        /// <summary>
        /// Normalises a mathematical expression by inserting spacing
        /// around operators and parentheses.
        /// </summary>
        /// <param name="exp">The expression to tidy.</param>
        /// <returns>A normalised expression string.</returns>
        private static string TidyExpression(string exp)
        {
            exp = Regex.Replace(exp, @"([\+\-\*/\(\)])", " $1 ");
            return Regex.Replace(exp, @"\s+", " ").Trim();
        }
    }
}
