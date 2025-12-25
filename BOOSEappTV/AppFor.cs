using BOOSE;
using System;
using System.Data;

namespace BOOSEappTV
{
    /// <summary>
    /// for var = start to end step n
    /// </summary>
    public class AppFor : Command
    {
        public string VarName;
        public string StartExpr;
        public string EndExpr;
        public string StepExpr = "1";

        public int ForLine;
        public int EndForLine;

        private bool initialized = false;

        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;

            if (string.IsNullOrWhiteSpace(parameters))
                throw new ParserException("for missing parameters");

            var parts = parameters.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 5 || parts[1] != "=" ||
                !parts[3].Equals("to", StringComparison.OrdinalIgnoreCase))
                throw new ParserException("Invalid for syntax");

            VarName = parts[0];
            StartExpr = parts[2];
            EndExpr = parts[4];

            if (parts.Length >= 7 && parts[5].Equals("step", StringComparison.OrdinalIgnoreCase))
                StepExpr = parts[6];

            // ✅ BOOSE-correct variable declaration
            if (!Program.VariableExists(VarName))
            {
                var loopVar = new AppInt();
                loopVar.Set(Program, VarName);
                Program.AddVariable(loopVar);
            }
        }

        public override void Compile()
        {
            // linking handled by parser
        }

        public override void Execute()
        {
            // initialise once
            if (!initialized)
            {
                int start = EvalInt(StartExpr);
                Program.UpdateVariable(VarName, start);
                initialized = true;
            }

            int current = int.Parse(Program.GetVarValue(VarName));
            int end = EvalInt(EndExpr);
            int step = EvalInt(StepExpr);

            bool condition =
                step >= 0 ? current <= end
                          : current >= end;

            if (!condition)
            {
                // skip loop body
                Program.PC = EndForLine + 1;
            }
        }

        // =====================
        // Helpers
        // =====================

        public int EvalInt(string expr)
        {
            string replaced = ReplaceVariables(expr);
            var table = new DataTable();
            object result = table.Compute(replaced, "");
            return Convert.ToInt32(result);
        }

        private string ReplaceVariables(string exp)
        {
            var tokens = exp.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tokens.Length; i++)
            {
                string t = tokens[i];

                if (int.TryParse(t, out _))
                    continue;

                if (t is "+" or "-" or "*" or "/" or "(" or ")")
                    continue;

                if (Program.VariableExists(t))
                {
                    tokens[i] = Program.GetVarValue(t);
                    continue;
                }

                throw new StoredProgramException($"Invalid expression '{exp}'");
            }

            return string.Join(" ", tokens);
        }

        public override void CheckParameters(string[] parameter) { }
    }
}
