using ApplicationUI.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Controls;

namespace ApplicationUI.Views
{
    /// <summary>
    /// Interaction logic for CommonToolsView.xaml
    /// </summary>
    public partial class CommonToolsView : UserControl
    {
        public CommonToolsView()
        {
            InitializeComponent();
            var viewModel = DataContext as CommonToolsViewModel;
            viewModel.DialogCoordinator = DialogCoordinator.Instance;
        }
    }
}
