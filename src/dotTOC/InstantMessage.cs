using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace dotTOC
{
    public class InstantMessage
    {
        public Buddy To;

        public Buddy From;

        public string RawMessage = string.Empty;

        public string Message
        {
            get { return Regex.Replace(RawMessage, @"<(.|\n)*?>", string.Empty); }
        }

        public bool Auto = false;

        public InstantMessage()
        {

        }

        public InstantMessage(string from, string to, string message)
        {
            From = new Buddy { Name = from };
            To = new Buddy { Name = to };
            RawMessage = message;
        }
    }
}
