using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SensingDownloader.Download.Common
{
    public static class PathHelper
    {
        public static string GetWithBackslash(string path)
        {
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                path += Path.DirectorySeparatorChar.ToString();
            }

            return path;
        }
    }
}
