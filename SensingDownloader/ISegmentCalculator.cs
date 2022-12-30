using System;
using System.Collections.Generic;
using System.Text;

namespace SensingDownloader.Download
{
    public interface ISegmentCalculator
    {
        CalculatedSegment[] GetSegments(int segmentCount, RemoteFileInfo fileSize);
    }
}
