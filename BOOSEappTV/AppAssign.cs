using BOOSE;
using System.Data;
using System.Text.RegularExpressions;

namespace BOOSEappTV
{
    public class AppAssign : Command
    {
        private readonly string varName;
        private readonly string expression;

        public AppAssign(string varName, string expression)
        {
            this.varName = varName;
            this.expression = expression;
        }

        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;
        }
        private string TidyExpression(string exp)
        {
            // replicate BOOSE.Parser.tidyExpression()
            exp = Regex.Replace(exp, @"([\+\-\*/\(\)])", " $1 ");
            return Regex.Replace(exp, @"\s+", " ").Trim();
        }

        private string ReplaceVariables(string exp)
        {
            var tokens = exp.Split(' ');

            for (int i = 0; i < tokens.Length; i++)
            {
                if (int.TryParse(tokens[i], out _))
                    continue;

                if (Program.VariableExists(tokens[i]))
                {
                    tokens[i] = Program.GetVarValue(tokens[i]);
                }
            }

            return string.Join(" ", tokens);
        }


        public override void Execute()
        {
            // 1️⃣ Normalise expression FIRST
            string tidy = TidyExpression(expression);

            // 2️⃣ Replace variables with values
            tidy = ReplaceVariables(tidy);

            // 3️⃣ Evaluate
            int result;
            try
            {
                var table = new DataTable();
                result = Convert.ToInt32(table.Compute(tidy, ""));
            }
            catch
            {
                throw new StoredProgramException(
                    $"Invalid expression, can't evaluate {expression}"
                );
            }

            // 4️⃣ Store result
            Program.UpdateVariable(varName, result);

            AppConsole.WriteLine(
                $"[DEBUG] Assigned '{varName}' = {result}"
            );
        }


        public override void CheckParameters(string[] parameter)
        {
            throw new NotImplementedException();
        }
    }
}
