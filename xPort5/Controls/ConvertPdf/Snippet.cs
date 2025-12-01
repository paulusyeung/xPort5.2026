using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

using System.Web.Configuration;
using xPort5.EF6;
using xPort5.Common;

namespace xPort5.Controls.ConvertPdf
{
    class Snippet
    {
        /// <summary>
        /// Filter the unwanted symbols for file name.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SafeFileName(string source)
        {
            string result = source.Trim();

            foreach (char lDisallowed in System.IO.Path.GetInvalidFileNameChars())
            {
                result = result.Replace(lDisallowed.ToString(), "_");
            }
            foreach (char lDisallowed in System.IO.Path.GetInvalidPathChars())
            {
                result = result.Replace(lDisallowed.ToString(), "_");
            }
            result = result.Replace(" ", "_");          // 2009-01-13 paulus: GhostScript has trouble with spaces.

            return result;
        }

        /// <summary>
        /// Get the GhostScript working folder which should contain the GsWin32c.exe and Read/Wrtie access to ASP.NET
        /// Default = C:\DAM\WorkFolder.
        /// appSettings key="Gswin32_WorkFolder"
        /// </summary>
        /// <returns></returns>
        public static string GetGsWorkFolder()
        {
            return Common.Config.GsWorkFolder;
        }

        /// <summary>
        /// Save a stream to a physical file
        /// </summary>
        /// <param name="readStream">Source Stream</param>
        /// <param name="writeStream">Destination File</param>
        public static void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            readStream.Position = 0;
            int length = 256;
            Byte[] buffer = new Byte[length];

            int bytesRead = readStream.Read(buffer, 0, length);
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, length);
                bytesRead = readStream.Read(buffer, 0, length);
            }
        }

        public static string GetImageKey(string indexKey)
        {
            string result = String.Empty;
            if (indexKey.Substring(indexKey.Length - 3).ToLower() == "jpg")
            {
                result = indexKey.Substring(0, indexKey.Length - 3) + "pdf.jpg";
            }
            else
            {
                result = indexKey + ".jpg";
            }
            return result;
        }

        public static string StripFileSuffix(string filename)
        {
            string extension = String.Empty;

            if (filename.Length > 4)
            {
                extension = filename.Substring(filename.Length - 4, 4).ToLower();
            }
            switch (extension)
            {
                case ".pdf":
                case ".jpg":
                    filename = filename.Substring(0, filename.Length - 4);
                    break;
            }
            return filename;
        }
    }
}
