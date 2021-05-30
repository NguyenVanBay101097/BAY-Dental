using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ApplicationCore.Utilities
{
    public static class StringUtils
    {

        private static readonly string[] VietnameseSigns = new string[]
        {
            "aAeEoOuUiIdDyY",

            "áàạảãâấầậẩẫăắằặẳẵ",

            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

            "éèẹẻẽêếềệểễ",

            "ÉÈẸẺẼÊẾỀỆỂỄ",

            "óòọỏõôốồộổỗơớờợởỡ",

            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

            "úùụủũưứừựửữ",

            "ÚÙỤỦŨƯỨỪỰỬỮ",

            "íìịỉĩ",

            "ÍÌỊỈĨ",

            "đ",

            "Đ",

            "ýỳỵỷỹ",

            "ÝỲỴỶỸ"
        };

        #region Extension Methods

        /// <summary>
        /// Remove signs vietnamese
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveSignVietnamese(this string input)
        {
            if (input == null)
            {
                return input;
            }

            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                {
                    input = input.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
                }
            }
            return input;
        }

        public static string RemoveSignVietnameseV2(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");

            input = input.Normalize(NormalizationForm.FormD);
            return regex.Replace(input, string.Empty).Replace(Convert.ToChar(273), 'd').Replace(Convert.ToChar(272), 'D');
        }

        public static string RemoveVietnamese(string text)
        {
            string result = text.ToLower();
            result = Regex.Replace(result, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ|/g", "a");
            result = Regex.Replace(result, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ|/g", "e");
            result = Regex.Replace(result, "ì|í|ị|ỉ|ĩ|/g", "i");
            result = Regex.Replace(result, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ|/g", "o");
            result = Regex.Replace(result, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ|/g", "u");
            result = Regex.Replace(result, "ỳ|ý|ỵ|ỷ|ỹ|/g", "y");
            result = Regex.Replace(result, "đ", "d");
            return result;
        }


        /// <summary>
        /// Checks whether the string is Null Or Empty
        /// </summary>
        /// <param name="theInput"></param>
        /// <returns></returns>
        public static bool IsNullEmpty(this string theInput)
        {
            return string.IsNullOrEmpty(theInput);
        }

        /// <summary>
        /// Converts the string to Int32
        /// </summary>
        /// <param name="theInput"></param>
        /// <returns></returns>
        public static int ToInt32(this string theInput)
        {
            return !string.IsNullOrEmpty(theInput) ? Convert.ToInt32(theInput) : 0;
        }

        /// <summary>
        /// Removes all line breaks from a string
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static string RemoveLineBreaks(this string lines)
        {
            return lines.Replace("\r\n", "")
                        .Replace("\r", "")
                        .Replace("\n", "");
        }

        //// Gets the full url including 
        //public static string ReturnCurrentDomain()
        //{
        //    var r = HttpContext.Current.Request;
        //    var builder = new UriBuilder(r.Url.Scheme, r.Url.Host, r.Url.Port);
        //    return builder.Uri.ToString().TrimEnd('/');
        //}

        /// <summary>
        /// Removes all line breaks from a string and replaces them with specified replacement
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string ReplaceLineBreaks(this string lines, string replacement)
        {
            return lines.Replace(Environment.NewLine, replacement);
        }

        /// <summary>
        /// Does a case insensitive contains
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ContainsCaseInsensitive(this string source, string value)
        {
            var results = source.IndexOf(value, StringComparison.CurrentCultureIgnoreCase);
            return results != -1;
        }
        #endregion

        #region Social Helpers
        public static string GetGravatarImage(string email, int size)
        {
            return IsValidEmail(email) ? string.Format("http://www.gravatar.com/avatar/{0}?s={1}", md5HashString(email), size) : "";
        }
        #endregion

        public static string md5HashString(string toHash)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            var md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(toHash));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (var i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();  // Return the hexadecimal string.
        }

        public static string Compound2Unicode(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            input = input.Replace("\u0065\u0309", "\u1EBB"); //ẻ
            input = input.Replace("\u0065\u0301", "\u00E9"); //é
            input = input.Replace("\u0065\u0300", "\u00E8"); //è
            input = input.Replace("\u0065\u0323", "\u1EB9"); //ẹ
            input = input.Replace("\u0065\u0303", "\u1EBD"); //ẽ
            input = input.Replace("\u00EA\u0309", "\u1EC3"); //ể
            input = input.Replace("\u00EA\u0301", "\u1EBF"); //ế
            input = input.Replace("\u00EA\u0300", "\u1EC1"); //ề
            input = input.Replace("\u00EA\u0323", "\u1EC7"); //ệ
            input = input.Replace("\u00EA\u0303", "\u1EC5"); //ễ
            input = input.Replace("\u0079\u0309", "\u1EF7"); //ỷ
            input = input.Replace("\u0079\u0301", "\u00FD"); //ý
            input = input.Replace("\u0079\u0300", "\u1EF3"); //ỳ
            input = input.Replace("\u0079\u0323", "\u1EF5"); //ỵ
            input = input.Replace("\u0079\u0303", "\u1EF9"); //ỹ
            input = input.Replace("\u0075\u0309", "\u1EE7"); //ủ
            input = input.Replace("\u0075\u0301", "\u00FA"); //ú
            input = input.Replace("\u0075\u0300", "\u00F9"); //ù
            input = input.Replace("\u0075\u0323", "\u1EE5"); //ụ
            input = input.Replace("\u0075\u0303", "\u0169"); //ũ
            input = input.Replace("\u01B0\u0309", "\u1EED"); //ử
            input = input.Replace("\u01B0\u0301", "\u1EE9"); //ứ
            input = input.Replace("\u01B0\u0300", "\u1EEB"); //ừ
            input = input.Replace("\u01B0\u0323", "\u1EF1"); //ự
            input = input.Replace("\u01B0\u0303", "\u1EEF"); //ữ
            input = input.Replace("\u0069\u0309", "\u1EC9"); //ỉ
            input = input.Replace("\u0069\u0301", "\u00ED"); //í
            input = input.Replace("\u0069\u0300", "\u00EC"); //ì
            input = input.Replace("\u0069\u0323", "\u1ECB"); //ị
            input = input.Replace("\u0069\u0303", "\u0129"); //ĩ
            input = input.Replace("\u006F\u0309", "\u1ECF"); //ỏ
            input = input.Replace("\u006F\u0301", "\u00F3"); //ó
            input = input.Replace("\u006F\u0300", "\u00F2"); //ò
            input = input.Replace("\u006F\u0323", "\u1ECD"); //ọ
            input = input.Replace("\u006F\u0303", "\u00F5"); //õ
            input = input.Replace("\u01A1\u0309", "\u1EDF"); //ở
            input = input.Replace("\u01A1\u0301", "\u1EDB"); //ớ
            input = input.Replace("\u01A1\u0300", "\u1EDD"); //ờ
            input = input.Replace("\u01A1\u0323", "\u1EE3"); //ợ
            input = input.Replace("\u01A1\u0303", "\u1EE1"); //ỡ
            input = input.Replace("\u00F4\u0309", "\u1ED5"); //ổ
            input = input.Replace("\u00F4\u0301", "\u1ED1"); //ố
            input = input.Replace("\u00F4\u0300", "\u1ED3"); //ồ
            input = input.Replace("\u00F4\u0323", "\u1ED9"); //ộ
            input = input.Replace("\u00F4\u0303", "\u1ED7"); //ỗ
            input = input.Replace("\u0061\u0309", "\u1EA3"); //ả
            input = input.Replace("\u0061\u0301", "\u00E1"); //á
            input = input.Replace("\u0061\u0300", "\u00E0"); //à
            input = input.Replace("\u0061\u0323", "\u1EA1"); //ạ
            input = input.Replace("\u0061\u0303", "\u00E3"); //ã
            input = input.Replace("\u0103\u0309", "\u1EB3"); //ẳ
            input = input.Replace("\u0103\u0301", "\u1EAF"); //ắ
            input = input.Replace("\u0103\u0300", "\u1EB1"); //ằ
            input = input.Replace("\u0103\u0323", "\u1EB7"); //ặ
            input = input.Replace("\u0103\u0303", "\u1EB5"); //ẵ
            input = input.Replace("\u00E2\u0309", "\u1EA9"); //ẩ
            input = input.Replace("\u00E2\u0301", "\u1EA5"); //ấ
            input = input.Replace("\u00E2\u0300", "\u1EA7"); //ầ
            input = input.Replace("\u00E2\u0323", "\u1EAD"); //ậ
            input = input.Replace("\u00E2\u0303", "\u1EAB"); //ẫ
            input = input.Replace("\u0045\u0309", "\u1EBA"); //Ẻ
            input = input.Replace("\u0045\u0301", "\u00C9"); //É
            input = input.Replace("\u0045\u0300", "\u00C8"); //È
            input = input.Replace("\u0045\u0323", "\u1EB8"); //Ẹ
            input = input.Replace("\u0045\u0303", "\u1EBC"); //Ẽ
            input = input.Replace("\u00CA\u0309", "\u1EC2"); //Ể
            input = input.Replace("\u00CA\u0301", "\u1EBE"); //Ế
            input = input.Replace("\u00CA\u0300", "\u1EC0"); //Ề
            input = input.Replace("\u00CA\u0323", "\u1EC6"); //Ệ
            input = input.Replace("\u00CA\u0303", "\u1EC4"); //Ễ
            input = input.Replace("\u0059\u0309", "\u1EF6"); //Ỷ
            input = input.Replace("\u0059\u0301", "\u00DD"); //Ý
            input = input.Replace("\u0059\u0300", "\u1EF2"); //Ỳ
            input = input.Replace("\u0059\u0323", "\u1EF4"); //Ỵ
            input = input.Replace("\u0059\u0303", "\u1EF8"); //Ỹ
            input = input.Replace("\u0055\u0309", "\u1EE6"); //Ủ
            input = input.Replace("\u0055\u0301", "\u00DA"); //Ú
            input = input.Replace("\u0055\u0300", "\u00D9"); //Ù
            input = input.Replace("\u0055\u0323", "\u1EE4"); //Ụ
            input = input.Replace("\u0055\u0303", "\u0168"); //Ũ
            input = input.Replace("\u01AF\u0309", "\u1EEC"); //Ử
            input = input.Replace("\u01AF\u0301", "\u1EE8"); //Ứ
            input = input.Replace("\u01AF\u0300", "\u1EEA"); //Ừ
            input = input.Replace("\u01AF\u0323", "\u1EF0"); //Ự
            input = input.Replace("\u01AF\u0303", "\u1EEE"); //Ữ
            input = input.Replace("\u0049\u0309", "\u1EC8"); //Ỉ
            input = input.Replace("\u0049\u0301", "\u00CD"); //Í
            input = input.Replace("\u0049\u0300", "\u00CC"); //Ì
            input = input.Replace("\u0049\u0323", "\u1ECA"); //Ị
            input = input.Replace("\u0049\u0303", "\u0128"); //Ĩ
            input = input.Replace("\u004F\u0309", "\u1ECE"); //Ỏ
            input = input.Replace("\u004F\u0301", "\u00D3"); //Ó
            input = input.Replace("\u004F\u0300", "\u00D2"); //Ò
            input = input.Replace("\u004F\u0323", "\u1ECC"); //Ọ
            input = input.Replace("\u004F\u0303", "\u00D5"); //Õ
            input = input.Replace("\u01A0\u0309", "\u1EDE"); //Ở
            input = input.Replace("\u01A0\u0301", "\u1EDA"); //Ớ
            input = input.Replace("\u01A0\u0300", "\u1EDC"); //Ờ
            input = input.Replace("\u01A0\u0323", "\u1EE2"); //Ợ
            input = input.Replace("\u01A0\u0303", "\u1EE0"); //Ỡ
            input = input.Replace("\u00D4\u0309", "\u1ED4"); //Ổ
            input = input.Replace("\u00D4\u0301", "\u1ED0"); //Ố
            input = input.Replace("\u00D4\u0300", "\u1ED2"); //Ồ
            input = input.Replace("\u00D4\u0323", "\u1ED8"); //Ộ
            input = input.Replace("\u00D4\u0303", "\u1ED6"); //Ỗ
            input = input.Replace("\u0041\u0309", "\u1EA2"); //Ả
            input = input.Replace("\u0041\u0301", "\u00C1"); //Á
            input = input.Replace("\u0041\u0300", "\u00C0"); //À
            input = input.Replace("\u0041\u0323", "\u1EA0"); //Ạ
            input = input.Replace("\u0041\u0303", "\u00C3"); //Ã
            input = input.Replace("\u0102\u0309", "\u1EB2"); //Ẳ
            input = input.Replace("\u0102\u0301", "\u1EAE"); //Ắ
            input = input.Replace("\u0102\u0300", "\u1EB0"); //Ằ
            input = input.Replace("\u0102\u0323", "\u1EB6"); //Ặ
            input = input.Replace("\u0102\u0303", "\u1EB4"); //Ẵ
            input = input.Replace("\u00C2\u0309", "\u1EA8"); //Ẩ
            input = input.Replace("\u00C2\u0301", "\u1EA4"); //Ấ
            input = input.Replace("\u00C2\u0300", "\u1EA6"); //Ầ
            input = input.Replace("\u00C2\u0323", "\u1EAC"); //Ậ
            input = input.Replace("\u00C2\u0303", "\u1EAA"); //Ẫ
            return input;
        }

        /// <summary>
        /// Checks to see if the string passed in is a valid email address
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string strIn)
        {
            if (strIn.IsNullEmpty())
            {
                return false;
            }

            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn,
                   @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
        }

        /// <summary>
        /// Converts a csv list of string guids into a real list of guids
        /// </summary>
        /// <param name="csv"></param>
        /// <returns></returns>
        public static List<Guid> CsvIdConverter(string csv)
        {
            return csv.TrimStart(',').TrimEnd(',').Split(',').Select(Guid.Parse).ToList();
        }


        /// <summary>
        /// Downloads a web page and returns the HTML as a string
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static HttpWebResponse DownloadWebPage(string url)
        {
            var ub = new UriBuilder(url);
            var request = (HttpWebRequest)WebRequest.Create(ub.Uri);
            request.Proxy = null;
            return (HttpWebResponse)request.GetResponse();
        }

        #region Numeric Helpers
        /// <summary>
        /// Strips numeric charators from a string
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string StripNonNumerics(string source)
        {
            var digitRegex = new Regex(@"[^\d]");
            return digitRegex.Replace(source, "");
        }

        /// <summary>
        /// Checks to see if the object is numeric or not
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(object expression)
        {
            double retNum;
            var isNum = Double.TryParse(Convert.ToString(expression), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }
        #endregion

        #region String content helpers

        private static readonly Random _rng = new Random();
        private const string _chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string RandomString(int size)
        {
            var buffer = new char[size];
            for (var i = 0; i < size; i++)
            {
                buffer[i] = _chars[_rng.Next(_chars.Length)];
            }
            return new string(buffer);
        }

        public static string RandomStringDigit(int size)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < size; i++)
            {
                builder.Append(_rng.Next(0, 10));
            }
            return builder.ToString();
        }

        /// <summary>
        /// Returns the number of occurances of one string within another
        /// </summary>
        /// <param name="text"></param>
        /// <param name="stringToFind"></param>
        /// <returns></returns>
        public static int NumberOfOccurrences(string text, string stringToFind)
        {
            if (text == null || stringToFind == null)
            {
                return 0;
            }

            var reg = new Regex(stringToFind, RegexOptions.IgnoreCase);

            return reg.Matches(text).Count;
        }

        /// <summary>
        /// reverses a string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StringReverse(string str)
        {
            var len = str.Length;
            var arr = new char[len];
            for (var i = 0; i < len; i++)
            {
                arr[i] = str[len - 1 - i];
            }
            return new string(arr);
        }

        /// <summary>
        /// Returns a capitalised version of words in the string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CapitalizeWords(string value)
        {
            if (value == null)
                return null;
            if (value.Length == 0)
                return value;

            var result = new StringBuilder(value);
            result[0] = char.ToUpper(result[0]);
            for (var i = 1; i < result.Length; ++i)
            {
                if (char.IsWhiteSpace(result[i - 1]))
                    result[i] = char.ToUpper(result[i]);
                else
                    result[i] = char.ToLower(result[i]);
            }
            return result.ToString();
        }


        /// <summary>
        /// Returns the amount of individual words in a string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int CountWordsInString(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }
            var tmpStr = text.Replace("\t", " ").Trim();
            tmpStr = tmpStr.Replace("\n", " ");
            tmpStr = tmpStr.Replace("\r", " ");
            while (tmpStr.IndexOf("  ") != -1)
                tmpStr = tmpStr.Replace("  ", " ");
            return tmpStr.Split(' ').Length;
        }

        /// <summary>
        /// Returns a specified amount of words from a string
        /// </summary>
        /// <param name="text"></param>
        /// <param name="wordAmount"></param>
        /// <returns></returns>
        public static string ReturnAmountWordsFromString(string text, int wordAmount)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            string tmpStr;
            string[] stringArray;
            var tmpStrReturn = "";
            tmpStr = text.Replace("\t", " ").Trim();
            tmpStr = tmpStr.Replace("\n", " ");
            tmpStr = tmpStr.Replace("\r", " ");

            while (tmpStr.IndexOf("  ") != -1)
            {
                tmpStr = tmpStr.Replace("  ", " ");
            }
            stringArray = tmpStr.Split(' ');

            if (stringArray.Length < wordAmount)
            {
                wordAmount = stringArray.Length;
            }
            for (int i = 0; i < wordAmount; i++)
            {
                tmpStrReturn += stringArray[i] + " ";
            }
            return tmpStrReturn;
        }

        /// <summary>
        /// Returns a string to do a related question/search lookup
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public static string ReturnSearchString(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return searchTerm;
            }

            // Lower case
            searchTerm = searchTerm.ToLower();

            // Firstly strip non alpha numeric charactors out
            searchTerm = Regex.Replace(searchTerm, @"[^\w\.@\- ]", "");

            // Now strip common words out and retun the final result
            return string.Join(" ", searchTerm.Split().Where(w => !CommonWords().Contains(w)).ToArray());
        }

        /// <summary>
        /// Returns a list of the most common english words
        /// TODO: Need to put this in something so people can add other language lists of common words
        /// </summary>
        /// <returns></returns>
        public static IList<string> CommonWords()
        {
            return new List<string>
                {
                    "the", "be",  "to",
                    "of",
                    "and",
                    "a",
                    "in",
                    "that",
                    "have",
                    "i",
                    "it",
                    "for",
                    "not",
                    "on",
                    "with",
                    "he",
                    "as",
                    "you",
                    "do",
                    "at",
                    "this",
                    "but",
                    "his",
                    "by",
                    "from",
                    "they",
                    "we",
                    "say",
                    "her",
                    "she",
                    "or",
                    "an",
                    "will",
                    "my",
                    "one",
                    "all",
                    "would",
                    "there",
                    "their",
                    "what",
                    "so",
                    "up",
                    "out",
                    "if",
                    "about",
                    "who",
                    "get",
                    "which",
                    "go",
                    "me",
                    "when",
                    "make",
                    "can",
                    "like",
                    "time",
                    "no",
                    "just",
                    "him",
                    "know",
                    "take",
                    "people",
                    "into",
                    "year",
                    "your",
                    "good",
                    "some",
                    "could",
                    "them",
                    "see",
                    "other",
                    "than",
                    "then",
                    "now",
                    "look",
                    "only",
                    "come",
                    "its",
                    "over",
                    "think",
                    "also",
                    "back",
                    "after",
                    "use",
                    "two",
                    "how",
                    "our",
                    "work",
                    "first",
                    "well",
                    "way",
                    "even",
                    "new",
                    "want",
                    "because",
                    "any",
                    "these",
                    "give",
                    "day",
                    "most",
                    "cant",
                    "us"
                };
        }

        #endregion

        //#region Sanitising

        ///// <summary>
        ///// Strips all non alpha/numeric charators from a string
        ///// </summary>
        ///// <param name="strInput"></param>
        ///// <param name="replaceWith"></param>
        ///// <returns></returns>
        //public static string StripNonAlphaNumeric(string strInput, string replaceWith)
        //{
        //    strInput = Regex.Replace(strInput, "[^\\w]", replaceWith);
        //    strInput = strInput.Replace(string.Concat(replaceWith, replaceWith, replaceWith), replaceWith)
        //                        .Replace(string.Concat(replaceWith, replaceWith), replaceWith)
        //                        .TrimStart(Convert.ToChar(replaceWith))
        //                        .TrimEnd(Convert.ToChar(replaceWith));
        //    return strInput;
        //}

        ///// <summary>
        ///// Get the current users IP address
        ///// </summary>
        ///// <returns></returns>
        //public static string GetUsersIpAddress()
        //{
        //    var context = HttpContext.Current;
        //    var serverName = context.Request.ServerVariables["SERVER_NAME"];
        //    if (serverName.ToLower().Contains("localhost"))
        //    {
        //        return serverName;
        //    }
        //    var ipList = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        //    return !string.IsNullOrEmpty(ipList) ? ipList.Split(',')[0] : HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        //}

        ///// <summary>
        ///// Used to pass all string input in the system  - Strips all nasties from a string/html
        ///// </summary>
        ///// <param name="html"></param>
        ///// <returns></returns>
        //public static string GetSafeHtml(string html)
        //{
        //    // Scrub html
        //    html = ScrubHtml(html);

        //    // remove unwanted html
        //    html = RemoveUnwantedTags(html);

        //    return html;
        //}


        ///// <summary>
        ///// Takes in HTML and returns santized Html/string
        ///// </summary>
        ///// <param name="html"></param>
        ///// <returns></returns>
        //public static string ScrubHtml(string html)
        //{
        //    if (string.IsNullOrEmpty(html))
        //    {
        //        return html;
        //    }

        //    var doc = new HtmlDocument();
        //    doc.LoadHtml(html);

        //    //Remove potentially harmful elements
        //    var nc = doc.DocumentNode.SelectNodes("//script|//link|//iframe|//frameset|//frame|//applet|//object|//embed");
        //    if (nc != null)
        //    {
        //        foreach (var node in nc)
        //        {
        //            node.ParentNode.RemoveChild(node, false);

        //        }
        //    }

        //    //remove hrefs to java/j/vbscript URLs
        //    nc = doc.DocumentNode.SelectNodes("//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'javascript')]|//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'jscript')]|//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'vbscript')]");
        //    if (nc != null)
        //    {

        //        foreach (var node in nc)
        //        {
        //            node.SetAttributeValue("href", "#");
        //        }
        //    }

        //    //remove img with refs to java/j/vbscript URLs
        //    nc = doc.DocumentNode.SelectNodes("//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'javascript')]|//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'jscript')]|//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'vbscript')]");
        //    if (nc != null)
        //    {
        //        foreach (var node in nc)
        //        {
        //            node.SetAttributeValue("src", "#");
        //        }
        //    }

        //    //remove on<Event> handlers from all tags
        //    nc = doc.DocumentNode.SelectNodes("//*[@onclick or @onmouseover or @onfocus or @onblur or @onmouseout or @ondoubleclick or @onload or @onunload or @onerror]");
        //    if (nc != null)
        //    {
        //        foreach (var node in nc)
        //        {
        //            node.Attributes.Remove("onFocus");
        //            node.Attributes.Remove("onBlur");
        //            node.Attributes.Remove("onClick");
        //            node.Attributes.Remove("onMouseOver");
        //            node.Attributes.Remove("onMouseOut");
        //            node.Attributes.Remove("onDoubleClick");
        //            node.Attributes.Remove("onLoad");
        //            node.Attributes.Remove("onUnload");
        //            node.Attributes.Remove("onError");
        //        }
        //    }

        //    // remove any style attributes that contain the word expression (IE evaluates this as script)
        //    nc = doc.DocumentNode.SelectNodes("//*[contains(translate(@style, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'expression')]");
        //    if (nc != null)
        //    {
        //        foreach (var node in nc)
        //        {
        //            node.Attributes.Remove("stYle");
        //        }
        //    }

        //    return doc.DocumentNode.WriteTo();
        //}

        //public static string RemoveUnwantedTags(string html)
        //{

        //    if (string.IsNullOrEmpty(html))
        //    {
        //        return html;
        //    }

        //    var unwantedTagNames = new List<string>
        //    {
        //        "span",
        //        "div"
        //    };

        //    var htmlDoc = new HtmlDocument();

        //    // load html
        //    htmlDoc.LoadHtml(html);

        //    var tags = (from tag in htmlDoc.DocumentNode.Descendants()
        //                where unwantedTagNames.Contains(tag.Name)
        //                select tag).Reverse();


        //    // find formatting tags
        //    foreach (var item in tags)
        //    {
        //        if (item.PreviousSibling == null)
        //        {
        //            // Prepend children to parent node in reverse order
        //            foreach (var node in item.ChildNodes.Reverse())
        //            {
        //                item.ParentNode.PrependChild(node);
        //            }
        //        }
        //        else
        //        {
        //            // Insert children after previous sibling
        //            foreach (var node in item.ChildNodes)
        //            {
        //                item.ParentNode.InsertAfter(node, item.PreviousSibling);
        //            }
        //        }

        //        // remove from tree
        //        item.Remove();
        //    }

        //    // return transformed doc
        //    return htmlDoc.DocumentNode.WriteContentTo().Trim();
        //}

        ///// <summary>
        ///// Url Encodes a string using the XSS library
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //public static string UrlEncode(string input)
        //{
        //    if (!string.IsNullOrEmpty(input))
        //    {
        //        return Microsoft.Security.Application.Encoder.UrlEncode(input);
        //    }
        //    return input;
        //}

        ///// <summary>
        ///// Decode a url
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //public static string UrlDecode(string input)
        //{
        //    if (!string.IsNullOrEmpty(input))
        //    {
        //        return HttpUtility.UrlDecode(input);
        //    }
        //    return input;
        //}

        ///// <summary>
        ///// decode a chunk of html or url
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //public static string HtmlDecode(string input)
        //{
        //    if (!string.IsNullOrEmpty(input))
        //    {
        //        return HttpUtility.HtmlDecode(input);
        //    }
        //    return input;
        //}

        ///// <summary>
        ///// Uses regex to strip HTML from a string
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //public static string StripHtmlFromString(string input)
        //{
        //    if (!string.IsNullOrEmpty(input))
        //    {
        //        input = Regex.Replace(input, @"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>", string.Empty, RegexOptions.Singleline);
        //        input = Regex.Replace(input, @"\[[^]]+\]", "");
        //    }
        //    return input;
        //}

        //public static string ConvertPlainTextToHtml(string text)
        //{
        //    if (String.IsNullOrEmpty(text))
        //        return string.Empty;

        //    text = text.Replace("\r\n", "<br />");
        //    text = text.Replace("\r", "<br />");
        //    text = text.Replace("\n", "<br />");
        //    text = text.Replace("\t", "&nbsp;&nbsp;");
        //    text = text.Replace("  ", "&nbsp;&nbsp;");

        //    return text;
        //}

        ///// <summary>
        ///// Returns safe plain text using XSS library
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //public static string SafePlainText(string input)
        //{
        //    if (!string.IsNullOrEmpty(input))
        //    {
        //        input = StripHtmlFromString(input);
        //        input = GetSafeHtml(input);
        //    }
        //    return input;
        //}
        //#endregion

        #region Html Element Helpers
        /// <summary>
        /// Returns a HTML link
        /// </summary>
        /// <param name="href"></param>
        /// <param name="anchortext"></param>
        /// <param name="openinnewwindow"></param>
        /// <returns></returns>
        public static string ReturnHtmlLink(string href, string anchortext, bool openinnewwindow = false)
        {
            return string.Format(openinnewwindow ? "<a rel='nofollow' target='_blank' href=\"{0}\">{1}</a>" : "<a rel='nofollow' href=\"{0}\">{1}</a>", href, anchortext);
        }

        public static string CheckLinkHasHttp(string url)
        {
            return !url.Contains("http://") ? string.Concat("http://", url) : url;
        }

        /// <summary>
        /// Returns a HTML image tag
        /// </summary>
        /// <param name="url"></param>
        /// <param name="alt"></param>
        /// <returns></returns>
        public static string ReturnImageHtml(string url, string alt)
        {
            return string.Format("<img src=\"{0}\" alt=\"{1}\" />", url, alt);
        }
        #endregion


        /// <summary>
        /// Creates a URL freindly string, good for SEO
        /// </summary>
        /// <param name="strInput"></param>
        /// <param name="replaceWith"></param>
        /// <returns></returns>
        //public static string CreateUrl(string strInput, string replaceWith)
        //{
        //    // Doing this to stop the urls getting encoded
        //    var url = RemoveAccents(strInput);
        //    return StripNonAlphaNumeric(url, replaceWith).ToLower();
        //}

        public static string RemoveAccents(string input)
        {
            // Replace accented characters for the closest ones:
            //var from = "ÂÃÄÀÁÅÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖØÙÚÛÜÝàáâãäåçèéêëìíîïðñòóôõöøùúûüýÿ".ToCharArray();
            //var to = "AAAAAACEEEEIIIIDNOOOOOOUUUUYaaaaaaceeeeiiiidnoooooouuuuyy".ToCharArray();
            //for (var i = 0; i < from.Length; i++)
            //{
            //    input = input.Replace(from[i], to[i]);
            //}

            //// Thorn http://en.wikipedia.org/wiki/%C3%9E
            //input = input.Replace("Þ", "TH");
            //input = input.Replace("þ", "th");

            //// Eszett http://en.wikipedia.org/wiki/%C3%9F
            //input = input.Replace("ß", "ss");

            //// AE http://en.wikipedia.org/wiki/%C3%86
            //input = input.Replace("Æ", "AE");
            //input = input.Replace("æ", "ae");

            //return input;


            var stFormD = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var t in stFormD)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(t);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(t);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));

        }

        /// <summary>
        /// Returns UK formatted amount from int
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string FormatCurrency(int? amount)
        {
            return amount != null ? string.Format("{0:C}", amount) : "n/a";
        }

        #region Rich Text Formatting
        /// <summary>
        /// Converts markdown into HTML
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        //public static string ConvertMarkDown(string str)
        //{
        //    var md = new MarkdownSharp.Markdown { AutoHyperlink = true, LinkEmails = false };
        //    return md.Transform(str);
        //}

        public static string EmbedVideosInPosts(string str)
        {
            // YouTube Insert Video, just add the video ID and it inserts video into post
            var exp = new Regex(@"\[youtube\]([^\]]+)\[/youtube\]");
            str = exp.Replace(str, "<div class=\"video-container\"><iframe title=\"YouTube video player\" width=\"500\" height=\"281\" src=\"http://www.youtube.com/embed/$1\" frameborder=\"0\" allowfullscreen></iframe></div>");

            // YouTube Insert Video, just add the video ID and it inserts video into post
            exp = new Regex(@"\[vimeo\]([^\]]+)\[/vimeo\]");
            str = exp.Replace(str, "<div class=\"video-container\"><iframe src=\"http://player.vimeo.com/video/$1?portrait=0\" width=\"500\" height=\"281\" frameborder=\"0\"></iframe></div>");

            // YouTube Screenr Video, just add the video ID and it inserts video into post
            exp = new Regex(@"\[screenr\]([^\]]+)\[/screenr\]");
            str = exp.Replace(str, "<div class=\"video-container\"><iframe src=\"http://www.screenr.com/embed/$1\" width=\"500\" height=\"281\" frameborder=\"0\"></iframe></div>");

            // Add tweets

            return str;
        }

        /// <summary>
        /// A method to convert basic BBCode to HTML
        /// </summary>
        /// <param name="str">A string formatted in BBCode</param>
        /// <param name="replaceLineBreaks">Whether or not to replace line breaks with br's</param>
        /// <returns>The HTML representation of the BBCode string</returns>
        public static string ConvertBbCodeToHtml(string str, bool replaceLineBreaks = true)
        {
            if (replaceLineBreaks)
            {
                // As this is a BBEditor we need to replace line breaks
                // or they won't show in the front end
                str = ReplaceLineBreaks(str, "<br>");
            }

            // format the bold tags: [b][/b]
            // becomes: <strong></strong>
            var exp = new Regex(@"\[b\](.+?)\[/b\]");
            str = exp.Replace(str, "<strong>$1</strong>");

            // format the italic tags: [i][/i]
            // becomes: <em></em>
            exp = new Regex(@"\[i\](.+?)\[/i\]");
            str = exp.Replace(str, "<em>$1</em>");

            // format the underline tags: [u][/u]
            // becomes: <u></u>
            exp = new Regex(@"\[u\](.+?)\[/u\]");
            str = exp.Replace(str, "<u>$1</u>");

            // format the underline tags: [ul][/ul]
            // becomes: <ul></ul>
            exp = new Regex(@"\[ul\](.+?)\[/ul\]");
            str = exp.Replace(str, "<ul>$1</ul>");

            // format the underline tags: [ol][/ol]
            // becomes: <ol></ol>
            exp = new Regex(@"\[ol\](.+?)\[/ol\]");
            str = exp.Replace(str, "<ol>$1</ol>");

            // format the underline tags: [li][/li]
            // becomes: <li></li>
            exp = new Regex(@"\[li\](.+?)\[/li\]");
            str = exp.Replace(str, "<li>$1</li>");

            // format the code tags: [code][/code]
            // becomes: <pre></pre>
            exp = new Regex(@"\[code\](.+?)\[/code\]");
            str = exp.Replace(str, "<pre>$1</pre>");

            // format the code tags: [quote][/quote]
            // becomes: <blockquote></blockquote>
            exp = new Regex(@"\[quote\](.+?)\[/quote\]");
            str = exp.Replace(str, "<blockquote>$1</blockquote>");

            // format the strike tags: [s][/s]
            // becomes: <strike></strike>
            exp = new Regex(@"\[s\](.+?)\[/s\]");
            str = exp.Replace(str, "<strike>$1</strike>");

            //### Before this replace links without http ###
            str.Replace("[url=www.", "[url=http://www.");
            // format the url tags: [url=www.website.com]my site[/url]
            // becomes: <a href="www.website.com">my site</a>
            exp = new Regex(@"\[url\=([^\]]+)\]([^\]]+)\[/url\]");
            str = exp.Replace(str, "<a rel=\"nofollow\" href=\"$1\">$2</a>");

            // format the img tags: [img]www.website.com/img/image.jpeg[/img]
            // becomes: <img src="www.website.com/img/image.jpeg" />
            exp = new Regex(@"\[img\]([^\]]+)\[/img\]");
            str = exp.Replace(str, "<img src=\"$1\" />");

            // format img tags with alt: [img=www.website.com/img/image.jpeg]this is the alt text[/img]
            // becomes: <img src="www.website.com/img/image.jpeg" alt="this is the alt text" />
            exp = new Regex(@"\[img\=([^\]]+)\]([^\]]+)\[/img\]");
            str = exp.Replace(str, "<img src=\"$1\" alt=\"$2\" />");

            // format the size tags: [size=1.2][/size]
            // becomes: <span style="font-size:1.2em;"></span>
            exp = new Regex(@"\[size\=([^\]]+)\]([^\]]+)\[/size\]");
            str = exp.Replace(str, "<span style=\"font-size:$1em;\">$2</span>");

            return str;
        }
        #endregion

        public static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
                return text;

            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length, text.Length - pos - search.Length);
        }

        public static string GetHash(this string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }
        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }

        //Hàm return về ký tự đầu của tất cả các từ trong input
        public static string GetAllFirstChar(this string input)
        {
            string[] finalstring = input.Split(" ");
            string output = string.Empty;
            foreach (var item in finalstring)
            {
                var str = item.Trim();
                if (!string.IsNullOrWhiteSpace(str))
                {
                    var firstChar = str.Substring(0, 1).RemoveSignVietnameseV2();
                    output += firstChar;
                }
            }

            return output;
        }

        public static string ToUpperWork(this string input)
        {
            if (input == null)
            {
                return input;
            }

            string output = new CultureInfo("en-US", false).TextInfo.ToTitleCase(input);
            return output;
        }

        public static string HashHMAC(string message, string key)
        {
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.ASCII.GetBytes(key)))
            {
                return BitConverter.ToString(hmac.ComputeHash(Encoding.ASCII.GetBytes(message))).Replace("-", "").ToLower();
            }
        }

        public static string StripHtmlFromString(this string input)
        {
            input = Regex.Replace(input, @"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>", string.Empty, RegexOptions.Singleline);
            input = Regex.Replace(input, @"\[[^]]+\]", "");
            return input;
        }

        public static string SafePlainText(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                input = input.Trim();
                input = StripHtmlFromString(input);
                input = GetSafeHtml(input);
            }
            return input;
        }

        public static string GetSafeHtml(string html)
        {
            if (html == null)
            {
                return null;
            }

            return ScrubHtml(html);
        }

        public static string ScrubHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return html;
            }
            return html;

            //var doc = new HtmlDocument();
            //doc.LoadHtml(html);

            ////Remove potentially harmful elements
            //var nc = doc.DocumentNode.SelectNodes("//script|//link|//iframe|//frameset|//frame|//applet|//object|//embed");
            //if (nc != null)
            //{
            //    foreach (var node in nc)
            //    {
            //        node.ParentNode.RemoveChild(node, false);

            //    }
            //}

            ////remove hrefs to java/j/vbscript URLs
            //nc = doc.DocumentNode.SelectNodes("//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'javascript')]|//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'jscript')]|//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'vbscript')]");
            //if (nc != null)
            //{

            //    foreach (var node in nc)
            //    {
            //        node.SetAttributeValue("href", "#");
            //    }
            //}

            ////remove img with refs to java/j/vbscript URLs
            //nc = doc.DocumentNode.SelectNodes("//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'javascript')]|//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'jscript')]|//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'vbscript')]");
            //if (nc != null)
            //{
            //    foreach (var node in nc)
            //    {
            //        node.SetAttributeValue("src", "#");
            //    }
            //}

            ////remove on<Event> handlers from all tags
            //nc = doc.DocumentNode.SelectNodes("//*[@onclick or @onmouseover or @onfocus or @onblur or @onmouseout or @ondoubleclick or @onload or @onunload or @onerror]");
            //if (nc != null)
            //{
            //    foreach (var node in nc)
            //    {
            //        node.Attributes.Remove("onFocus");
            //        node.Attributes.Remove("onBlur");
            //        node.Attributes.Remove("onClick");
            //        node.Attributes.Remove("onMouseOver");
            //        node.Attributes.Remove("onMouseOut");
            //        node.Attributes.Remove("onDoubleClick");
            //        node.Attributes.Remove("onLoad");
            //        node.Attributes.Remove("onUnload");
            //        node.Attributes.Remove("onError");
            //    }
            //}

            //// remove any style attributes that contain the word expression (IE evaluates this as script)
            //nc = doc.DocumentNode.SelectNodes("//*[contains(translate(@style, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'expression')]");
            //if (nc != null)
            //{
            //    foreach (var node in nc)
            //    {
            //        node.Attributes.Remove("stYle");
            //    }
            //}

            //return doc.DocumentNode.WriteTo();
        }

        public static string GetDefaultExtension(this string mimeType)
        {
            string defaultExt;
            RegistryKey key;
            object value;
            key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + mimeType, false);
            value = key != null ? key.GetValue("Extension", null) : null;
            defaultExt = value != null ? value.ToString() : string.Empty;
            return defaultExt;
        }
        public static string ConvertImageByteToBase64(this byte[] image)
        {
            StringBuilder _sb = new StringBuilder();

            // Byte[] _byte = GetImage(url);
            _sb.Append(Convert.ToBase64String(image, 0, image.Length));
            return _sb.ToString();
        }
        public static string ConvertImageURLToBase64(this string url)
        {
            StringBuilder _sb = new StringBuilder();

            Byte[] _byte = GetImage(url);

            _sb.Append(Convert.ToBase64String(_byte, 0, _byte.Length));

            return _sb.ToString();
        }

        public static byte[] GetImage(string url)
        {
            Stream stream = null;
            byte[] buf;

            try
            {
                WebProxy myProxy = new WebProxy();
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                stream = response.GetResponseStream();

                using (BinaryReader br = new BinaryReader(stream))
                {
                    int len = (int)(response.ContentLength);
                    buf = br.ReadBytes(len);
                    br.Close();
                }

                stream.Close();
                response.Close();
            }
            catch (Exception exp)
            {
                buf = null;
            }

            return (buf);
        }
        public static char cipher(char ch, int key)
        {
            //if (!char.IsLetter(ch))
            //{
            //    return ch;
            //}

            //char d = char.IsUpper(ch) ? 'A' : 'a';
            //return (char)((((ch + key) - d) % 26) + d);
            var code = (int)ch;

            if (code >= 65 && code <= 90)
            {
                var resultCode = ((code - 65) + key) % 26;
                return (char)(resultCode + 65);
            }
            else if (code >= 97 && code <= 122)
            {
                var resultCode = ((code - 97) + key) % 26;
                return (char)(resultCode + 97);
            }
            else
            {
                return (char)code;
            }

        }


        public static string Encipher(this string input, int key)
        {
            string output = string.Empty;

            foreach (char ch in input)
                output += cipher(ch, key);

            return output;
        }

        public static string Decipher(this string input, int key)
        {
            return Encipher(input, 26 - key);
        }




    }
}
