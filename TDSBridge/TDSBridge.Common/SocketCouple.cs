using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDSBridge.Common
{
    public class SocketCouple
    {
        public System.Net.Sockets.Socket ClientBridgeSocket { get; set; }
        public System.Net.Sockets.Socket BridgeSQLSocket { get; set; }

        public override string ToString()
        {
            try
            {
                return base.ToString() + "[ClientBridgeSocket.RemoteEndPoint=" + ClientBridgeSocket.RemoteEndPoint + ", BridgeSQLSocket.RemoteEndPoint=" + BridgeSQLSocket.RemoteEndPoint + "]";
            }
            catch
            {
                return base.ToString() + "[ClientBridgeSocket=" + ClientBridgeSocket + ", BridgeSQLSocket=" + BridgeSQLSocket + "]";
            }
        }
    }
}
