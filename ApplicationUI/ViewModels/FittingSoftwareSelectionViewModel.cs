using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FittingSoftware;
using FittingSoftwareEnums;
using ApplicationUI.Models;
using Utils;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProcessManager;

namespace ApplicationUI.ViewModels
{
    public class FittingSoftwareSelectionViewModel : ViewModelBase
    {
        public SelectableCollection<FS> FittingSoftwares { get; set; }

        public ICommand SelectCommand { get; set; }
        public ICommand AddSelectionCommand { get; set; }
        public ICommand SelectRangeCommand { get; set; }
        public ICommand RunCommand { get; set; }
        public ICommand KillCommand { get; set; }
        public ICommand CopyCommand { get; set; }
        public ICommand OpenExplorerCommand { get; set; }
        public ICommand OpenConfigCommand { get; set; }
        public ICommand ChangeMarketCommand { get; set; }
        public ICommand ChangeLogLevelCommand { get; set; }
        public ICommand UninstallCommand { get; set; }
        public ICommand DeleteLogsCommand { get; set; }
        public IDialogCoordinator DialogCoordinator { get; set; }

        private void Select(object obj)
        {
            if (!(obj is FS))
                return;

            var brand = obj as FS;

            foreach (var fittingSoftware in FittingSoftwares)
            {
                if (fittingSoftware.Value == brand)
                    continue;
                ;
                if (fittingSoftware.Value.Installed)
                {
                    fittingSoftware.Selected = false;
                }
            }
            FittingSoftwares.ToggleSelection(brand);
            OnPropertyChanged(nameof(FittingSoftwares));
        }

        private void AddSelection(object obj)
        {
            if (!(obj is FS))
                return;

            var brand = obj as FS;

            FittingSoftwares.ToggleSelection(brand);
            OnPropertyChanged(nameof(FittingSoftwares));
        }

        private void SelectRange(object obj)
        {
            if (!(obj is FS))
                return;

            var brand = obj as FS;
            FittingSoftwares.ToggleSelection(brand);

            if (FittingSoftwares.AnySelected())
            {
                var begin = FittingSoftwares.First(s => s.Selected);
                var end = FittingSoftwares.Last(s => s.Selected);
                if (begin != end)
                {
                    var beginIndex = FittingSoftwares.IndexOf(begin);
                    var endIndex = FittingSoftwares.IndexOf(end);
                    for (int i = beginIndex; i <= endIndex; i++)
                    {
                        if (FittingSoftwares[i].Value.Installed)
                        {
                            FittingSoftwares[i].Selected = true;
                        }
                    }

                }
            }
            OnPropertyChanged(nameof(FittingSoftwares));
        }

        public ObservableCollection<string> LogLeves { get; set; } = new ObservableCollection<string>
        {
            "ALL",
            "DEBUG",
            "ERROR"
        };

        public async void HandleRun(object obj)
        {
            if (FittingSoftwares.CountSelected() > 1)
            {
                try
                {
                    foreach (var selected in FittingSoftwares.GetAllSelected())
                    {
                        await selected.Value.TryStartFS();
                    }
                }
                catch (Exception)
                {

                }

            }
            else
            {
                if (obj is FS fittingSoftware)
                {
                    var result = await fittingSoftware.TryStartFS(Properties.Settings.Default.StartRecording);
                }
            }
        }

        public void Kill(object obj)
        {
            if (FittingSoftwares.CountSelected() > 1)
            {
                foreach (var selected in FittingSoftwares.GetAllSelected())
                {
                    selected.Value.TryKillFS();
                }
            }
            else
            {
                if (obj is FS fittingSoftware)
                {
                    fittingSoftware.TryKillFS();
                }
            }
        }

        private readonly CommonOperations commonOperations;
        public FittingSoftwareSelectionViewModel(CommonOperations commonOperations)
        {
            FittingSoftwares = new SelectableCollection<FS>(FittingSoftwareRepository.GetFittingSoftwares());
            this.commonOperations = commonOperations;

            SelectCommand = new RelayCommand(Select);
            RunCommand = new RelayCommand(HandleRun);
            KillCommand = new RelayCommand(Kill);
            CopyCommand = new RelayCommand(CopyToClipBoardFittingSoftware);
            OpenExplorerCommand = new RelayCommand(OpenExplorer);
            OpenConfigCommand = new RelayCommand(OpenFileEditor);
            ChangeMarketCommand = new RelayCommand(ChangeMarket);
            AddSelectionCommand = new RelayCommand(AddSelection);
            SelectRangeCommand = new RelayCommand(SelectRange);
            ChangeLogLevelCommand = new RelayCommand(ChangeLogLevel);
            DeleteLogsCommand = new RelayCommand(DeleteLogs);
            UninstallCommand = new RelayCommand(Uninstall);
        }

        private async void Uninstall(object obj)
        {
            List<Task> tasks = new List<Task>();
            var count = FittingSoftwares.GetAllSelected().Count();
            if (count > 1)
            {
                foreach (var selected in FittingSoftwares.GetAllSelected())
                {
                    tasks.Add(new Task(() => selected.Value.TryUInstallFSSync()));
                    tasks.Add(new Task(() => selected.Value.TryDeleteTrash()));
                }
                ProcessFSManager.RunQueueTasks(tasks);
            }
            else
            {
                if (obj is FS fittingSoftware)
                {
                    bool UIMode = !Properties.Settings.Default.SilentMode; // false = no UI switch off then no UI so NOT is needed
                    var temp = await fittingSoftware.TryUninstallFs(UIMode);
                    if (temp.Success == false)
                    {
                        //MessageBox.Show(temp.Message);
                        Console.WriteLine(temp.Message);
                    }
                    OnPropertyChanged(nameof(FittingSoftwares));
                }
            }
        }

