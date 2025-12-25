using BOOSE;
using System;
using System.Data;

namespace BOOSEappTV
{
    public class AppIf : Command
    {
        private string condition;

        public int IfLine { get; set; } = -1;
        public int ElseLine { get; set; } = -1;
        public int EndIfLine { get; set; } = -1;

        // This is what ELSE needs
        public bool LastConditionTrue { get; private set; }

        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;
            condition = parameters?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(condition))
                throw new ParserException("if missing condition");
        }

        public override void Compile()
        {
            // linking happens in parser
        }

        public override void Execute()
        {
            bool result = EvaluateCondition(condition);
            LastConditionTrue = result;

            if (!result)
            {
                if (ElseLine >= 0)
                {
                    // Jump to FIRST executable line inside ELSE block
                    Program.PC = ElseLine;
                }
                else
                {
                    // No ELSE → jump past END IF
                    Program.PC = EndIfLine;
                }
            }
        }


        private bool EvaluateCondition(string expr)
        {
            // replace variable names with values
            string replaced = ReplaceVariables(expr);

            try
            {
                var table = new DataTable();
                object result = table.Compute(replaced, "");

                // DataTable.Compute comparisons return bool (usually)
                if (result is bool b) return b;

                // fallback: numeric truthiness
                int n = Convert.ToInt32(result);
                return n != 0;
            }
            catch (Exception ex)
            {
                throw new StoredProgramException($"Invalid if condition '{expr}': {ex.Message}");
            }
        }

        private string ReplaceVariables(string exp)
        {
            // Tokenise by space (your parser tidies expressions)
            var tokens = exp.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tokens.Length; i++)
            {
                string t = tokens[i];

                // operators/comparators
                if (t is "<" or ">" or "<=" or ">=" or "==" or "!=" or "(" or ")")
                    continue;

                // numeric literal
                if (double.TryParse(t, out _))
                    continue;

                // boolean literal
                if (t.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                    t.Equals("false", StringComparison.OrdinalIgnoreCase))
                    continue;

                // variable
                if (!Program.VariableExists(t))
                    throw new StoredProgramException($"Unknown variable '{t}' in if condition");

                tokens[i] = Program.GetVarValue(t);
            }

            return string.Join(" ", tokens);
        }

        public override void CheckParameters(string[] parameter) { }
    }
}
