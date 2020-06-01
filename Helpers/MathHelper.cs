using System;

namespace AsciiPhoto.Helpers
{
    public static class MathHelper
    {
        public static decimal PercentToDecimal(int percent)
        {
            return (decimal)percent / 100;
        }

        /// <summary>
        /// Examples:
        /// (1000,   60) = 1020
        /// (2500,  999) = 2997
        /// ( 100,   52) =  104
        /// ( 100, 1060) = 1060
        /// </summary>
        /// <param name="original"></param>
        /// <param name="beDivisibleBy"></param>
        /// <returns></returns>
        public static int RoundUpToBeDivisible(int original, int beDivisibleBy)
        {
            if (beDivisibleBy == 0)
            {
                return original;
            }

            if (original % beDivisibleBy == 0)
            {
                return original;
            }

            return ((int)Math.Round((decimal)(original / beDivisibleBy), 0) * beDivisibleBy) + beDivisibleBy;
        }
    }
}