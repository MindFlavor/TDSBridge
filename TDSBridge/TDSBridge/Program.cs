using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TDSBridge.Common;

namespace TDSBridge
{
    class Program
    {
        static int iRPC = 0;

        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Usage();
                return;
            }

            System.Net.IPHostEntry iphe = System.Net.Dns.GetHostEntry(args[1]);

            BridgeAcceptor b = new BridgeAcceptor(
                int.Parse(args[0]),
                new System.Net.IPEndPoint(iphe.AddressList[0], int.Parse(args[2]))
                );

            b.TDSMessageReceived += new TDSMessageReceivedDelegate(b_TDSMessageReceived);
            b.TDSPacketReceived += new TDSPacketReceivedDelegate(b_TDSPacketReceived);
            b.ConnectionAccepted += new ConnectionAcceptedDelegate(b_ConnectionAccepted);
            b.ConnectionDisconnected += new ConnectionDisconnectedDelegate(b_ConnectionClosed);

            b.Start();

            Console.WriteLine("Press enter to kill this process...");
            Console.ReadLine();

            b.Stop();
        }

        static void b_ConnectionClosed(object sender, BridgedConnection bc, ConnectionType ct)
        {
            Console.WriteLine(FormatDateTime() + "|Connection " + ct + " closed (" + bc.SocketCouple + ")");
        }

        static void b_ConnectionAccepted(object sender, System.Net.Sockets.Socket sAccepted)
        {
            Console.WriteLine(FormatDateTime() + "|New connection from " + sAccepted.RemoteEndPoint);
        }

        static void b_TDSPacketReceived(object sender, BridgedConnection bc, Common.Packet.TDSPacket packet)
        {
            Console.WriteLine(FormatDateTime() + "|" + packet);
        }

        static void b_TDSMessageReceived(object sender, BridgedConnection bc, Common.Message.TDSMessage msg)
        {
            Console.WriteLine(FormatDateTime() + "|" + msg);
            if (msg is Common.Message.SQLBatchMessage)
            {
                Console.Write("\tSQLBatch message ");
                Common.Message.SQLBatchMessage b = (Common.Message.SQLBatchMessage)msg;
                string strBatchText = b.GetBatchText();
                Console.Write("({0:N0} chars worth of {1:N0} bytes of data)[", strBatchText.Length, strBatchText.Length * 2);
                Console.Write(strBatchText);
                Console.WriteLine("]");
            }
            else if (msg is Common.Message.RPCRequestMessage)
            {
                try
                {
                    Common.Message.RPCRequestMessage rpc = (Common.Message.RPCRequestMessage)msg;
                    byte[] bPayload = rpc.AssemblePayload();

                    #if DEBUG
                    //using (System.IO.FileStream fs = new System.IO.FileStream(
                    //    "C:\\temp\\dev\\" + (iRPC++) + ".raw",
                    //    System.IO.FileMode.Create,
                    //    System.IO.FileAccess.Write,
                    //    System.IO.FileShare.Read))
                    //{
                    //    fs.Write(bPayload, 0, bPayload.Length);
                    //}
                    #endif

                }
                catch (Exception exce)
                {
                    Console.WriteLine("Exception: " + exce.ToString());
                }
            }
        }

        static string FormatDateTime()
        {
            return DateTime.Now.ToString("yyyyMMdd HH:mm:ss.ffffff");

        }

        static void Usage()
        {
            Console.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + " <listen port> <sql server address> <sql server port>");
        }
    }
}
