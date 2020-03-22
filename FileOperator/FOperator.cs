using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using PathsManager;
using MarketManager;
using System.Diagnostics;
using FittingSoftwareEnums;
using System.Text.RegularExpressions;
using System.Reflection;

namespace FileOperator
{

    public static class FOperator
    {
        public static bool TrySaveMarket(MarketsEnum newMarket, Paths paths)
        {
            setValueXML(paths.ManufacturerInfo, "/ManufacturerInfo/MarketName", newMarket.ToString());

            if (V_saveMarket(newMarket.ToString(), paths))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static (bool Sucess, string Message) TrySaveLogMode(LogLevels newVelue, Paths paths)
        {
            if (File.Exists(paths.log4net)) // 20.2 release
            {
                var result = setValueXML(paths.log4net, "/configuration/log4net/root/level", newVelue.ToString(), false);
                if (!result.Sucess)
                {
                    return result;
                }
                SetMaximumFileSize(paths.log4net, "/configuration/log4net/appender/maximumFileSize");
            }
            else if (File.Exists(paths.log4net_Legacy))
            {
                var result = setValueXML(paths.log4net_Legacy, "/configuration/log4net/root/level", newVelue.ToString(), false);
                if (!result.Sucess)
                {
                    return result;
                }
                SetMaximumFileSize(paths.log4net_Legacy, "/configuration/log4net/appender/maximumFileSize");
            }
            else
            {
                return (false,"");
            }

            return V_saveLogMode(newVelue, paths);

        }
        private static void SetMaximumFileSize(string Path,string Key)
        {
            setValueXML(Path, Key, "50MB", false);
        }

        public static string getUninstallPath(string Brand)
        {
            var allfulls = Directory.EnumerateFiles(@"C:\ProgramData\Package Cache", "Install.exe", SearchOption.AllDirectories);
            var allmediums = Directory.EnumerateFiles(@"C:\ProgramData\Package Cache", "*Medium*.exe", SearchOption.AllDirectories);

            var allFiles = new List<string>();
            allFiles.AddRange(allfulls);
            allFiles.AddRange(allmediums);

            foreach (var item in allFiles)
            {
                try
                {
                    FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(item);
                    if (Brand.Contains("Medical"))
                    {
                        if ((myFileVersionInfo.CompanyName.Contains("Oticon Medical")))
                        {
                            return item;
                        }
                    }
                    else
                    if ((myFileVersionInfo.CompanyName.Contains(Brand) && !myFileVersionInfo.CompanyName.Contains("Medical")) || 
                        myFileVersionInfo.FileDescription.Contains(Brand))
                    {
                        return item;
                    }
                }
                catch (Exception)
                {
                }
            }
            return "";
        }

        public static bool IfExistFS(Paths paths)
        {
            if (File.Exists(paths.exe))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Market getMarket(Paths paths)
        {
            return new Market(readValueXML(paths.ManufacturerInfo, "/ManufacturerInfo/MarketName"));
        }
        public static string getLogMode(Paths paths)
        {
            if (File.Exists(paths.log4net)) // 20.2 release
            {
                return readValueXML(paths.log4net, "/configuration/log4net/root/level");
            }
            else
            {
                return readValueXML(paths.log4net_Legacy, "/configuration/log4net/root/level");
            }
        }

        public static (bool Success, List<string> Message) TryDeleteTrash(Paths paths)
        {
            var result = deleteTrash(paths);
            if (result.Success)
            {
                return (true, null);
            }
            else
            {
                return (false, result.Message);
            }
        }

        public static (bool Success, List<string> Message) deleteTrash(Paths paths) // true/false and list deleted paths or list paths with error
        {

            foreach (var item in paths.Trashes)
            {
                try
                {
                    Directory.Delete(item, true);
                }
                catch (Exception)
                {

                }

            }
            var result = V_deleteTrash(paths);

            if (result.Success)
            {
                return (result);
            }
            else
            {
                return (false, result.Message);
            }
        }
        public static (bool Success, List<string> Message) V_deleteTrash(Paths paths)
        {
            List<string> errorPaths = new List<string>();
            foreach (var item in paths.Trashes)
            {
                if (Directory.Exists(item))
                {
                    errorPaths.Add(item);
                }
            }
            if (errorPaths.Count > 0)
            {
                return (false, errorPaths);
            }
            return (true, null);

        }

        public static (bool Success, string Message) TryDeleteLogs(Paths paths)
        {
            if (!Directory.Exists(paths.Logs))
            {
                return (false, $"Directory doesnt exist - {paths.Logs}");
            }
            try
            {
                Directory.Delete(paths.Logs, true);
            }
            catch (Exception)
            {
                return (false,"");
            }
            
           
            var result = V_deleteLogs(paths);

            if (result.Success)
            {
                return (result);
            }
            else
            {
                return (false, result.Message);
            }
        }

        private static (bool Success, string Message) V_deleteLogs(Paths paths)
        {
            if (Directory.Exists(paths.Logs))
            {
                return (false, $"Problem! There is a {Directory.GetFiles(paths.Logs).Length} files \n (paths.Logs)");
            }
            else
            {
                return (true, "Done");
            }
        }

        public static string getBrand(Paths paths)
        {
            return readValueXML(paths.ManufacturerInfo, "/ManufacturerInfo/Brand");
        }

        public static string getOem(Paths paths)
        {
            return readValueXML(paths.ManufacturerInfo, "/ManufacturerInfo/OEM");
        }

        public static Version getVersionFS(Paths paths)
        {
            if (File.Exists(paths.ApplicationVersion)) // 20.2 release
            {
                string Major, Minor, Build, Revision;
                Major = readValueXML(paths.ApplicationVersion, "/ApplicationVersion/ProductVersion/Major");
                Minor = readValueXML(paths.ApplicationVersion, "/ApplicationVersion/ProductVersion/Minor");
                Build = readValueXML(paths.ApplicationVersion, "/ApplicationVersion/ProductVersion/Build");
                Revision = readValueXML(paths.ApplicationVersion, "/ApplicationVersion/ProductVersion/Revision");
                try
                {
                    if (Revision == "")
                    {
                        return new Version($"{Major}.{Minor}.{Build}.0");
                    }
                    return new Version($"{Major}.{Minor}.{Build}.{Revision}");
                }
                catch (Exception)
                {
                    //return new Version("0.0.0.0");
                    return null;
                }
            }
            else
            {
                //return new Version("0.0.0.0");
                return null;
            }
        }

        private static bool V_saveMarket(string newMarket, Paths path)
        {
            if (getMarket(path).ShortName == newMarket)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static (bool Success, string Message) V_saveLogMode(LogLevels newVelue, Paths path)
        {
            try
            {
                var operation = getLogMode(path);
                if (operation.Equals(newVelue.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    return (true, "OK");
                }
                else
                {
                    return (false, operation);
                }
            }
            catch (Exception x)
            {
                return (false, x.ToString());
            }
        }

        public static string readValueXML(string path, string Key)
        {
            if (File.Exists(path) == false)
                return string.Empty;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                string value = doc.SelectSingleNode(Key).InnerText;
                if (string.IsNullOrEmpty(value))
                {
                    if (doc.SelectSingleNode(Key).Attributes.Count > 0)
                    {
                        return doc.SelectSingleNode(Key).Attributes[0].Value;
                    }

                    return String.Empty;
                }
                else
                {
                    return value;
                }
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public static (bool Sucess, string Message) setValueXML(string path, string Key, string value, bool InnerTextMode = true)
        {
            if (File.Exists(path) == false)
                return (false,$"{path} - not exist");

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                if (InnerTextMode)
                {
                    doc.SelectSingleNode(Key).InnerText = value;
                }
                else
                {
                    if (doc.SelectSingleNode(Key).Attributes.Count > 0)
                    {
                        doc.SelectSingleNode(Key).Attributes[0].Value = value;
                    }
                }
                //TODO: FAIL/EXCEPTION ON SAVING/CHANGIN LOG LEVEL
                doc.Save(path);
                return (true, "");
            }
            catch (UnauthorizedAccessException exeption)
            {
                return (false, "Unauthorized Access - please run UC as Administrator");
            }
            catch (Exception ex)
            {
                return (false, ex.ToString());
            }
        }

        public static string RecognizeBuildType(Version version)
        {
            if (version == null)
            {
                return "";
            }

            string rootPath = @"\\demant.com\data\KBN\RnD\SWS\Build\Arizona\Phoenix\Phoenix.Installer";
            if (Directory.Exists(rootPath) == false)
                return string.Empty;

            var releasesPaths = Directory.EnumerateDirectories(rootPath);

            foreach (var item in releasesPaths)
            {
                var buildsNames = new DirectoryInfo(item).EnumerateDirectories();
                var buildsNamesString = buildsNames.Select(f => f.Name);

                var resultList = buildsNamesString.Where(b => b.Contains(version.ToString()));
                if (resultList.Any())
                {
                    string pattern = @"(rc-)|(IP([0-9]{0,4}))|(master)";
                    var matched = Regex.Match(resultList.First(), pattern);
                    return matched.Value.Trim(new []{'-'});
                }
            }
            return "";
        }
        public static string RecognizeRelease(Version version)
        {
            if (version == null)
            {
                return "";
            }
            string rootPath = @"\\demant.com\data\KBN\RnD\SWS\Build\Arizona\Phoenix\Phoenix.Installer";
            if (Directory.Exists(rootPath) == false)
                return string.Empty;

            var releasesPaths = Directory.EnumerateDirectories(rootPath);

            foreach (var item in releasesPaths)
            {
                var buildsNames = new DirectoryInfo(item).EnumerateDirectories();
                var buildsNamesString = buildsNames.Select(f => f.Name);

                if (buildsNamesString.Any(b => b.Contains(version.ToString())))
                {
                    return new DirectoryInfo(item).Name;
                }
            }
            return "";
        }

        public static bool checkInstanceNewPreconditioner()
        {
            return File.Exists(@"C:\Program Files (x86)\NewPreconditioner\NewPreconditioner.exe");
        }

        public static bool checkInstanceFakeVerifit()
        {
            return File.Exists(@"C:\Program Files (x86)\REMedy\REMedy.Launcher.exe");
        }

        public static (bool Status, List<string> Messages) CheckIfProd(Paths path)
        {
            List<string> listMesseges = new List<string>();

            var operationHattori = CheckHattori(path.exe);
            if (!operationHattori.Status)
            {
                listMesseges.Add(operationHattori.Message);
            }


            if (listMesseges.Any())            
                return (false, listMesseges);
             else
                return (true,listMesseges);
        }

        private static (bool Status,string Message) CheckHattori(string path)
        {
            string dllString = $"{Directory.GetParent(path).FullName}/Phoenix.Application.BuildInfo.dll";
            try
            {
                Assembly assembly = Assembly.LoadFrom(dllString);
                AppDomain.CurrentDomain.Load(assembly.GetName());
                Type type = assembly.GetType("Wdh.Phoenix.Application.GenieMini.PhoenixVersion");
                object instance = Activator.CreateInstance(type);
                MethodInfo[] methods = type.GetMethods();
                DateTime result = (DateTime)methods[5].Invoke(instance, new object[] {  });
                if (result.Year == 9999 && result.Month == 12)

                    return (true, "");
                else
                    return (false,result.ToShortDateString());
                // var tmp = new t();
            }
            catch (Exception x)
            {
                return (false, x.Message);
            }
        }
    }
}
