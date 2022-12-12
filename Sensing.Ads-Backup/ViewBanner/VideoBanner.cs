using SensingAds.Uitl;
using SensingAds.ViewBanner.Transitions;
using System;
using System.IO;
using System.Windows.Controls;

namespace SensingAds.ViewBanner
{
    public class VideoBanner : Banner
    {
        private MediaElement videoView;
        private String videoUrl;
        private bool isPrepared;

        public VideoBanner(String filePath, int defaultDurtion)
        {
            setDefaultDurtion(defaultDurtion);
            videoUrl = filePath;
            init(filePath);
        }

        private void init(String filePath)
        {
            videoView = new MediaElement();
            videoView.IsHitTestVisible = false;
            //videoView.Stretch = System.Windows.Media.Stretch.UniformToFill;
            videoView.Stretch = System.Windows.Media.Stretch.Uniform;
            videoView.LoadedBehavior = MediaState.Manual;
            videoView.MediaOpened += onPrepared;
            videoView.MediaEnded += onCompletion;
            videoView.MediaFailed += onError;
            this.Children.Add(videoView);
            bannerState = BannerState.Loaded;
        }


        public override void Play()
        {
            base.Play();
            try
            {
                videoView.Source = new Uri(FileUtil.MapLocalPath(videoUrl));
                videoView.Play();
            }
            catch (Exception ex)
            {
                ShowError($"打开失败:{Title}");
            }
        }

        public override void Replay()
        {
            base.Replay();
            bannerState = BannerState.Prepared;
            videoView.Position = TimeSpan.FromSeconds(0);
            videoView.Play();
        }

        public override void Stop()
        {
            base.Stop();
            videoView.Stop();
            isPrepared = false;
        }

        public override void Pasue()
        {
            base.Pasue();
            videoView.Pause();
        }

        public override void Resume()
        {
            base.Resume();
            videoView.Play();
        }


        public void onCompletion(object sender, System.Windows.RoutedEventArgs e)
        {
            bannerState = BannerState.Finished;
        }

        public void onPrepared(object sender, System.Windows.RoutedEventArgs e)
        {
            isPrepared = true;
            bannerState = BannerState.Prepared;
        }


        public void onError(object sender, System.Windows.ExceptionRoutedEventArgs e)
        {
            ShowError("文件打开失败");
        }



        //public override PageTransformer GetTransformer()
        //{
        //    return new NoneTransformer();
        //}
    }
}
