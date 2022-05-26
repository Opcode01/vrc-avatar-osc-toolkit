namespace AvatarController
{
    using System;
    using OSC;
    using HapticsModule;

    internal class Program
    {
        private static bool _isRunning = true;
        private static IOSCNetwork _network;
        private static IControllerHaptics _controllerHaptics;

        static void Main(string[] args)
        {
            //Exit on Ctrl+C
            Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs e) =>
            {
                e.Cancel = true;
                _isRunning = false;
            };

            //Initialize SharpOSCNetwork
            _network = new SharpOSCNetwork();
            var messageHandlers = new List<EventHandler<OSCMsgReceivedEventArgs>>() { 
                (object? sender, OSCMsgReceivedEventArgs msg) => { 
                    //Console.WriteLine($"We received a message! From: {msg.Address} with {msg.Contents.Count} arguments");
                }
            };            
            _isRunning &= _network.Initialize(messageHandlers);

            //Initialize HapticsModule
            _controllerHaptics = new OVRControllerHaptics();
            _isRunning &= _controllerHaptics.Initialize();

            //Main thread
            if (_isRunning)
            {
                Console.WriteLine("All modules loaded - controller running");
                while (_isRunning)
                {
                    // Do stuff
                }
            }

            Exit();
        }

        private static void Exit()
        {
            Console.WriteLine("Exiting...");
            _isRunning = false;
            _controllerHaptics.Dispose();
        }
    }
}
