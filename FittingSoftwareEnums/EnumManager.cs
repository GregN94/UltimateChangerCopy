using System;
using System.Collections.Generic;
using System.Linq;

namespace FittingSoftwareEnums
{
    public enum FittingSoftwares : byte
    {
        Genie = 0,
        GenieMedical = 1,
        Oasis = 2,
        ExpressFit = 3,
        HearSuite = 4,
        Noah4 = 5
    };

    public enum FittingSoftwaresExe { Genie2, GenieMedical, Oasis, ExpressFit, Hearsuite };
    public enum LogLevels { DEBUG, ERROR, ALL };
    public enum FsStatusEnum { Installed, Uninstalled, Trash };
    public enum MarketsEnum
    {
        AU, BR, CA, Default, DK, FI, FR, DE, IT, JP, KR, NL, NZ, NO, PL, PT, CN, SG, ZA, ES, SE, CH, UK, US, he
    };
    public enum HiStyleEnum { BTE, Rite, Custom };
    public enum PhysicalSide { Left,Right,Both,NA };
    public enum PricePointEnum
    {
        highest,
        high,
        medium,
        low,
        lowest
    };
    public enum BuildType { RC,Master,IP,NA};

    public static class EnumManager
    {
        public static List<string> GetListMarkets()
        {
            return Enum.GetNames(typeof(MarketsEnum)).ToList();
        }
        public static List<string> GetListLogLevel()
        {
            return Enum.GetNames(typeof(LogLevels)).ToList();
        }

        public static T ToEnum<T>(string input)
        {
            return (T)Enum.Parse(typeof(T), input);
        }
    }
}
