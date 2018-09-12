using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Helpers
{
    public static class Convert
    {

        public static string RemoveFormFeedCharacter(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }
            string formFeed = ((char)0x000C).ToString();
            return value.Replace(formFeed, string.Empty);
        }

       


        public static int ToInt(this object arg)
        {
            int ret = 0;

            int.TryParse(arg.ToStr(), out ret);

            return ret;
        }

        public static long ToLong(this object arg)
        {
            long ret = 0;

            long.TryParse(arg.ToStr(), out ret);

            return ret;
        }
        public static float ToFloat(this object arg)
        {
            float ret = 0;


            float.TryParse(arg.ToStr(), out ret);

            return ret;
        }

        public static double ToDouble(this object arg)
        {
            double ret = 0;


            double.TryParse(arg.ToStr(), out ret);

            return ret;
        }


        public static string ToStr(this object arg)
        {
            string ret = string.Empty;
            if (arg != null)
            {
                ret = arg.ToString();
            }
            return ret;
        }


        public static string ToStr(this object arg, int length)
        {
            string ret = string.Empty;
            if (arg != null)
            {
                ret = arg.ToString();
            }
            if (ret.Length > length)
            {
                return ret.Substring(0, length);
            }
            else
            {
                return ret;
            }
        }

        public static string ToStr(this string text, int minLen, int maxLen)
        {
            string s = text != null ? text : "";
            if (s.Length > maxLen) s = s.Substring(0, maxLen).Trim();

            int ix = 0;
            ix = s.LastIndexOf(".");
            if (ix > minLen)
            {
                s = s.Substring(0, ix + 1).Trim();
            }
            else if ((ix = s.LastIndexOf(",")) > minLen)
            {
                s = s.Substring(0, ix).Trim();

            }
            else if ((ix = s.LastIndexOf(" ")) > minLen)
            {
                s = s.Substring(0, ix).Trim();
            }

            return s;
        }

        public static bool HasValue(this object arg)
        {
            string ret = string.Empty;
            if (arg != null)
            {
                ret = arg.ToString();
            }
            return !string.IsNullOrEmpty(ret);
        }


        public static string ToTitleCase(this string text)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            return textInfo.ToTitleCase(text);
        }

        public static string HtmlDecode(this string arg)
        {
            return WebUtility.HtmlDecode(arg);
        }
        public static string HtmlEncode(this string arg)
        {
            return WebUtility.HtmlEncode(arg);
        }
        public static string NoRepeatedSpaces(string arg)
        {

            return Regex.Replace(arg, " {2,}", " ");
        }


        public static string NoBreakLine(string arg)
        {

            return arg.Replace("\r", " ").Replace("\n", " ");
        }


        public static string ToNormal(this string arg)
        {
            return NoRepeatedSpaces(NoBreakLine(arg.HtmlDecode()));
        }


        public static bool ToBool(this object arg, bool defaultValue = false)
        {
            bool ret = defaultValue;

            if (!bool.TryParse(arg.ToStr(), out ret))
            {
                if (arg.ToStr().ToLower().Contains((!defaultValue).ToString().ToLower()))
                {
                    ret = !defaultValue;
                }

            }



            return ret;
        }


        static Regex _dateRegex = new Regex(@"^(19|20)(\d\d)[- /.]?(0[1-9]|1[012])[- /.]?(0[1-9]|[12][0-9]|3[01])$", RegexOptions.Compiled);




        public static DateTime? ToNullableDateTime(this object arg)
        {
            DateTime ret = DateTime.MinValue;

            if (!DateTime.TryParse(arg.ToStr(), out ret))
            {
                Match md = _dateRegex.Match(arg.ToStr());

                if (md != null && md.Groups.Count == 5)
                {
                    int year = (md.Groups[1].Value + md.Groups[2].Value).ToInt();
                    int month = (md.Groups[3].Value).ToInt();
                    int day = (md.Groups[4].Value).ToInt();
                    try
                    {
                        ret = new DateTime(year, month, day);
                    }
                    catch { }
                }


            }

            if (ret != DateTime.MinValue)
            {
                return ret;
            }
            else
            {
                return null;
            }
        }



        public static DateTime ToDateTime(this object arg)
        {
            DateTime ret = DateTime.MinValue;

            if (!DateTime.TryParse(arg.ToStr(), out ret))
            {
                Match md = _dateRegex.Match(arg.ToStr());

                if (md != null && md.Groups.Count == 5)
                {
                    int year = (md.Groups[1].Value + md.Groups[2].Value).ToInt();
                    int month = (md.Groups[3].Value).ToInt();
                    int day = (md.Groups[4].Value).ToInt();
                    try
                    {
                        ret = new DateTime(year, month, day);
                    }
                    catch { }
                }


            }



            return ret;
        }



        public static string ToFlexDateTime(this DateTime dt)
        {

            string rt = "";
            if (dt > DateTime.Now.Date)
            {
                rt = dt.ToString("h:mmtt").ToLower();
            }
            //else if (dt > DateTime.Now.AddDays(-DateTime.Now.DayOfYear))
            //{

            //    rt = dt.ToString("MMM dd");
            //}
            else if (dt > DateTime.MinValue)
            {
                rt = dt.ToString("MMM dd, yyyy ");
            }
            return rt;
        }


        public static object DeepClone(object obj)
        {
            object objResult = null;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);

                ms.Position = 0;
                objResult = bf.Deserialize(ms);
            }
            return objResult;
        }



        public static string UrlEncode(this string text)
        {

            return UrlEncodeCustom(text);
            //return HttpUtility.UrlPathEncode(text.Replace(" ", "_")).ToLower();

            //char c;
            //((int) c).ToString("X");

        }

        public static string UrlDecode(this string text)
        {
            //return HttpUtility.UrlDecode(text);
            return UrlDecodeCustom(text);
            // return HttpUtility.UrlPathEncode(text).Replace("_", " ");
        }


        private static string UrlEncodeCustom(string text)
        {
            StringBuilder ret = new StringBuilder();

            foreach (var c in text)
            {
                if (c >= '0' && c <= '9' ||
                    c >= 'a' && c <= 'z' ||
                    c >= 'A' && c <= 'Z' ||
                    c == ' '
                    //|| c == '-'
                    )
                {
                    ret.Append(c);
                }
                else
                {
                    ret.Append("~" + ((int)c).ToString("X"));
                }
            }

            return ret.ToString().Replace(" ", "_");
        }


        private static string UrlDecodeCustom(string text)
        {

            var chars = text.Replace("_", " ").ToCharArray();

            StringBuilder ret = new StringBuilder();

            int i = 0;
            while (i < chars.Length)
            {
                char c = chars[i];
                if (c >= '0' && c <= '9' ||
                    c >= 'a' && c <= 'z' ||
                    c >= 'A' && c <= 'Z' ||
                    c == ' ' || c == '-')
                {
                    ret.Append(c);
                    i++;
                }
                else if (c == '~' && i + 2 < chars.Length)
                {

                    try
                    {
                        string hexValue = chars[i + 1].ToString() + chars[i + 2].ToString();
                        int intChar = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
                        ret.Append((char)intChar);
                    }
                    catch (Exception)
                    {
                    }

                    i = i + 3;

                }
                else
                {
                    break;
                }
            }

            return ret.ToString();
        }


    }
}
