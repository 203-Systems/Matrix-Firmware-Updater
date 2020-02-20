using System;
using System.Collections.Generic;
using System.Management;
using System.Text;
using System.Windows;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MatrixFirmwareUpdater.Data;
using static MatrixFirmwareUpdater.Data.StaticData;

namespace MatrixFirmwareUpdater
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            updateMatrixInfo();


        }


        private void updateMatrixInfo()
        {
            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_PnPEntity"))
                collection = searcher.Get();

            matrix = new MatrixInfo(
                            null,
                            null,
                            null,
                            null,
                            "NotConnected");

            foreach (var device in collection)
            {
                if (device.GetPropertyValue("DeviceID").ToString().Contains(@"VID_0203"))
                {
                    if (device.GetPropertyValue("DeviceID").ToString().Contains(@"PID_0003"))
                    {
                        matrix = new MatrixInfo(
                            device.GetPropertyValue("Name").ToString(),
                            "Matrix DFU",
                            "未知",
                            "未知",
                            "DFU");
                    }
                    else if (device.GetPropertyValue("PNPClass").ToString().Equals("MEDIA"))
                    {
                        matrix = new MatrixInfo(
                        device.GetPropertyValue("Name").ToString(),
                        "Matrix",
                        "未知",
                        "未知",
                        "Connected");
                    }
                }

            }

            this.Dispatcher.Invoke(() =>
            {
                UpdateUserControl();
            });
        }

        private void doThread()
        {
            //updateMatrixInfo();
            //UpdateUserControl();

            //if (Matrix == null)
            //{

            //}
            //else
            //{
            //    StatusToUserControl(Status.Connected);
            //}

            ChangeWindowMessageFilter(WM_COPYDATA, 1);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(doThread);
            thread.Start();
        }


        public const int WM_COPYDATA = 0x004A;

        [DllImport("user32")]
        public static extern bool ChangeWindowMessageFilter(uint msg, int flags);

        [StructLayout(LayoutKind.Sequential)]
        public struct CopyDataStruct
        {
            public IntPtr dwData;
            public int cbData;//字符串长度
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;//字符串
        }

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_DEVICECHANGE = 0x0219;
            if (msg == WM_COPYDATA)
            {

            }
            switch(msg)
            {
                case WM_COPYDATA:
                    CopyDataStruct cds = (CopyDataStruct)System.Runtime.InteropServices.Marshal.PtrToStructure(lParam, typeof(CopyDataStruct));
                    //MessageBox.Show(cds.lpData);
                    GetOtherWindowsMsg(cds.lpData);
                    break;
                case 0x0219:

                    updateMatrixInfo();
                    break;
            }

            return hwnd;
        }
            

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                IntPtr handle = hwndSource.Handle;
                hwndSource.AddHook(new HwndSourceHook(WndProc));
            }
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            gDrop.Visibility = Visibility.Visible;

            string fileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();

            if (fileName != null && !fileName.Equals(String.Empty) && fileName.EndsWith(".mxfw"))
            {
                e.Effects = DragDropEffects.Link;
            }
            else
            {
                //Cursor = Cursors.No;
                e.Effects = DragDropEffects.None;
                tbHint.Text = "非固件文件";
            }

            //if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
            //    e.Effects = DragDropEffects.Link;
            //    //WinForm中为e.Effect = DragDropEffects.Link
            //}
            //else
            //{
            //    e.Effects = DragDropEffects.None;
            //    //WinFrom中为e.Effect = DragDropEffects.None
            //}
        }

        private void Window_DragLeave(object sender, DragEventArgs e)
        {
            //tbHint.Text = "松开鼠标即可刷入固件";
            gDrop.Visibility = Visibility.Collapsed;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            tbHint.Text = "松开鼠标即可刷入固件";
            gDrop.Visibility = Visibility.Collapsed;
            string fileName =((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();

            //获得文件名后的操作...
            if (fileName.EndsWith(".mxfw")) {
                //确认是固件文件
                //更新操作
                ToUpdate(fileName);
            }
        }

        /// <summary>
        /// 固件文件路径
        /// </summary>
        /// <param name="filePath"></param>
        public void ToUpdate(String filePath)
        {
            //这里写更新固件的操作
        }

        /// <summary>
        /// 别的窗体传入的消息
        /// </summary>
        /// <param name="msg"></param>
        private void GetOtherWindowsMsg(String msg)
        {
            if (msg.Equals("HELLO")) {
                //Close();//关闭窗体，具体执行什么操作，可自行更改
                //恢复连接状态
                //tatusToUserControl(Status.Connected);
                //updateMatrixInfo();
            }
        }

        public enum Status {
            NotConnected = 0,
            Connected = 1,
            Ready = 2,
            DFU = 3,
        }

        public void UpdateUserControl() {
            gMain.Children.Clear();
            switch (matrix.Status)
            {
                case "NotConnected":
                    gMain.Children.Add(new NotConnectedUserControl(this));
                    break;
                case "Connected":
                    gMain.Children.Add(new ConnectedUserControl(this));
                    break;
                case "Ready":
                    gMain.Children.Add(new ReadyUserControl(this));
                    break;
                case "DFU":
                    gMain.Children.Add(new DFUUserControl(this));
                    break;
                default:
                    break;
            }
        }
    }
}
