using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace ApplicationUI.Models
{
    public class CommonOperations
    {
        public bool TryOpenInFileExplorer(string path)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
                return false;

            if (!Directory.Exists(path))
                return false;
            try
            {
                Process.Start("explorer.exe", path);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        public bool TryOpenInFileEditor(string path)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
                return false;

            if (!File.Exists(path))
                return false;
            try
            {
                //Process.Start("Notepad++.exe", path);

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "Notepad++.exe";
                startInfo.Arguments = "\"" + path + "\"";
                Process.Start(startInfo);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public bool TryCopyToClipboard(string text)
        {
            if (string.IsNullOrEmpty(text) == false)
            {
                Clipboard.SetText(text);
                return true;
            }

            return false;
        }
    }
}
