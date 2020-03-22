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
    public class LoadingViewModel : ObservableObject
    {
        private ViewModelBase parent;
        public string Message { get; }

        public event EventHandler<bool> CheckboxCheckedChanged;

        public LoadingViewModel(ViewModelBase parent, string message)
        {
            Message = message;
            this.parent = parent;
        }
    }
}
