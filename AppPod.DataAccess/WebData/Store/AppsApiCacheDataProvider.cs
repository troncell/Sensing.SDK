using AppPod.Data.DataProviders;
using AppPod.Data.Model;
using AppPod.DataAccess.WebData.Downloads;
using AppPod.Setting;
using LogService;
using Newtonsoft.Json;
using Sensing.SDK;
using Sensing.SDK.Contract;
using SensingBase.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPod.DataAccess.WebData.Store
{
    public class AppsApiCacheDataProvider
    {
        protected readonly IBizLogger logger = ServerLogFactory.GetLogger(typeof(AppsApiCacheDataProvider));

        private SensingWebClient _webClient;
        private string _appBaseFolder;
        protected IResourceCacheManager _cacheManager;

        public int FailedCount { get; internal set; }

        private List<DeviceSoftwareSdkModel> _webAppData = new List<DeviceSoftwareSdkModel>();
        private string _appJsonFullPath;
        private string _webAppsJsonData;

        private string _appLocalFullPath;
        public bool IsAppWebFailed { get; set; } = true;
        private string _resourceFolder = "res";

        private Settings _settings;

        private static object syncOjbect = new object();
        public int TotalDownloadCount { get; internal set; }
        public int DownloadedCount { get; internal set; }

        private DownloadAlgorithm _algorithm;

        public List<AppInfo> LocalApps = new List<AppInfo>();
        private ResourceUpdateTimeProvider _resourceUpdateProvider;

        public AppsApiCacheDataProvider(SensingWebClient webClient, string productBaseFolder, Settings settings, DownloadAlgorithm algorithm = DownloadAlgorithm.WebClient) //mydownloader.
        {
            _webClient = webClient;
            _appBaseFolder = productBaseFolder;
            _algorithm = algorithm;
            _appJsonFullPath = Path.Combine(productBaseFolder, "Apps.json");
            _appLocalFullPath = Path.Combine(productBaseFolder, "LocalApps.json");
            _settings = settings;
        }

        public List<DeviceSoftwareSdkModel> AppsData
        {
            get { return _webAppData; }
            set { _webAppData = value; }
        }


        public async Task Initialize(ResourceUpdateTimeProvider resourceUpdateProvider)
        {
            try
            {
                _resourceUpdateProvider = resourceUpdateProvider;

                if (_algorithm == DownloadAlgorithm.WebClient)
                {
                    _cacheManager = new WebClientLoalResourceCacheManager(_appBaseFolder, _resourceFolder, true);
                }
                if (_algorithm == DownloadAlgorithm.Mydownload)
                {
                    _cacheManager = new LocalResourcesCacheManager(_appBaseFolder, _resourceFolder, true);
                }
                _cacheManager.FileDownloaded += CacheManager_FileDownloaded;
                _cacheManager.AllDownloaded += CacheManager_AllDownloaded;
                ClearDownloadingFiles();
                await InitData();
                TotalDownloadCount = _cacheManager.GetDownloadFileCount();
            }
            catch (Exception ex)
            {
                logger.Error("Init Data Error", ex);
            }
        }

        private void ClearDownloadingFiles()
        {
            _cacheManager.RemoveDownloadFailedResources();
            var directory = new DirectoryInfo(FilePathHelper.CombinePath(_appBaseFolder, "res"));
            if (Directory.Exists(directory.FullName))
            {
                Parallel.ForEach(directory.GetFiles("*.downloading", SearchOption.AllDirectories), (path) => File.Delete(path.FullName));
            }
        }
        private void CacheManager_AllDownloaded(object sender, AllFilesDownloadedEventArgs arg)
        {
            OnDownloadedCompleted(new AllFilesDownloadedEventArgs());
        }

        public event FileDownloadedHandler FileDownloaded;
        public event AllFilesDownloadedHandler DownloadedCompleted;
        private void CacheManager_FileDownloaded(object sender, FileDownloadedEventArgs arg)
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
        private async Task InitData()
        {
            await AssembleProductData();
        }
        public void StartDownload()
        {
            _cacheManager.StartDownloadResources();
        }

        public bool ExistLocalData()
        {
            return File.Exists(_appJsonFullPath);
        }
        public async Task AssembleProductData()
        {
            //IsLocalOnly = True, if Local Data is miss,need to load from web.
            if (_settings.Apps.IsLocalOnly || !_resourceUpdateProvider.AppsHasUpdate)
            {
                if (ExistLocalData())
                {
                    AppsData = ReadAppsDataFromLocal();
                }
                else
                {
                    AppsData = await ReadAppsDataFromWeb();
                }
            }
            //Read from web first, then from local.
            else
            {
                AppsData = await ReadAppsDataFromWeb();
                if (IsAppWebFailed)
                {
                    if (ExistLocalData())
                    {
                        AppsData = ReadAppsDataFromLocal();
                    }
                }
            }

            IList<DownloadLink> newResources = ExtractAppsLinks(SensingWebClient.MainServiceHost);
            if (!IsAppWebFailed)
            {
                WriteDataToJsonFile(AppsData, _appJsonFullPath);
                _resourceUpdateProvider.AppsUpdated();
            }
            _cacheManager.AddResources(newResources);
        }

        private void StartDownloads()
        {
            _cacheManager.StartDownloadResources();
        }
        public virtual IList<DownloadLink> ExtractAppsLinks(string baseUrl)
        {
            if (AppsData == null) return null;
            List<DownloadLink> links = new List<DownloadLink>();
            foreach (var item in AppsData)
            {
                if (item == null) continue;
                if (item.MaterialPacketUrl != null)
                {
                    if (!links.Any(link => link.RelativeFileName == item.MaterialPacketUrl))
                    {
                        links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = item.MaterialPacketUrl, Type = "apps" });
                    }
                }
                if (item.Software != null)
                {
                    if (!string.IsNullOrEmpty(item.Software.PackageUrl))
                    {
                        if (!links.Any(link => link.RelativeFileName == item.Software.PackageUrl))
                        {
                            links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = item.Software.PackageUrl, Type = "apps" });
                        }
                    }

                    if (!string.IsNullOrEmpty(item.Software.LogoUrl))
                    {
                        if (!links.Any(link => link.RelativeFileName == item.Software.LogoUrl))
                        {
                            links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = item.Software.LogoUrl, Type = "apps" });
                        }
                    }

                    if (!string.IsNullOrEmpty(item.Software.LargeImageUrl))
                    {
                        if (!links.Any(link => link.RelativeFileName == item.Software.LargeImageUrl))
                        {
                            links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = item.Software.LargeImageUrl, Type = "apps" });
                        }
                    }
                }

                if (item.TenantAppSetting != null)
                {
                    if (!string.IsNullOrEmpty(item.TenantAppSetting.MaterialPacketUrl))
                    {
                        if (!links.Any(link => link.RelativeFileName == item.TenantAppSetting.MaterialPacketUrl))
                        {
                            links.Add(new DownloadLink { Host = baseUrl, RelativeFileName = item.TenantAppSetting.MaterialPacketUrl, Type = "apps" });
                        }
                    }
                }
            }

            TotalDownloadCount = links.Count;
            logger.Debug($"Ads Download Source Total is {TotalDownloadCount}");
            return links;
        }

        public async Task<List<DeviceSoftwareSdkModel>> ReadAppsDataFromWeb()
        {

            var data = await _webClient.GetApps();
            if (data == null)
            {
                IsAppWebFailed = true;
                return null;
            }
            IsAppWebFailed = false;
            var tempData = data.Items;

            _webAppsJsonData = JsonConvert.SerializeObject(tempData);
            return tempData as List<DeviceSoftwareSdkModel>;
        }

        public List<DeviceSoftwareSdkModel> ReadAppsDataFromLocal()
        {
            var jsonFile = _appJsonFullPath;
            if (File.Exists(jsonFile))
            {
                var jsondata = File.ReadAllText(jsonFile);
                return JsonConvert.DeserializeObject<List<DeviceSoftwareSdkModel>>(jsondata);
            }
            return null;
        }

        private List<DeviceSoftwareSdkModel> ReadDataFromJsonFile(string jsonFile)
        {
            try
            {
                if (File.Exists(jsonFile))
                {
                    return JsonConvert.DeserializeObject<List<DeviceSoftwareSdkModel>>(File.ReadAllText(jsonFile));
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
                try
                {
                    if (!string.IsNullOrEmpty(jsonData))
                    {
                        using (StreamWriter file = new StreamWriter(filePath, false))
                        {
                            file.WriteAsync(jsonData).Wait();
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("WriteDataToJsonFile error with exception =", ex);
                }
            }
        }


        #region App Install

        public void CompareApps()
        {
            var localdata = ReadAppsInfoFromLocal(_appLocalFullPath);
            if (localdata != null)
            {
                LocalApps = localdata;
            }
            if (LocalApps == null) LocalApps = new List<AppInfo>();

            if (AppsData != null)
            {
                foreach (var app in AppsData)
                {
                    var sid = app.Id;
                    var existApp = LocalApps.Find(a => a.Id == sid);
                    if (existApp != null)
                    {
                        existApp.SoftwareId = app.Software.Id;
                        //existApp.Package = app.Software.PackageUrl;
                        existApp.IsDefault = app.IsDefault;
                        //existApp.Code = app.Software.Code;
                        existApp.ExePath = app.Software.ExePath;
                        //
                        existApp.Logo = ToLocalPath(app.Software.LogoUrl);
                        existApp.Name = app.Software.Name;
                        existApp.Alias = app.Alias ?? app.TenantAppSetting.Alias;
                        if (existApp.Package != app.Software.PackageUrl
                            || existApp.Code != app.Software.Code
                            || existApp.MaterialPackage != app.MaterialPacketUrl
                            || existApp.TenantMaterialPackage != app.TenantAppSetting?.MaterialPacketUrl)
                        {
                            existApp.Package = app.Software.PackageUrl;
                            existApp.Code = app.Software.Code;
                            existApp.MaterialPackage = app.MaterialPacketUrl;
                            existApp.TenantMaterialPackage = app.TenantAppSetting.MaterialPacketUrl;
                            if (existApp.Status == AppStatus.Installed)
                            {
                                existApp.Status = AppStatus.NeedToUpdate;
                            }
                            else
                            {
                                existApp.Status = AppStatus.NotInstalled;
                            }
                        }
                    }
                    else
                    {
                        LocalApps.Add(new AppInfo
                        {
                            Id = app.Id,
                            Name = app.Software.Name,
                            Alias = app.Alias ?? app.TenantAppSetting.Alias,
                            MaterialPackage = app.MaterialPacketUrl,
                            Logo = ToLocalPath(app.Software.LogoUrl),
                            Code = app.Software.Code,
                            SoftwareId = app.Software.Id,
                            ExePath = app.Software.ExePath,
                            Package = app.Software.PackageUrl,
                            TenantMaterialPackage = app.TenantAppSetting?.MaterialPacketUrl,
                            Status = AppStatus.NotInstalled,
                            IsDefault = app.IsDefault
                        });
                    }
                }

                for (int index = LocalApps.Count - 1; index >= 0; index--)
                {
                    var app = LocalApps[index];
                    if (!AppsData.Any(a => a.Id == app.Id))
                    {
                        LocalApps.RemoveAt(index);
                        //todo:william. need to delete all the files.
                    }
                }
            }

            WriteDataToJsonFile(LocalApps, _appLocalFullPath);
        }

        private string ToLocalPath(string logoUrl)
        {
            if (string.IsNullOrEmpty(logoUrl)) return null;
            if (logoUrl.ToLower().StartsWith("http"))
            {
                var filePath = ExtractSchema(logoUrl);
                filePath = filePath.TrimStart('/', '\\');
                var path = Path.Combine(_appBaseFolder, _resourceFolder, filePath);
                return path;
            }
            return Path.Combine(_appBaseFolder, _resourceFolder, logoUrl);
        }

        public static string ExtractSchema(string fileName)
        {
            if (fileName == null) return null;
            string fileNamePath = fileName;
            if (fileName.StartsWith("http", true, CultureInfo.CurrentCulture))
            {
                var uri = new Uri(fileName).LocalPath;
                fileNamePath = uri;
            }
            return fileNamePath;
        }

        public void InstalledApp(long appId)
        {
            var app = LocalApps.Find(a => a.Id == appId);
            if (app != null) app.Status = AppStatus.Installed;
            WriteDataToJsonFile(LocalApps, _appLocalFullPath);
        }
        private List<AppInfo> ReadAppsInfoFromLocal(string appInfosPath)
        {
            try
            {
                if (File.Exists(appInfosPath))
                {
                    return JsonConvert.DeserializeObject<List<AppInfo>>(File.ReadAllText(appInfosPath));
                }
            }
            catch (Exception e)
            {
                logger.Warning("Parse Json Faile!", e);
            }
            return null;
        }

        #endregion
    }
}
