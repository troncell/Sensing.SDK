//-----------------------------------------------------------------------
// <copyright file="FilePathHelper.cs" company="troncell">
//     Copyright © troncell. All rights reserved.
// </copyright>
// <author>William Wu</author>
// <email>wulixu@troncell.com</email>
// <date>2012-10-15</date>
// <summary>File Path Helper is used for combining paths together.</summary>
//-----------------------------------------------------------------------
using System;
using System.IO;

namespace SensingBase.Utils
{
    public static class FilePathHelper
    {

        public static string ProjectFullFolder { get; set; }

        #region Private Method
        private readonly static string FileBackSlash = @"\";

        /// <summary>
        /// Sample:
        ///     Params: Resource,Page
        ///     return: Resource\Page
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        private static string CombineDoublePath(string path1, string path2)
        {
            if (string.IsNullOrWhiteSpace(path1)) return path2;
            if (string.IsNullOrWhiteSpace(path2)) return path1;

            string strRear = path1.Substring(path1.Length - 1, 1);
            string str2Head = path2.Substring(0, 1);
            if (FileBackSlash == strRear || FileBackSlash == str2Head)
            {
                return path1 + path2;
            }
            else
            {
                return path1 + FileBackSlash + path2;
            }
        }
        #endregion Private

        /// <summary>
        /// Sample:
        ///     Params: Resource,Page,pageName,Controls,c1
        ///     return: Resource\Page\pageName\Controls\c1
        /// 
        /// </summary>
        /// <param name="path">
        /// 
        /// </param>
        public static string CombinePath(params string[] path)
        {
            if (path.Length == 0)
            {
                return string.Empty;
            }
            else if (path.Length == 1)
            {
                return CombineDoublePath(path[0], string.Empty);
            }
            else
            {
                string result = path[0];
                for (int i = 1; i < path.Length; i++)
                {
                    result = CombineDoublePath(result, path[i]);
                }
                return result;
            }
        }

        public static string GetFullPath(string uriKind,string relativePath,string basePath = null)
        {
            //todo:william SecurityElement.Escape
            //Need to excape invalid XML characters when the path contains invalid char(ie. \page\image&\).
            string fullFileName = null;
            if ("Relative".Equals(uriKind, StringComparison.CurrentCultureIgnoreCase)) fullFileName = CombinePath(basePath, relativePath);
            if ("Absolute".Equals(uriKind, StringComparison.CurrentCultureIgnoreCase)) fullFileName = relativePath;
            if ("Application".Equals(uriKind, StringComparison.CurrentCultureIgnoreCase)) fullFileName = CombinePath(Environment.CurrentDirectory, relativePath);
            if ("Project".Equals(uriKind, StringComparison.CurrentCultureIgnoreCase)) fullFileName = CombinePath(ProjectFullFolder, relativePath);
            return fullFileName;
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}