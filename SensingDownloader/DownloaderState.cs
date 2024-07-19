using System;
using System.Collections.Generic;
using System.Text;

namespace SensingDownloader.Download
{
    public enum DownloaderState 
    {
        NeedToPrepare = 0,
        Preparing,
        WaitingForReconnect,
        Prepared,
        Working,
        Pausing,
        Paused,
        Ended,
        EndedWithError
    }
}
