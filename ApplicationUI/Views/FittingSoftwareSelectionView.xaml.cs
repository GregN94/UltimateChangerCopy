using ApplicationUI.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Controls;

namespace ApplicationUI.Views
{
    /// <summary>
    /// Interaction logic for FittingSoftwareSelectionView.xaml
    /// </summary>
    public partial class FittingSoftwareSelectionView : UserControl
    {
        public FittingSoftwareSelectionView()
        {
            InitializeComponent();
            var viewModel = DataContext as FittingSoftwareSelectionViewModel;
            viewModel.DialogCoordinator = DialogCoordinator.Instance;
        }
    }
}
