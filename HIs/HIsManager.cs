using System;
using System.Collections.Generic;
using System.Text;

namespace HIs
{
    public static class HIsManager
    {
        public static HI RandomOne()
        {
            return new HI();
        }

        public static HI RandomOneOnSide(FittingSoftwareEnums.PhysicalSide Side)
        {
            return HI.GetRandomHI(Side);
        }

        public static HI[] RandomBoth()
        {
            HI[] hIs = new HI[2];
            hIs[0] = HI.GetRandomHI(FittingSoftwareEnums.PhysicalSide.Left);
            hIs[1] = new HI(hIs[0].PricePoint,FittingSoftwareEnums.PhysicalSide.Right);
            return hIs;
        }
    }
}
