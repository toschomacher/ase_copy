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

        // Reference to stored program
        public new StoredProgram Program { get; set; } = null!;

        object IEvaluation.Value { get => Value; set => throw new NotImplementedException(); }

        // ----- ICommand methods -----
        public void CheckParameters(string[] parameters)
        {
            if (parameters.Length < 1)
                throw new CommandException("Int requires at least a variable name.");
        }

        public void Set(StoredProgram program, string parameters)
        {
            Program = program ?? throw new ArgumentNullException(nameof(program));

            // Split "varName = expression"
            var parts = parameters.Split('=', StringSplitOptions.TrimEntries);
            if (parts.Length > 0) VarName = parts[0];
            if (parts.Length > 1) Expression = parts[1];
        }

        public override void Compile()
        {
            if (string.IsNullOrWhiteSpace(VarName))
                throw new VarException("Variable name required.");

            if (Program == null)
                throw new InvalidOperationException("Program reference not set.");

            int intValue;

            // If variable exists, update its value
            if (Program.VariableExists(VarName))
            {
                var rhsResult = Program.EvaluateExpression(Expression);
                intValue = Convert.ToInt32(rhsResult);
                Program.UpdateVariable(VarName, intValue);
                Value = intValue;
                return;
            }

            // First-time creation
            if (!string.IsNullOrWhiteSpace(Expression))
            {
                if (int.TryParse(Expression, out int parsed))
                {
                    intValue = parsed;
                }
                else
                {
                    if (!Program.VariableExists(Expression))
                        throw new ParserException($"Undefined variable '{Expression}' for int '{VarName}'.");

                    var referenced = Program.GetVariable(Expression);
                    intValue = Convert.ToInt32(referenced.Value);
                }
            }
            else
            {
                intValue = 0; // default
            }

            Value = intValue;
            Program.AddVariable(this);
        }

        public override void Execute()
        {
            // Assignment handled in Compile()
        }
    }
}
