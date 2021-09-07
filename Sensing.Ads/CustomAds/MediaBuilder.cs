using Newtonsoft.Json;
using SensingAds.Uitl;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SensingAds.CustomAds
{
    public class MediaBuilder : ViewBuilder
    {
        public String resource;
        public bool autoplay;
        public bool loop;
        public bool muted;
        [JsonIgnore]
        private MediaElement videoView;

        public override FrameworkElement Build()
        {
            videoView = new MediaElement();
            setCommonProperties(videoView);
            if (!string.IsNullOrEmpty(resource))
            {
                videoView.Source = new Uri(FileUtil.MapLocalPath(resource));
            }
            videoView.LoadedBehavior = MediaState.Manual;
            videoView.MediaOpened += VideoView_MediaOpened;
            videoView.MediaEnded += VideoView_MediaEnded;
            videoView.MediaFailed += VideoView_MediaFailed;
            return videoView;
        }

        private void VideoView_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
        }

        private void VideoView_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (loop)
            {
                videoView.Position = TimeSpan.FromSeconds(0);
            }
        }

        private void VideoView_MediaOpened(object sender, RoutedEventArgs e)
        {
        }

        public override void Start()
        {
            base.Start();
            videoView.Play();
        }

        public override void Stop()
        {
            base.Stop();
            videoView.Stop();
        }

        public override IEnumerable<string> ExtractLinks()
        {
            yield return resource;

        }
    }
}
