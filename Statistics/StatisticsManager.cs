using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using FittingSoftwareEnums;
using System.Linq;
using Utils;

namespace Statistics
{
    public class KeyValuePair
    {
        public string Key { get; set; }
        public int Value { get; set; }
        public KeyValuePair() { }
        public KeyValuePair(string fs, int value)
        {

            this.Key = fs;
            this.Value = value;

        }
    }

    public class StatisticsManager : ObservableObject
    {
        public StatisticsManager()
        {

        }
        public List<Stats> statsList;
        public DateTime FirstRunDate;
        public DateTime LastRunDate;
        public List<KeyValuePair> AllTimePerVersion;


        private int countDaysInstallationLastFS;
        public int CountDaysInstallationLastFS
        {
            get => countDaysInstallationLastFS;
            set
            {
                if (value != this.countDaysInstallationLastFS)
                {
                    countDaysInstallationLastFS = value;
                    OnPropertyChanged();
                }
            }
        }

        public StatisticsManager(FittingSoftwares FittingSoftwareName)
        {
            var tmpStats = ReadFromFile(FittingSoftwareName + "_Stats.xml"); // if exists 

            if (tmpStats != null)
            {
                this.statsList = tmpStats.statsList;
                this.FirstRunDate = tmpStats.FirstRunDate;
                this.AllTimePerVersion = tmpStats.AllTimePerVersion;
                this.CountDaysInstallationLastFS = tmpStats.CountDaysInstallationLastFS;
            }
            else
            {
                this.statsList = new List<Stats>();
                this.FirstRunDate = DateTime.Now;
                this.LastRunDate = DateTime.Now;
                this.AllTimePerVersion = new List<KeyValuePair>();
                this.CountDaysInstallationLastFS = GetCountDaysInstallationLastFS();
            }

        }

        public int GetCountDaysInstallationLastFS()
        {
            string LastVarsionFS = AllTimePerVersion.LastOrDefault()?.Key;
            if (LastVarsionFS == null)
            {
                return 0;
            }
            var firstRunFS = statsList.Find(x=> x.FS_string==LastVarsionFS);
            if (firstRunFS == null)
            {
                return 0;
            }
            int timeSpan = (DateTime.Now - firstRunFS.EndTime).Days;

            return timeSpan;
        }

        public void updateTime(Version FS, int value)
        {
            this.statsList.Add(new Stats(FS, value, this.statsList.Count));
            this.LastRunDate = DateTime.Now;

            int index = this.AllTimePerVersion.FindIndex(r => r.Key == FS?.ToString());
            if (index >= 0) // key exist - update value
            {
                this.AllTimePerVersion[index].Value += value;
            }
            else // doesnt exist - create new value
            {
                this.AllTimePerVersion.Add(new KeyValuePair(FS?.ToString(), value));
            }
        }


        public void Save(string filename)
        {
            using (FileStream stream = new FileStream(filename, FileMode.Create))
            {
                var XML = new XmlSerializer(typeof(StatisticsManager));
                try
                {
                    XML.Serialize(stream, this);
                }
                catch (Exception x)
                {
                    //Logger.Error("Problem with save FittingSfotware information in xml: \n" + x.ToString());
                }
            }
        }

        public static StatisticsManager ReadFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return null;
            }
            using (FileStream stream = new FileStream(fileName, FileMode.Open))
            {
                var XML = new XmlSerializer(typeof(StatisticsManager));
                try
                {
                    return (StatisticsManager)XML.Deserialize(stream);
                }
                catch (Exception)
                {
                    return null;
                }

            }
        }
    }
}
