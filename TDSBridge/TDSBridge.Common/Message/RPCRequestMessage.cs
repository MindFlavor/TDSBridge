using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDSBridge.Common.Message
{
    public class RPCRequestMessage : TDSMessage
    {
        public RPCRequestMessage() { }

        public RPCRequestMessage(Packet.TDSPacket firtsPacket)
            : base(firtsPacket)
        { }
    }
}
