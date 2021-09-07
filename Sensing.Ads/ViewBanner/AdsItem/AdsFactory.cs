
using Sensing.SDK.Contract;
using System.IO;

namespace SensingAds.ViewBanner.AdsItem
{
    public class AdsFactory
    {
        public static BaseAd CreateAd(AdsSdkModel ad)
        {
            string fileType = ad.Type.ToLower();
            int durtion = (int)(ad.TimeSpan?.TotalSeconds ?? 0);
            if(ad.AuditStatus == "Offline")
            {
                return new OfflineAd { FileUrl = ad.FileUrl, Id = ad.Id, Name = ad.Name, TimeSpan = durtion };
            }
            else if (ad.IsCustom && !string.IsNullOrEmpty(ad.CustomContent))
            {
                return CustomAd.Parse(ad.CustomContent, ad.Id, ad.Name, durtion);
            }
            else if (fileType == "image")
            {
                return new ImageAd { FileUrl = ad.FileUrl, Id = ad.Id, Name = ad.Name, TimeSpan = durtion };
            }
            else if (fileType == "video")
            {
                return new VideoAd { FileUrl = ad.FileUrl, Id = ad.Id, Name = ad.Name, TimeSpan = durtion };
            }
            else if (fileType == "web")
            {
                return new H5Ad { FileUrl = ad.FileUrl, Id = ad.Id, DownloadMode = DownloadMode.Self, Name = ad.Name, TimeSpan = durtion };
            }
            else if (fileType == "pdf")
            {
                return new PdfAd { FileUrl = ad.FileUrl, Id = ad.Id, Name = ad.Name, TimeSpan = durtion };
            }
            else if (fileType == "windows")
            {
                return new WindowAppAd { FileUrl = ad.FileUrl, Id = ad.Id, Name = ad.Name, TimeSpan = durtion };
            }
            else if (fileType == "other")
            {
                return new H5Ad { FileUrl = ad.FileUrl, Id = ad.Id, DownloadMode = DownloadMode.None, Name = ad.Name, TimeSpan = durtion };
            }
            else
            {
                return new NotSupportAd { FileUrl = ad.FileUrl, Id = ad.Id, Name = ad.Name, TimeSpan = durtion };
            }
        }

        public static BaseAd CreateAd(string path)
        {
            string fileType = "image";
            if(path.EndsWith(".mp4") || path.EndsWith(".wmv"))
            {
                fileType = "video";
            }
            string filename = Path.GetFileName(path);
            int durtion = 0;
            int id = path.GetHashCode();
            if (fileType == "image")
            {
                return new ImageAd { FileUrl = path, Id = id, Name = filename, TimeSpan = durtion };
            }
            else if (fileType == "video")
            {
                return new VideoAd { FileUrl = path, Id = id, Name = filename, TimeSpan = 3600 };
            }
            else if (fileType == "web")
            {
                return new H5Ad { FileUrl = path, Id = id, DownloadMode = DownloadMode.Self, Name = filename, TimeSpan = durtion };
            }
            else if (fileType == "pdf")
            {
                return new PdfAd { FileUrl = path, Id = id, Name = filename, TimeSpan = durtion };
            }
            else if (fileType == "windows")
            {
                return new WindowAppAd { FileUrl = path, Id = id, Name = filename, TimeSpan = durtion };
            }
            else if (fileType == "other")
            {
                return new H5Ad { FileUrl = path, Id = id, DownloadMode = DownloadMode.None, Name = filename, TimeSpan = durtion };
            }
            else
            {
                return new NotSupportAd { FileUrl = path, Id = id, Name = filename, TimeSpan = durtion };
            }
        }

    }
}
