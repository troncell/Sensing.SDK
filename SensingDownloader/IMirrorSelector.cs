using System;
using System.Collections.Generic;
using System.Text;

namespace SensingDownloader.Download
{
    public interface IMirrorSelector
    {
        void Init(Downloader downloader);

        ResourceLocation GetNextResourceLocation(); 
    }
}
