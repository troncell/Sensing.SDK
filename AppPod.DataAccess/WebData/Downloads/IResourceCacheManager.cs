using AppPod.Data.Model;
using AppPod.DataAccess.WebData.Store;
using LogService;
using SensingBase.Utils;
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
    public delegate void FileDownloadedHandler(object sender, FileDownloadedEventArgs arg);

    public delegate void AllFilesDownloadedHandler(object sender, AllFilesDownloadedEventArgs arg);

    public interface IResourceCacheManager
    {
        void RemoveDownloadFailedResources();
        void AddResources(IList<DownloadLink> newLinks);
        void StartDownloadResources();

        event FileDownloadedHandler FileDownloaded;
        event AllFilesDownloadedHandler AllDownloaded;

        int GetDownloadFileCount();
    }

    

    public enum DownloadedStatus
    {
        Successed,
        Failed
    }
    public class FileDownloadedEventArgs : EventArgs
    {
        public string FileName { get; internal set; }
        public string FullPath { get; internal set; }
        public DownloadedStatus Status { get; internal set; } = DownloadedStatus.Successed;
        public FileDownloadedEventArgs(string fileName, string fullPath, DownloadedStatus status)
        {
            FileName = fileName;
            FullPath = fullPath;
            Status = status;
        }
    }

    public class AllFilesDownloadedEventArgs : EventArgs
    {
        public AllFilesDownloadedEventArgs()
        {

        }
    }
}