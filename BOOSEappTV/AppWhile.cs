using BOOSE;
using System;
using System.Data;

namespace BOOSEappTV
{
    public class AppWhile : Command
    {
        private string condition;

        public int WhileLine { get; set; }
        public int EndWhileLine { get; set; }

        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;
            condition = parameters?.Trim()
                ?? throw new ParserException("while missing condition");
        }

        public override void Compile()
        {
            // set when added to program
            WhileLine = Program.Count;
        }

        public override void Execute()
        {
            bool result = EvaluateCondition(condition);

            if (!result)
            {
                // jump past END WHILE
                Program.PC = EndWhileLine + 1;
            }
        }

        private bool EvaluateCondition(string expr)
        {
            string replaced = ReplaceVariables(expr);
            var table = new DataTable();
            object result = table.Compute(replaced, "");
            return Convert.ToBoolean(result);
        }

        private string ReplaceVariables(string expr)
        {
            var tokens = expr.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tokens.Length; i++)
            {
                if (double.TryParse(tokens[i], out _)) continue;
                if ("+-*/()<>=!".Contains(tokens[i])) continue;

                if (Program.VariableExists(tokens[i]))
                {
                    tokens[i] = Program.GetVarValue(tokens[i]);
                }
            }

            return string.Join(" ", tokens);
        }

        public override void CheckParameters(string[] parameter) { }
    }
}
