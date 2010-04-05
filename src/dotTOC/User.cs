using System;
using System.Collections.Generic;
using System.Text;

namespace dotTOC
{
    public enum PasswordFormat { Raw, Roasted }

    /// <summary>
    /// class for the screen name signed on using the TOC class
    /// </summary>
    public class User
    {
        public string Username = string.Empty;
        public string DisplayName = string.Empty;
        public string Password = string.Empty;

        public string GetNormalizeName()
        {
            return Normalize(Username);
        }

        public string GetPassword()
        {
            return GetPassword(PasswordFormat.Roasted);
        }

        public string GetPassword(PasswordFormat pt)
        {
            string strRetVal = Password;

            if (pt == PasswordFormat.Roasted)
            {
                strRetVal = RoastedString(strRetVal);
            }

            return strRetVal;
        }

        /// <summary>
        /// Returns a normalized version of the string, will concate the string to 16 chars
        /// if necessary
        /// </summary>
        public static string Normalize(string strScreenName)
        {
            string strName = strScreenName;
            strName = System.Text.RegularExpressions.Regex.Replace(strName," ", "");
            strName = strName.ToLower();

            if (strName.Length > 16)
            {
                strName = strName.Remove(16, strName.Length - 16);
            }
            
            return strName;
        }

        public static string RoastedString(string strOrig)
        {
            const string roaster = "Tic/TocTic/TocTic/TocTic/Toc";
            string retStr = "0x";

            for (int i = 0; i < strOrig.Length; i++)
            {
                retStr += string.Format("{0:x2}", strOrig[i] ^ roaster[i]);
            }

            return retStr;
        }
    }
}
