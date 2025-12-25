using BOOSE;
using System;
using System.Data;
using System.Text.RegularExpressions;

namespace BOOSEappTV
{
    /// <summary>
    /// Represents a star drawing command.
    /// </summary>
    /// <remarks>
    /// This command draws a star shape on the canvas using a specified size
    /// and fill mode. The size expression is evaluated at runtime, and the
    /// fill parameter must be a boolean literal or numeric equivalent.
    /// </remarks>
    public class AppStar : CommandTwoParameters
    {
        /// <summary>
        /// Executes the star drawing command.
        /// </summary>
        /// <remarks>
        /// This method evaluates the size and fill parameters at runtime and
        /// delegates the actual drawing to the <see cref="IAppCanvas"/> implementation.
        /// </remarks>
        /// <exception cref="CanvasException">
        /// Thrown when the canvas does not support star drawing, when the size
        /// is invalid, or when the fill parameter is not a valid boolean.
        /// </exception>
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

        /// <summary>
        /// Evaluates an integer expression at runtime.
        /// </summary>
        /// <param name="expr">The expression to evaluate.</param>
        /// <param name="name">
        /// The logical name of the parameter (used for error reporting).
        /// </param>
        /// <returns>The evaluated integer value.</returns>
        /// <exception cref="CanvasException">
        /// Thrown when the expression cannot be evaluated as an integer.
        /// </exception>
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
                throw new CanvasException(
                    $"{name} must be a valid integer expression."
                );
            }
        }

        /// <summary>
        /// Evaluates a boolean expression at runtime.
        /// </summary>
        /// <param name="expr">The expression to evaluate.</param>
        /// <param name="name">
        /// The logical name of the parameter (used for error reporting).
        /// </param>
        /// <returns>The evaluated boolean value.</returns>
        /// <exception cref="CanvasException">
        /// Thrown when the expression cannot be interpreted as a boolean.
        /// </exception>
        private bool EvaluateBoolExpression(string expr, string name)
        {
            expr = expr.Trim().ToLower();

            if (expr == "true" || expr == "1") return true;
            if (expr == "false" || expr == "0") return false;

            throw new CanvasException(
                $"{name} must be true/false or 1/0."
            );
        }

        /// <summary>
        /// Normalises a mathematical expression by inserting spacing
        /// around operators and parentheses.
        /// </summary>
        /// <param name="expr">The expression to tidy.</param>
        /// <returns>A normalised expression string.</returns>
        private string Tidy(string expr)
        {
            expr = Regex.Replace(expr, @"([+\-*/()])", " $1 ");
            return Regex.Replace(expr, @"\s+", " ").Trim();
        }

        /// <summary>
        /// Replaces variable names in an expression with their current values.
        /// </summary>
        /// <param name="expr">The expression containing variables.</param>
        /// <returns>An evaluable expression string.</returns>
        /// <exception cref="CanvasException">
        /// Thrown when an unknown variable is encountered.
        /// </exception>
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

                throw new CanvasException(
                    $"Unknown variable '{t}' in expression."
                );
            }

            return string.Join(" ", tokens);
        }
    }
}
