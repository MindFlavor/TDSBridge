using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDSBridge.Common.Message
{
    public class SQLBatchMessage : TDSMessage
    {
        public SQLBatchMessage() { }

        public SQLBatchMessage(Packet.TDSPacket firtsPacket)
            : base(firtsPacket)
        { }

        public string GetBatchText()
        {
            byte[] bPayload = this.AssemblePayload();
            Header.AllHeader _allHeader = new Header.AllHeader(bPayload);

            int iHeaderLength = (int)_allHeader.Length;

            return System.Text.Encoding.Unicode.GetString(
                bPayload,
                iHeaderLength,
                bPayload.Length - iHeaderLength);
        }
    }
}
