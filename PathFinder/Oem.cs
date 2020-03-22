using System.IO;
using System.Linq;

namespace PathFinder
{
    public class Oem
    {
        private string setupPath;
        public string Name { get; set; }
        public string Path { get; set; }

        public string SetupPath
        {
            get
            {
                if (setupPath == null)
                {
                    ResolveSetupPath();
                }
                return setupPath;
            }
            set => setupPath = value;
        }

        private void ResolveSetupPath()
        {
            SetupPath = Directory.GetFiles(Path, "setup*", SearchOption.TopDirectoryOnly).First();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
