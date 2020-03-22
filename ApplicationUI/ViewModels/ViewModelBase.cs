using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using Utils;
using Utils.Annotations;

namespace ApplicationUI.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        //public IDialogCoordinator DialogCoordinator { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableObject overlayViewModel;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void HideOverlay()
        {
            OverlayViewModel = null;
            CommandManager.InvalidateRequerySuggested();
        }

        public ObservableObject OverlayViewModel
        {
            get => this.overlayViewModel;
            set
            {
                if (this.overlayViewModel == value)
                    return;
                this.overlayViewModel = value;
                OnPropertyChanged();
            }
        }


    }
}
