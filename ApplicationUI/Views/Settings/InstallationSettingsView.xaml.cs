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
    /// Interaction logic for InstallationSettingsView.xaml
    /// </summary>
    public partial class InstallationSettingsView : UserControl
    {
        public InstallationSettingsView()
        {
            InitializeComponent();
        }
        private void TextBox_Enter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Properties.Settings.Default.InstallationPaths = txtInstallationPath.Text;
                Properties.Settings.Default.Save();
            }
        }
    }
}
