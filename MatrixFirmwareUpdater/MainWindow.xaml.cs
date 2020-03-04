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
using Midi;
using MatrixFirmwareUpdater.Data;

using static MatrixFirmwareUpdater.Data.StaticData;
using System.Linq;

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
            ChangeWindowMessageFilter(WM_COPYDATA, 1);
        }


        private void updateMatrixInfo()
        { 
            matrix = new MatrixInfo(null, null, null, null, null, "NotConnected");
            try
            {
                var searcher = new ManagementObjectSearcher(@"Select * From Win32_PnPEntity");
                foreach (var device in searcher.Get())
                {
                    if (device.GetPropertyValue("DeviceID").ToString().Contains(@"VID_0203"))
                    {
                        if (device.GetPropertyValue("DeviceID").ToString().Contains(@"PID_0003"))
                        {
                            matrix = new MatrixInfo(
                                device.GetPropertyValue("Name").ToString(),
                                "Matrix DFU",
                                "不可用",
                                null,
                                "不可用",
                                "DFU");
                            break;
                        }
                        else if (device.GetPropertyValue("PNPClass").ToString().Equals("MEDIA"))
                        {
                            matrix = new MatrixInfo(
                            device.GetPropertyValue("Name").ToString(),
                            "Matrix",
                            "未知",
                            null,
                            "未知",
                            "Connected");
                            requestMatrixInfo();
                            break;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }

                this.Dispatcher.Invoke(() =>
                {
                    UpdateUserControl();
                });

        }

        public Midi.InputDevice matrixIn; // Yeet Input in
        public Midi.OutputDevice matrixOut; // Yeet Output in

        private void requestMatrixInfo()
        {
            //Setup Midi
            Console.WriteLine("Looking for Midi input device " + matrix.Name);
            int i = 0;
            foreach (Midi.InputDevice inputDevice in Midi.InputDevice.InstalledDevices)
            {
                Console.WriteLine(inputDevice.Name);
                if (inputDevice.Name == matrix.Name)
                {
                    Console.WriteLine("Midi Input Connected: " + inputDevice.Name);
                    matrixIn = Midi.InputDevice.InstalledDevices[i];
                    matrixIn.Open();
                    matrixIn.SysEx += new Midi.InputDevice.SysExHandler(ReceiveSysEx);
                    matrixIn.StartReceiving(null, true);
                    break;
                }
                i++;
            }

            Console.WriteLine("Looking for Midi output device " + matrix.Name);

            i = 0;
            foreach (Midi.OutputDevice outputDevice in Midi.OutputDevice.InstalledDevices)
            {
                Console.WriteLine(outputDevice.Name);
                if (outputDevice.Name == matrix.Name)
                {
                    Console.WriteLine("Midi Output Connected: " + outputDevice.Name);
                    matrixOut = Midi.OutputDevice.InstalledDevices[i];
                    matrixOut.Open();
                    break;
                }
                i++;
            }

            matrixOut.SendSysEx(new byte[] { 240, 0, 2, 3, 1, 0, 17, 16, 247 }); //获取设备名字
            matrixOut.SendSysEx(new byte[] { 240, 0, 2, 3, 1, 0, 17, 17, 247 }); //获取设备序列号String
            matrixOut.SendSysEx(new byte[] { 240, 0, 2, 3, 1, 0, 17, 18, 0, 247 }); //获取设备固件String
            matrixOut.SendSysEx(new byte[] { 240, 0, 2, 3, 1, 0, 17, 18, 1, 247 }); //获取设备固件Bytes
        }
    private void ReceiveSysEx(SysExMessage msg)
    {
        byte[] rawData = new byte[msg.Data.Length - 2];
        Array.Copy(msg.Data, 1, rawData, 0, msg.Data.Length - 2); //去掉开头和结尾
        StringBuilder hex = new StringBuilder(rawData.Length * 2);
        foreach (byte b in rawData)
            hex.AppendFormat("{0:x2}", b);
        Console.Write("Sysex Recived ");
        Console.WriteLine(hex);
            if (new ArraySegment<byte>(rawData, 0, 5).SequenceEqual(new byte[] {0x00, 0x02, 0x03, 0x01, 0x00})) //确认下确实是Matrix发来的Sysex
        {
                if (rawData[5] == 0x12) //写 
            {
                var data = rawData.Skip(7).ToArray();
                switch (rawData[6])
                {
                    case 16:
                        matrix.DeviceName = Encoding.ASCII.GetString(data);
                        Console.WriteLine("Device Name: " + matrix.DeviceName);
                        break;
                    case 17:
                        matrix.Serial_number = Encoding.ASCII.GetString(data);
                        Console.WriteLine("Device Serial Number: " + matrix.Serial_number);
                            break;
                    case 18:
                        if(data[0] == 0)
                            {
                                matrix.FW_version = Encoding.ASCII.GetString(data.Skip(1).ToArray());
                                Console.WriteLine("Device FW Version: " + matrix.FW_version);
                            }
                        else if(data[0] == 1)
                            {
                                matrix.Version_byte = Array.ConvertAll(data.Skip(1).ToArray(), Convert.ToInt32);
                                Console.WriteLine("Device FW bytes: {0}.{1}.{2}.{3} ", matrix.Version_byte[0], matrix.Version_byte[1], matrix.Version_byte[2], matrix.Version_byte[3]);
                            }
                        break;

                }
            }
            else if(rawData[5] == 0x11) //读 不过我也不知道为什么要从电脑读数据。。。先加上吧
            {

            }

                //BaseUserControl::updateMatrix();
        }
    }

    private void doThread()
        {
            updateMatrixInfo();
            //UpdateUserControl();

            //if (Matrix == null)
            //{

            //}
            //else
            //{
            //    StatusToUserControl(Status.Connected);
            //}
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
            switch (msg)
            {
                case WM_COPYDATA:
                    CopyDataStruct cds = (CopyDataStruct)System.Runtime.InteropServices.Marshal.PtrToStructure(lParam, typeof(CopyDataStruct));
                    //MessageBox.Show(cds.lpData);
                    GetOtherWindowsMsg(cds.lpData);
                    break;
                case 0x0219:
                    Thread thread = new Thread(doThread);
                    thread.Start();
                    //updateMatrixInfo();
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
            string fileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();

            //获得文件名后的操作...
            if (fileName.EndsWith(".mxfw"))
            {
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
            if (msg.Equals("HELLO"))
            {
                //Close();//关闭窗体，具体执行什么操作，可自行更改
                //恢复连接状态
                //tatusToUserControl(Status.Connected);
                //updateMatrixInfo();
            }
        }

        public enum Status
        {
            NotConnected = 0,
            Connected = 1,
            Ready = 2,
            DFU = 3,
        }

        public void UpdateUserControl()
        {
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
