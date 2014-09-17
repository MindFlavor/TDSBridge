using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using TDSBridge.Common.Header;
using TDSBridge.Common.Packet;

namespace TDSBridge.Common
{
    public delegate void TDSMessageReceivedDelegate(object sender, BridgedConnection bc, Message.TDSMessage msg);
    public delegate void TDSPacketReceivedDelegate(object sender, BridgedConnection bc, Packet.TDSPacket packet);
    public delegate void ConnectionAcceptedDelegate(object sender, Socket sAccepted);
    public delegate void BridgeExceptionDelegate(object sender, BridgedConnection bc, ConnectionType ct, Exception exce);
    public delegate void ListeningThreadExceptionDelegate(object sender, Socket sListening, Exception exce);
    public delegate void ConnectionDisconnectedDelegate(object sender, BridgedConnection bc, ConnectionType ct);

    /// <summary>
    /// Classe che si pone in attesa di connessioni TCP e istanzia i socket di connessione bridged.
    /// </summary>
    public class BridgeAcceptor
    {
        #region Members
        protected int _iAcceptPort;
        protected System.Net.IPEndPoint _ipeSQLServer;

        protected Socket sAccept;
        private Thread tAccept = null;        
        #endregion

        #region Properties
        /// <summary>
        /// Se true, il socket e' in attesa.
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// Porta TCP in listening. In sola lettura, per modificarla creare un nuovo BridgeAcceptor.
        /// </summary>
        public int AcceptPort { get { return _iAcceptPort; } }
        /// <summary>
        /// Endpoint IP su cui SQL e' in ascolto. Al momento la libreria non supporta SQL Server Browser per cui e' 
        /// necessario specificare la porta di SQL.
        /// </summary>
        public System.Net.IPEndPoint SQLServerEndpoint { get { return _ipeSQLServer; } set { _ipeSQLServer = value; } }
        #endregion

        #region Events
        /// <summary>
        /// Avviene alla ricezione di un messaggio TDS completo.
        /// </summary>
        public event TDSMessageReceivedDelegate TDSMessageReceived;
        /// <summary>
        /// Avviene alla ricezione di un pacchetto TDS.
        /// </summary>
        public event TDSPacketReceivedDelegate TDSPacketReceived;
        /// <summary>
        /// Avviene quando una connessione viene accettata sul socket in attesa. *Qualsiasi* connessione viene accettata, 
        /// viene delegato a SQL interrompere eventuali connessioni non valide.
        /// </summary>
        public event ConnectionAcceptedDelegate ConnectionAccepted;
        /// <summary>
        /// Accade se uno dei due canali del bridge sollevano una eccezione.
        /// </summary>
        public event BridgeExceptionDelegate BridgeException;
        /// <summary>
        /// Accade se il thread di attesa connessioni solleva una eccezione.
        /// </summary>
        public event ListeningThreadExceptionDelegate ListeningThreadException;
        /// <summary>
        /// Accade alla disconnessione di una connessione bridged.
        /// </summary>
        public event ConnectionDisconnectedDelegate ConnectionDisconnected;
        #endregion

        #region Constructors
        /// <summary>
        /// Instanzia un nuovo BridgeAcceptor ma non apre le porte finche' non viene invocato Start().
        /// </summary>
        /// <param name="AcceptPort">Porta TCP su cui attendere connessioni.</param>
        /// <param name="SQLServerEndpoint">Indirizzo TCP/IP dell'instanza SQL Server.</param>
        public BridgeAcceptor(int AcceptPort, System.Net.IPEndPoint SQLServerEndpoint)
        {
            this._iAcceptPort = AcceptPort;
            this._ipeSQLServer = SQLServerEndpoint;
        }
        #endregion

        public void Start()
        {
            Enabled = true;

            sAccept = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            sAccept.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Any, AcceptPort));
            sAccept.Listen(16);

            tAccept = new Thread(new ThreadStart(AcceptThread));
            tAccept.IsBackground = true;
            tAccept.Start();
        }

        public void Stop()
        {
            Enabled = false;

            if ((tAccept != null) && (tAccept.IsAlive))
            {
                if (!tAccept.Join(300))
                    tAccept.Abort();

                tAccept = null;
            }
        }

        public void AcceptThread()
        {
            try
            {
                while (Enabled)
                {
                    SocketCouple sc = new SocketCouple();

                    ManualResetEvent mre = new ManualResetEvent(false);

                    IAsyncResult res= sAccept.BeginAccept(new AsyncCallback((IAsyncResult ia) =>
                        {
                            bool fic = ia.IsCompleted;

                            sc.ClientBridgeSocket = sAccept.EndAccept(ia);

                            OnConnectionAccepted(sc.ClientBridgeSocket);

                            sc.BridgeSQLSocket = new Socket(SQLServerEndpoint.AddressFamily, SocketType.Stream, ProtocolType.IP);
                            sc.BridgeSQLSocket.Connect(SQLServerEndpoint);

                            BridgedConnection bc = new BridgedConnection(this, sc);
                            bc.Start();
                            mre.Set();
                        }), null);

                    mre.WaitOne();
                }
            }
            catch (Exception exce)
            {
                OnListeningThreadException(sAccept, exce);
            }
        }

        #region Event handlers
        internal virtual void OnTDSMessageReceived(BridgedConnection bc, Message.TDSMessage msg)
        {
            if (TDSMessageReceived != null)
                TDSMessageReceived(this, bc, msg);
        }

        internal virtual void OnTDSMessageReceived(BridgedConnection bc, Packet.TDSPacket packet)
        {
            if (TDSPacketReceived != null)
                TDSPacketReceived(this, bc, packet);
        }

        protected virtual void OnConnectionAccepted(Socket s)
        {
            if (ConnectionAccepted != null)
                ConnectionAccepted(this, s);
        }

        protected virtual void OnListeningThreadException(Socket s, Exception exce)
        {
            if (ListeningThreadException != null)
                ListeningThreadException(this, s, exce);
        }

        internal virtual void OnBridgeException(BridgedConnection bc, ConnectionType ct, Exception exce)
        {
            if (BridgeException != null)
                BridgeException(this, bc, ct, exce);
        }

        internal virtual void OnConnectionDisconnected(BridgedConnection bc, ConnectionType ct)
        {
            if (ConnectionDisconnected != null)
                ConnectionDisconnected(this, bc, ct);
        }
        #endregion
    }
}
