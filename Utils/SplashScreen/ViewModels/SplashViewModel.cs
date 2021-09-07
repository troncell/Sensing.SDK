//-----------------------------------------------------------------------
// <copyright file="SplashViewModel.cs" company="troncell">
//     Copyright © troncell. All rights reserved.
// </copyright>
// <author>William Wu</author>
// <email>wulixu@troncell.com</email>
// <date>2012-10-24</date>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System;
using System.IO;

namespace SplashScreen.ViewModels
{
    using System.ComponentModel;
    using Events;
    using Prism.Events;
    public class SplashViewModel : INotifyPropertyChanged
    {
        #region Declarations

        private string _status;
        public string SplashFilePath { get; set; }
        public bool HasCustomerSplash { get; set; }

        #endregion

        #region ctor

        public SplashViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<MessageUpdateEvent>().Subscribe(e => UpdateMessage(e.Message));
            Status = "Loading Sensing Platform Services...";
            SplashFilePath = Path.Combine(Environment.CurrentDirectory, "Splash.png");
            HasCustomerSplash = File.Exists(SplashFilePath);
        }

        #endregion

        #region Public Properties

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                NotifyPropertyChanged("Status");
            }
        }

        #endregion

        #region Private Methods

        private void UpdateMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            Status = message;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}