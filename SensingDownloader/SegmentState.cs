using System;
using System.Collections.Generic;
using System.Text;

namespace SensingDownloader.Download
{
    public enum SegmentState
    {
        Idle,
        Connecting,
        Downloading,
        Paused,
        Finished,
        Error,
    }
}
