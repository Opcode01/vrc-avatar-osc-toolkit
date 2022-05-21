

namespace AvatarController
{
    using System;
    using OSC;

    internal class Program
    {
        private static bool _isRunning = true;
        private static IOSCNetwork _network;

        static void Main(string[] args)
        {
            //Exit on Ctrl+C
            Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs e) =>
            {
                e.Cancel = true;
                Exit();
            };

            //Initialize SharpOSCNetwork
            _network = new SharpOSCNetwork();
            _network.MessageReceived += (object? sender, OSCMsgReceivedEventArgs msg) =>
            {
                //Console.WriteLine($"We received a message! From: {msg.Address} with {msg.Contents.Count} arguments");
            };

            //Main thread
            Console.WriteLine("All modules loaded - controller running");
            while (_isRunning)
            {
                // Do stuff
            }
        }

        private static void Exit()
        {
            Console.WriteLine("Exiting...");
            _isRunning = false;
        }
    }
}
