using BOOSE;
using System;

namespace BOOSEappTV
{
    /// <summary>
    /// Int variable declaration command.
    /// - Compile() declares the variable in the variable table.
    /// - If there is an initializer, it queues an AppAssign runtime command.
    /// - Execute() does NOTHING (we do not want Evaluation.Execute() to run).
    /// </summary>
    public class AppInt : Evaluation, ICommand
    {
        public AppInt()
        {
            IsDouble = false;
        }

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

            // 2) If initializer exists, queue runtime assignment
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

        public override void Execute()
        {
            // Int declarations do nothing at runtime.
            // (If we didn't override, Evaluation.Execute() would run and cause issues.)
        }
    }
}
