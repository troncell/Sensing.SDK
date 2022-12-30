using System;
using System.Collections.Generic;
using System.Text;

namespace SensingDownloader.Download.Extensions
{
    public interface IExtension
    {
        string Name { get; }

        IUIExtension UIExtension { get; }
    }
}
