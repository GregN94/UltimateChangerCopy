using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FittingSoftware;
using FittingSoftwareEnums;
using PathFinder;
using ApplicationUI.Models;
using MahApps.Metro.Controls.Dialogs;
using Utils;
using System.Collections.Generic;
using ProcessManager;

namespace ApplicationUI.ViewModels
{
    public class BasicInstallationViewModel : ViewModelBase
    {
        private readonly SelectableCollection<FS> fittingSoftwares;
        private readonly SearchEngine searchEngine;
        private readonly CommonOperations commonOperations;

        public ICommand UninstallCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand InstallCommand { get; set; }
        public ICommand CopyPathCommand { get; set; }
        public ICommand ToggleSilentCommand { get; set; }
        public ICommand OpenInFileExplorerCommand { get; set; }
        public IDialogCoordinator DialogCoordinator { get; set; }

        public ICommand LoadSelectionTreeCommand { get; set; }

        private ObservableCollection<SoftwareRelease> releases;
        public ObservableCollection<SoftwareRelease> Releases
        {
            get => releases;
            set
            {
                releases = value;
                OnPropertyChanged();
            }
        }

        private SoftwareRelease selectedRelease;
        public SoftwareRelease SelectedRelease
        {
            get => selectedRelease;
            set
            {
                selectedRelease = value;
                SelectedBuild = null;
                Builds = value is null ? null : new ObservableCollection<Build>(selectedRelease.Builds);
            }
        }

        private ObservableCollection<Build> builds;
        public ObservableCollection<Build> Builds
        {
            get => builds;
            set
            {
                builds = value;
                OnPropertyChanged();
            }
        }

        private Build selectedBuild;
        public Build SelectedBuild
        {
            get => selectedBuild;
            set
            {
                selectedBuild = value;
                SelectedBrand = null;
                Task.Run((() =>
                {
                    Brands = value is null ? null : new ObservableCollection<Brand>(selectedBuild.Brands);
                    if (Brands?.Count == 1)
                    {
                        SelectedBrand = Brands.First();
                    }
                }));
            }
        }

        private ObservableCollection<Brand> brands;
        public ObservableCollection<Brand> Brands
        {
            get => brands;
            set
            {
                brands = value;
                OnPropertyChanged();
            }
        }

        private Brand selectedBrand;
        public Brand SelectedBrand
        {
            get => selectedBrand;
            set
            {
                selectedBrand = value;
                SelectedOem = null;
                Task.Run(() =>
                {
                    Oems = value is null ? null : new ObservableCollection<Oem>(selectedBrand.Oems);
                    if (Oems?.Count == 1)
                    {
                        SelectedOem = Oems.First();
                    }
                });
            }
        }

        private ObservableCollection<Oem> oems;
        public ObservableCollection<Oem> Oems
        {
            get => oems;
            set
            {
                oems = value;
                OnPropertyChanged();
            }
        }

        private Oem selectedOem;
        public Oem SelectedOem
        {
            get => selectedOem;
            set
            {
                selectedOem = value;
                OnPropertyChanged();
            }
        }

        private bool loading = true;
        public bool Loading
        {
            get => loading;
            set
            {
                loading = value;
                OnPropertyChanged();
            }
        }

        public BasicInstallationViewModel(SelectableCollection<FS> selectableFittingSoftwares, CommonOperations commonOperations)
        {
            OverlayViewModel = null;

            fittingSoftwares = selectableFittingSoftwares;
            this.commonOperations = commonOperations;
            this.searchEngine = new SearchEngine(new[] { Properties.Settings.Default.InstallationPaths });

            UninstallCommand = new RelayCommand(HandleUninstall);
            InstallCommand = new RelayCommand(_ => Install());
            CopyPathCommand = new RelayCommand(_ => CopyPath());
            OpenInFileExplorerCommand = new RelayCommand(_ => OpenInFileExplorer());
            ToggleSilentCommand = new RelayCommand(_ => ToggleSilent());
            RefreshCommand = new RelayCommand(_ => LoadSelectionPaths());
            UpdateCommand = new RelayCommand(_ => Update());

            LoadSelectionTreeCommand = new RelayCommand(_ => LoadSelectionPaths());
        }

