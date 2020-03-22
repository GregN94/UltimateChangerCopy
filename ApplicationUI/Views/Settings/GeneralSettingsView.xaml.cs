using ApplicationUI.ViewModels;
using MahApps.Metro.Controls.Dialogs;
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

namespace ApplicationUI.Views
{
    /// <summary>
    /// Interaction logic for GeneralSettingsView.xaml
    /// </summary>
    public partial class GeneralSettingsView : UserControl
    {
        public GeneralSettingsView()
        {
            InitializeComponent();
            var viewModel = DataContext as GeneralSettingsViewModel;
            viewModel.DialogCoordinator = DialogCoordinator.Instance;
            if (Properties.Settings.Default.GearboxWorkspacePath == "")
            {
                Properties.Settings.Default.GearboxWorkspacePath = $"C:\\Users\\{Environment.UserName}\\workspace";
                Properties.Settings.Default.Save();
            }
        }

        private void GearboxWorkspaceToggle_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            GearboxWorkspaceToggle.IsOn = false;
        }

        private void TextBox_DragEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Properties.Settings.Default.GearboxWorkspacePath = txtPathWorkspace.Text;
                Properties.Settings.Default.Save();
            }
        }
    }
}
