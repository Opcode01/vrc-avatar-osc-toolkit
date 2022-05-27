namespace AvatarController
{
    using System;
    using OSC;
    using HapticsModule;

    internal class Program
    {
        private static bool _isRunning = true;
        private static IOSCNetwork _network;
        private static IControllerHaptics _leftControllerHaptics;
        private static IControllerHaptics _rightControllerHaptics;

        static void Main(string[] args)
        {
            //Exit on Ctrl+C
            Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs e) =>
            {
                e.Cancel = true;
                _isRunning = false;
            };

            //Initialize HapticsModule
            _leftControllerHaptics = new OVRControllerHaptics();
            _isRunning &= _leftControllerHaptics.Initialize(ControllerType.LEFTHAND);
            _rightControllerHaptics = new OVRControllerHaptics();
            _isRunning &= _rightControllerHaptics.Initialize(ControllerType.RIGHTHAND);

            //Initialize SharpOSCNetwork
            _network = new SharpOSCNetwork();
            var messageHandlers = new List<EventHandler<OSCMsgReceivedEventArgs>>() {
                (object? sender, OSCMsgReceivedEventArgs msg) => { 
                    switch(msg.Address)
                    {
                        case "/avatar/parameters/RightHandTouch":
                        {
                            float proximity = (float)msg.Contents[0];
                            ushort intensity = (ushort)Math.Normalize(proximity, 0, 1, 100, 1000);
                            _rightControllerHaptics.TriggerHapticPulse(intensity);
                            Console.WriteLine($"Received {msg.Address} \t Proximity value: {proximity} \t Normalized Intensity: {intensity}");
                            break;
                        }
                        case "/avatar/parameters/LeftHandTouch":
                        {
                            float proximity = (float)msg.Contents[0];
                            ushort intensity = (ushort)Math.Normalize(proximity, 0, 1, 100, 1000);
                            _leftControllerHaptics.TriggerHapticPulse(intensity);
                            Console.WriteLine($"Received {msg.Address} \t Proximity value: {proximity} \t Normalized Intensity: {intensity}");
                            break;
                        }
                        default:
                            //Do nothing
                            break;
                    }                    
                }
            };
            _isRunning &= _network.Initialize(messageHandlers);

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
            _leftControllerHaptics.Dispose();
        }
    }

    public static class Math
    {
        /// <summary>
        /// Normalizes in the bounds specified
        /// </summary>
        public static float Normalize(float input, float maxValue, float minValue, float upperBounds, float lowerBounds)
        {
            return ((upperBounds - lowerBounds) * ((input - minValue) / (maxValue - minValue)) + lowerBounds);
        }

        /// <summary>
        /// Normalizes in -1 to 1
        /// </summary>
        public static float Normalize(float input, float maxValue, float minValue)
        {
            return Normalize(input, maxValue, minValue, 1f, -1f);
        }
    }
}
