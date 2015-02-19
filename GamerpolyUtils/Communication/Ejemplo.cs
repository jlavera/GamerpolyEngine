using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SBlackler.Networking {
    public sealed class HighPerformanceServer {
        private Int32 _currentConnections = 0;
        Socket listener;
        EndPoint ipeSender;


        #region "Properties"

        public Int32 Port { get; set; }
        public Int32 CurrentConnections { get { return _currentConnections; } }
        public Int32 MaxQueuedConnections { get; set; }
        public IPEndPoint Endpoint { get; set; }
        public ServerType Type { get; set; }

        #endregion

        #region "Constructors"

        private HighPerformanceServer() {
            // do nothing
        }

        public HighPerformanceServer(ServerType type, String IpAddress) {
            Init(type, IpAddress, 28930);
        }

        public HighPerformanceServer(ServerType type, String IpAddress, Int32 Port) {
            Init(type, IpAddress, Port);
        }

        private void Init(ServerType server, String IpAddress, Int32 Port) {
            IPAddress ip;
            // Check the IpAddress to make sure that it is valid
            if (!String.IsNullOrEmpty(IpAddress) && IPAddress.TryParse(IpAddress, out ip)) {
                this.Endpoint = new IPEndPoint(ip, Port);
                // Make sure that the port is greater than 100 as not to conflict with any other programs
                if (Port < 100) {
                    throw new ArgumentException("The argument 'Port' is not valid. Please select a value greater than 100.");
                } else {
                    this.Port = Port;
                }
            } else {
                throw new ArgumentException("The argument 'IpAddress' is not valid");
            }
            // We never want a ServerType of None, but we include it as it is recommended by FXCop.
            if (server != ServerType.None) {
                this.Type = server;
            } else {
                throw new ArgumentException("The argument 'ServerType' is not valid");
            }
        }
        #endregion

        #region "Events"

        public event EventHandler<EventArgs> OnServerStart;
        public event EventHandler<EventArgs> OnServerStarted;
        public event EventHandler<EventArgs> OnServerStopping;
        public event EventHandler<EventArgs> OnServerStoped;
        public event EventHandler<EventArgs> OnClientConnected;
        public event EventHandler<EventArgs> OnClientDisconnecting;
        public event EventHandler<EventArgs> OnClientDisconnected;
        public event EventHandler<EventArgs> OnDataReceived;

        #endregion

        public void Start() {
            // Tell anything that is listening that we have starting to work
            if (OnServerStart != null) {
                OnServerStart(this, null);
            }

            // Get either a TCP or UDP socket depending on what we specified when we created the class
            listener = GetCorrectSocket();

            if (listener != null) {
                // Bind the socket to the endpoint
                listener.Bind(this.Endpoint);

                // TODO :: Add throttleling (using SEMAPHORE's)

                if (this.Type == ServerType.TCP) {
                    // Start listening to the socket, accepting any backlog
                    listener.Listen(this.MaxQueuedConnections);

                    // Use the BeginAccept to accept new clients
                    listener.BeginAccept(new AsyncCallback(ClientConnected), listener);
                } else if (this.Type == ServerType.UDP) {
                    // So we can buffer and store information, create a new information class
                    SocketConnectionInfo connection = new SocketConnectionInfo();
                    connection.Buffer = new byte[SocketConnectionInfo.BufferSize];
                    connection.Socket = listener;
                    // Setup the IPEndpoint
                    ipeSender = new IPEndPoint(IPAddress.Any, this.Port);
                    // Start recieving from the client
                    listener.BeginReceiveFrom(connection.Buffer, 0, connection.Buffer.Length, SocketFlags.None, ref ipeSender, new AsyncCallback(DataReceived), connection);
                }

                // Tell anything that is listening that we have started to work
                if (OnServerStarted != null) {
                    OnServerStarted(this, null);
                }
            } else {
                // There was an error creating the correct socket
                throw new InvalidOperationException("Could not create the correct sever socket type.");
            }
        }

        internal Socket GetCorrectSocket() {
            if (this.Type == ServerType.TCP) {
                return new Socket(this.Endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            } else if (this.Type == ServerType.UDP) {
                return new Socket(this.Endpoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            } else {
                return null;
            }
        }

        public void Stop() {
            if (OnServerStopping != null) {
                OnServerStopping(this, null);
            }

            if (OnServerStoped != null) {
                OnServerStoped(this, null);
            }
        }

        internal void ClientConnected(IAsyncResult asyncResult) {
            // Increment our ConcurrentConnections counter
            Interlocked.Increment(ref _currentConnections);

            // So we can buffer and store information, create a new information class
            SocketConnectionInfo connection = new SocketConnectionInfo();
            connection.Buffer = new byte[SocketConnectionInfo.BufferSize];

            // We want to end the async event as soon as possible
            Socket asyncListener = (Socket)asyncResult.AsyncState;
            Socket asyncClient = asyncListener.EndAccept(asyncResult);

            // Set the SocketConnectionInformations socket to the current client
            connection.Socket = asyncClient;

            // Tell anyone that's listening that we have a new client connected
            if (OnClientConnected != null) {
                OnClientConnected(this, null);
            }

            // TODO :: Add throttleling (using SEMAPHORE's)

            // Begin recieving the data from the client
            if (this.Type == ServerType.TCP) {
                asyncClient.BeginReceive(connection.Buffer, 0, connection.Buffer.Length, SocketFlags.None, new AsyncCallback(DataReceived), connection);
            } else if (this.Type == ServerType.UDP) {
                asyncClient.BeginReceiveFrom(connection.Buffer, 0, connection.Buffer.Length, SocketFlags.None, ref ipeSender, new AsyncCallback(DataReceived), connection);
            }
            // Now we have begun recieving data from this client,
            // we can now accept a new client
            listener.BeginAccept(new AsyncCallback(ClientConnected), listener);
        }

        internal void DataReceived(IAsyncResult asyncResult) {
            try {
                SocketConnectionInfo connection = (SocketConnectionInfo)asyncResult.AsyncState;
                Int32 bytesRead;
                // End the correct async process
                if (this.Type == ServerType.UDP) {
                    bytesRead = connection.Socket.EndReceiveFrom(asyncResult, ref ipeSender);
                } else if (this.Type == ServerType.TCP) {
                    bytesRead = connection.Socket.EndReceive(asyncResult);
                } else {
                    bytesRead = 0;
                }
                // Increment the counter of BytesRead
                connection.BytesRead += bytesRead;
                // Check to see whether the socket is connected or not...
                if (IsSocketConnected(connection.Socket)) {
                    // If we have read no more bytes, raise the data received event
                    if (bytesRead == 0 || (bytesRead > 0 && bytesRead < SocketConnectionInfo.BufferSize)) {
                        byte[] buffer = connection.Buffer;
                        Int32 totalBytesRead = connection.BytesRead;
                        // Setup the connection info again ready for another packet
                        connection = new SocketConnectionInfo();
                        connection.Buffer = new byte[SocketConnectionInfo.BufferSize];
                        connection.Socket = ((SocketConnectionInfo)asyncResult.AsyncState).Socket;
                        // Fire off the receive event as quickly as possible, then we can process the data...
                        if (this.Type == ServerType.UDP) {
                            connection.Socket.BeginReceiveFrom(connection.Buffer, 0, connection.Buffer.Length, SocketFlags.None, ref ipeSender, new AsyncCallback(DataReceived), connection);
                        } else if (this.Type == ServerType.TCP) {
                            connection.Socket.BeginReceive(connection.Buffer, 0, connection.Buffer.Length, SocketFlags.None, new AsyncCallback(DataReceived), connection);
                        }
                        // Remove any extra data
                        if (totalBytesRead < buffer.Length) {
                            Array.Resize<Byte>(ref buffer, totalBytesRead);
                        }
                        // Now raise the event, sender will contain the buffer for now
                        if (OnDataReceived != null) {
                            OnDataReceived(buffer, null);
                        }
                        buffer = null;
                    } else {
                        // Resize the array ready for the next chunk of data
                        Array.Resize<Byte>(ref connection.Buffer, connection.Buffer.Length + SocketConnectionInfo.BufferSize);
                        // Fire off the receive event again, with the bigger buffer
                        if (this.Type == ServerType.UDP) {
                            connection.Socket.BeginReceiveFrom(connection.Buffer, 0, connection.Buffer.Length, SocketFlags.None, ref ipeSender, new AsyncCallback(DataReceived), connection);
                        } else if (this.Type == ServerType.TCP) {
                            connection.Socket.BeginReceive(connection.Buffer, 0, connection.Buffer.Length, SocketFlags.None, new AsyncCallback(DataReceived), connection);
                        }
                    }
                } else if (connection.BytesRead > 0) {
                    // We still have data
                    Array.Resize<Byte>(ref connection.Buffer, connection.BytesRead);
                    // call the event
                    if (OnDataReceived != null) {
                        OnDataReceived(connection.Buffer, null);
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        internal bool IsSocketConnected(Socket socket) {
            return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
        }

        internal void DisconnectClient(SocketConnectionInfo connection) {
            if (OnClientDisconnecting != null) {
                OnClientDisconnecting(this, null);
            }
            connection.Socket.BeginDisconnect(true, new AsyncCallback(ClientDisconnected), connection);
        }

        internal void ClientDisconnected(IAsyncResult asyncResult) {
            SocketConnectionInfo sci = (SocketConnectionInfo)asyncResult;
            sci.Socket.EndDisconnect(asyncResult);
            if (OnClientDisconnected != null) {
                OnClientDisconnected(this, null);
            }
        }
    }

    public class SocketConnectionInfo {
        public const Int32 BufferSize = 1048576;
        public Socket Socket;
        public byte[] Buffer;
        public Int32 BytesRead { get; set; }
    }


    public enum ServerType {
        None = 0,
        TCP = 1,
        UDP = 2
    }
}