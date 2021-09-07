
using SensingAds.Uitl;
using System;
using System.Reflection;
using System.Windows.Controls;

namespace SensingAds.ViewBanner
{
    public class H5Banner : Banner
    {
        private string localPath;
        private WebBrowser web;

        public H5Banner(string filePath, int defaultDurtion)
        {
            setDefaultDurtion(defaultDurtion);
            init(filePath);
        }
        private void init(string filePath)
        {
            localPath = filePath;
            web = new WebBrowser();
            web.Loaded += (s, e) =>
            {
                HideScriptErrors(web, true);
                try
                {
                    web.Source = new System.Uri(localPath);
                }
                catch (Exception ex)
                {
                    ShowError($"打开失败:{Title}");
                }
            };
            this.Children.Add(web);
            bannerState = BannerState.Loaded;
        }

        public void HideScriptErrors(WebBrowser wb, bool hide)
        {
            var fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;
            var objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null)
            {
                return;
            }
            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
        }

        public override void Play()
        {
            base.Play();
            bannerState = BannerState.Prepared;

        }

        public override void Stop()
        {
            base.Stop();
            web.Source = null;
        }
    }
}
