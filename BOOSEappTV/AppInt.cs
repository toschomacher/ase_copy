using BOOSE;
using System;

namespace BOOSEappTV
{
    /// <summary>
    /// Replacement for BOOSE.Int command.
    /// Implements integer variables and assignments.
    /// </summary>
    public class AppInt : Evaluation, ICommand, IEvaluation
    {
        // ----- IEvaluation properties -----
        public new string VarName { get; set; } = string.Empty;
        public new int Value { get; set; } = 0;
        public new string Expression { get; set; } = string.Empty;

        object IEvaluation.Value { get => Value; set => throw new NotImplementedException(); }

        // ----- ICommand methods -----
        public void CheckParameters(string[] parameters)
        {
            if (parameters.Length < 1)
                throw new CommandException("Int requires at least a variable name.");
        }

        /// <summary>
        /// Sets the program reference and parses the parameters string "varName = expression"
        /// </summary>
        public void Set(StoredProgram program, string parameters)
        {
            // assign the base property
            base.Program = program ?? throw new ArgumentNullException(nameof(program));

            var parts = parameters.Split('=', StringSplitOptions.TrimEntries);
            if (parts.Length > 0) VarName = parts[0];
            varName = VarName;
            if (parts.Length > 1) Expression = parts[1];
        }


        public override void Compile()
        {
            if (string.IsNullOrWhiteSpace(VarName))
                throw new VarException("Variable name required");

            if (Program == null)
                throw new InvalidOperationException("Program not set");

            // Evaluate expression first (if any)
            int intValue = 0;
            if (!string.IsNullOrWhiteSpace(Expression))
            {
                try
                {
                    var result = Program.EvaluateExpression(Expression);
                    intValue = Convert.ToInt32(result);
                }
                catch (Exception ex)
                {
                    AppConsole.WriteLine($"[DEBUG] Failed to evaluate expression '{Expression}': {ex.Message}");
                    intValue = 0;
                }
            }

            // Assign to Value property
            Value = intValue;

            // Declare variable if it doesn't exist
            if (!Program.VariableExists(VarName))
            {
                Program.AddVariable(this);

                AppConsole.WriteLine(
                    $"[DEBUG] Variable '{VarName}' declared (initial value = {Value})"
                );
            }
            else
            {
                // Update existing variable at compile time
                Program.UpdateVariable(VarName, Value);
                AppConsole.WriteLine(
                    $"[DEBUG] Variable '{VarName}' already exists, updated value = {Value}"
                );
            }
        }


        public override void Execute()
        {
            int intValue;

            if (!string.IsNullOrWhiteSpace(Expression))
            {
                var result = Program.EvaluateExpression(Expression);
                intValue = Convert.ToInt32(result);
            }
            else
            {
                intValue = 0;
            }

            Program.UpdateVariable(VarName, intValue);
            Value = intValue;

            AppConsole.WriteLine(
                $"[DEBUG] Stored '{VarName}' = {Value}"
            );
        }



    }
}
