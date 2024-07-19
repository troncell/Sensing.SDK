using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace SensingDownloader.Download.Extensions
{
    public interface IExtensionParameters
    {
        event PropertyChangedEventHandler ParameterChanged;
    }
}
