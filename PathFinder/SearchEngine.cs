using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utils;

namespace PathFinder
{
    public class SearchEngine : ObservableObject
    {
        private readonly IEnumerable<string> rootPaths;
        private IEnumerable<SoftwareRelease> selectionThree;

        public SearchEngine(IEnumerable<string> rootPaths)
        {
            this.rootPaths = rootPaths;
        }

        public IEnumerable<SoftwareRelease> ResolveSelectionTree()
        {
            string failedLinks = string.Empty;

            this.selectionThree = new List<SoftwareRelease>();

            foreach (var rootPath in rootPaths)
            {
                if (Directory.Exists(rootPath))
                {
                    var releasesDirs = Directory.EnumerateDirectories(rootPath, "*", SearchOption.TopDirectoryOnly);
                    this.selectionThree = this.selectionThree.Concat(releasesDirs
                        .Select(path => new SoftwareRelease { Path = path, Name = path.Split('\\').Last() }));
                }
                else
                {
                    failedLinks += rootPath;
                }
            }

            this.selectionThree = this.selectionThree.AsParallel().Select(release =>
            {
                var buildsPaths = Directory.EnumerateDirectories(release.Path, "*", SearchOption.TopDirectoryOnly)
                    .Where(b => b.Contains("rc") || b.Contains("master") || b.Contains("IP"))
                    .OrderByDescending(Directory.GetCreationTime)
                    .Where(b => Directory.EnumerateDirectories(b).Any()).Take(20);

                release.Builds.AddRange(buildsPaths.Select(p => new Build { Name = p.Split('\\').Last(), Path = p }));
                return release;
            });

            return this.selectionThree;
        }
    }
}
