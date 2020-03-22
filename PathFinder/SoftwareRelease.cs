using System.Collections.Generic;

namespace PathFinder
{
    public class SoftwareRelease
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public List<Build> Builds { get; set; } = new List<Build>();

        public override string ToString()
        {
            return Name;
        }
    }
}
