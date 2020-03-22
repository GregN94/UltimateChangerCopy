using System.Collections.Generic;
using Utils;


namespace MarketManager 
{
    public class Market : ObservableObject
    {
        public static SortedDictionary<string, string> marketFullNames = new SortedDictionary<string, string>
        {
            { "Australia (AU)", "AU"},
            { "Denmark (DK)", "DK"},
            { "Germany (DE)", "DE"},
            { "United Kingdom (UK)", "UK"},
            { "United States (US)", "US"},
            { "Canada (CA)", "CA"},
            { "Spain (ES)", "ES"},
            { "New Zeland (NZ)", "NZ"},
            { "Switzerland (CH)", "CH"},
            { "Finland (FI)", "FI"},
            { "France (FR)", "FR"},
            { "Italy (IT)", "IT"},
            { "Japan (JP)", "JP"},
            { "Korea (KR)", "KR"},
            { "Norway (NO)", "NO"},
            { "Nederland (NL)", "NL"},
            { "Brazil (BR)", "BR"},
            { "Poland (PL)", "PL"},
            { "Portugal (PT)", "PT"},
            { "Sweden (SE)", "SE"},
            { "Singapore (SG)", "SG"},
            { "PRC China (CN)", "CN"},
            { "South Africa (ZA)", "ZA"},
            { "Default", "Default"}
        };

        public List<string> MarketShortNames { get; set; } = new List<string>()
        {

            {"AU"},
            {"BR"},
            {"CA"},
            {"Default"},
            {"DK"},
            {"FI"},
            {"FR"},
            {"DE"},
            {"IT"},
            {"JP"},
            {"KR"},
            {"NL"},
            {"NZ"},
            {"NO"},
            {"PL"},
            {"PT"},
            {"CN"},
            {"SG"},
            {"ZA"},
            {"ES"},
            {"SE"},
            {"CH"},
            {"UK"},
            {"US"},
            {"he"}
        };

        public Market()
        {
        }

        public Market(string shortName, string longName = "")
        {
            ShortName = shortName;
            LongName = longName;
        }

        private string shortName;

        public string ShortName
        {
            get { return shortName; }
            set { shortName = value;
                OnPropertyChanged();
            }
        }
        
        public string LongName { get; set; }
    }
}
