using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SensingDownloader.Download
{
    public interface IProtocolProvider
    {
        // TODO: remove this method? Acoplamento ficara s?de um lado
        void Initialize(Downloader downloader);

        Stream CreateStream(ResourceLocation rl, long initialPosition, long endPosition);

        RemoteFileInfo GetFileInfo(ResourceLocation rl, out Stream stream);
    }
}
