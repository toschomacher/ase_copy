using BOOSE;
using System;

namespace BOOSEappTV
{
    public class AppParser : Parser
    {
        // 🔑 OUR OWN references (BOOSE Parser keeps theirs private)
        private readonly StoredProgram program;
        private readonly CommandFactory factory;

        public AppParser(CommandFactory factory, StoredProgram program)
            : base(factory, program)
        {
            this.factory = factory;
            this.program = program;
        }

        public override ICommand ParseCommand(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;

            line = line.Trim();

            // Ignore comments
            if (line.StartsWith("*"))
                return null;

            string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // --------------------------------------------------
            // REASSIGNMENT:  num = expression
            // --------------------------------------------------
            if (parts.Length >= 3 && parts[1] == "=")
            {
                string varName = parts[0];
                string expression = string.Join(" ", parts, 2, parts.Length - 2);

                if (!program.VariableExists(varName))
                    throw new ParserException(
                        $"unknown variable type at line {program.PC + 1}"
                    );

                Evaluation variable = program.GetVariable(varName);

                ICommand cmd;

                if (variable is AppInt)
                {
                    cmd = new AppInt();
                }
                else
                {
                    throw new ParserException(
                        $"unsupported variable type at line {program.PC + 1}"
                    );
                }

                cmd.Set(program, $"{varName} = {expression}");
                cmd.Compile();

                program.Add((Command)cmd);
                return cmd;
            }

            // --------------------------------------------------
            // NORMAL COMMAND / DECLARATION
            // --------------------------------------------------
            ICommand command = factory.MakeCommand(parts[0]);

            if (command == null)
                throw new ParserException(
                    $"unknown command '{parts[0]}' at line {program.PC + 1}"
                );

            string parameters = line.Substring(parts[0].Length).Trim();

            command.Set(program, parameters);
            command.Compile();

            program.Add((Command)command);
            return command;
        }
    }
}
