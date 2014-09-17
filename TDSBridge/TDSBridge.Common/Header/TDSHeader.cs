using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDSBridge.Common.Header
{
    public class TDSHeader
    {
        public const int HEADER_SIZE = 8;

        protected byte[] _Buffer = new byte[HEADER_SIZE];

         public TDSHeader(byte[] bPacket)
        {
            Array.Copy(bPacket, 0, this._Buffer, 0, HEADER_SIZE);
        }

        public HeaderType Type { get { return (HeaderType)_Buffer[0]; } }
        public byte StatusBitMask { get { return _Buffer[1]; } }

        public int LengthIncludingHeader { 
            get 
            {
                return ((int)_Buffer[2]) * 0x100 + ((int)_Buffer[3]);       
            } 
        }

        public int PayloadSize
        {
            get
            {
                return LengthIncludingHeader - HEADER_SIZE;
            }
        }

        public byte this[int idx]
        {
            get { return _Buffer[idx]; }
            set { _Buffer[idx] = value; }
        }

        public override string ToString()
        {
            return GetType().FullName +
                "[Type=" + Type +
                ";StatusBitMask=" + StatusBitMask +
                ";LengthIncludingHeader=" + LengthIncludingHeader +
                ";PayloadSize=" + PayloadSize +
                "]";
        }
    }
}
