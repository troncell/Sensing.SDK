//-----------------------------------------------------------------------
// <copyright file="SplashViewModel.cs" company="Cobon Tech">
//     Copyright © Cobon Tech. All rights reserved.
// </copyright>
// <author>William Wu</author>
// <email>wulixu@cobontech.com</email>
// <date>2012-10-24</date>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace SplashScreen.ViewModels
{
  using System;
  using System.ComponentModel;
  using Microsoft.Practices.Prism.Events;
    using SplashScreen.Events;

  public class SplashViewModel : INotifyPropertyChanged
  {
    #region Declarations

    private string _status;

    #endregion

    #region ctor
    public SplashViewModel(IEventAggregator eventAggregator)
    {
        eventAggregator.GetEvent<MessageUpdateEvent>().Subscribe(e => UpdateMessage(e.Message));
        Status = "Loading Sensing Platform Services...";
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