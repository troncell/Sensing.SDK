using System;
using System.Collections.Generic;
using System.Text;

namespace AppPod.DataAccess.WebData.Downloads
{
    public delegate void DownloadedCompletedHandler(object sender, DownloadedCompletedEventArgs arg);
    public delegate void DownloadedProgressChangedHandler(object sender, ProgressChangedEventArgs arg);

    public class DownloadedCompletedEventArgs
    {
        //public ResourceType Type { private set; get; }
        //public DownloadedCompletedEventArgs(ResourceType type)
        //{
        //    Type = type;
        //}
        public DownloadedCompletedEventArgs()
        {
            //Type = type;
        }
    }

    public class ProgressChangedEventArgs
    {
        public int Total { get; set; }
        public int FinishCount { get; set; }
        public int FailedCount { get; set; }
        public ProgressChangedEventArgs(int total, int finished, int failed)
        {
            Total = total;
            FinishCount = finished;
            FailedCount = failed;
        }
    }
}
