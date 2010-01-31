using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotTOC
{
    public enum BuddyStatus
    {
        Offline,
        Online,
        Idle
    }

    class Buddy
    {
        private string _strName = string.Empty;
        public string Name
        {
            get { return _strName; }
        }

        private BuddyStatus _status = BuddyStatus.Offline;
        public BuddyStatus Status
        {
            get { return _status; }
        }
    }
}
