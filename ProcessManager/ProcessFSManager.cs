using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using FittingSoftwareEnums;
using Utils;
using System.Management;

namespace ProcessManager
{
    public class ProcessFSManager : ObservableObject
    {
        private bool isLocked = false;

        public string ParentName { get; }
        public ProcessType ProcessType { get; set; }

        private int elapsedTime;
        public int ElapsedTime
        {
            get => this.elapsedTime;
            set
            {
                this.elapsedTime = value;
                OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }

        public ProcessFSManager(string parenName)
        {
            ParentName = parenName;
            WaitForProcess();
        }

        public ProcessFSManager() { }

        private bool AnyExternalProcesses(ProcessType type, out string externalProcessName)
        {
            var externalProcesses = Process.GetProcesses().Where(p =>
                p.ProcessName.Contains(ParentName) || p.MainWindowTitle.Contains(ParentName));
            if (externalProcesses.Any())
            {
                externalProcessName = externalProcesses.First().ProcessName;
                return true;
            }

            externalProcessName = null;
            return false;
        }

        public async Task<(bool Success, string Message)> TryRunProcessAsync(string path,
                                                                            ProcessType type, bool UI = true)
        {
            if (isLocked)
            {
                return (false, $"{ParentName} {ProcessType} is running");
            }

            //if (AnyExternalProcesses(type, out var externalProcessName) && type != ProcessType.Uninstall) // bugi...
            //{
            //    return (false, $"External process {externalProcessName} assioctated with {ParentName} is running");
            //}

            if (!System.IO.File.Exists(path))
            {
                return (false, $"File {path} doesn't exist. Make sure you got correct path");
            }

            var parameter = (type, UI) switch
            {
                (ProcessType.Uninstall, true) => "/uninstall",
                (ProcessType.Install, true) => "/install",
                (ProcessType.Uninstall, false) => "/uninstall /quiet",
                (ProcessType.Install, false) => "/install /quiet",
            };

            ProcessType = type;
            isLocked = true;
            var result = await TryRunProcessAsync(path, parameter);
            isLocked = false;
            ProcessType = ProcessType.None;
            return result;
        }

        public Process TryGetProcessToRun(string path,
                                          ProcessType type,
                                          bool UI = true)
        {
            if (!System.IO.File.Exists(path))
            {
                //return (false, $"File {path} doesn't exist. Make sure you got correct path");
                return null;
            }

            var parameter = (type, UI) switch
            {
                (ProcessType.Uninstall, true) => "/uninstall",
                (ProcessType.Install, true) => "/install",
                (ProcessType.Uninstall, false) => "/uninstall /quiet",
                (ProcessType.Install, false) => "/install /quiet",
            };

            return GetProcessForTask(path, parameter);
        }

        private Process GetProcessForTask(string path, string parameter = "")
        {
            var process = new Process
            {
                StartInfo = { FileName = path, CreateNoWindow = true, Arguments = parameter },
                EnableRaisingEvents = true
            };

            process.Start();
            process.WaitForExit();
            return process;
        }

        private async Task<(bool Status, string Message)> TryRunProcessAsync(string path, string parameter = "")
        {
            try
            {
                using (var process = new Process())
                {
                    process.StartInfo.FileName = path;
                    process.StartInfo.CreateNoWindow = true;
                    process.EnableRaisingEvents = true;
                    process.StartInfo.Arguments = parameter;

                    await Task.Run(() =>
                    {
                        process.Start();
                        process.WaitForExit();
                        ElapsedTime = process.TotalProcessorTime.Minutes;
                    });

                    var status = process.ExitCode == 0 ? true : false;
                    return (status, "");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred trying to print \":\n{ex.Message}");
                return (false, ex.ToString());
            }
        }

        //uninstal FS next start Install process 
        public async Task<(bool Success, string Message)> Uninstall_InstallFs(string UninstallPath, string InstallPath, bool UI = false)
        {
            var uninstallResult = await TryRunProcessAsync(UninstallPath, ProcessType.Uninstall, UI);
            if (uninstallResult.Success)
            {
                var installResult = await TryRunProcessAsync(InstallPath, ProcessType.Install, UI);
                return installResult;
            }

            return uninstallResult;
        }

        public void WaitForProcess()
        {
            try
            {
                var startWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
                startWatch.EventArrived += new EventArrivedEventHandler(startWatch_EventArrived);
                startWatch.Start();

                var stopWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
                stopWatch.EventArrived += new EventArrivedEventHandler(stopWatch_EventArrived);
                stopWatch.Start();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void stopWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            var proc = GetProcessInfo(e);
            if (!proc.Contains(TranslateParentNameToProcessName(ParentName)) || proc.Contains("PhilipsHearSuiteUpdaterService"))
            {
                return;
            }
            FSRunningStatus = false;
        }

        private string GetProcessInfo(EventArrivedEventArgs e)
        {
            return e.NewEvent.Properties["ProcessName"].Value.ToString();
        }

        private void startWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            try
            {
                var proc = GetProcessInfo(e);
                if (!proc.Contains(TranslateParentNameToProcessName(ParentName)) || proc.Contains("PhilipsHearSuiteUpdaterService"))
                {
                    return;
                }
                FSRunningStatus = true;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static async void RunQueueTasks(List<Task> tasks)
        {
            foreach (var item in tasks)
            {
                await Task.Run(() =>
                {
                    item.Start();
                    item.Wait();
                });
            }
        }

        private string TranslateParentNameToProcessName(string ParentName)
        {
            return ParentName switch
            {
                nameof(FittingSoftwares.Genie) => "Genie",
                nameof(FittingSoftwares.GenieMedical) => "Genie Medical",
                nameof(FittingSoftwares.ExpressFit) => "EXPRESSfit",
                nameof(FittingSoftwares.HearSuite) => "HearSuite",
                nameof(FittingSoftwares.Oasis) => "Oasis",
                nameof(FittingSoftwares.Noah4) => "Noah4",
                _ => "_none_",
            };
        }
    }

    public enum ProcessType
    {
        None,
        App,
        Install,
        Uninstall
    }
}
