using ScanUtilityLibraryVer2.LivoxSdk2.Samples;
using ScanUtilityLibraryVer2.LivoxSdk2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScanUtilityExample
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void Button_LivoxHap_Click(object sender, EventArgs e)
        {
            DllLoader.ConfigureDllPath();
            LivoxLidarQuickStart.FrameTime = 500;
            LivoxLidarQuickStart.Start("hap_config.json");
            Thread.Sleep(300000); // 保持程序运行
            LivoxLidarQuickStart.Stop();
        }
    }
}
