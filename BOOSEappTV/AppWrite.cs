using BOOSE;
using System;
using System.Data;
using static System.Net.Mime.MediaTypeNames;

namespace BOOSEappTV
{
    /// <summary>
    /// Implements the <c>write</c> command for outputting values and expressions.
    /// </summary>
    /// <remarks>
    /// This command evaluates its expression at runtime and outputs the result
    /// both to the application console and to the canvas text renderer.
    /// It supports:
    /// <list type="bullet">
    /// <item><description>Quoted string literals</description></item>
    /// <item><description>Variables (int, real, boolean)</description></item>
    /// <item><description>Numeric expressions</description></item>
    /// <item><description>Simple string concatenation using <c>+</c></description></item>
    /// </list>
    /// Evaluation is performed strictly at runtime in accordance with
    /// BOOSE’s two-pass execution model.
    /// </remarks>
    internal class AppWrite : Evaluation
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="AppWrite"/> class.
        /// </summary>
        public AppWrite() : base() { }

        /// <summary>
        /// Parses and stores the expression to be written.
        /// </summary>
        /// <param name="program">The active <see cref="StoredProgram"/> instance.</param>
        /// <param name="parameters">The expression to output.</param>
        public override void Set(StoredProgram program, string parameters)
        {
            base.Set(program, parameters);
            Program = program;

            if (!string.IsNullOrWhiteSpace(parameters))
                Expression = parameters.Trim();
        }

        /// <summary>
        /// Performs compile-time processing for the <c>write</c> command.
        /// </summary>
        /// <remarks>
        /// No compile-time work is required; evaluation occurs at runtime.
        /// </remarks>
        public override void Compile()
        {
            // No compile-time work
        }

        /// <summary>
        /// Executes the <c>write</c> command at runtime.
        /// </summary>
        /// <remarks>
        /// The expression is evaluated according to its form and the result
        /// is written to both the application console and the canvas.
        /// </remarks>
        /// <exception cref="EvaluationException">
        /// Thrown when the expression is missing or cannot be evaluated.
        /// </exception>
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

        /// <summary>
        /// Replaces variable names in an expression with their current values.
        /// </summary>
        /// <param name="exp">The expression containing variables.</param>
        /// <returns>An evaluable expression string.</returns>
        /// <exception cref="EvaluationException">
        /// Thrown when an unknown variable is encountered.
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

                if (!Program.VariableExists(t))
                    throw new EvaluationException($"Unknown variable '{t}'");

                tokens[i] = Program.GetVarValue(t);
            }

            return string.Join(" ", tokens);
        }
    }
}
