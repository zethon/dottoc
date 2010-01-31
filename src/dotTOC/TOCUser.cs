using System;
using System.Collections.Generic;
using System.Text;

namespace dotTOC
{
	public enum NameFormat { Raw,Normalized }
	public enum PasswordFormat { Raw,Roasted }

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
    }
}
