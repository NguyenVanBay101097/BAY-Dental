using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyERP.Utilities
{
    public static class FloatUtils
    {
        public static double FloatRound(double value, int? precisionDigits = null,
            double? precisionRounding = null, string roundingMethod = "HALF-UP")
        {
            var roundingFactor = FloatCheckPrecision(precisionDigits: precisionDigits, precisionRounding: precisionRounding);
            if (roundingFactor == 0 || value == 0)
                return 0.0;

            // NORMALIZE - ROUND - DENORMALIZE
            // In order to easily support rounding to arbitrary 'steps' (e.g. coin values),
            // we normalize the value before rounding it as an integer, and de-normalize
            // after rounding: e.g. float_round(1.3, precision_rounding=.5) == 1.5

            // TIE-BREAKING: HALF-UP (for normal rounding)
            // We want to apply HALF-UP tie-breaking rules, i.e. 0.5 rounds away from 0.
            // Due to IEE754 float/double representation limits, the approximation of the
            // real value may be slightly below the tie limit, resulting in an error of
            // 1 unit in the last place (ulp) after rounding.
            // For example 2.675 == 2.6749999999999998.
            // To correct this, we add a very small epsilon value, scaled to the
            // the order of magnitude of the value, to tip the tie-break in the right
            // direction.
            // Credit: discussion with OpenERP community members on bug 882036

            var normalizedValue = value / roundingFactor; // normalize
            var epsilonMagnitude = Math.Log(Math.Abs(normalizedValue), 2);
            var epsilon = Math.Pow(2, epsilonMagnitude - 53);
            double roundedValue = 0;
            if (roundingMethod == "HALF-UP")
            {
                normalizedValue += Math.Sign(normalizedValue) * epsilon;
                roundedValue = Math.Round(normalizedValue); //round to integer
            }
            else if (roundingMethod == "UP")
            {
                var sign = Math.Sign(normalizedValue);
                normalizedValue -= sign * epsilon;
                roundedValue = Math.Ceiling(Math.Abs(normalizedValue)) * sign; //ceil to integer
            }

            var result = roundedValue * roundingFactor; //de-normalize
            return result;
        }

        private static double FloatCheckPrecision(int? precisionDigits = null, double? precisionRounding = null)
        {
            //exactly one of precision_digits and precision_rounding must be specified
            if (!precisionDigits.HasValue && !precisionRounding.HasValue)
                throw new Exception("exactly one of precision_digits and precision_rounding must be specified");

            if (precisionDigits.HasValue)
            {
                return Math.Pow(10, -precisionDigits.Value);
            }

            return precisionRounding.Value;
        }

        public static bool FloatIsZero(double value, int? precisionDigits = null, double? precisionRounding = null)
        {
            var epsilon = FloatCheckPrecision(precisionDigits: precisionDigits, precisionRounding: precisionRounding);
            return Math.Abs(FloatRound(value, precisionRounding: epsilon)) < epsilon;
        }

        public static int FloatCompare(double value1, double value2, int? precisionDigits = null, double? precisionRounding = null)
        {
            var roundingFactor = FloatCheckPrecision(precisionDigits: precisionDigits, precisionRounding: precisionRounding);
            value1 = FloatRound(value1, precisionRounding: roundingFactor);
            value2 = FloatRound(value2, precisionRounding: roundingFactor);
            var delta = value1 - value2;
            if (FloatIsZero(delta, precisionRounding: roundingFactor))
                return 0;
            return delta < 0.0 ? -1 : 1;
        }
    }
}
