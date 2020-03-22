using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PathFinder
{
    public class Build
    {
        public string Name { get; set; }
        public string Path { get; set; }

        private List<Brand> brands;
        public List<Brand> Brands
        {
            get
            {
                if (brands == null)
                {
                    ResolveBrands();
                }
                return brands;
            }
            set { brands = value; }
        }

        public void ResolveBrands()
        {
            brands = new List<Brand>();
            var brandPaths = Directory.EnumerateDirectories(Path, "Installer*", SearchOption.TopDirectoryOnly);
            Brands.AddRange(brandPaths.Select(br => new Brand { Name = br.Split('-').Last(), Path = br }));
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
