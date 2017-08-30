using System;

namespace WktToImage
{
    public static class NumericHelper
    {
        public static double? StringToDoubleSafe(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            double value;
            return double.TryParse(input, out value) ? (double?)value : null;
        }

        /// <summary>
        /// Checks whether the two doubles are equal or close to equal
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool AreEqual(double value1, double value2) => Math.Abs(value1 - value2) < 0.001;

        public static bool AreEqual(double? value1, double? value2)
        {
            if (value1.HasValue && value2.HasValue)
            {
                return AreEqual(value1.Value, value2.Value);
            }

            if (value1.HasValue == false && value2.HasValue == false)
            {
                return true;
            }

            return false;
        }
    }
}