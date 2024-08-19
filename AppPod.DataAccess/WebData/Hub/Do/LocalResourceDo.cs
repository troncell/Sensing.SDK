using AppPod.Data.Model;

namespace AppPod.DataAccess.WebData.Store.Do
{
    public class LocalResourceDo
    {
        public string Url { get; set; }
        public string FileName { get; set; }
        public CheckStatus CheckStatus { get; set; }
        public DownloadStatus DownloadStatus { get; set; }
        public string LocalFile { get; set; }
        public string Extension { get; set; }
        public string Type { get; set; }
        public int RetryCount { get; set; }
        public string ErrorMessage { get; set; }
    }
}