        private async void ChangeLogLevel(object obj)
        {
            if (FittingSoftwares.CountSelected() > 1)
            {
                bool flag = false;
                string failureMessage = $"Impossible to change Log level in:\n";
                foreach (var selected in FittingSoftwares.GetAllSelected())
                {
                    var succeed = selected.Value.TrySetLogLevel(EnumManager.ToEnum<LogLevels>(selected.Value?.LogLevel));
                    if (!succeed.Sucess)
                        failureMessage += selected.Value.Name + " : " + succeed.Message + "\n";
                }
                if (flag)
                {
                    await DialogCoordinator.ShowMessageAsync(this, "Couldn't change Log level", failureMessage);
                }
            }
            if (obj is FS fittingSoftware)
            {
                var succeed = fittingSoftware.TrySetLogLevel(EnumManager.ToEnum<LogLevels>(fittingSoftware?.LogLevel));
                if (!succeed.Sucess)
                {
                    string failureMessage = $"Impossible to change Log level in: {fittingSoftware.Name} : {succeed.Message}";

                    await DialogCoordinator.ShowMessageAsync(this, "Couldn't change Log level", failureMessage);
                }
            }
        }
        private async void DeleteLogs(object obj)
        {
            if (FittingSoftwares.CountSelected() > 1)
            {
                bool flag = false;
                string failureMessage = $"Impossible to Delete Logs in:\n";
                foreach (var selected in FittingSoftwares.GetAllSelected())
                {
                    var succeed = selected.Value.TryDeleteLogs();
                    if (!succeed.Success)
                        failureMessage += selected.Value.Name + "\n";
                }
                if (flag)
                {
                    await DialogCoordinator.ShowMessageAsync(this, "Couldn't Delete Logs", failureMessage);
                }
            }
            if (obj is FS fittingSoftware)
            {
                var succeed = fittingSoftware.TryDeleteLogs();
                if (!succeed.Success)
                {
                    string failureMessage = $"Impossible to Delete Logs in: {fittingSoftware.Name}";

                    await DialogCoordinator.ShowMessageAsync(this, "Couldn't Delete Logs", $"{failureMessage}\n{succeed.Message}");
                }
            }
        }
        private async void ChangeMarket(object obj)
        {
            if (FittingSoftwares.CountSelected() > 1)
            {
                bool flag = false;
                string failureMessage = $"Impossible to change market in:\n";
                foreach (var selected in FittingSoftwares.GetAllSelected())
                {
                    var succeed = selected.Value.TrySetMarket(EnumManager.ToEnum<MarketsEnum>(selected.Value.ManufacturerInfo?.Market.ShortName));
                    if (!succeed)
                        failureMessage += selected.Value.Name + "\n";
                }
                if (flag)
                {
                    await DialogCoordinator.ShowMessageAsync(this, "Couldn't change market", failureMessage);
                }
            }
            if (obj is FS fittingSoftware)
            {
                var succeed = fittingSoftware.TrySetMarket(EnumManager.ToEnum<MarketsEnum>(fittingSoftware.ManufacturerInfo?.Market.ShortName));
                if (!succeed)
                {
                    string failureMessage = $"Impossible to change market in: {fittingSoftware.Name}";

                    await DialogCoordinator.ShowMessageAsync(this, "Couldn't change market", failureMessage);
                }
            }
        }

        private async void OpenExplorer(object obj)
        {
            if (FittingSoftwares.CountSelected() > 1)
            {
                foreach (var selected in FittingSoftwares.GetAllSelected())
                {
                    if (commonOperations
                            .TryOpenInFileExplorer(Directory.GetParent(selected.Value.paths.exe).FullName) == false)
                    {
                        await DialogCoordinator.ShowMessageAsync(this, "Couldn't open folder",  Directory.GetParent(selected.Value.paths.exe).FullName);
                    }
                }
            }
            if (obj is FS fittingSoftware)
            {
                if (commonOperations
                        .TryOpenInFileExplorer(Directory.GetParent(fittingSoftware.paths.exe).FullName) == false)
                {
                    await DialogCoordinator.ShowMessageAsync(this, "Couldn't open folder", Directory.GetParent(fittingSoftware.paths.exe).FullName);
                }
            }
        }
        private async void OpenFileEditor(object obj)
        {
            if (FittingSoftwares.CountSelected() > 1)
            {
                foreach (var selected in FittingSoftwares.GetAllSelected())
                {
                    if (commonOperations
                            .TryOpenInFileEditor(selected.Value.paths.exe) == false)
                    {
                        await DialogCoordinator.ShowMessageAsync(this, "Couldn't open folder", selected.Value.paths.exe);
                    }
                }
            }
            if (obj is FS fittingSoftware)
            {
                if (commonOperations
                        .TryOpenInFileEditor(fittingSoftware.paths.exe + ".config") == false)
                {
                    await DialogCoordinator.ShowMessageAsync(this, "Couldn't open folder", fittingSoftware.paths.exe + ".config");
                }
            }
        }

        private async void CopyToClipBoardFittingSoftware(object obj)
        {
            if (obj is FS fittingSoftware)
            {
                XmlSerializer serializer = new XmlSerializer(fittingSoftware.GetType());

                StringBuilder builder = new StringBuilder();
                using (var stream = new StringWriter(builder))
                {
                    serializer.Serialize(stream, fittingSoftware);
                    if (commonOperations.TryCopyToClipboard(builder.ToString()) == false)
                    {
                        await DialogCoordinator.ShowMessageAsync(this, "Couldn't copy to clipboard", "");
                    }
                }
            }
        }
    }
}
