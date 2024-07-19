using AppPod.Data.Model;
using LogService;
using SensingBase.Utils;
using SensingDownloader.Download;
using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AppPod.DataAccess.WebData.Downloads
{
    public abstract class ResourceCacheManagerBase : IResourceCacheManager
    {
        private readonly IBizLogger logger = ServerLogFactory.GetLogger(typeof(ResourceCacheManagerBase));

        public event FileDownloadedHandler FileDownloaded;
        public event AllFilesDownloadedHandler AllDownloaded;

        protected string _targetDir;
        protected string _downloadRootDir;
        protected string _tempDownloadDir;
        protected bool _useAnotherCache = false;
        protected bool _swapped = false;
        protected bool _isCache = true;
        protected bool _isFristRun = true;

        protected SQLiteConnection sqliteConnection;

        public int GetDownloadFileCount()
        {
            return GetAllUnDownloadedFiles().Count;
        }

        public virtual void AddResources(IList<DownloadLink> newLinks)
        {
            if (newLinks == null) return;
            //set checking tag
            var oldResources = FileCaches.ToList();
            var newResources = new List<FileCache>();

            //First, we tag the old resoruces with checking status.
            foreach (var item in oldResources)
            {
                //if downloading fail, need to delete local file.
                if (item.DownloadStatus != DownloaderState.Ended)
                {
                    var absoluteFile = Path.Combine(GetDownloadPath(), item.FileName);
                    if (File.Exists(absoluteFile))
                    {
                        File.Delete(absoluteFile);
                    }
                }
                item.CheckStatus = CheckStatus.Checking;
            }

            //diff resources
            foreach (var newLink in newLinks)
            {
                string relativename = newLink.RelativeFileName;
                if (relativename == null || relativename.Length <= 3)
                    continue;
                newLink.RelativeFileName = relativename.Trim();
                //var fullLink = $"{newLink.Host}/{relativename}";
                var fullLink = GetFullLinkUrl(newLink);
                var record = oldResources.Find(oldRes => oldRes.Url == fullLink);
                if (record == null)
                {
                    var extension = Path.GetExtension(newLink.RelativeFileName);
                    var newRecord = new FileCache
                    {
                        CheckStatus = CheckStatus.Added,
                        //DownloadStatus = DownloadStatus.InPlan,
                        Url = fullLink,
                        FileName = ExtractSchema(newLink.RelativeFileName),
                        Type = newLink.Type,
                        Extension = extension,
                        AddedTime = DateTime.Now,
                        OriginFileName = Path.GetFileName(newLink.RelativeFileName)
                    };
                    newResources.Add(newRecord);
                    //AddFileCache(newRecord);
                    logger.Debug("Insert newRecord" + newRecord);
                }
                else if (!IsCached(record))
                {
                    record.CheckStatus = CheckStatus.Added;
                }
                else
                {
                    record.CheckStatus = CheckStatus.Cached;
                }
            }
            UpdateFileCaches(oldResources);
            AddFileCaches(newResources);

            //TODO: Only first run deletes the old resources. To prevent delete the sources been used.
            if (_isFristRun)
            {
                _isFristRun = false;
                sqliteConnection.RunInTransaction(() =>
                {
                    var checkingResource = sqliteConnection.Table<FileCache>().Where(r => r.CheckStatus == CheckStatus.Checking).ToList();
                    foreach (var r in checkingResource)
                    {
                        var absoluteFile = FilePathHelper.CombinePath(GetDownloadPath(), r.LocalFile);
                        if (!File.Exists(absoluteFile))
                        {
                            r.CheckStatus = CheckStatus.Added;
                        }
                        else
                        {
                            r.CheckStatus = CheckStatus.Cached;
                        }
                        UpdateFileCache(r);
                    }
                });
            }

        }

        public abstract void StartDownloadResources();

        protected void OnFileDownloaded(FileDownloadedEventArgs args)
        {
            if (FileDownloaded != null)
            {
                FileDownloaded(this, args);
            }
        }

        protected void OnAllFilesDownloaded(AllFilesDownloadedEventArgs args)
        {
            if (AllDownloaded != null)
            {
                AllDownloaded(this, args);
            }
        }
        public string ExtractSchema(string fileName)
        {
            if (fileName == null) return null;
            if (fileName.Contains("&") || fileName.Contains("?"))
            {
                return ExtractSchemaMd5(fileName);
            }
            string fileNamePath = fileName;
            if (fileName.StartsWith("http", true, CultureInfo.CurrentCulture))
            {
                var uri = new Uri(fileName).LocalPath;
                fileNamePath = SanitizeFileName(uri);
            }
            return ChangeFileName(fileNamePath);
        }

        public static string ExtractSchemaMd5(string fileName)
        {
            if (fileName == null) return null;
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(fileName);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString() + ".jpg";
            }
        }

        public static string SanitizeFileName(string path)
        {
            int index = path.LastIndexOf("/");
            string filename = path.Substring(index + 1);
            string regex = string.Format(@"[{0}]+", Regex.Escape(new string(Path.GetInvalidFileNameChars())));
            filename = Regex.Replace(filename, regex, "_");
            return Path.Combine(path.Substring(0, index + 1), filename);
        }

        public static string ChangeFileName(string fileNamePath)
        {
            if (IsSS2File(fileNamePath))
            {
                var fileName = fileNamePath.Substring(0, fileNamePath.Length - 1 - 4);
                return fileName + ".jpg";
            }
            return fileNamePath;
        }

        public static bool IsSS2File(string f)
        {
            return f != null &&
                f.EndsWith(".SS2", StringComparison.Ordinal);
        }

        public static string GetFullLinkUrl(DownloadLink downloadLink)
        {
            if (downloadLink.RelativeFileName.StartsWith("http", true, CultureInfo.CurrentCulture))
            {
                return downloadLink.RelativeFileName;
            }
            var fileName = downloadLink.RelativeFileName.TrimStart('/', '\\');
            return $"{downloadLink.Host}{fileName}";
        }

        public string GetDownloadPath()
        {
            if (_useAnotherCache)
            {
                return _tempDownloadDir;
            }
            return _downloadRootDir;
        }

        public bool IsCached(FileCache record)
        {
            if (_useAnotherCache)
            {
                if (!File.Exists(FilePathHelper.CombinePath(_tempDownloadDir, record.LocalFile)))
                {
                    if (File.Exists(FilePathHelper.CombinePath(_tempDownloadDir, record.FileName)))
                    {
                        record.LocalFile = record.FileName;
                        record.DownloadStatus = DownloaderState.Ended;
                        record.CheckStatus = CheckStatus.Cached;
                        return true;
                    }
                    return false;
                }
                else
                {
                    record.DownloadStatus = DownloaderState.Ended;
                    record.CheckStatus = CheckStatus.Cached;
                    return true;
                }

            }
            else
            {

                if (!File.Exists(FilePathHelper.CombinePath(_downloadRootDir, record.LocalFile)))
                {
                    if (File.Exists(FilePathHelper.CombinePath(_downloadRootDir, record.FileName)))
                    {
                        record.LocalFile = record.FileName;
                        record.DownloadStatus = DownloaderState.Ended;
                        record.CheckStatus = CheckStatus.Cached;
                        return true;
                    }
                    return false;
                }
                else
                {
                    record.DownloadStatus = DownloaderState.Ended;
                    record.CheckStatus = CheckStatus.Cached;
                    return true;
                }

                //return File.Exists(FilePathHelper.CombinePath(_downloadRootDir, record.LocalFile));
            }
        }

        public void SwapResourceFolder()
        {
            //删除不再需要的文件
            var resources = GetAllFiles();
            var deleteResource = new List<FileCache>();
            foreach (var item in resources)
            {
                if (item.DownloadStatus != DownloaderState.Ended)
                {
                    var absoluteFile = FilePathHelper.CombinePath(GetDownloadPath(), item.LocalFile);
                    if (File.Exists(absoluteFile))
                    {
                        File.Delete(absoluteFile);
                    }
                    deleteResource.Add(item);
                }
            }

            UpdateFileCaches(resources);
            sqliteConnection.RunInTransaction(() =>
            {
                foreach (var r in deleteResource)
                {
                    sqliteConnection.Delete(r);
                }
            });

            if (_useAnotherCache)
            {
                MoveResources();
            }
            logger.Debug("Download Finished!");
        }

        private void MoveResources()
        {
            //下载目录拷贝到工作目录
            MoveFiles(_tempDownloadDir, _downloadRootDir);
        }


        public void MoveFiles(string sourceDir, string targetDir)
        {
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }
            DirectoryInfo sourceDirectory = new DirectoryInfo(sourceDir);
            foreach (var file in sourceDirectory.GetFiles())
            {
                File.Copy(file.FullName, FilePathHelper.CombinePath(targetDir, file.Name), true);
            }
            foreach (var folder in sourceDirectory.GetDirectories())
            {
                MoveFiles(folder.FullName, FilePathHelper.CombinePath(targetDir, folder.Name));
            }
        }

        private void SyncFileCaches()
        {
            //for (int index = 0; index < needToDownloadFiles.Count; index++)
            //{
            //    var file = needToDownloadFiles[index];
            //    fileDownloaderDict.TryGetValue(file.Url, out Downloader downloader);
            //    if(downloader != null)
            //    {
            //        file.DownloadStatus = downloader.State;
            //    }
            //}
        }

        #region DB
        public TableQuery<FileCache> FileCaches { get { return sqliteConnection.Table<FileCache>(); } }

        public List<FileCache> GetAllUnDownloadedFiles()
        {
            var addedList = sqliteConnection.Table<FileCache>().Where(item => item.DownloadStatus != DownloaderState.Ended).ToList();
            return addedList;
        }

        public bool IsAllCompleted
        {
            get
            {
                var unComplted = sqliteConnection.Table<FileCache>().Where(r => r.DownloadStatus != DownloaderState.Ended).ToList();
                //check if all resources is downloaded
                return unComplted.Count() == 0;
            }
        }
        public List<FileCache> GetAllFiles()
        {
            return sqliteConnection.Table<FileCache>().ToList();
        }
        public bool AddFileCaches(IEnumerable<FileCache> files)
        {
            var ret = sqliteConnection.InsertAll(files);
            return ret == 0;
        }

        public bool AddFileCache(FileCache file)
        {
            var ret = sqliteConnection.Insert(file);
            return ret == 0;
        }

        public bool UpdateFileCache(FileCache file)
        {
            var ret = sqliteConnection.Update(file);
            return ret == 0;
        }
        public bool UpdateFileCaches(IEnumerable<FileCache> files)
        {
            var ret = sqliteConnection.UpdateAll(files);
            return ret == 0;
        }

        public void RemoveDownloadFailedResources()
        {
            var unComplted = sqliteConnection.Table<FileCache>().Where(r => r.DownloadStatus == DownloaderState.EndedWithError).ToList();
            unComplted.ForEach(a => sqliteConnection.Delete(a));
        }
        #endregion
    }
}
