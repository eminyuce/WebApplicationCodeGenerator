﻿using System;
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
        public static Kontrol_Icerik GetPrimaryKeysItem(List<Kontrol_Icerik> list)
        {
            Kontrol_Icerik result = null;
            foreach (Kontrol_Icerik item in list)
            {
                if (item.primaryKey)
                {
                    result = item;
                }
            }
            if (result == null)
                result = list.FirstOrDefault();
            return result;
        }
        public static bool isInteger(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;
            Regex r = new Regex(@"^-{0,1}\d+$");
            return r.IsMatch(text);
        }
        public static string GetEntityPrefixName(string m)
        {
            String k = "";
            if (m.Contains("."))
            {
                m = m.Split(new string[] { "." }, StringSplitOptions.None).Skip(1).FirstOrDefault();
            }
            if (!String.IsNullOrEmpty(m))
            {
                var parts = m.Split(new string[] { "_" }, StringSplitOptions.None);
                if (parts.Length > 1)
                {
                    k = parts[0].Trim();
                }
            }
            return k;
        }
        //public static string GetEntityPrefixName(string m)
        //{
        //    String k = "";
        //    if (!String.IsNullOrEmpty(m))
        //    {
        //        var parts = m.Split(new string[] { "_" }, StringSplitOptions.None);
        //        if (parts.Length > 1)
        //        {
        //            k = parts[0].Trim();
        //        }
        //    }
        //    return k;
        //}
        public static string GetCleanEntityName(string m)
        {
            if (m.Contains("."))
            {
                m = m.Split(new string[] { "." }, StringSplitOptions.None).Skip(1).FirstOrDefault();
            }
            if (!String.IsNullOrEmpty(m))
            {
                var parts = m.Split(new string[] { "_" }, StringSplitOptions.None);
                if (parts.Length > 1)
                {
                    m = ProjectConstants.ClassNameConvention + GeneralHelper.UppercaseFirst(parts[1].Replace("ies", "y").TrimEnd('s'));
                }
                else
                {
                    m = ProjectConstants.ClassNameConvention + parts[0].ToStr().TrimEnd('s');
                }
            }
            return m;
        }

        public static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public static string GetPrimaryKeys(List<Kontrol_Icerik> list)
        {
            foreach (Kontrol_Icerik item in list)
            {
                if (item.primaryKey)
                {
                    return item.columnName;
                }
            }
            var firstOrDefault = list.FirstOrDefault();
            if (firstOrDefault != null)
                return firstOrDefault.columnName;
            else
                return "";
        }
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
            else
            {
                result = GetCLRType(item.dataType);
            }
            return result.Trim();
        }

        public static string GetCLRType(string dbType)
        {
            switch (dbType)
            {
                case "tinyint":
                case "smallint":
                case "mediumint":
                case "int":
                case "integer":
                    return "int";
                case "bigint":
                    return "long";
                case "double":
                    return "double";
                case "float":
                    return "float";
                case "decimal":
                    return "decimal";
                case "numeric":
                case "real":
                    return "decimal";
                case "bit":
                    return "bool";
                case "date":
                case "time":
                case "year":
                case "datetime":
                case "timestamp":
                    return "DateTime";
                case "tinyblob":
                case "blob":
                case "mediumblob":
                case "longblog":
                case "binary":
                case "varbinary":
                    return "byte[]";
                case "char":
                case "varchar":
                case "tinytext":
                case "text":
                case "mediumtext":
                case "longtext":
                    return "string";
                case "point":
                case "linestring":
                case "polygon":
                case "geometry":
                case "multipoint":
                case "multilinestring":
                case "multipolygon":
                case "geometrycollection":
                case "enum":
                case "set":
                default:
                    return "";
            }
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
            result = GetCLRType(key);
            if (!String.IsNullOrEmpty(result))
            {
                return result;
            }

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
