using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ApplicationUI.ViewModels
{
    public class UserCommandsViewModel : ViewModelBase
    {
        public ICommand AddNewCommand { get; set; }

        public UserCommandsViewModel()
        {
            AddNewCommand = new RelayCommand(AddNewUserCommand);
        }

        private void AddNewUserCommand(object obj)
        {
            if (UserCommands.Count < userMaxCount)
            {
                UserCommands.Add(new Commands{Color = "Green", Name = "New command"});
            }
        }

        private int userMaxCount = 8;
        public ObservableCollection<Commands> UserCommands { get; set; } = new ObservableCollection<Commands>
        {
            new Commands {Name = "Do nothing", Color = "Blue"},
            new Commands {Name = "Test", Color = "Red"},
        };

        public class Commands
        {
            public string Name { get; set; }
            public string Color { get; set; }
        }
    }
}
