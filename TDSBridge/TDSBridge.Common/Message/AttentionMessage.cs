using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDSBridge.Common.Message
{
    public class AttentionMessage : TDSMessage
    {
        public AttentionMessage() { }

        public AttentionMessage(Packet.TDSPacket firtsPacket)
            : base(firtsPacket)
        { }
    }
}
