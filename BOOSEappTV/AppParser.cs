using BOOSE;
using System;
using System.Text.RegularExpressions;

namespace BOOSEappTV
{
    public class AppParser : Parser
    {
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
            // REASSIGNMENT
            // --------------------------------------------------
            if (parts.Length >= 3 && parts[1] == "=")
            {
                string varName = parts[0];

                // tidy RHS so num+10 becomes "num + 10"
                string rhsRaw = string.Join(" ", parts, 2, parts.Length - 2);
                string rhs = TidyExpression(rhsRaw);

                // must already exist
                if (!program.VariableExists(varName))
                    throw new ParserException($"unknown variable type at line {program.PC + 1}");

                // queue runtime assignment (do NOT create AppInt here)
                var assign = new AppAssign(varName, rhs);
                assign.Set(program, null);           // just to set Program reference (your Set ignores params anyway)
                program.Add(assign);

                AppConsole.WriteLine($"[DEBUG] Assignment command queued: {varName} = {rhs}");
                return assign;
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

            return command; // BOOSE adds it
        }

        private string TidyExpression(string exp)
        {
            exp = Regex.Replace(exp, @"([\+\-\*/\(\)])", " $1 ");
            return Regex.Replace(exp, @"\s+", " ").Trim();
        }
    }
}
