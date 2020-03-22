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

namespace ApplicationUI.Views.Settings
{
    /// <summary>
    /// Interaction logic for LogsSettingsView.xaml
    /// </summary>
    public partial class LogsSettingsView : UserControl
    {
        public LogsSettingsView()
        {
            InitializeComponent();
        }

        private void SliberMBLogs_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Properties.Settings.Default.MBLogsSize = (Byte)SliberMBLogs.Value;
            Properties.Settings.Default.Save();           

        }
    }
}
