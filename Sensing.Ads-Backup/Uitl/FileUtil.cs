using AppPod.DataAccess;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SensingAds.Uitl
{
    public class FileUtil
    {
        public static string Md5(string fileName)
        {
            if (fileName == null) return null;
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(fileName);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static string AppPodFolder()
        {
            var folder = AppDomain.CurrentDomain.BaseDirectory;
            var appPod = Path.Combine(folder, "AppPod");
            if (!Directory.Exists(appPod))
                Directory.CreateDirectory(appPod);
            return appPod;
        }

        public static string FilesFolder()
        {
            var filesFolder = Path.Combine(AppPodFolder(), "Files");
            if (!Directory.Exists(filesFolder))
                Directory.CreateDirectory(filesFolder);
            return filesFolder;
        }

        public static string[] GetLocalFiles()
        {
            return Directory.GetFiles(FilesFolder());
        }

        public static string[] GetLocalFolders()
        {
            return Directory.GetDirectories(FilesFolder());
        }


        public static string MapLocalPath(string url)
        {
            if(url.StartsWith("http"))
                return SensingDataAccess.GetAdsLocalFile(url);
            return url;
        }

        public static string MapExeLocalPath(string url)
        {
            var installDirectory = Path.Combine(Environment.CurrentDirectory, "AppPodData", "Apps", url);
            return installDirectory;
        }

        private static string FindExe(string folder, string guessExeName)
        {
            //只保留文件前面部分例如 someapp.10.1.2.exe
            guessExeName = guessExeName.Substring(0, guessExeName.IndexOf(".")) + ".exe";
            var exePath = Path.Combine(folder, guessExeName);
            if (!File.Exists(exePath))
            {
                exePath = Directory.GetFiles(folder, "*.exe").FirstOrDefault();
            }
            return exePath;
        }

        public static string MapZipExtractFolder(string url)
        {
            return Path.Combine(FilesFolder(), Path.GetFileNameWithoutExtension(url));
        }

        public static string SchedulePath()
        {
            return Path.Combine(AppPodFolder(), "Schedule.json");
        }

        public static void DeleteFolder(string folder)
        {
            if (!Directory.Exists(folder))
                return;
            var files =  Directory.GetFiles(folder);
            foreach (var item in files)
            {
                try
                {
                    File.Delete(item);
                }
                catch (Exception) { }
            }
            var subFolders = Directory.GetDirectories(folder);
            foreach (var item in subFolders)
            {
                DeleteFolder(item);
            }
            try
            {
                Directory.Delete(folder);
            }
            catch (Exception) { }
        }
    }
}