        private string TranslateNameToOEM(FittingSoftwares name)
        {
            return name switch
            {
                FittingSoftwares.Genie => "Oticon",
                FittingSoftwares.ExpressFit => "Sonic",
                FittingSoftwares.HearSuite => "Philips",
                FittingSoftwares.Oasis => "Bernafon",
                FittingSoftwares.GenieMedical => "OticonMedical",
                _ => ""
            };
        }

        private void Update()
        {
            List<Task> tasks = new List<Task>();
            if (SelectedBrand == null && selectedBuild != null)
            {
                // add uninstall tasks for all FSs
                foreach (var item in fittingSoftwares)
                {
                    if (item.Value.Installed && item.Value.Name != FittingSoftwares.Noah4)
                    {
                        tasks.Add(new Task(() => item.Value.TryUInstallFSSync())); // uninstall
                    }
                }
                // delete trash
                tasks.Add(new Task(() => fittingSoftwares[0].Value.TryDeleteTrash()));
                // install all FSs with same OEM
                tasks.Add(new Task(() => Properties.Settings.Default.SilentMode = true)); // no UI
                tasks.Add(new Task(() => Properties.Settings.Default.Save()));
                foreach (var item in fittingSoftwares)
                {
                    if (item.Value.Installed && item.Value.Name != FittingSoftwares.Noah4)
                    {
                        tasks.Add(new Task(() => item.Value.TryInstallFSSync(
                            $"{selectedBuild.Path}/Installer-{SelectedRelease.Name}-{item.Value.Name.ToString()}/{TranslateNameToOEM(item.Value.Name)}/setup.exe"
                            )));
                        tasks.Add(new Task(() => TrySetAllLevelLog(item.Value)));
                    }
                }
                tasks.Add(new Task(() => Properties.Settings.Default.SilentMode = false)); // return state before update
                tasks.Add(new Task(() => Properties.Settings.Default.Save()));
            }
            else // selected one FS to update
            {
                tasks.Add(new Task(() => fittingSoftwares
                .First(f => f.Value.Name.Equals(Enum.Parse(typeof(FittingSoftwares), SelectedBrand.Name))).Value.TryUInstallFSSync())); // uninstall
                tasks.Add(new Task(() => fittingSoftwares
                    .First(f => f.Value.Name.Equals(Enum.Parse(typeof(FittingSoftwares), SelectedBrand.Name))).Value.TryDeleteTrash())); // delete trash
                if (Properties.Settings.Default.SilentMode != false) // no ui
                {
                    tasks.Add(new Task(() => Install())); // Install
                }
                else
                {
                    ToggleSilent();// no UI                    
                    tasks.Add(new Task(() => Install())); // Install
                    UnCheckedSilent(); // return state before update
                }
                tasks.Add(new Task(() => TrySetAllLevelLog(fittingSoftwares
                    .First(f => f.Value.Name.Equals(Enum.Parse(typeof(FittingSoftwares), SelectedBrand.Name))).Value)));
            }
            ProcessFSManager.RunQueueTasks(tasks);
        }


        private void UnCheckedSilent()
        {
            Properties.Settings.Default.SilentMode = false;
            Properties.Settings.Default.Save();
        }

        private void ToggleSilent()
        {
            Properties.Settings.Default.SilentMode = true;
            Properties.Settings.Default.Save();
        }

        private async Task ShowErrorMessage(string title, Exception e)
        {
            var settings = new MetroDialogSettings { FirstAuxiliaryButtonText = "Save log" };
            var result = await DialogCoordinator.ShowMessageAsync(this, title, $"Unknown error:\n{e}",
                                                            MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, settings);

            if (result == MessageDialogResult.FirstAuxiliary)
            {
            }
        }

        private void LoadSelectionPaths()
        {
            OverlayViewModel = new LoadingViewModel(this, "Resolving installation paths...");

            try
            {
                var selectionTree = searchEngine.ResolveSelectionTree();
                Releases = new ObservableCollection<SoftwareRelease>(selectionTree);
            }
            catch (Exception e)
            {
                Loading = false;
                LoadingFailure = true;
                OverlayViewModel = new FailedLoadingViewModel(this, "Couldn't load installation paths",
                    e.ToString(), LoadSelectionPaths);
            }

            Loading = false;
            HideOverlay();
        }

