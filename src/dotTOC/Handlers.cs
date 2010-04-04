using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotTOC
{
    public class FlapHandlers
    {
        public delegate void OnFlapSignOnHandler(FlapHeader fh, Byte[] buffer);
        public delegate void OnFlapDataHandler(FlapHeader fh, Byte[] buffer);
        public delegate void OnFlapUnknownHandler(FlapHeader fh, Byte[] buffer);
        public delegate void OnFlapKeepAliveHanlder(FlapHeader fh, Byte[] buffer);
    }

    public class TOCInMessageHandlers
    {
        public delegate void OnServerMessageHandler(string strIncoming);
        public delegate void OnSignedOnHandler();
        public delegate void OnConfigHandler(UserConfig config);

        public delegate void OnUpdateBubbyHandler(Buddy buddy);
        public delegate void OnIMInHandler(InstantMessage im);
        public delegate void OnEviledHandler(int iLvl, bool bAnonymous, string strSender);
        public delegate void OnChatJoinedHandler(string strRoomID, string strRoomName);
        public delegate void OnNickHandler(string strNick);
    }

    public class TOCOutMessageHandlers
    {
        public delegate void OnSendServerMessageHandler(string Outgoing);
        public delegate void OnSendIMHander(InstantMessage im);
    }
}
