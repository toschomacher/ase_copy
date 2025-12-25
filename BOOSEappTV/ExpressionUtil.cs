using System.Text.RegularExpressions;

namespace BOOSEappTV
{
    internal static class ExpressionUtil
    {
        /// <summary>
        /// Replicates BOOSE.Parser tidyExpression (but we can call it from anywhere).
        /// Turns "2*radius" into "2 * radius", collapses whitespace, trims.
        /// </summary>
        public static string Tidy(string exp)
        {
            if (string.IsNullOrWhiteSpace(exp)) return string.Empty;

            // Add spacing around arithmetic operators and parentheses
            exp = Regex.Replace(exp, @"([\+\-\*/\(\)])", " $1 ");
            // Collapse multiple whitespace
            exp = Regex.Replace(exp, @"\s+", " ").Trim();
            return exp;
        }

        public static bool IsOperatorToken(string token)
        {
            return token is "+" or "-" or "*" or "/" or "(" or ")";
        }
    }
}
