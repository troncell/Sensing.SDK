using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SensingAds.CustomAds
{
    public class H5Builder : ViewBuilder
    {
        public string resource;
        private WebBrowser web;
        public override FrameworkElement Build()
        {
            web = new WebBrowser();
            web.Loaded += (s, e) => {
                HideScriptErrors(web, true);
                if (!string.IsNullOrEmpty(resource))
                {
                    try
                    {
                        web.Source = new Uri(resource);
                    }
                    catch (Exception) { }
                }
            };
            setCommonProperties(web);

            return web;
        }

        public override void Stop()
        {
            base.Stop();
            web.Source = null;
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

    }
}
