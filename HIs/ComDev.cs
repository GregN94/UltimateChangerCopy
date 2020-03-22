using System;
using System.Collections.Generic;
using System.Text;

namespace HIs
{
    class ComDev
    {
        public string name;
        public bool wireless;
    }

    class WirelessComDev : ComDev
    {
    }

    class NoahLink : WirelessComDev
    {
        public NoahLink()
        {
            name = "NoahLink";
            wireless = true;
        }
    }

    class NoahLinkClassic : WirelessComDev
    {
        public NoahLinkClassic()
        {
            name = "NoahLinkClassic";
            wireless = true;
        }
    }

    class FittingLink : WirelessComDev
    {
        public FittingLink()
        {
            name = "FittingLink";
            wireless = true;
        }
    }

    class WiredComDev : ComDev
    {
        public WiredComDev()
        {
        }
    }

    class HiProClassic : WiredComDev
    {
        public HiProClassic()
        {
            name = "HiProClassic";
            wireless = false;
        }
    }

    class HiProUSB : WiredComDev
    {
        public HiProUSB()
        {
            name = "HiProUSB";
            wireless = false;
        }
    }

    class HiPro2 : WiredComDev
    {
        public HiPro2()
        {
            name = "HiPro2";
            wireless = false;
        }
    }

    class ExpressLink : WiredComDev
    {
        public ExpressLink()
        {
            name = "ExpressLink";
            wireless = false;
        }
    }
}
