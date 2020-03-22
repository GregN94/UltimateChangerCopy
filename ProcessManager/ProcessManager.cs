using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ProcessManager
{
    public static class ProcessManager
    {/// <summary>
     /// 
     /// </summary>
     /// <returns></returns>
        public static bool TryStartGearbox(bool workspace = false, string workspacePath = "")
        {
            string PathToGearbox = "C:\\toolsuites\\gearbox\\eclipseg\\";
            if (!Directory.Exists(PathToGearbox))
            {
                return false;
            }
            PathToGearbox += new DirectoryInfo("C:\\toolsuites\\gearbox\\eclipseg").GetDirectories()
                                   .OrderByDescending(d => d.LastWriteTimeUtc).First().ToString() + "\\eclipse.exe";

            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = PathToGearbox;
                if (workspace)
                {
                    processStartInfo.Arguments = $" -data {workspacePath}";
                }
                Process.Start(processStartInfo);
                return true;
            }
            catch (Exception x)
            {
                return false;
            }
        }

        public static bool TrykillRunningProcess(string name)
        {
            Process[] localAll = Process.GetProcesses();
            foreach (Process item in localAll)
            {
                string tmop = item.ProcessName.ToLower();
                if (tmop.Contains(name))
                {
                    item.Kill();
                    return true;
                }
            }
            return false;
        }

        public static bool checkRunningProcess(List<string> name)
        {
            Process[] localAll = Process.GetProcesses();
            foreach (var item_name in name)
            {
                foreach (Process item in localAll)
                {
                    string tmop = item.ProcessName;
                    if (!tmop.Contains("Updater"))
                    {
                        if (tmop == item_name)
                        {
                            return true;
                        }
                        if (tmop.Contains(item_name) && item_name != "Install")
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool checkRunningProcess_string(string name)
        {
            Process[] localAll = Process.GetProcesses();
            //Process[] proc = Process.GetProcessesByName(item_name);     
            foreach (Process item in localAll)
            {
                string tmop = item.ProcessName;
                if (tmop.Contains(name) && !tmop.Contains("Updater"))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool KillFS(string processName) // id of FS need to be change!!!!
        {
            try
            {
                TrykillRunningProcess(processName.ToLower());
                TrykillRunningProcess("firmwareupdater"); // hattori
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool TryStartNewPreconditioner()
        {
            string path = @"C:\Program Files (x86)\NewPreconditioner\NewPreconditioner.exe";
            if (!File.Exists(path))
            {
                return false;
            }
            try
            {
                Process.Start(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
