using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace dotTOC
{
    public enum BuddyStatus
    {
        Unknown,
        Offline,
        Online,
        Idle
    }

    public enum OscarClass
    {
        Unknown,
        Admin,
        Unconfirmed,
        Normal,
        Wireless
    } 

    public class Buddy : IComparable 
    {
        public int EvilAmount;

        public DateTime SignonTime;
        public int IdleTime;

        public bool OnAOL = false;
        public bool MarkedUnavailable = false;
        public bool Online = false;
        public OscarClass Class = OscarClass.Unknown;

        public string Name
        {
            set;
            get;
        }

        //public Buddy(string strName)
        //{
        //    Name = strName;
        //    Status = BuddyStatus.Unknown;
        //}
        
        //public Buddy(string strName, BuddyStatus buddyStatus)
        //{
        //    Name = strName;
        //    Status = buddyStatus;
        //}

        // IComparable implementation
        public int CompareTo(object obj)
        {
            Buddy buddy = obj as Buddy;

            if (buddy != null)
            {
                return buddy.Name.CompareTo(this.Name);
            }
            else
            {
                throw new ArgumentException("Object is not a Buddy");
            }
        }

        static public Buddy CreateBuddy(string strTOCInfo)
        {
            string[] data = Regex.Split(strTOCInfo, @"\:");
            DateTime dt = new DateTime(1970,1,1).AddSeconds(int.Parse(data[4]));

            bool bOnAOL = false;
            bool bUnavailable = false;
            OscarClass oc = OscarClass.Unknown;

            if (data[6].Length > 2)
            {
                bUnavailable = data[6][2] == 'U';
            }

            if (data[6].Length > 1)
            {
                switch (data[6][1])
                {
                    case 'A':
                        oc = OscarClass.Admin;
                        break;

                    case 'U':
                        oc = OscarClass.Unconfirmed;
                        break;

                    case 'O':
                        oc = OscarClass.Normal;
                        break;

                    case 'C':
                        oc = OscarClass.Wireless;
                        break;
                }
            }

            if (data[6].Length > 0)
            {
                bOnAOL = data[6][0] == 'A';
            }

            return new Buddy
            {
                Name = User.Normalize(data[1]),
                Online = (data[2] == @"T"),
                EvilAmount = int.Parse(data[3]),
                SignonTime = dt,
                IdleTime = int.Parse(data[5]),
                OnAOL = bOnAOL,
                MarkedUnavailable = bUnavailable,
                Class = oc
            };
        }
    }
}
