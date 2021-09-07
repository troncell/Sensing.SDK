using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SensingAds.ViewBanner
{
    public class Banner : Grid
    {
        public int Id { get; set; }
        public string Title { get; set; }
        protected int durtion;
        protected DateTime startTime;
        protected BannerState bannerState = BannerState.Unloaded;
        public string Transition { get; set; }

        public Banner()
        {
            this.Background = Brushes.Black;
        }
        protected void setDefaultDurtion(int durtion)
        {
            this.durtion = durtion;
        }

        public virtual void Play()
        {
            startTime = DateTime.Now;
        }

        public virtual void Replay()
        {
            startTime = DateTime.Now;
        }

        public virtual void Stop()
        {
            bannerState = BannerState.Finished;
        }

        public virtual void Pasue() { }

        public virtual void Resume() { }

        public virtual void Update()
        {
            if (bannerState == BannerState.Prepared)
            {
                bannerState = BannerState.Playing;
            }
            else if (bannerState == BannerState.Playing)
            {
                if (DateTime.Now > startTime.AddSeconds(durtion)) 
                {
                    bannerState = BannerState.Finished;
                }
            }
        }

        public BannerState getBannerState()
        {
            return bannerState;
        }

        protected async void ShowError(string message)
        {
            TextBlock tb = new TextBlock();
            tb.Text = message;
            tb.FontSize = 18;
            tb.Foreground = Brushes.White;
            tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            tb.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            this.Children.Add(tb);
            await Task.Delay(1000);
            bannerState = BannerState.Finished;
        }


        public void setClipPath(Geometry clipGeometry)
        {
            if(clipGeometry != null)
            {

                this.Clip = clipGeometry;
            }
            else
            {
                this.Clip = null;
            }
        }
    }
}
