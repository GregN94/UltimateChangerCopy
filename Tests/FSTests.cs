using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using FittingSoftware;
using FittingSoftwareEnums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class FSTests
    {
        private Process myProcess;
        private int elapsedTime;
        private DateTime StartProcessTime;
        private bool eventHandled;

        public void test()
        {
            using (myProcess = new Process())
            {
                try
                {
                    // Start a process to print a file and raise an event when done.
                    myProcess.StartInfo.FileName = "notepad.exe";
                    myProcess.StartInfo.CreateNoWindow = true;
                    myProcess.EnableRaisingEvents = true;
                    myProcess.Exited += new EventHandler(myProcess_Exited);
                    myProcess.Start();
                    StartProcessTime = DateTime.Now;

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred trying to print \":\n{ex.Message}");
                    return;
                }

                // Wait for Exited event, but not more than 30 seconds.
                const int SleepAmount = 100;
                while (!eventHandled)
                {
                    elapsedTime += SleepAmount;
                    if (elapsedTime > 30000)
                    {
                        break;
                    }

                    Thread.Sleep(SleepAmount);
                }
            }
        }
        [TestMethod]
        public void TstRandomHI()
        {
            var tmp = HIs.HIsManager.RandomBoth();

        }

        [TestMethod]
        public void TestFS()
        {
            List<FS> listFS = FS.GetListFittingSoftwares();

            foreach (var fS in listFS)
            {
                Console.WriteLine($"test for: {fS.GetBrand()}");
                if (fS.CheckIfInstalled())
                {
                    Console.WriteLine($"CheckIfInstalled works - {fS.GetBrand()}");
                }
                else
                {
                    Console.WriteLine($"CheckIfInstalled false - {fS.GetBrand()}");
                }
                if (fS.TrySetMarket(MarketsEnum.PL))
                {
                    Console.WriteLine($"TrySetMarket works - {fS.GetBrand()}");
                }
                else
                {
                    Console.WriteLine($"TrySetMarket(3) error - {fS.GetBrand()}");
                }
                if (fS.TrySetLogLevel(LogLevels.ALL).Sucess)
                {
                    Console.WriteLine($"TrySetLogLevel works - {fS.GetBrand()}");
                }
                else
                {
                    Console.WriteLine($"TrySetLogLevel(1) error - {fS.GetBrand()}");
                }
                var result = fS.TryDeleteLogs();
                if (result.Success)
                {
                    Console.WriteLine($"TryDeleteLogs works - {fS.GetBrand()}");
                }
                else
                {
                    Console.WriteLine($"TryDeleteLogs(1) error - {fS.GetBrand()} - {result.Message}");
                }
                var result2 = fS.TryDeleteTrash();
                if (result2.Success)
                {
                    Console.WriteLine($"TryDeleteTrash works - {fS.GetBrand()}");
                }
                else
                {
                    foreach (var item in result2.Message)
                    {
                        Console.Write($"TryDeleteTrash(1) error - {fS.GetBrand()} - {item}");
                    }
                    
                }
                Console.WriteLine();
            }



           
           
           // var tmp = fS.TryDeleteTrash();
            //if (tmp.Success)
           // {
            //    Console.WriteLine($"TryDeleteTrash works {tmp.Message[0]}");
            //}
           // else
           // {
           //     Console.WriteLine($"TryDeleteTrash {tmp.Message[0]}");
            //}
            //var tmp3 = fS.TryDeleteLogs();
            //if (tmp3.Success)
            //{
            //    Console.WriteLine($"TryDeleteLogs works {tmp3.Message}");
            //}
            //else
            //{
            //    Console.WriteLine($"TryDeleteLogs {tmp3.Message}");
            //}
            //var tmpp = fS.TryUninstallFs();
            //if (tmpp.Success)
            //{
            //    Console.WriteLine($"TryUninstallFs works {tmpp.Message[0]}");
            //}
            //else
            //{
            //    Console.WriteLine($"TryUninstallFs {tmpp.Message[0]}");
            //}

            // Wait for Exited event, but not more than 30 seconds.

            //List<FS> fsList = new List<FS>();

            //    fsList.Add(new FS(FittingSoftwares.Genie));
            //    fsList.Add(new FS(FittingSoftwares.GenieMedical));
            //    fsList.Add(new FS(FittingSoftwares.Oasis));
            //    fsList.Add(new FS(FittingSoftwares.ExpressFit));
            //    fsList.Add(new FS(FittingSoftwares.Hearsuite));

            //var tmp = new Program();
            //tmp.test();

        }

        // Handle Exited event and display process information.
        private  void myProcess_Exited(object sender, System.EventArgs e)
        {
            eventHandled = true;
            Console.WriteLine(
                $"Exit time    : {myProcess.ExitTime}\n" +
                $"Exit code    : {myProcess.ExitCode}\n" +
                $"roznica datetime   : {myProcess.ExitTime - StartProcessTime}\n" +
                $"Elapsed time : {elapsedTime}");
        }

    }
}
