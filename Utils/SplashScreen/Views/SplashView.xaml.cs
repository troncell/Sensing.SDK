//-----------------------------------------------------------------------
// <copyright file="SplashView.xaml.cs" company="troncell">
//     Copyright © troncell. All rights reserved.
// </copyright>
// <author>William Wu</author>
// <email>wulixu@troncell.com</email>
// <date>2012-10-24</date>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Media.Imaging;

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
#if LOGO
            if (viewModel.HasCustomerSplash)
            {
                antuImage.Source = new BitmapImage(new Uri(viewModel.SplashFilePath, UriKind.Absolute));
            }
#endif

#if Main
            antuImage.Visibility = Visibility.Visible;
#else

#endif
            DataContext = viewModel;
        }
    }
}