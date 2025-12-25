using BOOSE;

namespace BOOSEappTV
{
    public class AppEndMethod : Command
    {
        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;
        }

        public override void Execute()
        {
            if (Program is not AppStoredProgram asp)
                throw new StoredProgramException("Invalid program type.");

            asp.ExitMethod();
        }

        public override void Compile() { }
        public override void CheckParameters(string[] parameter) { }
    }
}
