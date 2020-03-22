using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FittingSoftware;

namespace ApplicationUI.Models
{
    public class InstallatorModel
    {
        public async Task<(bool Success, string Message)> TryInstallFittingSoftwareAsync(string brand, string path, bool mode)
        {
            if (FittingSoftwareRepository.TryGetFittingSoftware(fs => fs.Name.ToString() == brand,
                out var fittingSoftware))
            {
                var result = await fittingSoftware.TryInstallFS(path, mode);
                return result;
            }

            return (false, $"Couldn't find FittingSoftware with name {brand}");
        }
    }
}
