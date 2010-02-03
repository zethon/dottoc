using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotTOC
{
    public enum BuddyStatus
    {
        Unknown,
        Offline,
        Online,
        Idle
    }

    public class Buddy
    {
        public string Name
        {
            set;
            get;
        }

        public BuddyStatus Status
        {
            set;
            get;
        }

        public Buddy(string strName)
        {
            Name = strName;
            Status = BuddyStatus.Unknown;
        }
        
        public Buddy(string strName, BuddyStatus buddyStatus)
        {
            Name = strName;
            Status = buddyStatus;
        }
    }
}
