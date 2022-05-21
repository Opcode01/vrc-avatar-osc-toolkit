/// <summary>
/// Namespace OSC - A wrapper library to provide interoperability across various OSC communication implementations
/// </summary>
namespace OSC
{
    using SharpOSC;

    /// <summary>
    /// An implementation of OSCNetwork using SharpOSC
    /// </summary>
    public class SharpOSCNetwork : IOSCNetwork, IDisposable
    {
        private string HOST = "127.0.0.1";
        private int SEND_PORT = 9000;
        private int RECV_PORT = 9001;

        private UDPSender _udpSender = null;
        private UDPListener _udpListener = null;

        public SharpOSCNetwork()
        {
            Console.WriteLine("Initializing SharpOSCNetwork...");

            _udpSender = new SharpOSC.UDPSender(HOST, SEND_PORT);
            _udpListener = new SharpOSC.UDPListener(RECV_PORT, OnMessageReceived); 
        }

        public SharpOSCNetwork(string host, int sendPort, int receivePort)
        {
            Console.WriteLine("Initializing SharpOSCNetwork...");

            HOST = host;
            SEND_PORT = sendPort;   
            RECV_PORT = receivePort;

            _udpSender = new SharpOSC.UDPSender(HOST, SEND_PORT);
            _udpListener = new SharpOSC.UDPListener(RECV_PORT, OnMessageReceived); 
        }

        public void SendMessage(string address, object value)
        {
            var message = new SharpOSC.OscMessage(address, value);
            _udpSender.Send(message);
            Console.WriteLine($"SEND: \t ENDPOINT: {HOST} \t ADDRESS: {address} \t VALUE: {value}");
        }

        public event EventHandler<OSCMsgReceivedEventArgs> MessageReceived;

        private void OnMessageReceived(OscPacket packet)
        {
            if (packet != null)
            {
                var messageReceived = (OscMessage)packet;
                foreach(var value in messageReceived.Arguments)
                {
                    Console.WriteLine($"RECEIVE: \t ENDPOINT: {HOST}:{RECV_PORT} \t ADDRESS: {messageReceived.Address} \t {value.GetType()}: {value}");
                }

                MessageReceived?.Invoke(this, new OSCMsgReceivedEventArgs(messageReceived.Address, messageReceived.Arguments));
            }
        }

        public void Dispose()
        {
            _udpSender.Close();
            _udpListener.Close();
            Console.WriteLine("UDP Connections closed.");
        }
    }
}
