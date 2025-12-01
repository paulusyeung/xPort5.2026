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

            // Handle SQL Server Unicode string literals (N'...') - strip N prefix and convert quotes
            linqExpression = Regex.Replace(linqExpression, @"\bN'([^']*)'", "\"$1\"", RegexOptions.IgnoreCase);
            
            // Replace single quotes with double quotes for string literals
            linqExpression = Regex.Replace(linqExpression, @"'([^']*)'", "\"$1\"");

            // Remove square brackets around column names (SQL Server identifier syntax)
            // Pattern: [ColumnName] -> ColumnName
            linqExpression = Regex.Replace(linqExpression, @"\[(\w+)\]", "$1");

            // Convert SQL SUBSTRING function to C# Substring method
            // Pattern: SUBSTRING(column, start, length) -> column.Substring(start - 1, length)
            // Note: SQL SUBSTRING is 1-based, C# Substring is 0-based, so we subtract 1 from start
            linqExpression = Regex.Replace(linqExpression,
                @"SUBSTRING\s*\(\s*(\w+)\s*,\s*(\d+)\s*,\s*(\d+)\s*\)",
                match =>
                {
                    string column = match.Groups[1].Value;
                    int start = int.Parse(match.Groups[2].Value);
                    int length = int.Parse(match.Groups[3].Value);
                    // SQL SUBSTRING is 1-based, C# Substring is 0-based
                    int csharpStart = start - 1;
                    return $"{column}.Substring({csharpStart}, {length})";
                },
                RegexOptions.IgnoreCase);

            // Convert SQL NOT BETWEEN ... AND to range exclusion (must be before BETWEEN to avoid conflicts)
            // Pattern: value NOT BETWEEN start AND end -> value < start || value > end
            // Matches expressions including method calls like ColumnName.Substring(0, 1)
            linqExpression = Regex.Replace(linqExpression,
                @"([\w.]+(?:\([^)]*\))?)\s+NOT\s+BETWEEN\s+([^\s]+)\s+AND\s+([^\s]+)",
                "$1 < $2 || $1 > $3",
                RegexOptions.IgnoreCase);

            // Convert SQL BETWEEN ... AND to range comparison
            // Pattern: value BETWEEN start AND end -> value >= start && value <= end
            // Matches expressions including method calls like ColumnName.Substring(0, 1)
            linqExpression = Regex.Replace(linqExpression,
                @"([\w.]+(?:\([^)]*\))?)\s+BETWEEN\s+([^\s]+)\s+AND\s+([^\s]+)",
                "$1 >= $2 && $1 <= $3",
                RegexOptions.IgnoreCase);

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
