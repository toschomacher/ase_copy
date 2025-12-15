using BOOSE;
using System;

namespace BOOSEappTV
{
    /// <summary>
    /// Replacement for the BOOSE StoredProgram.
    /// Handles variable storage and command execution.
    /// </summary>
    public class AppStoredProgram : StoredProgram
    {
        // Constructor must call base with ICanvas
        public AppStoredProgram(ICanvas canvas) : base(canvas) { }

        // Override AddVariable with correct signature
        public void AddVariable(AppInt variable)
        {
            base.AddVariable(variable); // call base method to actually store the variable
        }



        // Override EvaluateExpression to return string (as per documentation)
        public override string EvaluateExpression(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ParserException("Empty expression");

            // Try literal integer
            if (int.TryParse(expression, out int literal))
                return literal.ToString();

            // Check if variable exists
            if (VariableExists(expression))
            {
                var variable = GetVariable(expression);
                return variable.Value.ToString();
            }

            throw new ParserException($"Undefined variable '{expression}'");
        }

        // Execute all commands in sequence, manipulating PC
        public new void Run()
        {
            for (PC = 0; PC < Count; PC++)
            {
                if (this[PC] is ICommand cmd)
                {
                    cmd.Execute();
                }
                else
                {
                    throw new InvalidOperationException($"Command at line {PC} is null or not ICommand");
                }
            }
        }


    }
}
