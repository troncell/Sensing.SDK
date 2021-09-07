
using SensingAds.Qrcode;

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SensingAds.CustomAds
{

    public class QrcodeBuilder : ViewBuilder
    {
        public string qrcodeType;
        public string qrcodeUrl;
        public int? activityId;
        public int? gameId;

        public override FrameworkElement Build()
        {
            var qrcodeImage = new Image();
            setCommonProperties(qrcodeImage);
            if (qrcodeType.Equals("normal"))
            {
                if (!string.IsNullOrEmpty(qrcodeUrl))
                {
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrcodeUrl, ECCLevel.Q);
                    XamlQRCode qrCode = new XamlQRCode(qrCodeData);
                    var qrCodeImage = qrCode.GetGraphic(5);
                    qrcodeImage.Source = qrCodeImage;
                }
            }
            else if (qrcodeType.Equals("activity"))
            {
                //if (activityId != null && gameId != null)
                //{
                //    GetActivityQrcodeFile(activityId.Value, gameId.Value).ContinueWith((t) =>
                //    {
                //        string qrcodeUrl = t.Result;
                //        if (qrcodeUrl != null)
                //        {
                //            ImageLoaderUtil.Load(qrcodeUrl, qrcodeImage);
                //        }
                //    }, TaskScheduler.FromCurrentSynchronizationContext());
                //}
            }
            return qrcodeImage;
        }

        //private async Task<string> GetActivityQrcodeFile(int activityId, int gameId)
        //{
        //    return await Task.Factory.StartNew(() =>
        //    {
        //        var device = StoreDatabase.GetInstance().GetDeviceInfo();
        //        ActivityRecord activityRecord = StoreDatabase.GetInstance().FindActivityRecord((t) => t.SubKey == device.SubKey && t.ActivityId == activityId && t.ActivityGameId == gameId, 1).FirstOrDefault();
        //        if (activityRecord == null)
        //        {
        //            var responseData = GameApi.GetInstance().GetActivityKey(device.SubKey, activityId, gameId).Result;
        //            string securityKey = responseData.Result;
        //            if (securityKey != null)
        //            {
        //                StoreDatabase.GetInstance().AddActivityRecord(device.SubKey, activityId, gameId, securityKey);
        //            }
        //        }

        //        bool qrcodeFileCached = activityRecord != null && activityRecord.QrCodeImage != null
        //                                && File.Exists(FileUtil.MapLocalPath(activityRecord.QrCodeImage))
        //                                && activityRecord.UpdateTime.AddDays(1) > DateTime.Now;
        //        if (qrcodeFileCached)
        //        {
        //            return activityRecord.QrCodeImage;
        //        }
        //        activityRecord = StoreDatabase.GetInstance().FindActivityRecord((t) => t.SubKey == device.SubKey && t.ActivityId == activityId && t.ActivityGameId == gameId, 1).FirstOrDefault();
        //        if (activityRecord != null)
        //        {
        //            var qrcodeResponseData = GameApi.GetInstance().PostPlayerData4ActionQrcode(0, "", "AfterGame", "WeChat", activityRecord.SecurityKey).Result;
        //            string qrcodeImage = qrcodeResponseData.Result?.QrCodeImage;
        //            if (qrcodeImage != null)
        //            {
        //                DownloadQrcode(qrcodeImage);
        //                SaveActivityQrcodeImage(activityRecord, qrcodeImage);
        //                return qrcodeImage;
        //            }
        //        }
        //        return null;
        //    });

        //}

        //private string DownloadQrcode(string qrcodeImage)
        //{
        //    try
        //    {
        //        WebClient webClient = new WebClient();
        //        string output = FileUtil.MapLocalPath(qrcodeImage);
        //        string tmp = output + ".downloading";
        //        webClient.DownloadFile(qrcodeImage, tmp);
        //        File.Move(tmp, output);
        //        return output;
        //    }
        //    catch (Exception ex)
        //    { }
        //    return null;
        //}

        //private void SaveActivityQrcodeImage(ActivityRecord activityRecord, string qrcodeImage)
        //{
        //    activityRecord.QrCodeImage = qrcodeImage;
        //    activityRecord.UpdateTime = DateTime.Now;
        //    StoreDatabase.GetInstance().SaveActivityRecord(activityRecord);
        //}
    }
}
//{
//        "show": true,
//        "lock": false,
//        "selected": true,
//        "type": "qrcode",
//        "width": 282,
//        "height": 282,
//        "left": 304,
//        "top": 150,
//        "rotate": 0,
//        "extensionData": "",
//        "zIndex": 100,
//        "transformScale": "",
//        "qrcodeType": "activity",
//        "activityId": 1,
//        "gameId": 1,
//        "gameList": [],
//        "displayName": "qrcode1"
//    }
