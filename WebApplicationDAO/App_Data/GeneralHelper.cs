using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MagazineStoriesCalaisItems.Domain.Helpers
{
    public class GeneralHelper
    {
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
