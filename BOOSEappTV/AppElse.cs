using BOOSE;

namespace BOOSEappTV
{
    public class AppElse : Command
    {
        public int EndIfLine { get; set; } = -1;

        // MUST be set by parser
        public AppIf MatchingIf { get; set; }

        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;
        }

        public override void Compile()
        {
            // linking happens in parser
        }

        public override void Execute()
        {
            if (MatchingIf == null)
                throw new StoredProgramException("else without matching if");

            // If the IF was true, skip ELSE body
            if (MatchingIf.LastConditionTrue)
            {
                if (EndIfLine < 0)
                    throw new StoredProgramException("else missing matching end if");

                Program.PC = EndIfLine; // IMPORTANT: -1 due to PC++ after Execute
            }
            // else (IF was false): do nothing → execute else body in sequence
        }

        public override void CheckParameters(string[] parameter) { }
    }
}
