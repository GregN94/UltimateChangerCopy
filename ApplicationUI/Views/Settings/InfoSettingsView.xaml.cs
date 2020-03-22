using System;
using System.Collections.Generic;
//using System.Deployment.Application;
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
    /// Logika interakcji dla klasy InfoSettingsView.xaml
    /// </summary>
    public partial class InfoSettingsView : UserControl
    {
        public InfoSettingsView()
        {
            InitializeComponent();
            //ApplicationDeployment updateCheck = null;
            //try
            //{
            //    updateCheck = ApplicationDeployment.CurrentDeployment;
            //    lblVersion.Content = $"Version: {updateCheck.CurrentVersion}";
            //}
            //catch (InvalidDeploymentException)
            //{
            //    System.Version version2 = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            //    lblVersion.Content = $"Version: { (String.Format("{0}.{1}.{2}.{3}", version2.Major, version2.Minor, version2.Revision, version2.Build))}";
               
            //}
            string log = "\u2022 Version: 1.0.0.26\n\n";
            log += "\t- Install All FS (not installed) select release and build\n";
            log += "\t- Set All level log mode after install/update FS (settings)\n\n";
            log += "\u2022 Version: 1.0.0.25\n\n";
            log += "\t- Fix - new name directory - FullInstaller changed to Installer\n\n";
            log += "\u2022 Version: 1.0.0.24\n\n";
            log += "\t- Update > 1 FSs select release and build -> press Update\n\n";
            log += "\u2022 Version: 1.0.0.22\n\n";
            log += "\t- Change log\n";
            log += "\t- Restore User settings after update\n";
            log += "\t- Fix for uninstall one FS\n";
            log += "\t- Update FS - uninstall, delete trash and install selected FS\n\n";
            log += "\u2022 Version: 1.0.0.21\n\n";
            log += "\t- Uninstall > 1 FS !! Select FSs, Click right and uninstall\n\n";
            log += "\u2022 Version: 1.0.0.20\n\n";
            log += "\t- Fixed bug with search of uninstaller for Genie when Genie Medical is installed\n";
            log += "\t- Removed unnecessary message box\n";
            log += "\t- Change log in Settings Tab\n\n";
            log += "\u2022 Version: 1.0.0.19\n\n";
            log += "\t- Added function for recording of Fitting Software:\n";
            log += " \tSettings->General\n\n";
            log += "\u2022 Version: 1.0.0.18\n\n";
            log += "\t- Check update without using a shortcut:\n\n update will be presented also with starting UC with Windows";            

            TextChangeLog.Text = log;

        }
    }
}
