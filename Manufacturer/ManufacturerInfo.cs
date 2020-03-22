using FileOperator;
using MarketManager;
using System;
using PathsManager;

namespace Manufacturer
{
    public class ManufacturerInfo
    {
        public string Brand { get; set; }
        public Market Market { get; set; }
        public string Oem { get; set; }
        public Version version { get; set; }
        public string versionString { get; set; }
        public string Release { get; set; }
        public string BuildType { get; set; }

        public ManufacturerInfo(string brand, string oem, Market market, Version version)
        {
            Brand = brand;
            Oem = oem;
            Market = market;
            this.version = version;
            this.versionString = version.ToString();
        }

        public ManufacturerInfo(Paths path)
        {
            Brand = FOperator.getBrand(path);
            Oem = FOperator.getOem(path);
            Market = FOperator.getMarket(path);
            this.version = FOperator.getVersionFS(path);
            this.versionString = version?.ToString();
            this.Release = FOperator.RecognizeRelease(this.version);
            this.BuildType = FOperator.RecognizeBuildType(this.version);
        }
        public void Clear()
        {
            Brand = "";
            Oem = "";
            Market.ShortName = "";
            this.version = null;
            this.versionString = "";
            this.BuildType = "";
        }
        public override string ToString()
        {
            return $"Brand: {Brand}\nOem {Oem}\nMarket {Market.ShortName}\nVersion {versionString}\nRelease {Release}\nBuildType {BuildType}";
        }
        public ManufacturerInfo() { }
    }
}
