using BOOSE;
using System;
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

        // Fallback tidy if Program is not AppStoredProgram
        private static string LocalTidy(string expr)
        {
            if (expr == null) return "";

            // Normalise && and || first so we don't split them into '&' '&'
            expr = expr.Replace("&&", " && ").Replace("||", " || ");

            // Space around arithmetic + parentheses + !
            expr = Regex.Replace(expr, @"([+\-*/()!])", " $1 ");

            // Collapse whitespace
            expr = Regex.Replace(expr, @"\s+", " ").Trim();
            return expr;
        }

        private string Tidy(string expr)
        {
            if (Program is AppStoredProgram asp)
                return asp.TidyExpression(expr);

            return LocalTidy(expr);
        }

        private string ReplaceVariables(string exp)
        {
            var tokens = exp.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tokens.Length; i++)
            {
                string t = tokens[i];

                // Boolean literal words
                if (t.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                    t.Equals("false", StringComparison.OrdinalIgnoreCase))
                    continue;

                // Numeric literal
                if (double.TryParse(t, out _))
                    continue;

                // Operators / parentheses
                if (t is "+" or "-" or "*" or "/" or "(" or ")" or "&&" or "||" or "!")
                    continue;

                // Variable
                if (Program.VariableExists(t))
                {
                    var v = Program.GetVariable(t);

                    // For boolean evaluation, use 1/0, not "true"/"false"
                    if (v is AppBoolean ab)
                        tokens[i] = ab.Value ? "1" : "0";
                    else
                        tokens[i] = Program.GetVarValue(t);

                    continue;
                }

                throw new StoredProgramException("Invalid variable or expression.");
            }

            return string.Join(" ", tokens);
        }

        private bool EvaluateBoolean(string expr)
        {
            expr = expr.Trim();

            // strip outer spaces
            if (expr.Equals("true", StringComparison.OrdinalIgnoreCase) || expr == "1") return true;
            if (expr.Equals("false", StringComparison.OrdinalIgnoreCase) || expr == "0") return false;

            // OR has lowest precedence
            int orIndex = expr.IndexOf("||", StringComparison.Ordinal);
            if (orIndex >= 0)
            {
                string left = expr.Substring(0, orIndex).Trim();
                string right = expr.Substring(orIndex + 2).Trim();
                return EvaluateBoolean(left) || EvaluateBoolean(right);
            }

            // AND next
            int andIndex = expr.IndexOf("&&", StringComparison.Ordinal);
            if (andIndex >= 0)
            {
                string left = expr.Substring(0, andIndex).Trim();
                string right = expr.Substring(andIndex + 2).Trim();
                return EvaluateBoolean(left) && EvaluateBoolean(right);
            }

            // NOT highest
            if (expr.StartsWith("!"))
                return !EvaluateBoolean(expr.Substring(1).Trim());

            throw new StoredProgramException($"Invalid boolean expression '{expr}'.");
        }

        public override void Execute()
        {
            var variable = Program.GetVariable(varName);

            // BOOLEAN
            if (variable is AppBoolean boolVar)
            {
                string tidy = Tidy(expression);
                string evaluable = ReplaceVariables(tidy);

                bool value = EvaluateBoolean(evaluable);

                boolVar.Value = value;

                AppConsole.WriteLine($"[DEBUG] Assigned '{varName}' = {value}");
                return;
            }

            // NUMERIC (int/real)
            string tidyExpr = Tidy(expression);
            string numericExpr = ReplaceVariables(tidyExpr);

            object result;
            try
            {
                var table = new DataTable();
                result = table.Compute(numericExpr, "");
            }
            catch
            {
                throw new StoredProgramException("Invalid variable or expression.");
            }

            if (variable is AppInt)
            {
                int value = Convert.ToInt32(result);
                Program.UpdateVariable(varName, value);
                AppConsole.WriteLine($"[DEBUG] Assigned '{varName}' = {value}");
            }
            else if (variable is AppReal)
            {
                double value = Convert.ToDouble(result);
                Program.UpdateVariable(varName, value);
                AppConsole.WriteLine($"[DEBUG] Assigned '{varName}' = {value}");
            }
            else
            {
                throw new StoredProgramException($"Unsupported assignment target '{varName}'.");
            }
        }

        public override void CheckParameters(string[] parameter) { }
    }
}
