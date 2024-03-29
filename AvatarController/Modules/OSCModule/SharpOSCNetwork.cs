﻿namespace OSCModule
{
    using AvatarController.Infrastructure;
    using global::OSCModule.Interfaces;
    using SharpOSC;

    /// <summary>
    /// An implementation of OSCNetwork using SharpOSC
    /// </summary>
    public class SharpOSCNetwork : IOSCNetwork
    {
        private string HOST = "127.0.0.1";
        private int SEND_PORT = 9000;
        private int RECV_PORT = 9001;

        private UDPSender _udpSender = null;
        private UDPListener _udpListener = null;

        private bool _isInitialized = false;

        public SharpOSCNetwork()
        {

        }

        public SharpOSCNetwork(string host, int sendPort, int receivePort)
        {
            HOST = host;
            SEND_PORT = sendPort;   
            RECV_PORT = receivePort;
        }

        public bool Initialize()
        {
            Console.WriteLine($"{this.GetType().Name} -- Initializing...");      //TODO: Better logging
            try
            {
                _udpSender = new SharpOSC.UDPSender(HOST, SEND_PORT);
                _udpListener = new SharpOSC.UDPListener(RECV_PORT, OnMessageReceived);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{this.GetType().Name} -- Error with initialization {ex.Message}, \n {ex.StackTrace}"); //TODO: Better logging
            }

            _isInitialized = (_udpListener != null && _udpSender != null);
            Console.WriteLine($"{this.GetType().Name} -- Init result: {_isInitialized}"); //TODO: Better logging
            return _isInitialized;
        }

        public void SendMessage(string address, object value)
        {
            var message = new SharpOSC.OscMessage(address, value);
            _udpSender.Send(message);
            Console.WriteLine($"SEND: \t ENDPOINT: {HOST} \t ADDRESS: {address} \t VALUE: {value}"); //TODO: Better logging
        }

        public event EventHandler<MsgReceivedEventArgs>? MessageReceived;

        private void OnMessageReceived(OscPacket packet)
        {
            if (packet != null)
            {
                var messageReceived = (OscMessage)packet;
                foreach(var value in messageReceived.Arguments)
                {
                    //DEBUG: Console.WriteLine($"RECEIVE: \t ENDPOINT: {HOST}:{RECV_PORT} \t ADDRESS: {messageReceived.Address} \t {value.GetType()}: {value}"); //TODO: Better logging
                }

                MessageReceived?.Invoke(this, new MsgReceivedEventArgs(messageReceived.Address, messageReceived.Arguments));
            }
        }

        public void Dispose()
        {
            _udpSender.Close();
            _udpListener.Close();
            Console.WriteLine("UDP Connections closed."); //TODO: Better logging
        }
    }
}
