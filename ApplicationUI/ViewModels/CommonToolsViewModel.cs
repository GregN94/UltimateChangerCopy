using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using FittingSoftware;
using Microsoft.Win32;
using MahApps.Metro.Controls.Dialogs;
using Utils;

namespace ApplicationUI.ViewModels
{
    public class CommonToolsViewModel : ViewModelBase
    {
        public SelectableCollection<FS> FittingSoftwares { get; set; }

        public ICommand SelectAllCommand { get; set; }
        public ICommand KillCommand { get; set; }
        public ICommand GearBoxCommand { get; set; }
        public ICommand NewPreconditionerCommand { get; set; }
        public ICommand PETCommand { get; set; }
        public ICommand TrashCommand { get; set; }
        public ICommand ConfigStatusCommand { get; set; }
        public IDialogCoordinator DialogCoordinator { get; set; }

        public CommonToolsViewModel(SelectableCollection<FS> fittingSoftwares)
        {
            FittingSoftwares = fittingSoftwares;
            SelectAllCommand = new RelayCommand(SelectAll);
            KillCommand = new RelayCommand(KillFittingSoftwares);
            GearBoxCommand = new RelayCommand(GearBox);
            NewPreconditionerCommand = new RelayCommand(NewPreconditioner);
            PETCommand = new RelayCommand(PET);
            TrashCommand = new RelayCommand(Trash);
            ConfigStatusCommand = new RelayCommand(ConfigurationStatus);
        }

        private async void ConfigurationStatus(object obj)
        {
            foreach (var selected in FittingSoftwares.GetAllSelected())
            {
                var operation = selected.Value.CheckIfProd();
                if (operation.Status)
                {
                    await DialogCoordinator.ShowMessageAsync(this, "Configuation Status", $"{selected.Value.Name} is on PROD {operation.Messages[0]}");
                }
                else
                {
                    await DialogCoordinator.ShowMessageAsync(this, "Configuation Status", $"{selected.Value.Name} is NOT on PROD because: {operation.Messages[0]}");
                }
                
            }
        }


        private void PET(object obj)
        {
            string path = Properties.Settings.Default.PETpath;
            if (!File.Exists(path))
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    path = openFileDialog.FileName;
                    Properties.Settings.Default.PETpath = path;
                    Properties.Settings.Default.Save();
                }
            }
            try
            {
                Process.Start(path);
            }
            catch (Exception)
            {}

        }

        private void Trash(object obj)
        {
            foreach (var selected in FittingSoftwares)
            {
                if (!selected.Value.Installed)
                {
                    selected.Value.TryDeleteTrash();
                }
            }
        }

        private void NewPreconditioner(object obj)
        {
            FS.TryStartNewPreconditioner();
        }

        private void KillFittingSoftwares(object obj)
        {
            foreach (var selected in FittingSoftwares.GetAllSelected())
            {
                selected.Value.TryKillFS();
            }
        }
        private void GearBox(object obj)
        {
           FS.TryStartGearbox();
        }

        private void SelectAll(object obj)
        {
            foreach (var selectable in FittingSoftwares)
            {
                if (selectable.Value.Installed)
                {
                    selectable.ToggleSelection();
                }
            }
        }
    }
}
