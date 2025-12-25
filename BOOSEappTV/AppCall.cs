using BOOSE;
using System.Collections.Generic;

namespace BOOSEappTV
{
    public class AppCall : Command
    {
        private string methodName = "";
        private readonly List<string> args = new();

        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;

            var parts = parameters.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 1)
                throw new StoredProgramException("call requires a method name.");

            methodName = parts[0];

            for (int i = 1; i < parts.Length; i++)
                args.Add(parts[i]);
        }

        public override void Execute()
        {
            if (Program is not AppStoredProgram asp)
                throw new StoredProgramException("Invalid program type.");

            if (!asp.TryGetMethod(methodName, out var def))
                throw new StoredProgramException($"Unknown method '{methodName}'.");

            asp.EnterMethod(def, args);
        }

        public override void Compile() { }
        public override void CheckParameters(string[] parameter) { }
    }
}
