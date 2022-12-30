using AppPod.Data.DataProviders;
using AppPod.Data.Model;
using AppPod.DataAccess.WebData.Downloads;
using AppPod.Setting;
using LogService;
using Newtonsoft.Json;
using Sensing.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPod.DataAccess.WebData.Store
{
    public abstract class WebApiCacheDataProvider<T> where T : class
    {
        protected readonly IBizLogger logger = ServerLogFactory.GetLogger(typeof(WebApiCacheDataProvider<T>));

        protected IResourceCacheManager _cacheManager;
        protected T _webData;
        protected string _webJsonData;
        protected string _jsonDataFullPath;

        protected string _baseFolder;

        protected DownloadAlgorithm _algorithm = DownloadAlgorithm.WebClient;
        public int TotalDownloadCount { get; internal set; }
        public int DownloadedCount { get; internal set; }

        protected static object syncOjbect = new object();


        public event FileDownloadedHandler FileDownloaded;
        public event AllFilesDownloadedHandler DownloadedCompleted;

        private Settings _settings;

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


        public WebApiCacheDataProvider()
        {
        }
        public T Data
        {
            get { return _webData; }
            set { _webData = value; }
        }

        public bool IsWebFailed { get; set; } = true;
        public virtual void Initialize()
        {
            try
            {
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

                InitData();
            }
            catch (Exception ex)
            {
                logger.Error("Init Data Error", ex);
            }
        }

        private void CacheManager_AllDownloaded(object sender, AllFilesDownloadedEventArgs arg)
        {
            OnDownloadedCompleted(new AllFilesDownloadedEventArgs());
        }

        private void CacheManager_FileDownloaded(object sender, FileDownloadedEventArgs arg)
        {
            DownloadedCount++;
            Task.Run(() =>
            {
                OnFileDownloaded(arg);
            });
        }



        //public virtual void ReloadData(ReloadDataEventArgs args)
        //{
        //    lock (syncOjbect)
        //    {
        //        AssembleWebData();
        //    }
        //}
        /// <summary>
        /// Default is Cached
        /// </summary>
        private async Task InitData()
        {
            await AssembleWebData();
        }

        public async Task AssembleWebData()
        {
            //IsLocalOnly = True, if Local Data is miss,need to load from web.
            if (_settings.Apps.IsLocalOnly)
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

            IList<DownloadLink> newResources = ExtractLinks(null);
            if (!IsWebFailed)
            {
                WriteDataToJsonFile(Data, _jsonDataFullPath);
                _cacheManager.AddResources(newResources);
            }
        }


        public bool ExistLocalData()
        {
            return File.Exists(_jsonDataFullPath);
        }

        private void StartDownloads()
        {
            _cacheManager.StartDownloadResources();
        }
        public virtual IList<DownloadLink> ExtractLinks(string baseUrl)
        {
            return null;
        }


        public virtual async Task<T> ReadDataFromWeb()
        {
            return await Task.FromResult(default(T));
        }

        public virtual T ReadDataFromLocal()
        {
            var jsonFile = _jsonDataFullPath;
            if (File.Exists(jsonFile))
            {
                var jsondata = File.ReadAllText(jsonFile);
                return JsonConvert.DeserializeObject<T>(jsondata);
            }
            return default;
        }

        private T ReadDataFromFile(string jsonFile)
        {
            try
            {
                if (File.Exists(jsonFile))
                {
                    return JsonConvert.DeserializeObject<T>(File.ReadAllText(jsonFile));
                }
            }
            catch (Exception e)
            {
                logger.Warning("Parse Json Faile!", e);
            }
            return default;
        }

        private T ReadDataFromJson(string jsonData)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonData);
            }
            catch (Exception e)
            {
                logger.Warning("Parse Json Faile!", e);
            }
            return default;
        }

        private void WriteDataToJsonFile(object data, string filePath)
        {
            //if (!File.Exists(filePath))
            //{
            //    Directory.CreateDirectory(_dbDataSourceModel.CacheFullFolder);
            //}

            if (data != null)
            {
                var jsonData = JsonConvert.SerializeObject(data);
                using (StreamWriter file = new StreamWriter(filePath, false))
                {
                    file.WriteAsync(jsonData).Wait();
                }
            }
        }

    }
}
