using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Net;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Threading;
using static MatrixFirmwareUpdater.Data.StaticData;
using MatrixFirmwareUpdater.Data;
using static MatrixFirmwareUpdater.MainWindow;

namespace MatrixFirmwareUpdater
{
    /// <summary>
    /// OnNotConnectedUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class DFUUserControl : BaseUserControl
    {
        public DFUUserControl(MainWindow mw) : base(mw)
        {
            InitializeComponent();

            ImageName = "DFU.png";
            image = iRight;

            _tbDeviceName = tbDeviceName;
            _tbNowVersionName = tbNowVersionName;
            UpdateMatrix();

            AllowDrop = false;
            ToDownload();
        }

        private String filePath;
        public void ToDownload()
        {
            if (matrixFW.File_URL == null || matrixFW.File_URL.Equals(String.Empty))
            {
                return;
            }

            string url = matrixFW.File_URL;
            try
            {
                WebClient webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;

                //这里使用DownloadString方法，如果是不需要对文件的文本内容做处理，直接保存，那么可以直接使用功能DownloadFile(url,savepath)直接进行文件保存。
                webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;

                filePath = AppDomain.CurrentDomain.BaseDirectory + @"Download\" + url.Substring(url.LastIndexOf("/") + 1);

                webClient.DownloadFileAsync(new Uri(url), filePath);
            }
            catch (Exception)
            {
                //System.Windows.Forms.MessageBox.Show(e.Message);
            }
        }

        private void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //完成下载后
            //更新操作
            mw.ToUpdate(filePath);
            //System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\Blog\DLL\matrix uploader\Matrix Firmware Uploader.bat", "\"" + filePath + "\"");
        }
    }
}
