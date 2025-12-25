using BOOSE;

namespace BOOSEappTV
{
    public class AppEndWhile : Command
    {
        public AppWhile MatchingWhile { get; set; }

        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;
        }

        public override void Compile()
        {
            // nothing needed
        }

        public override void Execute()
        {
            // jump back to WHILE
            Program.PC = MatchingWhile.WhileLine;
        }

        public override void CheckParameters(string[] parameter) { }
    }
}
