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
    public partial class ReadyUserControl : BaseUserControl
    {
        public ReadyUserControl(MainWindow mw) : base(mw)
        {
            InitializeComponent();

            ImageName = "ready.png";
            //SetMatrixFWMetaData();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //mw.StatusToUserControl(Status.Connected);
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //mw.StatusToUserControl(Status.DFU);
        }

      
    }
}
