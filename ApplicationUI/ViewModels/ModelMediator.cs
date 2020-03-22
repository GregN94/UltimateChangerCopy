using ApplicationUI.Models;

namespace ApplicationUI.ViewModels
{
    public class ModelMediator
    {
        public ApplicationViewModel ApplicationViewModel { get; set; }
        public FittingSoftwareSelectionViewModel FittingSoftwareSelectionViewModel { get; set; }
        public BasicInstallationViewModel BasicInstallationViewModel { get; set; }
        public AdvancedInstallationViewModel AdvancedInstallationViewModel { get; set; }
        public CommonToolsViewModel CommonToolsViewModel { get; set; }
        public CommonOperations CommonOperations { get; set; }
        public UserCommandsViewModel UserCommandsViewModel { get; set; }
        public GeneralSettingsViewModel GeneralSettingsViewModel { get; set; }
        public LogSettingsViewModel LogsSettingsViewModel { get; set; }
        public StatsViewModel StatsViewModel { get; set; }
        public InstallationSettingsViewModel InstallationSettingsViewModel { get; set; }
    }
}
