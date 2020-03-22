using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Utils;

namespace ApplicationUI.ViewModels
{
    public class FailedLoadingViewModel : ObservableObject
    {
        private ViewModelBase parent;
        public string Header { get; }
        public string Message { get; }
        public ICommand RefreshCommand { get; set; }

        public FailedLoadingViewModel(ViewModelBase parent, string header, string message, Action action)
        {
            Header = header;
            Message = message;
            this.parent = parent;
            RefreshCommand = new RelayCommand(_ => action());
        }
    }
}
