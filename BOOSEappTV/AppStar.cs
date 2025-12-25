using BOOSE;
using System;
using System.Data;
using System.Text.RegularExpressions;

namespace BOOSEappTV
{
    public class AppStar : CommandTwoParameters
    {
        public override void Execute()
        {
            AppConsole.WriteLine("My AppStar method called");

            if (canvas is not IAppCanvas appCanvas)
                throw new CanvasException("Star command requires AppCanvas.");

            int size = EvaluateIntExpression(Parameters[0], "Star size");
            bool filled = EvaluateBoolExpression(Parameters[1], "Filled");

            if (size < 1)
                throw new CanvasException("Star size must be a positive integer.");

            appCanvas.Star(size, filled);
        }

        // Helpers
        private int EvaluateIntExpression(string expr, string name)
        {
            expr = Tidy(expr);
            string evaluable = ReplaceVariables(expr);

            try
            {
                var table = new DataTable();
                object result = table.Compute(evaluable, "");
                return Convert.ToInt32(result);
            }
            catch
            {
                throw new CanvasException($"{name} must be a valid integer expression.");
            }
        }

        private bool EvaluateBoolExpression(string expr, string name)
        {
            expr = expr.Trim().ToLower();

            if (expr == "true" || expr == "1") return true;
            if (expr == "false" || expr == "0") return false;

            throw new CanvasException($"{name} must be true/false or 1/0.");
        }

        private string Tidy(string expr)
        {
            expr = Regex.Replace(expr, @"([+\-*/()])", " $1 ");
            return Regex.Replace(expr, @"\s+", " ").Trim();
        }

        private string ReplaceVariables(string expr)
        {
            var tokens = expr.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tokens.Length; i++)
            {
                string t = tokens[i];

                if (double.TryParse(t, out _))
                    continue;

                if (t is "+" or "-" or "*" or "/" or "(" or ")")
                    continue;

                if (Program.VariableExists(t))
                {
                    tokens[i] = Program.GetVarValue(t);
                    continue;
                }

                throw new CanvasException($"Unknown variable '{t}' in expression.");
            }

            return string.Join(" ", tokens);
        }
    }
}
