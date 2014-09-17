using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TDSBridge.Common.Header;

namespace TDSBridge.Common.Packet
{
    public class TDSPacket
    {
        //static int iCnt = 0;

        protected Header.TDSHeader _header = null;        
        protected byte[] _payload = null;

        public byte[] Payload { get { return _payload; } }
        public Header.TDSHeader Header { get { return _header; } }

        public TDSPacket(byte[] bBuffer)
        {
            _header = new Header.TDSHeader(bBuffer);

            _payload = new byte[_header.LengthIncludingHeader - TDSHeader.HEADER_SIZE];
            Array.Copy(bBuffer, TDSHeader.HEADER_SIZE, _payload, 0, _payload.Length);
        }

        public TDSPacket(byte[] bHeader, byte[] bPayload, int iPayloadSize)
        {
            _header = new Header.TDSHeader(bHeader);

            _payload = new byte[iPayloadSize];
            Array.Copy(bPayload, 0, _payload, 0, iPayloadSize);
        }

        public override string ToString()
        {
            return base.ToString() + "[Header=" + Header + "]";
        }
    }
}