        private async void OpenInFileExplorer()
        {
            var selectedPath = ResolveMostInnerPath();
            if (commonOperations.TryOpenInFileExplorer(selectedPath) == false)
            {
                await DialogCoordinator.ShowMessageAsync(this, "Couldn't open path,", "Installation path can't be empty");
            }
        }

        private string ResolveMostInnerPath()
        {
            if (SelectedOem != null)
            {
                return SelectedOem.Path;
            }

            if (SelectedBrand != null)
            {
                return SelectedBrand.Path;
            }

            if (SelectedBuild != null)
            {
                return selectedBuild.Path;
            }

            return SelectedRelease?.Path;
        }

        private async void CopyPath()
        {
            var selectedPath = ResolveMostInnerPath();

            if (commonOperations.TryCopyToClipboard(selectedPath) == false)
            {
                await DialogCoordinator.ShowMessageAsync(this, "Couldn't copy to Clipboard",
                                                        "Installation path can't be empty");
            }
        }

        public async void Install()
        {
            if (SelectedOem == null && SelectedBuild == null) // 0 FS Error
            {
                await DialogCoordinator.ShowMessageAsync(this, "Couldn't Install",
                                                        "Installation path can't be empty");
                return;
            }

            if (SelectedBuild != null && selectedBrand == null) // All FSs when is uninstalled
            {
                List<Task> tasks = new List<Task>();
                // delete trash
                tasks.Add(new Task(() => fittingSoftwares[0].Value.TryDeleteTrash()));
                // install all FSs
                tasks.Add(new Task(() => Properties.Settings.Default.SilentMode = true)); // no UI
                tasks.Add(new Task(() => Properties.Settings.Default.Save()));
                foreach (var item in fittingSoftwares)
                {
                    if (item.Value.Name != FittingSoftwares.Noah4 && !item.Value.Installed)
                    {
                        tasks.Add(new Task(() => item.Value.TryInstallFSSync(
                            $"{selectedBuild.Path}/Installer-{SelectedRelease.Name}-{item.Value.Name.ToString()}/{TranslateNameToOEM(item.Value.Name)}/setup.exe"
                            )));
                        tasks.Add(new Task(() => TrySetAllLevelLog(item.Value)));
                    }
                }
                tasks.Add(new Task(() => Properties.Settings.Default.SilentMode = false)); // return state before update
                tasks.Add(new Task(() => Properties.Settings.Default.Save()));
                await DialogCoordinator.ShowMessageAsync(this, "Installation many FSs",
                                                       $"Genie, Genie Medical, Oasis, ExpressFit and HearSuite new version = {selectedBuild.Name}");
                ProcessFSManager.RunQueueTasks(tasks);
            }
            else if (SelectedOem != null) // 1 FS
            {
                var path = SelectedOem.SetupPath;
                bool UIMode = !Properties.Settings.Default.SilentMode; // false = no UI switch off then no UI so NOT is needed
                var result = await fittingSoftwares
                    .First(f => f.Value.Name.Equals(Enum.Parse(typeof(FittingSoftwares), SelectedBrand.Name))).Value.TryInstallFS(path, UIMode);
                TrySetAllLevelLog(fittingSoftwares
                    .First(f => f.Value.Name.Equals(Enum.Parse(typeof(FittingSoftwares), SelectedBrand.Name))).Value);
                if (result.Success)
                {
                    OnPropertyChanged(nameof(fittingSoftwares));
                }
            }

        }

        public async void HandleUninstall(object obj)
        {
            if (obj is FS fittingSoftware)
            {
                bool UIMode = !Properties.Settings.Default.SilentMode; // false = no UI switch off then no UI so NOT is needed
                var temp = await fittingSoftware.TryUninstallFs(UIMode);
                if (temp.Success == false)
                {
                    // MessageBox.Show(temp.Message);
                    Console.WriteLine(temp.Message);
                }
                OnPropertyChanged(nameof(fittingSoftwares));
            }
        }

        //if user set  Properties.Settings.Default.AllLevelModeInstallation true 
        //then set log level to all 
        private void TrySetAllLevelLog(FS fS)
        {
            if (Properties.Settings.Default.AllLevelModeInstallation)
            {
                fS.TrySetLogLevel(LogLevels.ALL);
            }
        }

        private bool loadingFailure;
        public bool LoadingFailure
        {
            get => loadingFailure;
            set
            {
                loadingFailure = value;
                OnPropertyChanged();
            }
        }
    }
}
