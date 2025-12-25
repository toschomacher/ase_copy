using BOOSE;
using System;
using System.Text.RegularExpressions;

namespace BOOSEappTV
{
    /// <summary>
    /// Real number variable replacement for BOOSE.Real (sealed).
    /// Declarations add the variable; initializers queue an AppAssign for runtime.
    /// </summary>
    public class AppReal : Evaluation, ICommand
    {
        // IMPORTANT: must be settable so AppStoredProgram.UpdateVariable can update it.
        public double Value { get; set; }

        public AppReal()
        {
            IsDouble = true;
        }

        public override void Set(StoredProgram program, string parameters)
        {
            // Always set Program reference
            Program = program;

            if (string.IsNullOrWhiteSpace(parameters))
                return;

            // parameters example: "num = 55.5" or "circ = 2*pi*radius"
            var parts = parameters.Split('=', 2, StringSplitOptions.TrimEntries);

            VarName = parts[0];

            Expression = parts.Length > 1 ? parts[1] : "";
        }

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

            // Initializer: queue runtime assignment (must be tidied!)
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

        public override void Execute()
        {
            // Declarations do nothing at runtime (assignments handled via AppAssign).
        }

        private static string TidyExpression(string exp)
        {
            exp = Regex.Replace(exp, @"([\+\-\*/\(\)])", " $1 ");
            return Regex.Replace(exp, @"\s+", " ").Trim();
        }
    }
}
