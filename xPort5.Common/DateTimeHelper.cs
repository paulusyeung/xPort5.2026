using System;
using Gizmox.WebGUI.Forms;

namespace xPort5.Common
{
    /// <summary>
    /// Helper methods for DateTime formatting with culture-specific formats.
    /// Migrated from xPort5.DAL.Common.cs
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// Convert the datetime value to string with time or without.
        /// If the value is equaled to 1900-01-01, it would return an empty value.
        /// </summary>
        /// <param name="value">DateTime value to convert</param>
        /// <param name="withTime">Include time in the output</param>
        /// <returns>Formatted date/datetime string</returns>
        public static string DateTimeToString(DateTime value, bool withTime)
        {
            string formatString = GetDateFormat();
            if (withTime)
            {
                formatString = GetDateTimeFormat();
            }

            if (!value.Equals(new DateTime(1900, 1, 1)))
            {
                return value.ToString(formatString);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Convert the datetime string to formatted string with time or without.
        /// If the value is equaled to 1900-01-01, it would return an empty value.
        /// </summary>
        /// <param name="value">String value to convert</param>
        /// <param name="withTime">Include time in the output</param>
        /// <returns>Formatted date/datetime string</returns>
        public static string DateTimeToString(string value, bool withTime)
        {
            string result = String.Empty;
            string formatString = GetDateFormat();
            if (withTime)
            {
                formatString = GetDateTimeFormat();
            }
            try
            {
                DateTime source = DateTime.Parse(value);
                if (!source.Equals(new DateTime(1900, 1, 1)))
                {
                    result = source.ToString(formatString);
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// Get the date format string based on current UI culture.
        /// </summary>
        /// <returns>Date format string</returns>
        public static string GetDateFormat()
        {
            string result = String.Empty;

            switch (VWGContext.Current.CurrentUICulture.ToString())
            {
                case "zh-CHS":
                    result = "yyyy-MM-dd";
                    break;
                case "zh-CHT":
                    result = "dd/MM/yyyy";
                    break;
                case "en-US":
                default:
                    result = "dd/MM/yyyy";
                    break;
            }

            return result;
        }

        /// <summary>
        /// Get the datetime format string based on current UI culture.
        /// </summary>
        /// <returns>DateTime format string</returns>
        public static string GetDateTimeFormat()
        {
            string result = String.Empty;

            switch (VWGContext.Current.CurrentUICulture.ToString())
            {
                case "zh-CHS":
                    result = "yyyy-MM-dd HH:mm";
                    break;
                case "zh-CHT":
                    result = "dd/MM/yyyy HH:mm";
                    break;
                case "en-US":
                default:
                    result = "dd/MM/yyyy HH:mm";
                    break;
            }

            return result;
        }
    }
}
