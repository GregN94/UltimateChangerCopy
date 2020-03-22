using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using PathsManager;
using FileOperator;
using Manufacturer;
using FittingSoftwareEnums;
using System.ComponentModel;
using log4net;
using ProcessManager;
using Statistics;
using Utils;
using System.Linq;
using System.Timers;
using System.Diagnostics;
using System.Reflection;

namespace FittingSoftware
{
    public class FS : ObservableObject
    {
        public static List<FS> GetListFittingSoftwares()
        {
            return new List<FS>
            {
                new FS(FittingSoftwares.Genie),
                new FS(FittingSoftwares.GenieMedical),
                new FS(FittingSoftwares.ExpressFit),
                new FS(FittingSoftwares.HearSuite),
                new FS(FittingSoftwares.Oasis),
                new FS(FittingSoftwares.Noah4)
            };
        }

        private static readonly ILog Logger = LogManager.GetLogger(typeof(FS));

        public StatisticsManager StatisticsManager;
        public ProcessFSManager ProcessFSManager;

        private Timer Timer_CheckInstanceFS;
        private ManufacturerInfo manufacturerInfo;
        public ManufacturerInfo ManufacturerInfo
        {
            get => manufacturerInfo;
            set
            {
                if (value != this.manufacturerInfo)
                {
                    manufacturerInfo = value;
                    Logger.Debug(manufacturerInfo);
                    OnPropertyChanged();
                }
            }
        }
        private bool fsRunningStatus;
        public bool FSRunningStatus
        {
            get => fsRunningStatus;
            set
            {
                if (value != this.fsRunningStatus)
                {
                    fsRunningStatus = value;
                    Logger.Debug(fsRunningStatus);
                    OnPropertyChanged();
                }
            }
        }

        private bool installed;
        public bool Installed
        {
            get => this.installed;
            set
            {
                if (value != this.installed)
                {
                    this.installed = value;
                    Logger.Debug($"installed: {installed}");
                    FsStatus = GetFsStatus(this.installed);
                    Logger.Debug($"FsStatus: {FsStatus}");
                    OnPropertyChanged();
                }
            }
        }

        public Paths paths;

        private FittingSoftwares name;
        public FittingSoftwares Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        private string logLevel;
        public string LogLevel
        {
            get => logLevel;
            set
            {
                logLevel = value;
                OnPropertyChanged();
            }
        }
        private FsStatusEnum fsStatus;
        public FsStatusEnum FsStatus
        {
            get => fsStatus;
            set
            {
                fsStatus = value;
                OnPropertyChanged();
            }
        }

        FileSystemWatcher watcherExeFile, watcherMarket, watcherApplicationVersion;
        FileSystemWatcher[] watcherLogLevel = new FileSystemWatcher[2];

        public FS() { }

        public FS(FittingSoftwares name, PathsContainer paths)
        {
            Name = name;
            this.paths = paths.Paths[Name];
            ManufacturerInfo = new ManufacturerInfo(paths.Paths[Name]);
        }

        public FS(FittingSoftwares name)
        {
            Name = name;
            this.paths = new PathsManager.PathsContainer().Paths[Name];
            Logger.Debug($"StartUpFS: {Name}");
            new System.Threading.Thread(StartUpFS).Start();
            //StartUpFS();
        }

        // startup methods

