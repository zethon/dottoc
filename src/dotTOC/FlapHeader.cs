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

        public  FlapHeader(byte[] buffer)
        {
            Asterik = (char)buffer[0];
            FlapType = (FLAPTYPE)Enum.ToObject(typeof(FLAPTYPE), (byte)buffer[1]);

            byte[] byteTemp = { buffer[5], buffer[4] };
            DataLength = BitConverter.ToInt16(byteTemp, 0);
        }
    }
}
