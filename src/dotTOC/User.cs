using System;
using System.Collections.Generic;
using System.Text;

namespace dotTOC
{
    public enum NameFormat { Raw, Normalized }
    public enum PasswordFormat { Raw, Roasted }

    /// <summary>
    /// class for the screen name signed on using the TOC class
    /// </summary>
    public class User
    {
        public string DisplayName = string.Empty;

        private string _strName;
        public string UserName
        {
            get { return _strName; }
        }

        private string _strPW;
        public string Password
        {
            get { return _strPW; }
        }

        public User()
        {
        }

        public User(string strName, string strPW)
        {
            _strName = strName;
            _strPW = strPW;
        }

        public string GetName()
        {
            return GetName(NameFormat.Normalized);
        }

        public string GetName(NameFormat nt)
        {
            string strRetVal = _strName;

            if (nt == NameFormat.Normalized)
            {
                strRetVal = Normalize(strRetVal);
            }

            return strRetVal;
        }

        public string GetPassword()
        {
            return GetPassword(PasswordFormat.Roasted);
        }

        public string GetPassword(PasswordFormat pt)
        {
            string strRetVal = _strPW;

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
