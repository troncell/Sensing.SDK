//-----------------------------------------------------------------------
// <copyright file="Module.cs" company="troncell Tech">
//     Copyright © troncell Tech. All rights reserved.
// </copyright>
// <author>William Wu</author>
// <email>wulixu@troncelltech.com</email>
// <date>2012-10-25</date>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace SplashScreen
{
    using System;
    using System.Threading;
    using System.Windows.Threading;
    using Microsoft.Practices.Unity;
    using ViewModels;
    using Views;
    using Events;
    using Prism.Modularity;
    using Prism.Events;
    public class SplashModule : IModule
    {
        #region ctors
        public SplashModule(IUnityContainer container, IEventAggregator eventAggregator)
        {
            Container = container;
            EventAggregator = eventAggregator;
            //Shell = shell;
        }
        #endregion

        #region Private Properties
        private IUnityContainer Container { get; set; }

        private IEventAggregator EventAggregator { get; set; }

        private AutoResetEvent WaitForCreation { get; set; }
        #endregion

        public void Initialize()
        {

            //Dispatcher.CurrentDispatcher.BeginInvoke(
            //  (Action)(() =>
            //  {
            //      var jumpOut = false;
            //      while (!jumpOut)
            //      {
            //          if (Application.Current !=null && Application.Current.MainWindow != null)
            //          {
            //              jumpOut=true;
            //              Application.Current.MainWindow.Show();
            //              Application.Current.MainWindow.Activate();
            //              EventAggregator.GetEvent<CloseSplashEvent>().Publish(new CloseSplashEvent());
            //          }
            //          else
            //          {
            //              Dispatcher.Run();
            //          }
            //      }

            //  }));

            WaitForCreation = new AutoResetEvent(false);

            ThreadStart showSplash =
              () =>
              {
                  Dispatcher.CurrentDispatcher.BeginInvoke(
                    (Action)(() =>
                                {
                                    Container.RegisterType<SplashViewModel, SplashViewModel>();
                                    Container.RegisterType<SplashView, SplashView>();

                                    var splash = Container.Resolve<SplashView>();
                                    EventAggregator.GetEvent<CloseSplashEvent>().Subscribe(
                                      e =>
                                          splash.Dispatcher.BeginInvoke((Action)splash.Close),
                                      ThreadOption.PublisherThread, true);

                                    splash.Show();

                                    WaitForCreation.Set();
                                }));

                  Dispatcher.Run();
              };

            var thread = new Thread(showSplash) { Name = "Splash Thread", IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            

            WaitForCreation.WaitOne();
        }
    }
}