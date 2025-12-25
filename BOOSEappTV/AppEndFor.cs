using BOOSE;
using System;

namespace BOOSEappTV
{
    /// <summary>
    /// end for
    /// </summary>
    public class AppEndFor : Command
    {
        public AppFor MatchingFor;

        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;
        }

        public override void Compile()
        {
            // linking handled by parser
        }

        public override void Execute()
        {
            int current = int.Parse(Program.GetVarValue(MatchingFor.VarName));
            int step = MatchingFor.EvalInt(MatchingFor.StepExpr);

            Program.UpdateVariable(MatchingFor.VarName, current + step);

            // jump back to for
            Program.PC = MatchingFor.ForLine;
        }

        public override void CheckParameters(string[] parameter) {}
    }
}
