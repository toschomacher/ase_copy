using BOOSE;
using System;
using System.Data;
using System.Text.RegularExpressions;

namespace BOOSEappTV
{
    /// <summary>
    /// Represents a runtime assignment command responsible for evaluating expressions
    /// and updating variable values during program execution.
    /// </summary>
    /// <remarks>
    /// This class implements BOOSE-correct deferred evaluation semantics.
    /// Expressions are captured at parse time and evaluated only at runtime
    /// using the current program state.
    /// </remarks>
    public class AppAssign : Command
    {
        /// <summary>
        /// The name of the variable being assigned to.
        /// </summary>
        private readonly string varName;

        /// <summary>
        /// The expression to be evaluated and assigned at runtime.
        /// </summary>
        private readonly string expression;

        /// <summary>
        /// Initialises a new instance of the <see cref="AppAssign"/> class.
        /// </summary>
        /// <param name="varName">The target variable name.</param>
        /// <param name="expression">The expression to evaluate at runtime.</param>
        public AppAssign(string varName, string expression)
        {
            this.varName = varName;
            this.expression = expression;
        }

        /// <summary>
        /// Associates this command with the current stored program.
        /// </summary>
        /// <param name="program">The active <see cref="StoredProgram"/> instance.</param>
        /// <param name="parameters">Unused parameter string.</param>
        public override void Set(StoredProgram program, string parameters)
        {
            Program = program;
        }

        /// <summary>
        /// Performs local expression normalisation when the program is not an
        /// <see cref="AppStoredProgram"/> instance.
        /// </summary>
        /// <param name="expr">The expression to tidy.</param>
        /// <returns>A normalised expression string.</returns>
        /// <remarks>
        /// This method ensures consistent spacing around operators and parentheses
        /// to support reliable tokenisation and evaluation.
        /// </remarks>
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

        /// <summary>
        /// Tidies the expression using the program-level implementation if available.
        /// </summary>
        /// <param name="expr">The expression to tidy.</param>
        /// <returns>A normalised expression string.</returns>
        private string Tidy(string expr)
        {
            if (Program is AppStoredProgram asp)
                return asp.TidyExpression(expr);

            return LocalTidy(expr);
        }

        /// <summary>
        /// Replaces variable references in the expression with their current runtime values.
        /// </summary>
        /// <param name="exp">The expression containing variable names.</param>
        /// <returns>An expression suitable for evaluation.</returns>
        /// <exception cref="StoredProgramException">
        /// Thrown when an invalid variable or token is encountered.
        /// </exception>
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

        /// <summary>
        /// Evaluates a boolean expression using recursive logical precedence.
        /// </summary>
        /// <param name="expr">The boolean expression to evaluate.</param>
        /// <returns>The evaluated boolean result.</returns>
        /// <exception cref="StoredProgramException">
        /// Thrown when the expression is invalid or unsupported.
        /// </exception>
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

        /// <summary>
        /// Executes the assignment by evaluating the stored expression
        /// and updating the target variable.
        /// </summary>
        /// <remarks>
        /// Evaluation is deferred until runtime in accordance with BOOSE’s
        /// two-pass execution model.
        /// </remarks>
        /// <exception cref="StoredProgramException">
        /// Thrown when evaluation fails or the target variable type is unsupported.
        /// </exception>
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

        /// <summary>
        /// Performs parameter validation.
        /// </summary>
        /// <param name="parameter">The parameter array.</param>
        /// <remarks>
        /// This implementation intentionally performs no validation,
        /// as parameters are handled during parsing.
        /// </remarks>
        public override void CheckParameters(string[] parameter) { }
    }
}
