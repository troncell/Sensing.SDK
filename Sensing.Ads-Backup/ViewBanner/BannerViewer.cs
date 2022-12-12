using SensingAds.ViewBanner.Transitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace SensingAds.ViewBanner
{
    public class BannerViewer : Grid
    {
        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register("Offset", typeof(double), typeof(BannerViewer), new PropertyMetadata(0d, OnOffsetPropertyChangedCallback));

        public double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        public static void OnOffsetPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as BannerViewer;
            self.onAnimationUpdate((double)e.NewValue, (double)e.OldValue);
        }

        public event EventHandler<Banner> OnBannerPlay;

        private int playIndex;
        private Storyboard valueAnimator;
        private PageTransformer pageTransformer;
        private BannerViewerAdapter adapter;
        private bool animating;
        private Banner candidateBanner;
        private Grid root;
        private List<PageTransformer> pageTransformers = new List<PageTransformer>();
        private PageTransformer defaultPageTransform = new SlideUpTransformer();
        static int seed = 1;
        private Random rnd = new Random(seed++);
        private DispatcherTimer timer = new DispatcherTimer();
        

        public BannerViewer()
        {
            init();
        }

        public void SetAdapter(BannerViewerAdapter adapter)
        {
            this.adapter = adapter;
        }

        public bool IsAnimating()
        {
            return animating;
        }


        public BannerViewer AddPageTransfrom(PageTransformer pageTransformer)
        {
            pageTransformers.Add(pageTransformer);
            return this;
        }


        public void SetPageTransformer(PageTransformer pageTransformer)
        {
            this.defaultPageTransform = pageTransformer;
        }

        public Banner GetCurrentBanner()
        {
            return (Banner)root.Children[root.Children.Count - 1];
        }

        private void init()
        {
            root = new Grid();
            root.ClipToBounds = true;
            this.Children.Add(root);
            valueAnimator = new Storyboard();
            valueAnimator.FillBehavior = FillBehavior.Stop;
            DoubleAnimation da = new DoubleAnimation(0,1, TimeSpan.FromMilliseconds(1000));
            da.EasingFunction = new PowerEase
            {
                Power = 2,
                EasingMode = EasingMode.EaseOut
            };
            Storyboard.SetTarget(da, this);
            Storyboard.SetTargetProperty(da, new PropertyPath("Offset"));
            //valueAnimator.setInterpolator(new DecelerateInterpolator());
            valueAnimator.Children.Add(da);
            //valueAnimator.Completed += (s, e) => onAnimationEnd();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {

            if (root.Children.Count == 0 && adapter.Size() > 0)
            {
                PlayBanner(adapter.Get(playIndex));
            }
            else if(root.Children.Count > 0)
            {
                Banner banner = (Banner)root.Children[root.Children.Count - 1];
                BannerState bannerState = banner.getBannerState();
                if (bannerState == BannerState.Finished)
                {
                    if (adapter.Size() == 1)
                    {
                        banner.Replay();
                    }
                    else if(adapter.Size() > 1)
                    {
                        playIndex = (playIndex + 1) % adapter.Size();
                        //updateIndicator();
                        PlayBanner(adapter.Get(playIndex));
                    }
                }
                else if (bannerState == BannerState.Prepared)
                {
                    if (root.Children.Count > 1)
                    {
                        onAnimationStart();
                        valueAnimator.Begin();
                    }
                }
                banner.Update();
            }
            
        }

        private void PlayBanner(Banner banner)
        {
            if (root.Children.Count > 0)
            {
                banner.Opacity = 0;
                animating = true;
            }
            pageTransformer = pageTransformers.FirstOrDefault(t => t.GetName() == banner.Transition);
            if (pageTransformer == null)
            {
                pageTransformer = getRandomPageTransformer();
            }
            root.Children.Add(banner);
            banner.Play();
            OnBannerPlay?.Invoke(this, banner);
        }

        private PageTransformer getRandomPageTransformer()
        {
            if (pageTransformers.Count == 0)
                return defaultPageTransform;
            return pageTransformers[rnd.Next(pageTransformers.Count)];
            //return  pageTransformers.get((index++)%pageTransformers.size());
        }


        int index = 0;

        /***
         * 插播
         * @param banner
         */
        public void PlayBannerImmediately(Banner banner)
        {
            if (animating)
            {
                candidateBanner = banner;
                return;
            }
            Banner currentBanner = null;
            if (root.Children.Count > 0)
            {
                currentBanner = (Banner)root.Children[0];
            }
            if (currentBanner != banner)
            {
                currentBanner?.Stop();
                PlayBanner(banner);
                RestartTimer();
            }
            else
            {
                banner.Replay();
            }
        }

        private void RestartTimer()
        {
            timer.Stop();
            timer.Start();
        }

        private void StopTimer()
        {
            timer.Stop();
        }

        public void Play()
        {
            if (adapter.Size() > 0 && root.Children.Count == 0)
            {
                PlayBanner(adapter.Get(playIndex));
                UpdateIndicator();
            }
            RestartTimer();
        }

        private void UpdateIndicator()
        {

        }

        public void Stop()
        {
            StopTimer();
            var banners = root.Children.OfType<Banner>();
            foreach (var banner in banners)
            {
                banner.Stop();
            }
        }

        public void onAnimationUpdate( double fraction, double old)
        {
            if(fraction > 0)
            {
                FrameworkElement lastView = (FrameworkElement)root.Children[0];
                FrameworkElement currentView = (FrameworkElement)root.Children[1];
                pageTransformer.OnAnimationUpdate(lastView, -fraction);
                pageTransformer.OnAnimationUpdate(currentView, fraction);
            }
            else
            {
                onAnimationEnd();
            }

        }
        public void onAnimationEnd()
        {
            Banner last = (Banner)root.Children[0];
            Banner current = (Banner)root.Children[1];
            pageTransformer.OnAnimationEnd(last, -1);
            pageTransformer.OnAnimationEnd(current, 1);
            current.Resume();
            last.Stop();
            root.Children.Remove(last);
            animating = false;
            if (candidateBanner != null)
            {
                PlayBannerImmediately(candidateBanner);
                candidateBanner = null;
            }
        }

        public void onAnimationStart()
        {
            Banner last = (Banner)root.Children[0];
            last.Pasue();
            Banner current = (Banner)(Banner)root.Children[1];
            current.Pasue();
            pageTransformer.OnAnimationStart(last, -1);
            pageTransformer.OnAnimationStart(current, 1);
            animating = true;
        }
    };
}
