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
    public class TOCUser
    {
        private string m_strName;
        private string m_strPW;

        public TOCUser()
        {
        }

        public TOCUser(string strName, string strPW)
        {
            m_strName = strName;
            m_strPW = strPW;
        }

        public string GetName()
        {
            return GetName(NameFormat.Normalized);
        }

        public string GetName(NameFormat nt)
        {
            if (nt == NameFormat.Normalized)
                return Normalize(m_strName);

            return m_strName;
        }

        public string GetPassword()
        {
            return GetPassword(PasswordFormat.Roasted);
        }

        public string GetPassword(PasswordFormat pt)
        {
            if (pt == PasswordFormat.Roasted)
                return RoastedString(m_strPW);

            return m_strPW;
        }


        /// <summary>
        /// Returns a normalized version of the string, will concate the string to 16 chars
        /// if necessary
        /// </summary>
        public static string Normalize(string strScreenName)
        {
            string strName = strScreenName;
            strName = System.Text.RegularExpressions.Regex.Replace(strName, " ", "");
            strName = strName.ToLower();

            if (strName.Length > 16)
                strName = strName.Remove(16, strName.Length - 16);

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
