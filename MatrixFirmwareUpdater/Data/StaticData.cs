using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MatrixFirmwareUpdater.Data
{
    public class StaticData
    {
        public class MatrixInfo
        {
            public MatrixInfo(string name, string FW_Version, string Serial_number, string status)
            {
                this.Name = name;
                this.FW_Version = FW_Version;
                this.Serial_number = FW_Version;
                this.Status = status;
            }
            public string Name { get; set; }
            public string FW_Version { get; set; }
            public string Serial_number { get; set; }
            public string Status { get; set; }
        }

        public static MatrixInfo Matrix { get; set; }

        public static String fw_link;

        public static MatrixFWMeta buildmeta;

        public class MatrixFWMeta
        {
            public string version { get; set; }
            public string buildtype { get; set; }
            public DateTime buildtime { get; set; }
            public IList<string> supported_device { get; set; }
            public string patchnote_en { get; set; }
            public string patchnote_zh_cn { get; set; }
        }

        public static void SetMatrixFWMetaData(TextBlock tbVersion, TextBlock tbPatchnote)
        {
            Application.Current.Dispatcher.Invoke(delegate ()
            {
                //要更新的UI代码
                try
                {
                    tbVersion.Text = buildmeta.version;
                    tbPatchnote.Text = buildmeta.patchnote_zh_cn;
                }
                catch (Exception)
                {

                }
            });
        }
    }
}
