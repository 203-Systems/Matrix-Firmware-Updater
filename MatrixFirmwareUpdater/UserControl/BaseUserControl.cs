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
        protected String ImageName = "";
        protected Image image;
        protected MainWindow mw;

        protected TextBlock _tbDeviceName, _tbNowVersionName;

        public BaseUserControl(MainWindow mw) {
            this.mw = mw;
            Loaded += BaseUserControl_Loaded;
        }

        private void BaseUserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!ImageName.Equals(String.Empty) && image != null) {
                image.Source = new BitmapImage(new Uri("pack://application:,,,/Images/"+ ImageName));
            }
        }

        protected void UpdateMatrix()
        {
            Application.Current.Dispatcher.Invoke(delegate ()
            {
                if (_tbDeviceName != null && _tbNowVersionName != null)
                {
                    _tbDeviceName.Text = StaticData.matrix.Name;
                    _tbNowVersionName.Text = StaticData.matrix.FW_Version;
                }
            });
        }
    }
}
