using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RemoteControlService.MessageReception
{
    public class MessageReceptionist : IMessageReceptionist
    {
        readonly ManualResetEvent allDone = new ManualResetEvent(false);
        const char MessageTerminator = '\\';
        const int Port = 11000;
        IPEndPoint localEndPoint;
        Socket listener;
        bool shouldRun = false;
        Thread messageListenerThread;
        private UdpClient udpClient;

        public MessageReceptionist()
        {
            SetLocalEndPoint();
            SubscribeToNetworkAddressChangedEvent();
        }

        public void Start()
        {
            shouldRun = true;
            StartMessageListener();
            StartNameLookupReceptionist();
        }

        private void StartMessageListener()
        {
            messageListenerThread = new Thread(() =>
            {
                // Create a TCP/IP socket.  
                listener = new Socket(addressFamily: localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Bind the socket to the local endpoint and listen for incoming connections.  
                try
                {
                    listener.Bind(localEndPoint);
                    listener.Listen(10);
                    Trace.WriteLine("Listening for a connection...");
                    while (shouldRun)
                    {
                        // Set the event to nonsignaled state.  
                        allDone.Reset();

                        // Start an asynchronous socket to listen for connections.                          
                        listener.BeginAccept(
                            new AsyncCallback(AcceptCallback),
                            listener);

                        // Wait until a connection is made before continuing.
                        allDone.WaitOne();
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.ToString());
                }
                finally
                {
                    listener.Dispose();
                }
            });

            messageListenerThread.Start();
        }

        public void Stop()
        {
            shouldRun = false;
            StopMessageListener();
            StopNameLookupReceptionist();
        }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        private void StopMessageListener()
        {
            allDone.Set();
            WaitForMessageListenerThread();
        }

        private void StopNameLookupReceptionist()
        {
            udpClient.Close();
        }

        private void SetLocalEndPoint()
        {
            IPAddress ip = GetMyIP();
            while (!IPAddressValid(ip))
            {
                Trace.TraceWarning("I don't have a valid IP yet.");
                Thread.Sleep(TimeSpan.FromSeconds(5));
                ip = GetMyIP();
            }

            Trace.WriteLine($"My IP:{ip}");

            localEndPoint = new IPEndPoint(ip, Port);
        }

        private IPAddress GetMyIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }

            return IPAddress.Loopback;
        }

        private static bool IPAddressValid(IPAddress ip)
        {
            return !IPAddress.Loopback.Equals(ip);
        }

        private void SubscribeToNetworkAddressChangedEvent()
        {
            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(AddressChangedCallback);
        }

        private void AddressChangedCallback(object sender, EventArgs e)
        {
            IPAddress myIP = GetMyIP();
            if (!IPAddressValid(myIP) || localEndPoint.Address.Equals(myIP))
            {
                return;
            }

            Trace.WriteLine("Address changed");
            SetLocalEndPoint();
            if (shouldRun)
            {
                Restart();
            }
        }

        private void Restart()
        {
            Stop();
            Start();
        }

        private void WaitForMessageListenerThread()
        {
            if (messageListenerThread != null)
            {
                messageListenerThread.Join();
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            // Signal thread to continue.
            allDone.Set();

            Socket listener = ar.AsyncState as Socket;
            Socket handler;
            try
            {
                handler = listener.EndAccept(ar);
            }
            catch (ObjectDisposedException)
            {
                // We don't care why the socket was disposed.
                return;
            }
            var state = new StateObject
            {
                workSocket = handler
            };

            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            var message = string.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read
                // more data.
                if (state.sb[state.sb.Length - 1] == MessageTerminator)
                {
                    // All the data has been read from the client.
                    message = state.sb.ToString(0, state.sb.Length - 1);

                    Trace.WriteLine($"Read {message.Length} bytes from {((IPEndPoint)handler.RemoteEndPoint).Address}. Data : {message}");

                    OnMessageReceived(message);

                    CloseHandler(handler);
                }
                else
                {
                    // Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private static void CloseHandler(Socket handler)
        {
            try
            {
                handler.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException ex)
            {
                Trace.WriteLine($"Error closing handler: {ex}");
            }
            finally
            {
                handler.Close();
            }
        }

        private void OnMessageReceived(string message)
        {
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
        }

        private void StartNameLookupReceptionist()
        {
            new Thread(() =>
            {
                Trace.WriteLine("Listening for name lookup");
                using (udpClient = new UdpClient(Port))
                {
                    var clientEndpoint = new IPEndPoint(IPAddress.Any, 0);

                    while (shouldRun)
                    {
                        TryListeningAndRespondingToNameLookup(udpClient, clientEndpoint);
                    }
                }
            }).Start();
        }

        private void TryListeningAndRespondingToNameLookup(UdpClient udpClient, IPEndPoint clientEndpoint)
        {
            try
            {
                ListenAndRespondToNameLookup(udpClient, clientEndpoint);
            }
            catch (Exception e)
            {
                if (shouldRun)
                {
                    Trace.TraceError($"Error occured while listening and responding to name lookup: {e.Message}");
                }
            }
        }

        private static void ListenAndRespondToNameLookup(UdpClient udpClient, IPEndPoint clientEndpoint)
        {
            var clientRequestBytes = udpClient.Receive(ref clientEndpoint);
            var clientRequestMessage = Encoding.ASCII.GetString(clientRequestBytes);
            Trace.WriteLine($"Received \"{clientRequestMessage}\" from {clientEndpoint.Address}");
            var response = Encoding.ASCII.GetBytes(Environment.MachineName);
            udpClient.Send(response, response.Length, clientEndpoint);
        }
    }
}
