using BOOSE;

namespace BOOSEappTV
{
    /// <summary>
    /// Boolean variable declaration (BOOSE-correct).
    /// Runtime assignments are handled by AppAssign.
    /// </summary>
    public class AppBoolean : Evaluation, ICommand
    {
        public bool Value { get; set; }

        public AppBoolean()
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
            Expression = parts.Length > 1 ? parts[1] : "";
        }

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

        public override void Execute()
        {
            // Boolean declarations do nothing at runtime
        }
    }

}
