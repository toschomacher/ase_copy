using BOOSE;

namespace BOOSEappTV
{
    public class AppInt : Evaluation, ICommand
    {
        public AppInt()
        {
            IsDouble = false;
        }

        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;

            if (parameters.Contains("="))
            {
                var parts = parameters.Split('=');
                varName = parts[0].Trim();
                expression = parts[1].Trim();
            }
            else
            {
                varName = parameters.Trim();
                expression = "";
            }
        }

        public override void Compile()
        {
            if (string.IsNullOrWhiteSpace(varName))
                throw new ParserException("Variable name missing");

            // DECLARE variable only
            if (!Program.VariableExists(varName))
            {
                Value = 0;
                Program.AddVariable(this);

                AppConsole.WriteLine(
                    $"[DEBUG] Variable '{varName}' declared (initial value = 0)"
                );
            }

            // QUEUE runtime assignment if needed
            if (!string.IsNullOrWhiteSpace(expression))
            {
                var assign = new AppAssign(varName, expression);
                assign.Program = Program;
                Program.Add(assign);

                AppConsole.WriteLine(
                    $"[DEBUG] Assignment command queued: {varName} = {expression}"
                );
            }
        }

        // 🔑 VERY IMPORTANT
        public override void Execute()
        {
            // Int declarations do NOTHING at runtime
        }
    }
}
