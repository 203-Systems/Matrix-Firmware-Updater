using MatrixFirmwareUpdater.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MatrixFirmwareUpdater
{
    public class BaseUserControl : UserControl
    {
        public String ImageName = "";
        protected MainWindow mw;


        public BaseUserControl(MainWindow mw) {
            this.mw = mw;
        }
    }
}
