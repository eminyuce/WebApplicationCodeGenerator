using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Helpers
{
    public class GeneralHelper
    {
        public static string GetSqlDataTypeFromColumnDataType(Kontrol_Icerik ki)
        {

            String result = "SqlDbType.{0}";
            var item = ki;
            if (item.dataType.IndexOf("varchar") > -1 || item.dataType.IndexOf("nchar") > -1 || item.dataType.IndexOf("nchar") > -1 || item.dataType.IndexOf("text") > -1 || item.dataType.IndexOf("xml") > -1)
            {
                result = String.Format(result, "NVarChar");
            }
            else if (item.dataType.IndexOf("int") > -1)
            {
                result = String.Format(result, "Int");
            }
            else if (item.dataType.IndexOf("date") > -1)
            {
                result = String.Format(result, "DateTime");
            }
            else if (item.dataType.IndexOf("bit") > -1)
            {
                result = String.Format(result, "Bit");
            }
            else if (item.dataType.IndexOf("float") > -1)
            {
                result = String.Format(result, "Float");
            }
            else if (item.dataType.IndexOf("char") > -1)
            {
                result = String.Format(result, "NVarChar");
            }


            return result;
        }

        public static string GetCSharpDataType(Helpers.Kontrol_Icerik ki)
        {
            var item = ki;
            String result = "";
            if (item.dataType.IndexOf("varchar") > -1 || item.dataType.IndexOf("nchar") > -1 || item.dataType.IndexOf("nchar") > -1 || item.dataType.IndexOf("text") > -1 || item.dataType.IndexOf("xml") > -1)
            {
                result = "String";
            }
            else if (item.dataType.IndexOf("int") > -1)
            {
                result = "int";
            }
            else if (item.dataType.IndexOf("date") > -1)
            {
                result = "DateTime ";
            }
            else if (item.dataType.IndexOf("bit") > -1)
            {
                result = "Boolean ";
            }
            else if (item.dataType.IndexOf("float") > -1)
            {
                result = "float ";
            }
            else if (item.dataType.IndexOf("char") > -1)
            {
                result = "char ";
            }

            return result.Trim();
        }


      
        public static string FirstCharacterToLower(string str)
        {
            if (String.IsNullOrEmpty(str) || Char.IsLower(str, 0))
                return str;

            return Char.ToLowerInvariant(str[0]).ToString() + str.Substring(1);
        }
        public static String convertSqlDataTypeToCSharp(String key)
        {
            String result = "";

            StringDictionary map = new StringDictionary();
            map.Add("binary", "Byte[]");
            map.Add("varbinary", "Byte[]");
            map.Add("image", "None");
            map.Add("varchar", "String");
            map.Add("char", "String");
            map.Add("nvarchar", "String");
            map.Add("nchar", "String");
            map.Add("text", "String");
            map.Add("ntext", "String");
            map.Add("uniqueidentifier", "Guid");
            map.Add("rowversion", "Byte[]");
            map.Add("bit", "Boolean");
            map.Add("tinyint", "Byte");
            map.Add("smallint", "int");
            map.Add("int", "int");
            map.Add("bigint", "int");
            map.Add("smallmoney", "Decimal");
            map.Add("money", "Decimal");
            map.Add("numeric", "Decimal");
            map.Add("decimal", "Decimal");
            map.Add("real", "Single");
            map.Add("float", "double");
            map.Add("smalldatetime", "DateTime");
            map.Add("datetime", "DateTime");
            map.Add("datetime2", "DateTime");
            map.Add("timestamp", "DateTime");
            map.Add("xml", "String");

            var lines = File.ReadAllLines(HttpContext.Current.Server.MapPath(@"~\dataTypes.txt"));
            for (var i = 0; i < lines.Length; i += 1)
            {
                try
                {
                    var line = lines[i].ToStr();
                    if (!String.IsNullOrEmpty(line))
                    {
                        if (!map.ContainsKey(key))
                        {
                            var parts = line.Split("-".ToCharArray());
                            map[parts[0]] = parts[1];
                        }
                    }
                }
                catch (Exception ex)
                {


                }


                // Process line
            }

            if (map.ContainsKey(key))
            {
                return map[key.ToLower()];
            }
            else
            {
                result = "UNDEFINED_DATA_TYPE";
            }

            return result;

        }
        public static string ToTitleCase(string s)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());
        }

        public static readonly Regex CarriageRegex = new Regex(@"(\r\n|\r|\n)+");
        //remove carriage returns from the header name
        public static string RemoveCarriage(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return "";
            }
            return CarriageRegex.Replace(text, string.Empty).Trim();
        }


       
        public static string GetUrlString(string strIn)
        {
            if (String.IsNullOrEmpty(strIn))
                return String.Empty;

            strIn = Regex.Replace(strIn, @"\s{2,}", " ");
            // Replace invalid characters with empty strings. 
            strIn = strIn.Trim().ToLower();
            char[] szArr = strIn.ToCharArray();
            var list = new List<char>();
            foreach (char c in szArr)
            {
                int ci = c;
                if ((ci >= 'a' && ci <= 'z') || (ci >= '0' && ci <= '9') || ci == ' ')
                {
                    list.Add(c);
                }
            }
            return new String(list.ToArray()).Trim().Replace(" ", "-");
        }
        public static string GetUrlStringPages(string strIn)
        {
            if (String.IsNullOrEmpty(strIn))
                return String.Empty;

            strIn = Regex.Replace(strIn, @"\s{2,}", " ");
            // Replace invalid characters with empty strings. 
            strIn = strIn.Trim().ToLower();
            char[] szArr = strIn.ToCharArray();
            var list = new List<char>();
            foreach (char c in szArr)
            {
                int ci = c;
                if ((ci >= 'a' && ci <= 'z') || (ci >= '0' && ci <= '9') || ci == ' ' || ci == '/')
                {
                    list.Add(c);
                }
            }
            return new String(list.ToArray()).Trim().Replace(" ", "-");
        }
    }
}
