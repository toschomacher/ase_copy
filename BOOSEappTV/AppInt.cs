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
            if (parts.Length > 1) Expression = parts[1];
        }


        public override void Compile()
        {
            if (string.IsNullOrWhiteSpace(VarName))
                throw new VarException("Variable name required.");

            if (Program == null)
                throw new InvalidOperationException("Program reference not set.");

            // ONLY syntax validation here
            if (!string.IsNullOrWhiteSpace(Expression))
            {
                if (!Program.IsExpression(Expression) && !int.TryParse(Expression, out _))
                    throw new ParserException($"Invalid expression '{Expression}'");
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

            if (Program.VariableExists(VarName))
                Program.UpdateVariable(VarName, intValue);
            else
                Program.AddVariable(this);

            Value = intValue;
            Program.AddVariable(this);   // ✅ REQUIRED

        }


    }
}
