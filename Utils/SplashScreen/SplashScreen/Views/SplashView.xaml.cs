//-----------------------------------------------------------------------
// <copyright file="SplashView.xaml.cs" company="Cobon Tech">
//     Copyright © Cobon Tech. All rights reserved.
// </copyright>
// <author>William Wu</author>
// <email>wulixu@cobontech.com</email>
// <date>2012-10-24</date>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace SplashScreen.Views
{
  using ViewModels;

  /// <summary>
  /// SplashView.xaml 的交互逻辑
  /// </summary>
  public partial class SplashView
  {
    public SplashView(SplashViewModel viewModel)
    {
      InitializeComponent();

      DataContext = viewModel;
    }
  }
}