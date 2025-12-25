using BOOSE;

namespace BOOSEappTV
{
    public class AppEndIf : Command
    {
        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;
        }

        public override void Compile() { }
        public override void Execute() { }
        public override void CheckParameters(string[] parameter) { }
    }
}
