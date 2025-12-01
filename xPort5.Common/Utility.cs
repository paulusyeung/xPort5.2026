using System;
using System.Text.RegularExpressions;

namespace xPort5.Common
{
    /// <summary>
    /// Utility methods for common validation tasks.
    /// Migrated from xPort5.DAL.Common.cs
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Validates if a string is a valid GUID.
        /// </summary>
        /// <param name="expression">The string to validate</param>
        /// <returns>True if the string is a valid GUID, false otherwise</returns>
        public static bool IsGUID(string expression)
        {
            var guidOutput = new Guid();
            bool isValid = Guid.TryParse(expression, out guidOutput);
            return isValid;
        }

        /// <summary>
        /// Validates if a string is a numeric value.
        /// Matches any unsigned or signed floating point number/numeric string.
        /// </summary>
        /// <param name="expression">The string to validate</param>
        /// <returns>True if the string is numeric, false otherwise</returns>
        public static bool IsNumeric(string expression)
        {
            if (expression != null)
            {
                Regex numericRegEx = new Regex(@"^-?\d+(\.\d+)?$");
                return numericRegEx.IsMatch(expression);
            }
            return false;
        }
    }
}
