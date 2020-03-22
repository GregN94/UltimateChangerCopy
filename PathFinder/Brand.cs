using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PathFinder
{
    public class Brand
    {
        public string Name { get; set; }
        public string Path { get; set; }

        private List<Oem> oems;

        public List<Oem> Oems
        {
            get
            {
                if (oems == null)
                {
                    oems = new List<Oem>();
                    ResolveOems();
                }
                return oems;
            }
            set { oems = value; }
        }

        public void ResolveOems()
        {
            var oems = Directory.EnumerateDirectories(Path, "*", SearchOption.TopDirectoryOnly);
            Oems.AddRange(oems.Where(oem => !oem.Contains("Doc") && !oem.Contains("SingleMsi"))
                .Select(o => new Oem {Name = o.Split('\\').Last(), Path = o}));
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
