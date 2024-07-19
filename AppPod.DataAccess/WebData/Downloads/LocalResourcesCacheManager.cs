using AppPod.Data.Model;
using AppPod.DataAccess.WebData.Downloads;
using LogService;
using SensingBase.Utils;
using SensingDownloader.Download;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppPod.Data.DataProviders
{
    #region MyDownloader
    public class LocalResourcesCacheManager : ResourceCacheManagerBase
    {
        private readonly IBizLogger logger = ServerLogFactory.GetLogger(typeof(LocalResourcesCacheManager));

        private Timer timer;

        private DownloadManager _downloadManager;

        private bool _isFristRun = true;

        SynchronizedCollection<FileCache> needToDownloadFiles;

        Dictionary<string, Downloader> fileDownloaderDict = null;

        public object lockObject = new object();

        static LocalResourcesCacheManager()
        {
            ProtocolProviderFactory.RegisterProtocolHandler("http", typeof(HttpProtocolProvider));
            ProtocolProviderFactory.RegisterProtocolHandler("https",typeof(HttpProtocolProvider));
        }

        public LocalResourcesCacheManager(string targetDir, string cacheDir, bool isCache, bool useAnotherCache = false)
        {
            _targetDir = targetDir;
            _isCache = isCache;
            _downloadRootDir = FilePathHelper.CombinePath(_targetDir, cacheDir);
            _useAnotherCache = useAnotherCache;
            if (useAnotherCache)
            {
                _tempDownloadDir = FilePathHelper.CombinePath(_targetDir, ".Cache", cacheDir);
            }
            else
            {
                if (Directory.Exists(FilePathHelper.CombinePath(_targetDir, ".Cache")))
                {
                    try
                    {
                        Directory.Delete(FilePathHelper.CombinePath(_targetDir, ".Cache"), true);
                    }
                    catch (Exception exception)
                    {
                        logger.Error("Cannot delete .cache folder.", exception);
                    }
                }
            }
            timer = new Timer(TimeClick, null, 10000, 30000);
            _downloadManager = new DownloadManager();
            _downloadManager.DownloadAdded += DownloadManager_DownloadAdded;
            _downloadManager.DownloadEnded += new EventHandler<DownloaderEventArgs>(OnDownloadEnded);
            _downloadManager.DownloadStarted += new EventHandler<DownloaderEventArgs>(Instance_DownloadStarted);
            _downloadManager.DownloadRemoved += Instance_DownloadRemoved;

            var dbfile = FilePathHelper.CombinePath(_targetDir, "Cache.db");
            var dbDirectory = FilePathHelper.CombinePath(_targetDir);
            if (!Directory.Exists(dbDirectory))
            {
                Directory.CreateDirectory(dbDirectory);
            }
            this.sqliteConnection = new SQLiteConnection(dbfile);
            this.sqliteConnection.CreateTable<FileCache>();
        }
        private void TimeClick(object state)
        {
            if(_downloadManager.Downloads.Count > 0)
            {
                _downloadManager.ClearEnded();
            }
            else
            {
                StartDownloads();
            }
        }

        private void DownloadManager_DownloadAdded(object sender, DownloaderEventArgs e)
        {
            var key = e.Downloader.KeyWords;
            if (!fileDownloaderDict.ContainsKey(key))
            {
                fileDownloaderDict.Add(key, e.Downloader);
                AddDownload(e.Downloader);
            }
        }

        private void AddDownload(Downloader d)
        {
            d.RestartingSegment += new EventHandler<SegmentEventArgs>(Download_RestartingSegment);
            d.SegmentStoped += new EventHandler<SegmentEventArgs>(Download_SegmentEnded);
            d.SegmentFailed += new EventHandler<SegmentEventArgs>(download_SegmentFailed);
            d.SegmentStarted += new EventHandler<SegmentEventArgs>(download_SegmentStarted);
            d.InfoReceived += new EventHandler(download_InfoReceived);
            d.SegmentStarting += new EventHandler<SegmentEventArgs>(Downloader_SegmentStarting);
            //d.StateChanged += D_StateChanged;

            string ext = Path.GetExtension(d.LocalFile);

            
        }

        private void D_StateChanged(object sender, EventArgs e)
        {
            var downloader = sender as Downloader;
            var file = needToDownloadFiles.First(f => f.Url == downloader.KeyWords);
            if(file != null)
            {
                file.DownloadStatus = downloader.State;
                file.ErrorMessage = downloader.LastError?.Message;
                UpdateFileCache(file);
            }
        }

        private static string GetResumeStr(Downloader d)
        {
            return (d.RemoteFileInfo != null && d.RemoteFileInfo.AcceptRanges ? "Yes" : "No");
        }

        private void Downloader_SegmentStarting(object sender, SegmentEventArgs e)
        {
        }

        private void download_InfoReceived(object sender, EventArgs e)
        {
        }

        private void download_SegmentStarted(object sender, SegmentEventArgs e)
        {
        }

        private void download_SegmentFailed(object sender, SegmentEventArgs e)
        {
        }

        private void Download_SegmentEnded(object sender, SegmentEventArgs e)
        {
        }

        private void Download_RestartingSegment(object sender, SegmentEventArgs e)
        {
        }

        private void Instance_DownloadRemoved(object sender, DownloaderEventArgs e)
        {
            var key = e.Downloader.KeyWords;
            var cacheFile = needToDownloadFiles.FirstOrDefault(f => f.Url == key);
            //needToDownloadFiles.Remove(cacheFile);
            UpdateFileCache(cacheFile);
            fileDownloaderDict.Remove(key);
            //if (_downloadManager.Downloads.Count == 0)
            //{
            //    if (IsAllCompleted)
            //    {
            //        if (!_swapped)
            //        {

            //            SwapResourceFolder();
            //            _swapped = true;
            //            logger.Debug("All is complete");
            //        }
            //    }
            //    else
            //    {
            //        //StartDownloadResources();
            //    }
            //}
        }

        void Instance_DownloadStarted(object sender, DownloaderEventArgs e)
        {
            var resource = FileCaches.FirstOrDefault(item => item.Url == e.Downloader.KeyWords);
            //resource.DownloadStatus = DownloadStatus.Downloading;
            //UpdateFileCache(resource);
        }

        private void OnDownloadEnded(object sender, DownloaderEventArgs e)
        {
            var key = e.Downloader.KeyWords;
            var resource = needToDownloadFiles.FirstOrDefault(item => item.Url == key);
            if (resource == null)
            {
                logger.Error("OnDownloadEnded with Error.");
                return;
            }
            resource.DownloadStatus = e.Downloader.State;
            if (e.Downloader.State == DownloaderState.Ended)
            {
                resource.LocalFile = resource.FileName;
                //if (IsCached(resource))
                //{
                //resource.LocalFile = resource.FileName;
                
                resource.CheckStatus = CheckStatus.Cached;
                resource.ErrorMessage = null;
                
                OnFileDownloaded(new FileDownloadedEventArgs(resource.OriginFileName, FilePathHelper.CombinePath(GetDownloadPath(), resource.LocalFile), DownloadedStatus.Successed));
                _downloadManager.RemoveDownload(e.Downloader);
                //}
                //else
                //{
                //resource.CheckStatus = CheckStatus.Added;
                //resource.DownloadStatus = DownloadStatus.Error;
                //}

            }
            else if (e.Downloader.State == DownloaderState.EndedWithError)
            {
                logger.Error($"Failed to download Url={key} with Error.");
                resource.ErrorMessage = e.Downloader.LastError.Message;
            }
            else
            {
                logger.Error($"Failed to download Url={key}");
            }

            if(_downloadManager.Downloads.Count == 0)
            {
                OnAllFilesDownloaded(new AllFilesDownloadedEventArgs());
            }
        }

        public override void StartDownloadResources()
        {
            Thread downloadThread = new Thread(StartDownloads);
            downloadThread.Start();
        }

        private void StartDownloads()
        {
            if (_isCache)
            {
                _swapped = false;
                var allFiles = GetAllUnDownloadedFiles();
                fileDownloaderDict = new Dictionary<string, Downloader>();
                needToDownloadFiles = new SynchronizedCollection<FileCache>(lockObject,allFiles.ToList());
                try
                {
                    _downloadManager.OnBeginAddBatchDownloads();
                    for (int index = 0; index < needToDownloadFiles.Count; index++)
                    {
                        var r = needToDownloadFiles[index];
                        var fileName = ExtractSchema(r.FileName);
                        var outPath = GetDownloadPath() + "/" + fileName;
                        var webLink = r.Url;
                        _downloadManager.Add(webLink, outPath, 1, true, webLink);
                    }
                }
                finally
                {
                    _downloadManager.OnEndAddBatchDownloads();
                }
            }
        }
    }

    public class FileDownloadedCompletedEventArgs:EventArgs
    {
        public string Uri { get; set; }
        public FileDownloadedCompletedEventArgs(string uri)
        {
            Uri = uri;
        }
    }
    #endregion

    #region WebClient
    public class DownloadItem
    {
        public string Path { get; set; }
        public string OutPath { get; set; }
        public int FailedCount { get; set; }
    }
    public class WebClientsDownloadManager
    {
        private readonly IBizLogger logger = ServerLogFactory.GetLogger(typeof(WebClientsDownloadManager));

        public event EventHandler<FileDownloadedCompletedEventArgs> FileDownloadCompleted;
        public event EventHandler<DownloadedCompletedEventArgs> FileAllCompleted;
        public event EventHandler<FileDownloadedCompletedEventArgs> FileDownloadFailed;
        //private List<DownloadItem> links = new List<DownloadItem>();

        private ImmutableList<DownloadItem> links = ImmutableList.Create<DownloadItem>();
        List<WebDownload> clients = new List<WebDownload>(8);
        public int MaxRetryCount = 1;

        public WebClientsDownloadManager()
        {
            clients.Add(new WebDownload(60000));
            clients.Add(new WebDownload(60000));
            clients.Add(new WebDownload(60000));
            clients.Add(new WebDownload(60000));
            //clients.Add(new WebDownload(60000));
            //clients.Add(new WebDownload(60000));
        }

        public void Add(DownloadItem link)
        {
            links = links.Add(link);
        }

        public void Remove(DownloadItem link)
        {
            links = links.Remove(link);
        }

        public async Task Start()
        {
            if(links != null)
            {
                while(links.Count > 0)
                {
                    var item = links[0];
                    
                    var client = clients.Where(web => !web.IsBusy).FirstOrDefault();
                    if (client != null && item != null)
                    {
                        
                        Task.Run(async () => {
                            Remove(item);
                            await DownloadResourceItem(client, item); });
                        Thread.Sleep(10);
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                    //if (successed)
                    //{
                    //    OnFileDownloadedCompleted(new FileDownloadedCompletedEventArgs(item.Path));
                    //}
                    //else
                    //{
                    //    OnFileDownloadedFailed(new FileDownloadedCompletedEventArgs(item.Path));
                    //}
                    //links.Remove(item);
                    
                } 
            }
        }

        

        private async Task DownloadResourceItem(WebDownload client,DownloadItem item)
        {
            try
            {
                
                var tmp = item.OutPath +  ".downloading";
                var uri = new FileInfo(tmp);
               
                if (!uri.Directory.Exists)
                {
                    uri.Directory.Create();
                }
                if(File.Exists(tmp))
                {
                    File.Delete(tmp);
                }
                //client.DownloadFileCompleted += Client_DownloadFileCompleted;
                await client.DownloadFileTaskAsync(item.Path, tmp);
                File.Move(tmp, item.OutPath);
                //links.Remove(item);
                OnFileDownloadedCompleted(new FileDownloadedCompletedEventArgs(item.Path));
                if(links.Count == 0)
                {
                    OnFileAllCompleted(new DownloadedCompletedEventArgs());
                }
            }
            catch(Exception exception)
            {
                item.FailedCount++;
                logger.Error($"Download Resource Failed with url={item.Path}", exception);
                if (item.FailedCount >= MaxRetryCount)
                {
                    OnFileDownloadedFailed(new FileDownloadedCompletedEventArgs(item.Path));
                }
                else
                {
                    //need to retry for download.
                    //links.Add(item);
                    Add(item);
                }
                
                if (links.Count == 0)
                {
                    OnFileAllCompleted(new DownloadedCompletedEventArgs());
                }
            }
        }

        //private async void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        //{
        //    links.Remove(currentItem);
        //    if (e.Error == null)
        //    {
        //        OnFileDownloadedCompleted(new FileDownloadedCompletedEventArgs(currentItem.Path));
        //    }
        //    else
        //    {
        //        OnFileDownloadedFailed(new FileDownloadedCompletedEventArgs(currentItem.Path));
        //    }
        //    await Start();
        //}


        protected void OnFileDownloadedCompleted(FileDownloadedCompletedEventArgs args)
        {
            FileDownloadCompleted(this, args);
        }

        protected void OnFileAllCompleted(DownloadedCompletedEventArgs args)
        {
            FileAllCompleted(this, args);
        }

        protected void OnFileDownloadedFailed(FileDownloadedCompletedEventArgs args)
        {
            FileDownloadFailed(this, args);
        }
    }

    public class WebDownload : WebClient
    {
        /// <summary>
        /// Time in milliseconds
        /// </summary>
        public int Timeout { get; set; }

        public WebDownload() : this(60000) { }

        public WebDownload(int timeout)
        {
            this.Timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = this.Timeout;
            }
            return request;
        }
    }

    public class WebClientLoalResourceCacheManager : ResourceCacheManagerBase
    {
        private readonly IBizLogger logger = ServerLogFactory.GetLogger(typeof(WebClientLoalResourceCacheManager));
        private WebClientsDownloadManager _downloadManager;
        public object lockObject = new object();

        static WebClientLoalResourceCacheManager()
        {
        }


        public WebClientLoalResourceCacheManager(string targetDir, string cacheDir, bool isCache, bool useAnotherCache = false)
        {
            _targetDir = targetDir;
            _isCache = isCache;
            _downloadRootDir = FilePathHelper.CombinePath(_targetDir, cacheDir);
            _useAnotherCache = useAnotherCache;
            if (useAnotherCache)
            {
                _tempDownloadDir = FilePathHelper.CombinePath(_targetDir, ".Cache", cacheDir);
            }
            else
            {
                if (Directory.Exists(FilePathHelper.CombinePath(_targetDir, ".Cache")))
                {
                    try
                    {
                        Directory.Delete(FilePathHelper.CombinePath(_targetDir, ".Cache"), true);
                    }
                    catch (Exception exception)
                    {
                        logger.Error("Cannot delete .cache folder.", exception);
                    }
                }
            }

            _downloadManager = new WebClientsDownloadManager();
            _downloadManager.FileDownloadCompleted += DownloadManager_FileDownloadCompleted;
            _downloadManager.FileAllCompleted += DownloadManager_FileAllCompleted;
            _downloadManager.FileDownloadFailed += DownloadManager_FileDownloadFailed;
            //_downloadManager.DownloadRemoved += Instance_DownloadRemoved;

            var dbfile = FilePathHelper.CombinePath(_targetDir, "Cache.db");
            var dbDirectory = FilePathHelper.CombinePath(_targetDir);
            if (!Directory.Exists(dbDirectory))
            {
                Directory.CreateDirectory(dbDirectory);
            }
            this.sqliteConnection = new SQLiteConnection(dbfile);
            this.sqliteConnection.CreateTable<FileCache>();
        }

        private void DownloadManager_FileDownloadFailed(object sender, FileDownloadedCompletedEventArgs e)
        {
            var resource = FileCaches.FirstOrDefault(item => item.Url == e.Uri);
            if (resource == null)
            {
                logger.Error("OnDownloadEnded with Error.");
                return;
            }

            resource.LocalFile = resource.FileName;
            resource.DownloadStatus = DownloaderState.NeedToPrepare;
            //resource.DownloadStatus = DownloadStatus.Error;
            resource.CheckStatus = CheckStatus.Added;
            UpdateFileCache(resource);
            OnFileDownloaded(new FileDownloadedEventArgs(resource.OriginFileName, FilePathHelper.CombinePath(GetDownloadPath(), resource.LocalFile), DownloadedStatus.Failed));
        }

        private void DownloadManager_FileAllCompleted(object sender, DownloadedCompletedEventArgs e)
        {
            OnAllFilesDownloaded(new AllFilesDownloadedEventArgs());
        }

        private void DownloadManager_FileDownloadCompleted(object sender, FileDownloadedCompletedEventArgs e)
        {
            var resource = FileCaches.FirstOrDefault(item => item.Url == e.Uri);
            if (resource == null)
            {
                logger.Error("OnDownloadEnded with Error.");
                return;
            }

            resource.LocalFile = resource.FileName;
            //if (IsCached(resource))
            //{
            //resource.LocalFile = resource.FileName;
            resource.DownloadStatus = DownloaderState.Ended;
            resource.CheckStatus = CheckStatus.Cached;
            UpdateFileCache(resource);
            OnFileDownloaded(new FileDownloadedEventArgs(resource.OriginFileName, FilePathHelper.CombinePath(GetDownloadPath(), resource.LocalFile), DownloadedStatus.Successed));
        }

        void Instance_DownloadStarted(object sender, DownloaderEventArgs e)
        {
            var resource = FileCaches.FirstOrDefault(item => item.Url == e.Downloader.KeyWords);
            //resource.DownloadStatus = DownloadStatus.Downloading;
            UpdateFileCache(resource);
        }
        public override void StartDownloadResources()
        {
            Task.Factory.StartNew(async() => await StartDownloads());
        }

        private async Task StartDownloads()
        {
            if (_isCache)
            {
                _swapped = false;
                var addedList = GetAllUnDownloadedFiles();
                try
                {
                    foreach (var r in addedList)
                    {
                        var fileName = ExtractSchema(r.FileName);
                        var outPath = GetDownloadPath() + "/" + fileName;
                        var webLink = r.Url;
                        //need to delete downloading file.
                        //if(File.Exists(outPath+".downloading"))
                        //{
                        //    File.Delete(outPath);
                        //}
                        _downloadManager.Add(new DownloadItem { Path = webLink, OutPath = outPath });
                    }
                }
                finally
                {

                }
                await _downloadManager.Start();
            }
        }
    }
    #endregion
}