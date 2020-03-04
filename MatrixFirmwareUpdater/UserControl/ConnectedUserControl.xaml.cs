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

            if (pullLatestFirmware())
                SetMatrixFWMetaData();

            UpdateMatrix();
        }

        private bool pullLatestFirmware()
        {
            bool beta = true;
            //const string URL = "https://api.github.com/repos/203Industries/Matrix/releases";
            const string URL = "C:\\Users\\caine\\Documents\\demoGithubApi.txt";
            using (var webClient = new System.Net.WebClient())
            {
                try
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; //加上这一句

                    webClient.Headers.Add("User-Agent", "Nothing");
                    webClient.Encoding = Encoding.UTF8;
                    var json = webClient.DownloadString(URL);
                    var releases = JsonConvert.DeserializeObject<List<GithubRelease>>(json);

                    foreach (var release in releases)
                    {
                        if (!release.prerelease || beta)
                        {
                            String body = Regex.Escape(release.body);
                            int[] version_byte = Array.ConvertAll(Regex.Unescape(Regex.Match(body, @"(?<=Version\\ byte:\\ )(.*?)(?=-->\\r\\n)").Value).Split('.'), int.Parse);
                            String release_type = "Release";
                            if (release.prerelease)
                                release_type = "PreRelease";
                            string patchnote_zh_CN = Regex.Unescape(Regex.Match(body, @"(?<=<!--\\ patchnote_zh_CN\\ -->\\r\\n)(.*?)(?=</details>)").Value);
                            string patchnote_en = Regex.Unescape(Regex.Match(body, @"(?<=<!--\\ patchnote_en\\ -->\\r\\n)(.*?)(?=</details>)").Value);
                            IList<string> supported_devices = Regex.Unescape(Regex.Match(body, @"(?<=<!--\\ supported_devices\\ -->\\r\\n)(.*?)(?=</details>)").Value).Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                            string file_URL = release.assets[0].browser_download_url;

                            matrixFW = new MatrixFWMeta
                            (
                                release.name,
                                version_byte,
                                release_type,
                                release.published_at,
                                supported_devices,
                                patchnote_en,
                                patchnote_zh_CN,
                                file_URL
                        );
                            return true;
                        }
                    }

                }
                catch (Exception e)
                {
                    //请求服务器失败之后的操作
                    System.Windows.Forms.MessageBox.Show(e.Message);
                    FailGetJson();
                    return false;
                }
            }
            return false;
        }

        private void SetMatrixFWMetaData()
        {
            Application.Current.Dispatcher.Invoke(delegate ()
            {
                //要更新的UI代码
                try
                {
                    tbVersion.Text = matrixFW.Version;
                    tbPatchnote.Text = matrixFW.Patchnote_zh_CN;
                }
                catch (Exception)
                {

                }
            });
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
            if (matrixFW != null) {
                //mw.StatusToUserControl(Status.Ready);
            }
        }

    }
}
