using SensingAds.Uitl;
using SensingAds.ViewBanner.Transitions;
using System;
using System.Diagnostics;
using System.IO;

namespace SensingAds.ViewBanner
{
    public class ExeBanner : Banner
    {
        private String fileUrl;
        private bool isPrepared;
        private Process process;

        public ExeBanner(String filePath, int defaultDurtion)
        {
            setDefaultDurtion(defaultDurtion);
            init(filePath);
        }

        private void init(String filePath)
        {
            bannerState = BannerState.Loaded;
            this.fileUrl = filePath;
        }


        public override void Play()
        {
            base.Play();
            var appFile = FileUtil.MapExeLocalPath(fileUrl);
            if (File.Exists(appFile))
            {
                try
                {
                    FileInfo info = new FileInfo(appFile);
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = info.Name;
                    startInfo.WorkingDirectory = info.DirectoryName;
                    process = Process.Start(startInfo);
                    
                    bannerState = BannerState.Prepared;
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
            else
            {
                ShowError("执行文件不存在");
            }
        }

        public override void Replay()
        {
            base.Replay();
        }

        public override void Stop()
        {
            base.Stop();
            isPrepared = false;
            process.CloseMainWindow();
        }

        public override void Pasue()
        {
            base.Pasue();
        }

        public override void Resume()
        {
            base.Resume();
            process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
        }

    }
}
