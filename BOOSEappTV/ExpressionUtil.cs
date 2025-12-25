using System.Text.RegularExpressions;

namespace BOOSEappTV
{
    /// <summary>
    /// Utility methods for normalising and analysing expressions.
    /// </summary>
    /// <remarks>
    /// This helper class centralises common expression-processing logic,
    /// allowing consistent behaviour across the parser, assignment handling,
    /// and runtime evaluation.
    /// </remarks>
    internal static class ExpressionUtil
    {
        /// <summary>
        /// Normalises an expression by inserting whitespace around operators
        /// and parentheses.
        /// </summary>
        /// <remarks>
        /// This method replicates the behaviour of BOOSE.Parser's
        /// <c>tidyExpression</c> method, but is exposed as a reusable utility.
        /// For example, it converts <c>2*radius</c> into <c>2 * radius</c>,
        /// collapses multiple whitespace characters, and trims the result.
        /// </remarks>
        /// <param name="exp">The raw expression string.</param>
        /// <returns>
        /// A normalised expression string suitable for tokenisation
        /// and evaluation.
        /// </returns>
        public static string Tidy(string exp)
        {
            if (string.IsNullOrWhiteSpace(exp)) return string.Empty;

            // Add spacing around arithmetic operators and parentheses
            exp = Regex.Replace(exp, @"([\+\-\*/\(\)])", " $1 ");
            // Collapse multiple whitespace
            exp = Regex.Replace(exp, @"\s+", " ").Trim();
            return exp;
        }

        /// <summary>
        /// Determines whether a token represents an arithmetic operator
        /// or a parenthesis.
        /// </summary>
        /// <param name="token">The token to test.</param>
        /// <returns>
        /// <c>true</c> if the token is an operator or parenthesis;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool IsOperatorToken(string token)
        {
            return token is "+" or "-" or "*" or "/" or "(" or ")";
        }
    }
}
