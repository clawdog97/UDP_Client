using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;

namespace Tutorial2
{
    public class UDPClient
    {
        public const int PORT = 5000;

        private Socket? _socket;
        private EndPoint? _ep;
        private IPAddress? _ip_address;

        private byte[]? _buffer_recv;
        private ArraySegment<byte> _buffer_recv_segment;

        public void Initialize(string ipString, int port)
        {
            _buffer_recv = new byte[4096];
            _buffer_recv_segment = new(_buffer_recv);
            _ip_address = IPAddress.Parse(ipString);
            _ep = new IPEndPoint(_ip_address, port);

            _socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);
            _socket.Connect(_ep); //probably dont need as device doesnt need to listen to server
        }

        public void StartMessageLoop()
        {
            _ = Task.Run(async () =>
            {
                SocketReceiveMessageFromResult res;
                while (true)
                {
                    res = await _socket.ReceiveMessageFromAsync(_buffer_recv_segment, SocketFlags.None, _ep);
                    Console.WriteLine($"Received message: \"{Encoding.UTF8.GetString(_buffer_recv, 0, res.ReceivedBytes)}\".");
                }
            });
        }

        public async Task SendTo(byte[] data)
        {
            var s = new ArraySegment<byte>(data);
            await _socket.SendToAsync(s, SocketFlags.None, _ep);
        }
    }

    class Program
    {
        static async Task Main(/*String[] args*/)
        {
            string asciiPayload = "STT;0360000001;3FFFFF;36;1.0.14;1;20161117;08:37:39;0000004F;450;0;0014;20;+37.479323;+126.887827;62.03;65.43;10;1;00000101;00001000;1;2;0492";
            byte[] asciiData = Encoding.ASCII.GetBytes(asciiPayload);
            //string ipAddress = "15.200.50.1"; // public ip
            //string ipAddress = "172.31.17.37"; // private ip
            string ipAddress = "10.49.200.240"; // local wifi ip

            var client = new UDPClient();            
            client.Initialize(ipAddress, 11000);
            client.StartMessageLoop();
            await client.SendTo(asciiData);
            Console.WriteLine("Message sent!");
            Console.ReadLine();
        }
    }
}