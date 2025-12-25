using BOOSE;
using System;
using System.Data;
using System.Text.RegularExpressions;

namespace BOOSEappTV
{
    internal class AppRect : CommandTwoParameters
    {
        public override void Execute()
        {
            AppConsole.WriteLine("My AppRect method called");

            int width = EvaluateIntExpression(Parameters[0], "Width");
            int height = EvaluateIntExpression(Parameters[1], "Height");

            if (width < 1 || height < 1)
                throw new CanvasException("Width and height must be positive integers.");

            canvas.Rect(width, height, false);
        }

        // Helpers
        private string Tidy(string expr)
        {
            expr = Regex.Replace(expr, @"([+\-*/()])", " $1 ");
            return Regex.Replace(expr, @"\s+", " ").Trim();
        }

        private int EvaluateIntExpression(string expr, string name)
        {
            string evaluable = ReplaceVariables(Tidy(expr));

            try
            {
                var table = new DataTable();
                object result = table.Compute(evaluable, "");

                // Truncate "real" into "int"
                return Convert.ToInt32(result);
            }
            catch
            {
                throw new CanvasException($"{name} must be a valid integer expression.");
            }
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
