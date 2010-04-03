using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotTOC
{
    public class FlapHeader
    {
        public char Asterik;
        public FLAPTYPE FlapType;
        public short DataLength;

        public FlapHeader()
        {
        }

        public  FlapHeader(int iBytesRead, byte[] buffer)
        {
            Asterik = (char)buffer[iBytesRead];
            FlapType = (FLAPTYPE)Enum.ToObject(typeof(FLAPTYPE), (byte)buffer[iBytesRead + 1]);

            byte[] byteTemp = { buffer[iBytesRead + 5], buffer[iBytesRead + 4] };
            DataLength = BitConverter.ToInt16(byteTemp, 0);
        }
    }
}
