using System;
using System.IO;

namespace xPort5.Common
{
    /// <summary>
    /// Client-specific directory management utilities.
    /// Migrated from xPort5.DAL.Common.cs
    /// </summary>
    public static class Client
    {
        /// <summary>
        /// Gets or creates the inbox directory for a specific client.
        /// </summary>
        /// <param name="clientId">The client ID</param>
        /// <returns>Full path to the client's inbox directory, or empty string if creation fails</returns>
        public static string InBox(int clientId)
        {
            string result = String.Empty;
            string fullpath = Path.Combine(Config.InBox, clientId.ToString());

            try
            {
                if (!(Directory.Exists(fullpath)))
                {
                    Directory.CreateDirectory(fullpath);
                }
                result = fullpath;
            }
            catch { }

            return result;
        }

        /// <summary>
        /// Gets or creates the dropbox directory for a specific client.
        /// </summary>
        /// <param name="clientId">The client ID</param>
        /// <returns>Full path to the client's dropbox directory, or empty string if creation fails</returns>
        public static string DropBox(int clientId)
        {
            string result = String.Empty;
            string fullpath = Path.Combine(Config.DropBox, clientId.ToString());

            try
            {
                if (!(Directory.Exists(fullpath)))
                {
                    Directory.CreateDirectory(fullpath);
                }
                result = fullpath;
            }
            catch { }

            return result;
        }
    }
}
