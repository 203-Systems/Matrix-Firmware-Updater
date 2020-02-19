using System;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Net;
using System.Collections.Generic;
using System.Management;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static MatrixFirmwareUpdater.Data.StaticData;
using MatrixFirmwareUpdater.Data;
using static MatrixFirmwareUpdater.MainWindow;
using System.Windows.Controls;

namespace MatrixFirmwareUpdater
{
    /// <summary>
    /// OnNotConnectedUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class ConnectedUserControl : BaseUserControl
    {
        public ConnectedUserControl(MainWindow mw) :base(mw)
        {
            InitializeComponent();

            ImageName = "Ready.png";
            image = iRight;

            _tbDeviceName = tbDeviceName;
            _tbNowVersionName = tbNowVersionName;
            UpdateMatrix();

            pullLatestFirmware();
        }

        private bool pullLatestFirmware()
        {
            bool beta = false;
            const string URL_BETA = "https://api.github.com/repos/203Industries/Matrix/releases/tags/Beta";
            const string URL_STABLE = "https://api.github.com/repos/203Industries/Matrix/releases/tags/Stable";
            string json;
            using (var webClient = new System.Net.WebClient())
            {
                try
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; //加上这一句

                    webClient.Headers.Add("User-Agent", "Nothing");
                    webClient.Encoding = Encoding.UTF8;
                    json = webClient.DownloadString(URL_STABLE);

                    if (beta)
                    {
                        webClient.Headers.Add("User-Agent", "Nothing");
                        webClient.Encoding = Encoding.UTF8;
                        var json_beta = webClient.DownloadString(URL_BETA);

                        DateTime stableTime = Convert.ToDateTime(Regex.Match(json, "(?<=published_at\":\")(.*?)(Z)").Value);
                        DateTime betaTime = Convert.ToDateTime(Regex.Match(json_beta, "(?<=published_at\":\")(.*?)(Z)").Value);
                        //Console.WriteLine(stableTime);
                        //Console.WriteLine(betaTime);
                        if (betaTime > stableTime)
                            json = json_beta;
                    }
                    fw_link = Regex.Match(json, "(?<=browser_download_url\":\")(.*?)(mxfw)").Value;
                }
                catch (Exception e)
                {
                    //请求服务器失败之后的操作
                    System.Windows.Forms.MessageBox.Show(e.Message);
                    FailGetJson();
                    return false;
                }
            }
            json = Regex.Match(json, @"(?<=<buildmeta>\\r\\n)(.*?)(?=\\r\\n</buildmeta>)", RegexOptions.Multiline).Value;
            json = Regex.Unescape(json);
            //Matrix Build Meta
            buildmeta = JsonConvert.DeserializeObject<MatrixFWMeta>(json);

            if (buildmeta != null) {
                SetMatrixFWMetaData(tbVersion, tbPatchnote);
            }
            return true;
        }

     

        /// <summary>
        /// 请求Json数据失败
        /// </summary>
        /// <param name="msg"></param>
        private void FailGetJson()
        {
            Application.Current.Dispatcher.Invoke(delegate ()
            {
                //要更新的UI代码
                tbTitle.Text = "加载失败";
                tbVersion.Text = null;
                tbPatchnote.Text = null;
            });
        }


        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "固件文件|*.mxfw|所有文件|*.*";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)//注意，此处一定要手动引入System.Window.Forms空间，否则你如果使用默认的DialogResult会发现没有OK属性
            {
                mw.ToUpdate(openFileDialog.FileName);
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //说明网络请求没有问题
            if (buildmeta != null) {
                mw.StatusToUserControl(Status.Ready);
            }
        }

    }
}