        private void StartUpWatchers()
        {
            Logger.Debug($"StartUpWatchers for {name} Start");
            watcherExeFile = new FileSystemWatcher();
            watcherMarket = new FileSystemWatcher();
            watcherApplicationVersion = new FileSystemWatcher();
            watcherLogLevel[0] = new FileSystemWatcher();
            watcherLogLevel[1] = new FileSystemWatcher();

            try
            {
                watcherExeFile.Path = Directory.GetParent(this.paths.exe).FullName;
            }
            catch (Exception)
            {
                watcherExeFile.Path = @"C:\Program Files (x86)\";
                watcherExeFile.IncludeSubdirectories = true;
            }

            try
            {
                watcherMarket.Path = Directory.GetParent(this.paths.ManufacturerInfo).FullName;
            }
            catch (Exception)
            {
                watcherMarket.Path = @"C:\ProgramData";
                watcherMarket.IncludeSubdirectories = true;
            }

            try
            {
                watcherApplicationVersion.Path = Directory.GetParent(this.paths.ApplicationVersion).FullName;
            }
            catch (Exception)
            {
                watcherApplicationVersion.Path = @"C:\ProgramData";
                watcherApplicationVersion.IncludeSubdirectories = true;
            }

            if (File.Exists(this.paths.log4net))
            {
                watcherLogLevel[0].Path = Directory.GetParent(this.paths.log4net).FullName;
                watcherLogLevel[1].Path = @"C:\Program Files (x86)";
                watcherLogLevel[1].IncludeSubdirectories = true;
            }
            else if (File.Exists(this.paths.log4net_Legacy))
            {
                watcherLogLevel[1].Path = Directory.GetParent(this.paths.log4net_Legacy).FullName;
                watcherLogLevel[0].Path = @"C:\ProgramData";
                watcherLogLevel[0].IncludeSubdirectories = true;
            }
            else
            {
                watcherLogLevel[0].Path = @"C:\ProgramData";
                watcherLogLevel[0].IncludeSubdirectories = true;
                watcherLogLevel[1].Path = @"C:\Program Files (x86)";
                watcherLogLevel[1].IncludeSubdirectories = true;
            }

            watcherExeFile.NotifyFilter = NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            watcherMarket.NotifyFilter = NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            watcherApplicationVersion.NotifyFilter = NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            foreach (var item in watcherLogLevel)
            {
                item.NotifyFilter = NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                item.Filter = $"Configure.log4net";

                // Add event handlers.
                item.Created += new FileSystemEventHandler(OnChangedLogLevel);
                item.Deleted += new FileSystemEventHandler(OnChangedLogLevel);
                item.Changed += new FileSystemEventHandler(OnChangedLogLevel);

                // Begin watching.
                item.EnableRaisingEvents = true;
            }

            watcherExeFile.Filter = $"{Name}.exe";
            watcherMarket.Filter = $"ManufacturerInfo.xml";
            watcherApplicationVersion.Filter = $"ApplicationVersion.xml";

            // Add event handlers.
            watcherExeFile.Created += new FileSystemEventHandler(OnChanged);
            watcherExeFile.Deleted += new FileSystemEventHandler(OnChanged);

            watcherMarket.Changed += new FileSystemEventHandler(OnChangedMarket);
            watcherMarket.Created += new FileSystemEventHandler(OnChangedMarket);
            watcherMarket.Deleted += new FileSystemEventHandler(OnChangedMarket);

            watcherApplicationVersion.Created += new FileSystemEventHandler(OnChangedApplicationVersion);
            watcherApplicationVersion.Deleted += new FileSystemEventHandler(OnChangedApplicationVersion);
            watcherApplicationVersion.Changed += new FileSystemEventHandler(OnChangedApplicationVersion);

            // Begin watching.
            watcherExeFile.EnableRaisingEvents = true;
            watcherMarket.EnableRaisingEvents = true;
            watcherApplicationVersion.EnableRaisingEvents = true;

            //------------
            Logger.Debug($"StartUpWatchers for {name} Stop");
        }

        private void StartUpManufacturer()
        {
            try
            {
                ManufacturerInfo = new ManufacturerInfo(paths);
            }
            catch (Exception)
            {
                ManufacturerInfo?.Clear();
            }
        }

        private void StartUpManagers()
        {
            ProcessFSManager = new ProcessFSManager(Name.ToString());
            StatisticsManager = new StatisticsManager(this.Name);
            ProcessFSManager.PropertyChanged += ProcessFSManager_PropertyChanged;
        }

        private void StartUpFileGetters()
        {
            if (Installed && name != FittingSoftwares.Noah4)
            {
                LogLevel = FOperator.getLogMode(this.paths);
                this.paths.uninstall = FOperator.getUninstallPath(ManufacturerInfo?.Brand);
                this.Save(Name + ".xml");
                Logger.Info($"Fitting Software Installed : {Name} and saved in xml file ");
            }
            else
            {
                Logger.Info($"Fitting Software absent : {Name}");
            }

        }
        private void StartUpTimer()
        {
            Timer_CheckInstanceFS = new System.Timers.Timer();
            Timer_CheckInstanceFS.Interval = 30000; // 30 s
                                                    // Hook up the Elapsed event for the timer. 
            Timer_CheckInstanceFS.Elapsed += OnTimed_heckInstanceFS_Event;

            // Have the timer fire repeated events (true is the default)
            Timer_CheckInstanceFS.AutoReset = true;

            // Start the timer
            if (!Installed)
            {
                Timer_CheckInstanceFS.Enabled = true;
            }
        }

