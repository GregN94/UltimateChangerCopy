using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace ApplicationUI.Views
{
    /// <summary>
    /// Interaction logic for ApplicationView.xaml
    /// </summary>
    public partial class ApplicationView : MetroWindow
    {
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int nCmdShow);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool IsIconic(IntPtr handle);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        const int SW_RESTORE = 9;
        //ILogger logger = new Logger(typeof(ApplicationView));
        FileSystemWatcher UpdateFilesWatcher;

        public ApplicationView()
        {
            InitializeComponent();
            //logger.LogInfoMessage("Starting UC");
            var arrayUCInstance = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
            if (arrayUCInstance.Length > 1)
            {
                arrayUCInstance[0].Refresh();
                IntPtr handle = arrayUCInstance[0].MainWindowHandle;
                if (IsIconic(handle))
                {
                    ShowWindow(handle, SW_RESTORE);
                }
                SetForegroundWindow(handle);
                IntPtr hWnd = FindWindow("UltimateChanger | Administrator", null);
                ShowWindow(hWnd, 9);
                //The bring the application to focus
                SetForegroundWindow(hWnd);
                Environment.Exit(0);
            }
            this.Title = "UltimateChanger | Normal User";

            //var viewModel = DataContext as ApplicationViewModel;
            //viewModel.DialogCoordinator = DialogCoordinator.Instance;

            this.Title = "UltimateChanger";
            this.TitleCharacterCasing = CharacterCasing.Normal;

            CheckUpdate();

            if (Properties.Settings.Default.FirstRun)
            {
                Properties.Settings.Default.FirstRun = false;
                Properties.Settings.Default.Save();
                // display ChangeLog
                // restore settings if availble
                RestoreUserSettings();
                string changelogmessage = getChangeLog();
                ShowChangeLog(changelogmessage);
            }
            try
            {
                UpdateFilesWatcher = new FileSystemWatcher
                {
                    Path = @"\\10.128.3.1\DFS_data_SSC_FS_Images-SSC\PAZE\change_market\UC_V\",
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                    Filter = "UltimateChangerV_*",
                    IncludeSubdirectories = true
                };

                UpdateFilesWatcher.Created += NewUpdate;
                UpdateFilesWatcher.EnableRaisingEvents = true;
            }
            catch (Exception)
            {
                // no connection to VPN
            }

        }

        private async void NewUpdate(object sender, FileSystemEventArgs e)
        {
            await Task.Delay(240000); //240 s
            CheckUpdate();
        }

        private async void ShowChangeLog(string changelog)
        {
            await this.ShowMessageAsync("ChangeLog", changelog);
        }

        private string getChangeLog() //from xml changelog.xml
        {
            string log = "\u2022 Version: 1.0.0.26\n\n";
            log += "\t- Install All FS (not installed) select release and build\n";
            log += "\t- Set All level log mode after install/update FS (settings)\n\n";
            log += "\u2022 Version: 1.0.0.25\n\n";
            log += "\t- Fix - new name directory - FullInstaller changed to Installer\n\n";
            log += "\u2022 Version: 1.0.0.24\n\n";
            log += "\t- Update > 1 FSs select release and build -> press Update\n\n";

            return log;
        }

        private void RestoreUserSettings()
        {
            string UserSettingBackupPath = @"C:\ProgramData\UltimateChanger\UserSettings.xml";
            if (Directory.Exists(@"C:\ProgramData\UltimateChanger"))
            {
                if (File.Exists(UserSettingBackupPath))
                {
                    try
                    {
                        CompareAndSetSettings(UserSettingBackupPath);
                    }
                    catch (Exception x)
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        private void CompareAndSetSettings(string source)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(source);

            foreach (System.Configuration.SettingsProperty currentProperty in
                Properties.Settings.Default.Properties)
            {
                if (currentProperty.PropertyType == Type.GetType("System.Boolean"))
                {
                    try
                    {
                        bool value = (bool)Properties.Settings.Default[currentProperty.Name];
                        XmlNode xmlNode = xml.SelectSingleNode($"/Settings/seting[@name='{currentProperty.Name}']");
                        bool userValue = Convert.ToBoolean(xmlNode.InnerText);
                        if (value != userValue)
                        {
                            Properties.Settings.Default[currentProperty.Name] = userValue;
                            Properties.Settings.Default.Save();
                        }
                    }
                    catch (Exception)
                    { }
                }
                else if (currentProperty.PropertyType == Type.GetType("System.String"))
                {
                    try
                    {
                        string value = Properties.Settings.Default[currentProperty.Name].ToString();
                        XmlNode xmlNode = xml.SelectSingleNode($"/Settings/seting[@name='{currentProperty.Name}']");
                        string userValue = xmlNode.InnerText;
                        if (value != userValue)
                        {
                            Properties.Settings.Default[currentProperty.Name] = userValue;
                            Properties.Settings.Default.Save();
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                else if (currentProperty.PropertyType == Type.GetType("System.Byte"))
                {
                    try
                    {
                        byte value = Convert.ToByte(Properties.Settings.Default[currentProperty.Name].ToString());
                        XmlNode xmlNode = xml.SelectSingleNode($"/Settings/seting[@name='{currentProperty.Name}']");
                        byte userValue = Convert.ToByte(xmlNode.InnerText);
                        if (value != userValue)
                        {
                            Properties.Settings.Default[currentProperty.Name] = userValue;
                            Properties.Settings.Default.Save();
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {

                }
            }
        }

        void Window_Closing(object sender, CancelEventArgs e)
        {
            // save user settings to backup file
            //check if file exist 
            string UserSettingsPath = @"C:\ProgramData\UltimateChanger\UserSettings.xml";
            try
            {
                File.Delete(UserSettingsPath);
            }
            catch (Exception)
            {

            }
            if (!Directory.Exists(@"C:\ProgramData\UltimateChanger"))
            {
                Directory.CreateDirectory(@"C:\ProgramData\UltimateChanger");
            }
            if (!File.Exists(UserSettingsPath))
            {
                //File.Create(UserSettingsPath);
                XmlDocument doc = new XmlDocument();
                XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                doc.AppendChild(docNode);

                XmlNode productsNode = doc.CreateElement("Settings");
                foreach (System.Configuration.SettingsProperty currentProperty in
                Properties.Settings.Default.Properties)
                {
                    doc.AppendChild(productsNode);
                    XmlNode productNode = doc.CreateElement("seting");
                    XmlAttribute productAttribute = doc.CreateAttribute("name");
                    productAttribute.Value = currentProperty.Name;
                    productNode.Attributes.Append(productAttribute);
                    productNode.InnerText = Properties.Settings.Default[currentProperty.Name].ToString();
                    productsNode.AppendChild(productNode);

                }
                doc.Save(UserSettingsPath);
            }
        }

        public async Task ShowMessage(string UpdatePath = "")
        {
            var settings = new MetroDialogSettings { FirstAuxiliaryButtonText = "Update", NegativeButtonText = "Later" };

            var result = await this.ShowMessageAsync("Update is available", "Newer version is available",
                                                        MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, settings);
            switch (result)
            {
                case MessageDialogResult.Negative:
                {
                    break;
                }
                case MessageDialogResult.FirstAuxiliary:
                {
                    Process.Start(UpdatePath + @"\UltimateChangerV.application");
                    Environment.Exit(0);
                    break;
                }
            }
        }
        private async Task<Task> CheckUpdate()
        {
            string UpdatePath = @"\\10.128.3.1\DFS_data_SSC_FS_Images-SSC\PAZE\change_market\UC_V\Application Files";
            string SourceUpdate = "";
            await Task.Run(() =>
            {
                if (!Directory.Exists(UpdatePath))
                {
                    return;
                }
                var listReleases = Directory.GetDirectories(UpdatePath);
                char[] delimiterChars = { '_' };
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                foreach (var item in listReleases)
                {
                    var splited = item.Split(delimiterChars);
                    int count = splited.Length;
                    try
                    {
                        Version updateVersion = new Version($"{splited[count - 4]}.{splited[count - 3]}.{splited[count - 2]}.{splited[count - 1]}");
                        if (updateVersion.Major > version.Major ||
                            updateVersion.Minor > version.Minor ||
                            updateVersion.Build > version.Build ||
                            updateVersion.Revision > version.Revision)
                        {
                            SourceUpdate = item;
                            break;
                        }
                    }
                    catch (Exception)
                    {
                    }

                }
            });

            if (SourceUpdate == "")
            {
                return null;
            }
            else
            {
                return ShowMessage(SourceUpdate);
            }
        }



        private void RestartAsAdmin()
        {
            var processStartInfo = new ProcessStartInfo(Assembly.GetEntryAssembly()?.CodeBase.Replace("dll", "exe"))
            {
                UseShellExecute = true,
                Verb = "runas"
            };

            Process.Start(processStartInfo);
            Environment.Exit(0);
        }

        private bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.UserCommandsFlyOut.IsOpen = true;
        }
    }
}
