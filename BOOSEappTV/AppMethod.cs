using BOOSE;
using System;

namespace BOOSEappTV
{
    // method <type> <name> <type> <param>, ...
    public class AppMethod : Command
    {
        public AppStoredProgram.MethodDef Def = new();

        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;
            if (program is not AppStoredProgram)
                throw new StoredProgramException("AppMethod requires AppStoredProgram.");

            if (string.IsNullOrWhiteSpace(parameters))
                throw new ParserException("method missing parameters");
        }

        public override void Compile()
        {
            // Parser fills Def and registers it
        }

        public override void Execute()
        {
            // When program reaches a method definition during normal run:
            // skip over the body.
            if (Program is AppStoredProgram asp)
            {
                Program.PC = Def.EndMethodLine + 1;
                return;
            }

            throw new StoredProgramException("Method executed without AppStoredProgram.");
        }

        public override void CheckParameters(string[] parameter) { }
    }
}
