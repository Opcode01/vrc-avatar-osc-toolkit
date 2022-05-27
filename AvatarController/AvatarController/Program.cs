namespace AvatarController
{
    using System;
    using HapticsModule;
    using OSCModule;
    using AvatarController.Infrastructure;
    using AvatarController.Infrastructure.Interfaces;

    internal class Program
    {
        private static bool _isRunning = true;
        private static IMessageDistributor _messageDistributor;
        private static IModule _OSCNetworkModule;
        private static IModule _hapticsModule;

        static void Main(string[] args)
        {
            //Exit on Ctrl+C
            Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs e) =>
            {
                e.Cancel = true;
                _isRunning = false;
            };

            //Construct MessageDistributor
            _messageDistributor = new MessageDistributor();

            //Construct modules
            _OSCNetworkModule = new OSCModuleBase(_messageDistributor);
            _hapticsModule = new HapticsModuleBase(_messageDistributor);   //TODO: Eventually construct all these with a DI framework - load up additional plugin .dlls at runtime
            
            //Initialize modules
            _isRunning &= _OSCNetworkModule.Initialize();
            _isRunning &= _hapticsModule.Initialize();

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
            _hapticsModule.Dispose();
            _OSCNetworkModule.Dispose();
        }
    }
}
