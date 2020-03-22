using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System.IO;
using System.Windows.Input;

namespace ApplicationUI.ViewModels
{
    public class GeneralSettingsViewModel : ViewModelBase
    {
        public ICommand ToggleStartUC_Command { get; set; }
        public ICommand ToggleStartGearboxCommand { get; set; }
        public ICommand ToggleStartNPCommand { get; set; }
        public ICommand ToggleStartWorkspaceCommand { get; set; }
        public ICommand ToggleStartPETCommand { get; set; }
        public ICommand ToggleRecordingCommand { get; set; }
        public IDialogCoordinator DialogCoordinator { get; set; }

        public GeneralSettingsViewModel()
        {
            ToggleStartUC_Command = new RelayCommand(_ => ToggleStartUp());
            ToggleStartGearboxCommand = new RelayCommand(_ => ToggleStartGearbox());
            ToggleStartWorkspaceCommand = new RelayCommand(_ => ToggleStartWorkspace());
            ToggleStartNPCommand = new RelayCommand(_ => ToggleStartNP());
            ToggleStartPETCommand = new RelayCommand(_ => ToggleStartPET());
            ToggleRecordingCommand = new RelayCommand(_ => ToggleRecording());

            //read from settings and execute if needed

            if (Properties.Settings.Default.StartGearbox)
            {
                FittingSoftware.FS.TryStartGearbox(Properties.Settings.Default.StartWorkspace, Properties.Settings.Default.GearboxWorkspacePath);
            }

            if (Properties.Settings.Default.StartNP)
            {
                FittingSoftware.FS.TryStartNewPreconditioner();
            }

            if (Properties.Settings.Default.StartPET)
            {
                string path = Properties.Settings.Default.PETpath;

                if (File.Exists(path))
                {
                    System.Diagnostics.Process.Start(path);
                }
            }

            //read settings and put on UI 
        }

        private async void ToggleRecording()
        {
            if (Properties.Settings.Default.StartRecording)
            {
                if (!File.Exists(@"C:\Program Files\ShareX\ShareX.exe"))
                {
                    await DialogCoordinator.ShowMessageAsync(this, "Couldn't Start Recording", "Go to : https://getsharex.com/ \nand Install");
                    return;
                }
            }

            Properties.Settings.Default.Save();
        }

        private void ToggleStartGearbox()
        {
            //We don't have to change it here, as it it's already binded TwoWay, only save is useful, should be changed to some usefull impleentation
            //Properties.Settings.Default.StartGearbox = true;
            Properties.Settings.Default.Save();
        }

        private void ToggleStartNP()
        {
            //We don't have to change it here, as it it's already binded TwoWay, only save is useful, should be changed to some usefull impleentation
            //Properties.Settings.Default.StartNP = true;
            Properties.Settings.Default.Save();
        }

        private void ToggleStartPET()
        {
            //We don't have to change it here, as it it's already binded TwoWay, only save is useful, should be changed to some usefull impleentation
            //Properties.Settings.Default.StartPET = true;
            Properties.Settings.Default.Save();
        }

        private void ToggleStartWorkspace()
        {
            //We don't have to change it here, as it it's already binded TwoWay, only save is useful, should be changed to some usefull impleentation
            //Properties.Settings.Default.StartPET = true;
            Properties.Settings.Default.Save();
        }

        private void ToggleStartUp()
        {
            var startUC = Properties.Settings.Default.StartUC;

            var startUpRegistry = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (startUpRegistry == null)
            {
                return;
            }

            if (startUC)
            {

                startUpRegistry.SetValue("UltimateChangerV", System.Reflection.Assembly.GetExecutingAssembly().Location);
                Properties.Settings.Default.StartUC = true;
            }
            else
            {
                startUpRegistry.DeleteValue("UltimateChangerV", false);
                Properties.Settings.Default.StartUC = false;
            }

            Properties.Settings.Default.Save();
        }
    }
}
