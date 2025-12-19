using BOOSE;
using System;
using System.Data;
using System.Globalization;

namespace BOOSEappTV
{
    internal class AppWrite : Evaluation
    {
        private AppCanvas canvas;
        private string expression;

        public AppWrite() : base() { }

        public override void CheckParameters(string[] parameters)
        {
            if (parameters.Length < 1)
                throw new EvaluationException("write requires an expression to output");
        }

        public override void Execute()
        {
            base.Execute();

            canvas = AppCanvas.GetCanvas();

            if (Parameters == null || Parameters.Length == 0)
                throw new EvaluationException("No expression provided to write.");

            expression = string.Join(" ", Parameters);

            // Special canvas debug keywords
            string token = expression.Trim().ToLowerInvariant();

            if (token == "circle" && canvas is AppCanvas ac1 && ac1.HasLastCircle)
            {
                AppConsole.WriteLine(ac1.LastCircleRadius.ToString());
                return;
            }

            if (token == "rect" && canvas is AppCanvas ac2 && ac2.HasLastRect)
            {
                AppConsole.WriteLine(ac2.LastRectParameters);
                return;
            }

            // Normal expression evaluation
            string evaluable = ReplaceVariables(expression);

            try
            {
                var table = new DataTable();
                object result = table.Compute(evaluable, "");

                AppConsole.WriteLine(Convert.ToString(result, CultureInfo.InvariantCulture));
            }
            catch
            {
                // fallback: print literal
                AppConsole.WriteLine(expression);
            }
        }

        /// <summary>
        /// Replace variables in expression with their current values.
        /// Expression must already be space-separated.
        /// </summary>
        private string ReplaceVariables(string exp)
        {
            var tokens = exp.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tokens.Length; i++)
            {
                // literal → leave
                if (double.TryParse(tokens[i], NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                    continue;

                // operator → leave
                if ("+-*/()".Contains(tokens[i]))
                    continue;

                // variable → replace
                if (Program.VariableExists(tokens[i]))
                {
                    var v = Program.GetVariable(tokens[i]);

                    if (v is AppInt ai)
                        tokens[i] = ai.Value.ToString(CultureInfo.InvariantCulture);
                    //else if (v is AppReal ar)
                    //    tokens[i] = ar.Value.ToString(CultureInfo.InvariantCulture);
                    else
                        throw new EvaluationException($"Unsupported variable '{tokens[i]}'");
                }
            }

            return string.Join(" ", tokens);
        }
    }
}
