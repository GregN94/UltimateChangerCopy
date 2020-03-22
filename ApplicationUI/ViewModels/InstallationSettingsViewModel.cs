using System.Windows.Input;

namespace ApplicationUI.ViewModels
{
    public class InstallationSettingsViewModel
    {
        public ICommand ToggleUImodeCommand { get; set; }
        public ICommand ToggleAllInstallation { get; set; }

        public InstallationSettingsViewModel()
        {
            ToggleUImodeCommand = new RelayCommand(_ => ToggleUImode());
            ToggleAllInstallation = new RelayCommand(_ => ToggleAllModeLvInstallation());
        }

        private void ToggleAllModeLvInstallation()
        {
            Properties.Settings.Default.Save();
        }

        private void ToggleUImode()
        {
            Properties.Settings.Default.Save();
        }
    }
}
