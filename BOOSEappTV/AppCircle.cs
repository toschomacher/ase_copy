using BOOSE;
using System;
using System.Text.RegularExpressions;

namespace BOOSEappTV
{
    /// <summary>
    /// Represents a circle drawing command that evaluates a radius expression
    /// at runtime and renders a circle on the canvas.
    /// </summary>
    /// <remarks>
    /// This command supports variable-based expressions for the radius.
    /// Expression evaluation is deferred until execution time in accordance
    /// with BOOSE’s two-pass execution model.
    /// </remarks>
    public class AppCircle : CommandOneParameter
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="AppCircle"/> class.
        /// </summary>
        public AppCircle() : base() { }

        /// <summary>
        /// Executes the circle command by evaluating the radius expression
        /// and drawing the circle on the canvas.
        /// </summary>
        /// <exception cref="StoredProgramException">
        /// Thrown when the radius expression cannot be evaluated.
        /// </exception>
        /// <exception cref="CanvasException">
        /// Thrown when the evaluated radius is invalid.
        /// </exception>
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

        /// <summary>
        /// Normalises an arithmetic expression by inserting spacing
        /// around operators and parentheses.
        /// </summary>
        /// <param name="expr">The expression to tidy.</param>
        /// <returns>A normalised expression string.</returns>
        private static string Tidy(string expr)
        {
            expr = Regex.Replace(expr, @"([+\-*/()])", " $1 ");
            return Regex.Replace(expr, @"\s+", " ").Trim();
        }

        /// <summary>
        /// Replaces variable names in the expression with their current runtime values.
        /// </summary>
        /// <param name="exp">The expression containing variables.</param>
        /// <returns>An evaluable expression string.</returns>
        /// <exception cref="CanvasException">
        /// Thrown when an invalid variable or token is encountered.
        /// </exception>
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

        /// <summary>
        /// Returns the name of the command.
        /// </summary>
        /// <returns>The string <c>"Circle"</c>.</returns>
        public override string ToString()
        {
            return "Circle";
        }
    }
}
