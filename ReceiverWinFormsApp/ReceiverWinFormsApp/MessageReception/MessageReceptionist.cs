using Domain.Common.Utilities;
using Domain.MessageReception;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReceiverWinFormsApp.MessageReception
{
    public class MessageReceptionist : IMessageReceptionist
    {
        const char MessageTerminator = '\n';
        const int Port = 11000;
        readonly ILogger<MessageReceptionist> logger;
        readonly IPEndPoint localEndpoint;

        bool shouldRun = false;
        TcpListener messageListener;
        UdpClient udpClient;

        public MessageReceptionist(ILogger<MessageReceptionist> logger)
        {
            this.logger = logger;

            localEndpoint = GetLocalEndPoint();
            NetworkChange.NetworkAddressChanged += AddressChangedCallback;
        }

        public void Start()
        {
            shouldRun = true;
            Task.Run(StartMessageReceptionist);
            StartNameLookupReceptionist();
        }

        public void Stop()
        {
            shouldRun = false;
            messageListener.Stop();
            udpClient.Close();
        }

        public Func<string, Maybe<string>> MessageProcessor { get; set; } = x => Maybe<string>.None();

        private IPEndPoint GetLocalEndPoint()
        {
            IPAddress ip = GetMyIP();
            while (!IPAddressValid(ip))
            {
                logger.LogWarning("I don't have a valid IP yet.");
                Thread.Sleep(TimeSpan.FromSeconds(5));
                ip = GetMyIP();
            }

            logger.LogInformation("My IP:{IP}", ip);
            return new IPEndPoint(ip, Port);
        }

        private static IPAddress GetMyIP()
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

        private static bool IPAddressValid(IPAddress ip) =>
            !IPAddress.Loopback.Equals(ip);

        private void AddressChangedCallback(object sender, EventArgs e)
        {
            IPAddress myIP = GetMyIP();
            if (!IPAddressValid(myIP) || localEndpoint.Address.Equals(myIP))
            {
                return;
            }
            logger.LogInformation("Address changed");
            GetLocalEndPoint();
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

        private async Task StartMessageReceptionist()
        {
            messageListener = new(localEndpoint);
            try
            {
                messageListener.Start();

                while (shouldRun)
                {
                    using TcpClient handler = await messageListener.AcceptTcpClientAsync();
                    await using NetworkStream stream = handler.GetStream();
                    using var memoryOwner = MemoryPool<byte>.Shared.Rent(128);
                    var numberOfBytesRead = await stream.ReadAsync(memoryOwner.Memory);
                    var message = Encoding.ASCII.GetString(memoryOwner.Memory[..numberOfBytesRead].Span);
                    logger.LogInformation("Received message: {Message}", message);
                    if (message[^1] != MessageTerminator)
                    {
                        logger.LogError("Received message does not end with the expected terminator character");
                        return;
                    }
                    var response = MessageProcessor(message);
                    response.Match(
                        none: () => logger.LogInformation("No response generated for the message"),
                        some: async responseMessage =>
                        {
                            var responseBytes = Encoding.ASCII.GetBytes(responseMessage);
                            await stream.WriteAsync(responseBytes);
                            logger.LogInformation("Sent response: {ResponseMessage}", responseMessage);
                        });
                }
            }
            finally
            {
                messageListener.Stop();
                logger.LogInformation("Message receptionist is stopped");
            }
        }

        private void StartNameLookupReceptionist()
        {
            new Thread(() =>
            {
                logger.LogInformation("Listening for name lookup");
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

        private void TryListeningAndRespondingToNameLookup(
            UdpClient udpClient,
            IPEndPoint clientEndpoint)
        {
            try
            {
                ListenAndRespondToNameLookup(udpClient, clientEndpoint);
            }
            catch (Exception ex)
            {
                if (shouldRun)
                {
                    logger.LogError(ex, "Error occured while listening and responding to name lookup");
                }
            }
        }

        private void ListenAndRespondToNameLookup(
            UdpClient udpClient,
            IPEndPoint clientEndpoint)
        {
            var clientRequestBytes = udpClient.Receive(ref clientEndpoint);
            var clientRequestMessage = Encoding.ASCII.GetString(clientRequestBytes);
            logger.LogInformation("Received \"{ClientRequestMessage}\" from {ClientEndpointAddress}", clientRequestMessage, clientEndpoint.Address);
            var response = Encoding.ASCII.GetBytes(Environment.MachineName);
            udpClient.Send(response, response.Length, clientEndpoint);
        }
    }
}
