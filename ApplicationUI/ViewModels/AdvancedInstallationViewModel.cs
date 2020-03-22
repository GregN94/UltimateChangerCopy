using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ApplicationUI.ViewModels
{
    public class AdvancedInstallationViewModel : ViewModelBase
    {
        public AdvancedInstallationViewModel()
        {
            AddOperationCommand = new RelayCommand(ExecuteAddOperation);
            DeleteOperationCommand = new RelayCommand(ExecuteDelayOperation);
            PlayBigCommand = new RelayCommand(ExecutePlayBig);
            StopCommand = new RelayCommand(ExecuteStop);
        }

        private void ExecuteStop(object obj)
        {
            BigRunning = false;
        }

        private void ExecutePlayBig(object obj)
        {
            BigRunning = !BigRunning;
        }

        private void ExecuteDelayOperation(object obj)
        {
            if (Operations.Count > 1)
            {
                Operations.RemoveAt(Operations.Count - 1);
            }
        }

        private void ExecuteAddOperation(object obj)
        {
            if (Operations.Count < 5)
            {
                operations.Add("Hello");
            }
        }

        public ICommand AddOperationCommand { get; set; }
        public ICommand DeleteOperationCommand { get; set; }
        public ICommand PlayBigCommand { get; set; }
        public ICommand StopCommand { get; set; }

        private bool bigRunning;
        public bool BigRunning
        {
            get => bigRunning;
            set
            {
                bigRunning = value;
                OnPropertyChanged();
            }
        }


        private ObservableCollection<string> operations = new ObservableCollection<string> { ""};
        public ObservableCollection<string> Operations
        {
            get => operations;
            set
            {
                operations = value;
                OnPropertyChanged();
            }
        }

    }
}
