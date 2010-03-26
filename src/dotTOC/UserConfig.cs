using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotTOC
{
    public enum PermitDenyMode
    {
        PermitAll = 1,
        DenyAll = 2,
        PermitSome = 3,
        DenySome = 4,
        PermitBuddiesOnly = 5
    }

    public class UserConfig : IEnumerator, IEnumerable
    {
        public PermitDenyMode Mode;

        public Dictionary<string, List<Buddy>> BuddyList = new Dictionary<string, List<Buddy>>();
        
        public List<Buddy> PermitList = new List<Buddy>();
        
        public List<Buddy> DenyList = new List<Buddy>();



        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this;
        }

        public bool MoveNext()
        {
            return true;
        }

        public void Reset()
        {

        }

        public object Current
        {
            get { return 1; }
        }


    }
}
