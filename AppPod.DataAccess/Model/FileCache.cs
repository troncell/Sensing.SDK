using SensingDownloader.Download;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPod.Data.Model
{
    public class FileCache
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Url { get; set; }

        public string FileName { get; set; }

        public CheckStatus CheckStatus { get; set; }

        public DownloaderState DownloadStatus { get; set; }

        public string LocalFile { get; set; }

        public string Extension { get; set; }

        public string OriginFileName { get; set; }

        public string Type { get; set; }

        public int RetryCount { get; set; }

        public string ErrorMessage { get; set; }

        public DateTime? AddedTime { get; set; }
    }

    public enum CheckStatus
    {
        Checking,
        Cached,
        Added,
        Deleted
    }

    public enum DownloadStatus
    {
        InPlan,
        Downloading,
        Completed,
        Error
    }

    public enum ResourceType
    {
        Apps,
        Products,
        Ads
    }
}