        private void OnTimed_heckInstanceFS_Event(object sender, ElapsedEventArgs e)
        {
            //check if exe is available, if yes then restar instance of FS
            if (File.Exists(this.paths.exe))
            {
                this.Timer_CheckInstanceFS.Enabled = false;
                this.ResetFSInstance();
            }
        }

        private void StartUpFS()
        {
            Installed = CheckIfInstalled();
            Logger.Info($"{Name} is Installed: {Installed}");
            FsStatus = GetFsStatus(Installed);
            StartUpManufacturer();
            StartUpTimer();
            StartUpManagers();

            Task.Run(() =>
            {
                StartUpWatchers();
                StartUpFileGetters();
            });

        }

        private FsStatusEnum GetFsStatus(bool installed)
        {
            switch (installed)
            {
                case true:
                    return FsStatusEnum.Installed;

                case false:
                    try
                    {
                        if (Directory.Exists(paths.Trashes[1]) && Directory.EnumerateFiles(paths.Trashes[1]).Any() ||
                            Directory.Exists(paths.Trashes[0]) && Directory.GetDirectories(paths.Trashes[0]).Length > 0)
                        {
                            return FsStatusEnum.Trash;
                        }

                        if (Directory.Exists(paths.ManufacturerInfo))
                        {
                            return FsStatusEnum.Trash;
                        }

                        return FsStatusEnum.Uninstalled;
                    }
                    catch (ArgumentOutOfRangeException y)
                    {
                        if (File.Exists(paths.exe))
                        {
                            return FsStatusEnum.Installed;
                        }

                        return FsStatusEnum.Uninstalled;
                    }
                    catch
                    {
                        return FsStatusEnum.Trash;
                    }
            }
        }

        private void ResetFSInstance()
        {
            Logger.Debug($"Reset FS for {name}");
            StartUpFS();
        }

        //--------------------

