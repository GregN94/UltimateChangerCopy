using System;
using System.Collections.Generic;
using System.Text;
using FittingSoftwareEnums;

namespace HIs
{
    public class HI
    {
        public HiStyleEnum HiStyle;
        public PhysicalSide PhysicalSide;
        public PricePointEnum PricePoint;

        public HI(PricePointEnum pricePoint, HiStyleEnum hiStyle)
        {
            PricePoint = pricePoint;
            HiStyle = hiStyle;
        }

        public HI(PricePointEnum pricePoint, PhysicalSide HiSide)
        {
            PricePoint = pricePoint;
            PhysicalSide = HiSide;

        }

        public HI()
        {
        }

        public static HI GetRandomHI(PhysicalSide physicalSide = PhysicalSide.NA)
        {
            var random = new Random();
            var pricePoint = (PricePointEnum)random.Next(Enum.GetNames(typeof(PricePointEnum)).Length);

            if (physicalSide == PhysicalSide.NA)
            {
                physicalSide = (PhysicalSide)random.Next(Enum.GetNames(typeof(PhysicalSide)).Length);
            }

            var style = (HiStyleEnum)random.Next(Enum.GetNames(typeof(HiStyleEnum)).Length);

            return new HI { HiStyle = style, PhysicalSide = physicalSide, PricePoint = pricePoint };
        }
    }
}
