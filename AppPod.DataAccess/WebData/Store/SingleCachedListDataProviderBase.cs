using AppPod.Data.DataProviders;
using AppPod.DataAccess.WebData.Downloads;
using AppPod.Setting;
using LogService;
using Newtonsoft.Json;
using Sensing.SDK;
using SensingBase.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPod.DataAccess.WebData.Store
{
    public abstract class SingleCachedListDataProviderBase<T>
    {
        protected readonly IBizLogger logger = ServerLogFactory.GetLogger(typeof(SingleCachedListDataProviderBase<T>));
        protected string _baseFolder;

        protected IResourceCacheManager _cacheManager;

        protected List<T> _webData = new List<T>();
        protected string _jsonFullPath;
        public bool IsWebFailed { get; set; } = true;

        private string _resourceFolder = "res";


        protected static object syncOjbect = new object();
        public int TotalDownloadCount { get; internal set; }
        public int DownloadedCount { get; internal set; }

        public int FailedCount { get; internal set; }

        protected DownloadAlgorithm _algorithm;
        protected ResourceUpdateTimeProvider _resourceUpdateProvider;


        public SingleCachedListDataProviderBase(string baseFolder, string jsonFileName, DownloadAlgorithm algorithm = DownloadAlgorithm.WebClient) //mydownloader.
        {
            _baseFolder = baseFolder;
            _algorithm = algorithm;
            _jsonFullPath = Path.Combine(baseFolder, jsonFileName);
        }

        public List<T> Data
        {
            get { return _webData; }
            set { _webData = value; }
        }



        public virtual async Task Initialize(ResourceUpdateTimeProvider resourceUpdateProvider)
        {
            try
            {
                _resourceUpdateProvider = resourceUpdateProvider;
                if (_algorithm == DownloadAlgorithm.WebClient)
                {
                    _cacheManager = new WebClientLoalResourceCacheManager(_baseFolder, "res", true);
                }
                if (_algorithm == DownloadAlgorithm.Mydownload)
                {
                    _cacheManager = new LocalResourcesCacheManager(_baseFolder, "res", true);
                }
                _cacheManager.FileDownloaded += CacheManager_FileDownloaded;
                _cacheManager.AllDownloaded += CacheManager_AllDownloaded;
                ClearDownloadingFiles();
                await InitData();
            }
            catch (Exception ex)
            {
                logger.Error("Init Data Error", ex);
            }
        }

        protected void ClearDownloadingFiles()
        {
            _cacheManager.RemoveDownloadFailedResources();
            var directory = new DirectoryInfo(FilePathHelper.CombinePath(_baseFolder, "res"));
            if (Directory.Exists(directory.FullName))
            {
                Parallel.ForEach(directory.GetFiles("*.downloading", SearchOption.AllDirectories), (path) => File.Delete(path.FullName));
            }
        }

        protected void CacheManager_AllDownloaded(object sender, AllFilesDownloadedEventArgs arg)
        {
            OnDownloadedCompleted(new AllFilesDownloadedEventArgs());
        }

        public event FileDownloadedHandler FileDownloaded;
        public event AllFilesDownloadedHandler DownloadedCompleted;


        protected void CacheManager_FileDownloaded(object sender, FileDownloadedEventArgs arg)
        {
            DownloadedCount++;
            if (arg.Status == DownloadedStatus.Failed)
            {
                FailedCount++;
            }
            Task.Run(() =>
            {
                OnFileDownloaded(arg);
            });
        }

        protected void OnFileDownloaded(FileDownloadedEventArgs args)
        {
            if (FileDownloaded != null)
            {
                FileDownloaded(this, args);
            }
        }

        protected void OnDownloadedCompleted(AllFilesDownloadedEventArgs args)
        {
            if (FileDownloaded != null)
            {
                DownloadedCompleted(this, args);
            }
        }
        /// <summary>
        /// Default is Cached
        /// </summary>
        protected virtual async Task InitData()
        {
            await AssembleData();
            TotalDownloadCount = _cacheManager.GetDownloadFileCount();
        }

        public void StartDownload()
        {
            _cacheManager.StartDownloadResources();
        }

        public bool ExistLocalData()
        {
            return File.Exists(_jsonFullPath);
        }
        public async Task AssembleData()
        {
            //IsLocalOnly = True, if Local Data is miss,need to load from web.
            if (IsLocalOnly() && !HasUpdate())
            {
                if (ExistLocalData())
                {
                    Data = ReadDataFromLocal();
                }
                else
                {
                    Data = await ReadDataFromWeb();
                }
            }
            //Read from web first, then from local.
            else
            {
                Data = await ReadDataFromWeb();
                if (IsWebFailed)
                {
                    if (ExistLocalData())
                    {
                        Data = ReadDataFromLocal();
                    }
                }
            }

            IList<DownloadLink> newResources = ExtractLinks(SensingWebClient.MainServiceHost);
            if (!IsWebFailed)
            {
                WriteDataToJsonFile(Data, _jsonFullPath);
                TimeUpdated();
            }
            if (newResources != null && newResources.Count > 0)
            {
                _cacheManager.AddResources(newResources);
            }
        }



        private void StartDownloads()
        {
            _cacheManager.StartDownloadResources();
        }

        public abstract bool IsLocalOnly();
        public abstract bool HasUpdate();
        public abstract void TimeUpdated();
        public abstract IList<DownloadLink> ExtractLinks(string baseUrl);

        public abstract Task<List<T>> ReadDataFromWeb();


        public List<T> ReadDataFromLocal()
        {
            var jsonFile = _jsonFullPath;
            if (File.Exists(jsonFile))
            {
                var jsondata = File.ReadAllText(jsonFile);
                return JsonConvert.DeserializeObject<List<T>>(jsondata);
            }
            return null;
        }

        private List<T> ReadDataFromJsonFile(string jsonFile)
        {
            try
            {
                if (File.Exists(jsonFile))
                {
                    return JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(jsonFile));
                }
            }
            catch (Exception e)
            {
                logger.Warning("Parse Json Faile!", e);
            }
            return null;
        }
        private void WriteDataToJsonFile(object data, string filePath)
        {
            if (data != null)
            {
                var jsonData = JsonConvert.SerializeObject(data);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    using (StreamWriter file = new StreamWriter(filePath, false))
                    {
                        file.WriteAsync(jsonData).Wait();
                    }
                }
            }
        }
    }
}
