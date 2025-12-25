using BOOSE;
using System;
using System.Text.RegularExpressions;

namespace BOOSEappTV
{
    public class AppCircle : CommandOneParameter
    {
        public AppCircle() : base() { }

        public override void Execute()
        {
            AppConsole.WriteLine("My AppCircle method called");

            // 1. Normalise expression
            string expr = Tidy(Parameters[0]);

            // 2. Replace variables
            string evaluable = ReplaceVariables(expr);

            // 3. Evaluate
            int radius;
            try
            {
                var table = new System.Data.DataTable();
                object result = table.Compute(evaluable, "");
                radius = Convert.ToInt32(result);
            }
            catch
            {
                throw new StoredProgramException(
                    $"Invalid expression, can't evaluate {Parameters[0]}"
                );
            }

            if (radius < 1)
                throw new CanvasException("Radius must be a positive integer.");

            canvas.Circle(radius, false);
        }

        // Helpers
        private static string Tidy(string expr)
        {
            expr = Regex.Replace(expr, @"([+\-*/()])", " $1 ");
            return Regex.Replace(expr, @"\s+", " ").Trim();
        }

        private string ReplaceVariables(string exp)
        {
            var tokens = exp.Split(' ', StringSplitOptions.RemoveEmptyEntries);

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

                throw new CanvasException($"Invalid expression '{exp}'");
            }

            return string.Join(" ", tokens);
        }

        public override string ToString()
        {
            return "Circle";
        }
    }
}
