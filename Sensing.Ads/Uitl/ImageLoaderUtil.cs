using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SensingAds.Uitl
{
    public class ImageLoaderUtil
    {
        public static void Load(string filePath, Image image)
        {
            var localPath = FileUtil.MapLocalPath(filePath);
            if (File.Exists(localPath))
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(localPath);
                    bitmap.EndInit();
                    image.Source = bitmap;
                }
                catch (Exception) { }
            }
            //todo download
        }

        public static void LoadBackground(string filePath, Panel panel)
        {
            var localPath = FileUtil.MapLocalPath(filePath);
            if (File.Exists(localPath))
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(localPath);
                    bitmap.EndInit();
                    panel.Background = new ImageBrush(bitmap);
                }
                catch (Exception) { }
            }
            //todo download
        }
    }
}
