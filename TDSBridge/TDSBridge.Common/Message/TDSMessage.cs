using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDSBridge.Common.Message
{
    public class TDSMessage
    {
        protected List<Packet.TDSPacket> _lPackets = new List<Packet.TDSPacket>();

        public List<Packet.TDSPacket> Packets { get { return _lPackets; } }

        public TDSMessage() { }

        public TDSMessage(Packet.TDSPacket firtsPacket)
        {
            Packets.Add(firtsPacket);
        }

        public bool IsComplete
        {
            get
            {
                if (Packets.Count == 0)
                    return false;
                return (Packets[Packets.Count - 1].Header.StatusBitMask & Header.StatusBitMask.END_OF_MESSAGE) == Header.StatusBitMask.END_OF_MESSAGE;
            }
        }

        public bool HasIgnoreBitSet
        {
            get
            {
                if (Packets.Count == 0)
                    return false;
                return (Packets[Packets.Count - 1].Header.StatusBitMask & Header.StatusBitMask.IGNORE_EVENT) == Header.StatusBitMask.IGNORE_EVENT;
            }
        }


        public byte[] AssemblePayload()
        {
            List<byte> lPayLoad = new List<byte>(4096 * 4);

            for (int i = 0; i < Packets.Count; i++)
            {
                lPayLoad.AddRange(Packets[i].Payload);                
            }

            return lPayLoad.ToArray();
        }

        public static TDSMessage CreateFromFirstPacket(Packet.TDSPacket firstPacket)
        {
            switch (firstPacket.Header.Type)
            {
                case Header.HeaderType.SQLBatch:
                    return new SQLBatchMessage(firstPacket);
                case Header.HeaderType.AttentionSignal:
                    return new AttentionMessage(firstPacket);
                case Header.HeaderType.RPC:
                    return new RPCRequestMessage(firstPacket);
                default:
                    return new TDSMessage(firstPacket);
            }
        }

        public override string ToString()
        {
            if (IsComplete)
            {
                StringBuilder sb = new StringBuilder(this.GetType().FullName);

                sb.Append("[#Packets=" + Packets.Count +
                    ";IsComplete=" + IsComplete +
                    ";HasIgnoreBitSet=" + HasIgnoreBitSet +
                    ";TotalPayloadSize=" + AssemblePayload().Length);

                for (int i = 0; i < Packets.Count; i++)
                {
                    sb.Append("\n\t[P" + i + "[");
                    sb.Append(Packets[i]);
                    sb.Append("]]");
                }

                sb.Append("]");

                return sb.ToString();
            }
            else
            {
                return this.GetType().FullName + "{Incomplete message}";
            }

        }
    }
}