        // pobieram informacje z ProcessFSManager i zapisuje informacje statystyczne o czasie dzialania FS
        private void ProcessFSManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ManufacturerInfo == null)
            {
                return;
            }
            if (e.PropertyName == "FSRunningStatus")
            {
                FSRunningStatus = ProcessFSManager.FSRunningStatus;
                return;
            }
            this.StatisticsManager.updateTime(ManufacturerInfo.version, this.ProcessFSManager.ElapsedTime);
            StatisticsManager.Save(Name + "_Stats.xml"); // change to DataBase?? 
        }

        public async Task<(bool Success, string Message)> TryUninstallFs(bool UI = true)
        {
            return await ProcessFSManager.TryRunProcessAsync(this.paths.uninstall, ProcessType.Uninstall, UI);
        }

        //This method is called when a file is created, changed, or deleted.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            //Show that a file has been created, changed, or deleted.
            WatcherChangeTypes wct = e.ChangeType;
            if (wct == WatcherChangeTypes.Created)
            {
                ResetFSInstance();
            }
            else
            {
                Installed = false;
                Timer_CheckInstanceFS.Enabled = true;
                this.ManufacturerInfo.Clear();
                this.FSRunningStatus = false;
            }

            //Console.WriteLine("File {0} {1}", e.FullPath, wct.ToString());
        }

        //This method is called when a file is created, changed, or deleted.
        private void OnChangedMarket(object source, FileSystemEventArgs e)
        {
            //Show that a file has been created, changed, or deleted.
            WatcherChangeTypes wct = e.ChangeType;
            if (wct == WatcherChangeTypes.Changed || wct == WatcherChangeTypes.Created || wct == WatcherChangeTypes.Deleted)
            {
                try
                {
                    this.ManufacturerInfo.Market = FOperator.getMarket(paths);
                    this.ManufacturerInfo.Oem = FOperator.getOem(paths);
                    OnPropertyChanged(nameof(ManufacturerInfo));
                }
                catch (Exception)
                {

                }
            }
        }

        private void OnChangedLogLevel(object sender, FileSystemEventArgs e)
        {
            LogLevel = FOperator.getLogMode(this.paths);
        }

        private void OnChangedApplicationVersion(object sender, FileSystemEventArgs e)
        {
            if (this.ManufacturerInfo == null)
            {
                return;
            }
            this.ManufacturerInfo.version = FOperator.getVersionFS(this.paths);
            this.ManufacturerInfo.versionString = this.ManufacturerInfo.version?.ToString();
            OnPropertyChanged(nameof(ManufacturerInfo));
        }

        public bool TrySetMarket(MarketsEnum idMarket)
        {
            if (FOperator.TrySaveMarket(idMarket, this.paths))
            {
                Logger.Info("Marker changed");
                ManufacturerInfo.Market.ShortName = idMarket.ToString();
                return true;
            }

            Logger.Error($"Can not set market in: {ManufacturerInfo.Brand}");
            return false;
        }

        public async Task<(bool Success, string Message)> TryStartFS(bool recording = false)
        {
            // NotifyPropertyChanged (elapsedTime information about time run FS)
            if (recording)
            {
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = @"C:\Program Files\ShareX\ShareX.exe";
                info.Arguments = " -ScreenRecorder";
                //https://github.com/ShareX/ShareX/blob/master/ShareX/Enums.cs
                Process.Start(info);
            }
            return await ProcessFSManager.TryRunProcessAsync(this.paths.exe, ProcessType.App, true);
        }
        // For > 1 FS
        public Process TryUInstallFSSync()
        {
            return ProcessFSManager.TryGetProcessToRun(this.paths.uninstall, ProcessType.Uninstall, false);
        }
        // For > 1 FS
        public Process TryInstallFSSync(string path)
        {
            return ProcessFSManager.TryGetProcessToRun(path, ProcessType.Install, false);
        }

        private bool CheckIfIsRunning()
        {
            return ProcessManager.ProcessManager.checkRunningProcess_string(this.Name.ToString());
        }

        public bool CheckIfInstalled()
        {
            return FOperator.IfExistFS(this.paths);
        }

        public string GetBrand()
        {
            return this.Name.ToString();
        }

        public (bool Sucess, string Message) TrySetLogLevel(LogLevels idLogLevel)
        {
            var operation = FOperator.TrySaveLogMode(idLogLevel, this.paths);
            if (operation.Sucess)
            {
                Logger.Info("Log level changed");
                LogLevel = idLogLevel.ToString();
                return (true, "Log level changed");
            }

            Logger.Error($"Can not set Log level in: {this.ManufacturerInfo?.Brand}");
            return operation;
        }

        public (bool Success, List<string> Message) TryDeleteTrash()
        {
            if (Installed) // updated by event
            {
                return (false, new List<string>() { "Error, FS installed" }); // FS installed can not delete
            }

            var result = FOperator.TryDeleteTrash(this.paths);
            if (result.Success)
            {
                FsStatus = GetFsStatus(Installed);
                return (true, null);
            }

            return (false, result.Message);
        }

        public (bool Success, string Message) TryDeleteLogs()
        {
            if (!CheckIfIsRunning()) // when not running
            {
                var operation = FOperator.TryDeleteLogs(this.paths);
                if (operation.Success)
                {
                    return (true, "All logs deleted properly");
                }

                return (false, operation.Message);
            }

            return (false, $"FS ({this.Name.ToString()}) is running");
        }

        public async Task<(bool Success, string Message)> TryInstallFS(string path, bool Mode = false)
        {
            if (Installed)
            {
                return (false, "Uninstall FS");
            }

            return await ProcessFSManager.TryRunProcessAsync(path, ProcessType.Install, Mode);
        }

        public (bool Success, string Message) TryUnInstall_Install(string path, bool mode = false)
        {
            return ProcessFSManager.Uninstall_InstallFs(this.paths.uninstall, path, mode).GetAwaiter().GetResult();
        }

        public bool TryKillFS()
        {
            return ProcessManager.ProcessManager.KillFS(Name.ToString());
        }

        public static bool TryStartGearbox(bool workspace = false, string workspacePath = "")
        {
            return ProcessManager.ProcessManager.TryStartGearbox(workspace, workspacePath);
        }

        public static bool TryStartNewPreconditioner()
        {
            return ProcessManager.ProcessManager.TryStartNewPreconditioner();
        }

        public (bool Status, List<string> Messages) CheckIfProd()
        {
            return FOperator.CheckIfProd(this.paths);
        }

        public void Save(string filename)
        {
            using (FileStream stream = new FileStream(filename, FileMode.Create))
            {
                var XML = new XmlSerializer(typeof(FS));
                try
                {
                    XML.Serialize(stream, this);
                }
                catch (Exception x)
                {
                    Logger.Error("Problem with save FittingSfotware information in xml: \n" + x.ToString());
                }
            }
        }

        public static FS ReadFromFile(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open))
            {
                var XML = new XmlSerializer(typeof(FS));
                return (FS)XML.Deserialize(stream);
            }
        }

    }
}
