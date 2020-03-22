using System;
using System.Collections.Generic;
using System.Linq;
using FittingSoftware;
using FittingSoftwareEnums;

namespace ApplicationUI.Models
{
    public static class FittingSoftwareRepository
    {
        private static IEnumerable<FS> allFittingSoftwares = new List<FS>
        {
            new FS(FittingSoftwares.Genie),
            new FS(FittingSoftwares.GenieMedical),
            new FS(FittingSoftwares.ExpressFit),
            new FS(FittingSoftwares.HearSuite),
            new FS(FittingSoftwares.Oasis),
            new FS(FittingSoftwares.Noah4)
        };

        public static IEnumerable<FS> GetFittingSoftwares()
        {
            return allFittingSoftwares;
        }

        public static bool TryGetFittingSoftware(Predicate<FS> predicate, out FS fittingSoftware)
        {
            if (allFittingSoftwares.Any(f => predicate(f)))
            {
                fittingSoftware = allFittingSoftwares.FirstOrDefault(f => predicate(f));
                return true;
            }

            fittingSoftware = null;
            return false;
        }
    }
}
