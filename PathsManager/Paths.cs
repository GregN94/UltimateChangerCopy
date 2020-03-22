using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using FittingSoftwareEnums;

namespace PathsManager
{

    public class PathsContainer
    {
        public Dictionary<FittingSoftwares, Paths> Paths { get; set; } = new Dictionary<FittingSoftwares, Paths>
        {
            {FittingSoftwares.Genie, new OticonPaths()},
            {FittingSoftwares.GenieMedical, new OticonMedicalPaths()},
            {FittingSoftwares.Oasis, new BernafonPaths()},
            {FittingSoftwares.HearSuite, new PhilipsPaths()},
            {FittingSoftwares.ExpressFit, new SonicPaths()},
            {FittingSoftwares.Noah4, new NoahPaths()}
        };
    }

    [XmlInclude(typeof(OticonPaths)), XmlInclude(typeof(OticonPaths)), XmlInclude(typeof(OticonMedicalPaths)), XmlInclude(typeof(BernafonPaths)), XmlInclude(typeof(PhilipsPaths)), XmlInclude(typeof(SonicPaths)), XmlInclude(typeof(NoahPaths))]
    public class Paths
    {
        public string ManufacturerInfo;
        public string log4net;
        public string log4net_Legacy;
        public string exe;
        public string uninstall;
        public string ApplicationVersion;
        public string Logs;
        public List<string> Trashes;

        public Paths()
        {
            uninstall = "";
            Trashes = new List<string>();
        }
    }

    public class OticonPaths : Paths
    {
        public OticonPaths()
        {
            ManufacturerInfo = @"C:\ProgramData\Oticon\Common\ManufacturerInfo.xml";
            log4net = @"C:\ProgramData\Oticon\Genie2\Configure.log4net";
            log4net_Legacy = @"C:\Program Files (x86)\Oticon\Genie\Genie2\Configure.log4net";
            exe = @"C:\Program Files (x86)\Oticon\Genie\Genie2\Genie.exe";
            Logs = @"C:\ProgramData\Oticon\Genie2\Logfiles";
            //exe = @"notepad.exe";
            ApplicationVersion = @"C:\ProgramData\Oticon\Genie2\ApplicationVersion.xml";
            Trashes.Add(@"C:\ProgramData\Oticon");
            Trashes.Add(@"C:\Program Files (x86)\Oticon");
        }
    }
    public class OticonMedicalPaths : Paths
    {
        public OticonMedicalPaths()
        {
            ManufacturerInfo = @"C:\ProgramData\Oticon Medical\Common\ManufacturerInfo.xml";
            log4net = @"C:\ProgramData\Oticon Medical\GenieMedical2\Configure.log4net";
            log4net_Legacy = @"C:\Program Files (x86)\Oticon Medical\Genie Medical BAHS\GenieMedical2\Configure.log4net";
            exe = @"C:\Program Files (x86)\Oticon Medical\Genie Medical BAHS\GenieMedical2\GenieMedical.exe";
            Logs = @"C:\ProgramData\Oticon Medical\Logfiles";
            ApplicationVersion = @"C:\ProgramData\Oticon Medical\GenieMedical2\ApplicationVersion.xml";
            Trashes.Add(@"C:\ProgramData\Oticon Medical");
            Trashes.Add(@"C:\Program Files (x86)\Oticon Medical");
        }
    }
    public class BernafonPaths : Paths
    {
        public BernafonPaths()
        {
            ManufacturerInfo = @"C:\ProgramData\Bernafon\Common\ManufacturerInfo.xml";
            log4net = @"C:\ProgramData\Bernafon\Oasis2\Configure.log4net";
            log4net_Legacy = @"C:\Program Files (x86)\Bernafon\Oasis\Oasis2\Configure.log4net";
            exe = @"C:\Program Files (x86)\Bernafon\Oasis\Oasis2\Oasis.exe";
            Logs = @"C:\ProgramData\Bernafon\Oasis2\Logfiles";
            ApplicationVersion = @"C:\ProgramData\Bernafon\Oasis2\ApplicationVersion.xml";
            Trashes.Add(@"C:\ProgramData\Bernafon");
            Trashes.Add(@"C:\Program Files (x86)\Bernafon");
        }
    }
    public class PhilipsPaths : Paths
    {
        public PhilipsPaths()
        {
            ManufacturerInfo = @"C:\ProgramData\Philips HearSuite\Common\ManufacturerInfo.xml";
            log4net = @"C:\ProgramData\Philips HearSuite\HearSuite2\Configure.log4net";
            log4net_Legacy = @"C:\Program Files (x86)\Philips HearSuite\HearSuite\HearSuite2\Configure.log4net";
            exe = @"C:\Program Files (x86)\Philips HearSuite\HearSuite\HearSuite2\HearSuite.exe";
            Logs = @"C:\ProgramData\Philips HearSuite\HearSuite2\Logfiles";
            ApplicationVersion = @"C:\ProgramData\Philips HearSuite\Hearsuite2\ApplicationVersion.xml";
            Trashes.Add(@"C:\ProgramData\Philips HearSuite");
            Trashes.Add(@"C:\Program Files (x86)\Philips HearSuite");
        }
    }
    public class SonicPaths : Paths
    {
        public SonicPaths()
        {
            ManufacturerInfo = @"C:\ProgramData\Sonic\Common\ManufacturerInfo.xml";
            log4net = @"C:\ProgramData\Sonic\Expressfit2\Configure.log4net";
            log4net_Legacy = @"C:\Program Files (x86)\Sonic\Expressfit\Expressfit2\Configure.log4net";
            exe = @"C:\Program Files (x86)\Sonic\Expressfit\Expressfit2\Expressfit.exe";
            Logs = @"C:\ProgramData\Sonic\Expressfit2\Logfiles";
            ApplicationVersion = @"C:\ProgramData\Sonic\Expressfit2\ApplicationVersion.xml";
            Trashes.Add(@"C:\ProgramData\Sonic");
            Trashes.Add(@"C:\Program Files (x86)\Sonic");
        }
    }
    public class NoahPaths : Paths
    {
        public NoahPaths()
        {
            exe = @"C:\Program Files (x86)\HIMSA\Noah 4\Noah4.exe";
        }
    }
}
