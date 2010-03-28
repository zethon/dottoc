using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotTOC
{
    public class InstantMessage
    {
        public Buddy To;

        public Buddy From;
        
        public string Message = string.Empty;
        public bool Auto = false;

        public InstantMessage()
        {

        }

        public InstantMessage(string from, string to, string message)
        {
            From = new Buddy { Name = from };
            To = new Buddy { Name = to };
            Message = message;
        }
    }
}
