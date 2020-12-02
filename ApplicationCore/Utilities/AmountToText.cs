using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Utilities
{
    public static class AmountToText
    {
        private static readonly string[] to_19 = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy",
            "tám", "chín", "mười", "mười một", "mười hai", "mười ba", "mười bốn", "mười lăm", "mười sáu",
            "mười bảy", "mười tám", "mười chín"
        };

        private static readonly string[] tens = new string[] { "hai mươi", "ba mươi", "bốn mươi", "năm mươi",
            "sáu mươi", "bảy mươi", "tám mươi", "chín mươi"
        };

        private static readonly string[] denom = new string[] { "", "nghìn", "triệu", "tỉ", "nghìn tỉ" };

        //convert a value < 100.
        private static string _convert_nn(long val)
        {
            if (val < 20)
                return to_19[val];
            for (var v = 0; v < tens.Length; v++)
            {
                var dval = 20 + (10 * v);
                var dcap = tens[v];
                if (dval + 10 > val)
                {
                    if (val % 10 > 0)
                    {
                        var donVi = val % 10;
                        var hangChuc = val / 10;
                        if (donVi == 1 && hangChuc >= 2)
                            return dcap + " mốt";
                        if (donVi == 4 && hangChuc >= 2)
                            return dcap + " tư";
                        if (donVi == 5 && hangChuc > 0)
                            return dcap + " lăm";

                        return dcap + " " + to_19[(val % 10)];
                    }

                    return dcap;
                }
            }

            return string.Empty;
        }

        //convert a value < 1000.
        private static string _convert_nnn(long val)
        {
            var word = "";
            var mod = val % 100;
            var rem = (long)Math.Floor((double)val / 100);
            if (rem > 0)
            {
                word = to_19[rem] + " trăm";
                if (mod > 0)
                {
                    word += " ";
                    if (mod < 10)
                        word += "linh ";
                }
            }

            if (mod > 0)
            {
                word += _convert_nn(mod);
            }

            return word;
        }

        private static string vietname_number(long val)
        {

            if (val < 100)
                return _convert_nn(val);
            if (val < 1000)
                return _convert_nnn(val);
            for (var v = 0; v < denom.Length; v++)
            {
                var didx = v - 1;
                var dval = (decimal)Math.Pow(1000, v);
                if (dval > val)
                {
                    var mod = (long)Math.Pow(1000, didx);
                    var l = (long)Math.Floor((double)val / mod);
                    var r = val - (l * mod);
                    var ret = _convert_nnn(l) + " " + denom[didx];
                    if (r > 0)
                        ret = ret + " " + vietname_number(r);
                    return ret;
                }
            }

            return string.Empty;
        }

        public static string amount_to_text(decimal number, string currency = "VND")
        {
            var units_name = "";
            if (string.IsNullOrEmpty(currency) || currency == "VND")
            {
                units_name = "đồng";
                var word = "";
                if (number < 0)
                {
                    word = "âm ";
                    word = word + vietname_number(Math.Abs((long)decimal.Round(number, 0)));
                }
                else
                {
                    word = vietname_number((long)decimal.Round(number, 0));
                }

                var result = word + " " + units_name;

                //in hoa chữ cái đầu tiên
                result = char.ToUpper(result[0]) + result.Substring(1);
                return result;
            }

            return string.Empty;
        }
    }
}
