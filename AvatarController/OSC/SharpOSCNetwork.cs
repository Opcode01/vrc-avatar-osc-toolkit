/// <summary>
/// Namespace OSC - A wrapper library to provide interoperability across various OSC communication implementations
/// </summary>
namespace OSC
{
    using SharpOSC;

    /// <summary>
    /// An implementation of OSCNetwork using SharpOSC
    /// </summary>
    internal class SharpOSCNetwork : IOSCNetwork, IDisposable
    {
        private string HOST = "127.0.0.1";
        private int SEND_PORT = 9000;
        private int RECV_PORT = 9001;

        private UDPSender _udpSender = null;
        private UDPListener _udpListener = null;

        public SharpOSCNetwork()
        {
            _udpSender = new SharpOSC.UDPSender(HOST, SEND_PORT);
            _udpListener = new SharpOSC.UDPListener(RECV_PORT); //Don't worry about actually receiving anything yet
        }

        public SharpOSCNetwork(string host, int sendPort, int receivePort)
        {
            HOST = host;
            SEND_PORT = sendPort;   
            RECV_PORT = receivePort;

            _udpSender = new SharpOSC.UDPSender(HOST, SEND_PORT);
            _udpListener = new SharpOSC.UDPListener(RECV_PORT); //Don't worry about actually receiving anything yet
        }

        public void SendMessage(string address, float value)
        {
            var message = new SharpOSC.OscMessage(address, value);
            _udpSender.Send(message);
            Console.WriteLine($"SEND: \t ENDPOINT: {HOST} \t ADDRESS: {address} \t FLOAT: {value}");
        }

        public void Dispose()
        {
            _udpSender.Close();
            _udpListener.Close();
            Console.WriteLine("UDP Connections closed.");
        }
    }
}
