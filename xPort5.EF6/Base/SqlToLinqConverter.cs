using System.Text.RegularExpressions;

namespace xPort5.EF6.Base
{
    /// <summary>
    /// Converts SQL WHERE clause syntax to LINQ Dynamic expression syntax
    /// for backward compatibility with legacy xPort5.DAL code
    /// </summary>
    public static class SqlToLinqConverter
    {
        public static string ConvertWhereClause(string sqlWhereClause)
        {
            if (string.IsNullOrEmpty(sqlWhereClause))
                return sqlWhereClause;

            string linqExpression = sqlWhereClause;

            // Replace single quotes with double quotes for string literals
            linqExpression = Regex.Replace(linqExpression, @"'([^']*)'", "\"$1\"");

            // Replace SQL operators with C# operators
            // Must replace = before == to avoid double replacement
            linqExpression = Regex.Replace(linqExpression, @"(?<![<>!])\s*=\s*(?!=)", " == ", RegexOptions.IgnoreCase);
            linqExpression = Regex.Replace(linqExpression, @"\s*<>\s*", " != ", RegexOptions.IgnoreCase);
            
            // Replace SQL LIKE with Contains/StartsWith/EndsWith
            // Pattern: FieldName LIKE "%value%" -> FieldName.Contains("value")
            linqExpression = Regex.Replace(linqExpression, 
                @"(\w+)\s+LIKE\s+""%(.*?)%""", 
                "$1.Contains(\"$2\")", 
                RegexOptions.IgnoreCase);
            
            // Pattern: FieldName LIKE "value%" -> FieldName.StartsWith("value")
            linqExpression = Regex.Replace(linqExpression, 
                @"(\w+)\s+LIKE\s+""(.*?)%""", 
                "$1.StartsWith(\"$2\")", 
                RegexOptions.IgnoreCase);
            
            // Pattern: FieldName LIKE "%value" -> FieldName.EndsWith("value")
            linqExpression = Regex.Replace(linqExpression, 
                @"(\w+)\s+LIKE\s+""%(.*)""", 
                "$1.EndsWith(\"$2\")", 
                RegexOptions.IgnoreCase);

            // Replace SQL AND/OR with C# &&/||
            linqExpression = Regex.Replace(linqExpression, @"\bAND\b", "&&", RegexOptions.IgnoreCase);
            linqExpression = Regex.Replace(linqExpression, @"\bOR\b", "||", RegexOptions.IgnoreCase);
            
            // Replace SQL NOT with C# !
            linqExpression = Regex.Replace(linqExpression, @"\bNOT\b", "!", RegexOptions.IgnoreCase);

            return linqExpression;
        }
    }
}
