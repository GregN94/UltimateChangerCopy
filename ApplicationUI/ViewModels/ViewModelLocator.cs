using ApplicationUI.Models;

namespace ApplicationUI.ViewModels
{
    public class ViewModelLocator
    {
        public ModelMediator Mediator { get; set; }

        public void Initialize()
        {
            var applicationViewModel = new ApplicationViewModel();
            var commonOperations = new CommonOperations();
            var selectionViewModel = new FittingSoftwareSelectionViewModel(commonOperations);
            var advancedInstallation = new AdvancedInstallationViewModel();
            var installationViewModel = new BasicInstallationViewModel(selectionViewModel.FittingSoftwares, commonOperations);
            var commonToolViewModel = new CommonToolsViewModel(selectionViewModel.FittingSoftwares);
            var userCommandsViewModel = new UserCommandsViewModel();
            var generalSettingsViewModel = new GeneralSettingsViewModel();
            var installationSettingsViewModel = new InstallationSettingsViewModel();
            var statsViewModel = new StatsViewModel(selectionViewModel.FittingSoftwares);

            Mediator = new ModelMediator
            {
                ApplicationViewModel = applicationViewModel,
                FittingSoftwareSelectionViewModel = selectionViewModel,
                BasicInstallationViewModel = installationViewModel,
                CommonToolsViewModel = commonToolViewModel,
                CommonOperations = commonOperations,
                UserCommandsViewModel = userCommandsViewModel,
                AdvancedInstallationViewModel = advancedInstallation,
                GeneralSettingsViewModel = generalSettingsViewModel,
                StatsViewModel = statsViewModel
            };
        }
    }
}
