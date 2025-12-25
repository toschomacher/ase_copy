using BOOSE;
using System;
using System.Data;
using static System.Net.Mime.MediaTypeNames;

namespace BOOSEappTV
{
    internal class AppWrite : Evaluation
    {
        public AppWrite() : base() { }

        public override void Set(StoredProgram program, string parameters)
        {
            base.Set(program, parameters);
            Program = program;

            if (!string.IsNullOrWhiteSpace(parameters))
                Expression = parameters.Trim();
        }

        public override void Compile()
        {
            // No compile-time work
        }

        public override void Execute()
        {
            var canvas = AppCanvas.GetCanvas();
            if (string.IsNullOrWhiteSpace(Expression))
                throw new EvaluationException("No expression provided to write.");

            string expr = Expression.Trim();

            // String concatenation e.g. "£"+y or "x="+x

            if (expr.Contains("+") && expr.Contains("\""))
            {
                var parts = expr.Split('+', StringSplitOptions.TrimEntries);
                string output = "";

                foreach (var part in parts)
                {
                    // Quoted string literal
                    if (part.StartsWith("\"") && part.EndsWith("\"") && part.Length >= 2)
                    {
                        output += part[1..^1];
                    }
                    // Variable
                    else if (Program.VariableExists(part))
                    {
                        string rawValue = Program.GetVarValue(part);
                        var variable = Program.GetVariable(part);

                        if (variable is AppBoolean)
                            output += rawValue == "1" ? "true" : "false";
                        else
                            output += rawValue;
                    }
                    else
                    {
                        throw new EvaluationException($"Unknown variable '{part}'");
                    }
                }

                AppConsole.WriteLine(output);
                canvas.WriteText(output);
                return;
            }

            // Quoted string literal
            if (expr.StartsWith("\"") && expr.EndsWith("\"") && expr.Length >= 2)
            {
                AppConsole.WriteLine(expr[1..^1]);
                return;
            }

            // Single variable
            if (Program.VariableExists(expr))
            {
                var variable = Program.GetVariable(expr);

                // BOOLEAN — always read Value directly
                if (variable is AppBoolean ab)
                {
                    string text = ab.Value ? "true" : "false";
                    AppConsole.WriteLine(text);
                    canvas.WriteText(text);
                    return;
                }

                // INT / REAL
                string rawValue = Program.GetVarValue(expr);
                AppConsole.WriteLine(rawValue);
                canvas.WriteText(rawValue);
                return;
            }

            // Numeric expression
            string evaluable = ReplaceVariables(expr);

            try
            {
                var table = new DataTable();
                object result = table.Compute(evaluable, "");
                AppConsole.WriteLine(Convert.ToString(result));
                canvas.WriteText(Convert.ToString(result));
            }
            catch
            {
                throw new EvaluationException("Invalid expression in write.");
            }
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

                if (!Program.VariableExists(t))
                    throw new EvaluationException($"Unknown variable '{t}'");

                tokens[i] = Program.GetVarValue(t);
            }

            return string.Join(" ", tokens);
        }
    }
}
