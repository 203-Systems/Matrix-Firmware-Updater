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

namespace MatrixFirmwareUpdater
{
    /// <summary>
    /// OnNotConnectedUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class NotConnectedUserControl : BaseUserControl
    {
        public NotConnectedUserControl(MainWindow mw) : base(mw)
        {
            InitializeComponent();

            ImageName = "Not Connected.png";
            image = iRight;

            _tbDeviceName = tbDeviceName;
            _tbNowVersionName = tbNowVersionName;
            //UpdateMatrix();
        }
    }
}
