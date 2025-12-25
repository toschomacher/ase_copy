using BOOSE;
using System;
using System.Data;
using System.Text.RegularExpressions;

namespace BOOSEappTV
{
    /// <summary>
    /// Represents a rectangle drawing command.
    /// </summary>
    /// <remarks>
    /// This command evaluates two integer expressions at runtime to determine
    /// the width and height of the rectangle, and then draws an unfilled
    /// rectangle starting at the current canvas cursor position.
    /// </remarks>
    internal class AppRect : CommandTwoParameters
    {
        /// <summary>
        /// Executes the rectangle drawing command.
        /// </summary>
        /// <remarks>
        /// The width and height expressions are evaluated at runtime.
        /// Both values must be positive integers.
        /// </remarks>
        /// <exception cref="CanvasException">
        /// Thrown when width or height is invalid or non-positive.
        /// </exception>
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
                throw new CanvasException(
                    $"{name} must be a valid integer expression."
                );
            }
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
