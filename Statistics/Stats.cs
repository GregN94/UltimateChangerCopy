using System;
using System.Collections.Generic;
using System.Text;

namespace Statistics
{

    public class Stats
    {
        public Version FS;
        public string FS_string;
        public int minutes;
        public DateTime EndTime;
        public int ID;
        //public FittingSoftwareEnums.FittingSoftwares FittingSoftwareName;


        public Stats()
        {

        }
        public Stats(Version FS, int minutes, int ID)
        {
            this.FS = FS;
            this.FS_string = FS?.ToString();
            this.minutes = minutes;
            this.EndTime = DateTime.Now;
            this.ID = ID;
            // this.FittingSoftwareName = FittingSoftwareName;
        }



    }

}
