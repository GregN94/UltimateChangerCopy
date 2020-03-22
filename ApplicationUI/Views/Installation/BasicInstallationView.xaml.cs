using System.Windows;
using System.Windows.Controls;
using ApplicationUI.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace ApplicationUI.Views
{
    /// <summary>
    /// Interaction logic for BasicInstallationView.xaml
    /// </summary>
    public partial class BasicInstallationView : UserControl
    {
        public BasicInstallationView()
        {
            InitializeComponent();
            var viewModel = DataContext as BasicInstallationViewModel;
            viewModel.DialogCoordinator = DialogCoordinator.Instance;
            SilentToggle.IsOn = Properties.Settings.Default.SilentMode;
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);
            object obj = parentWindow.FindName("AdvancedInstallation");
            Flyout flyout = (Flyout) obj;  
            flyout.IsOpen = !flyout.IsOpen;
        }
    }
}
