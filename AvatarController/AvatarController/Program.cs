namespace AvatarController
{
    using System;
    using HapticsModule;
    using OSCModule;
    using EyeTrackingModule;
    using AvatarController.Infrastructure;
    using AvatarController.Infrastructure.Interfaces;

    internal class Program
    {
        private static bool _isRunning = true;
        private static IMessageDistributor _messageDistributor;
        private static IModule _OSCNetworkModule;
        private static IModule _hapticsModule;
        private static IModule _eyeTrackingModule;
        private static ICollection<Task> _tasks;

        static async Task Main(string[] args)
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
            _OSCNetworkModule = new OSCModule(_messageDistributor);
            _hapticsModule = new HapticsModule(_messageDistributor);   //TODO: Eventually construct all these with a DI framework - load up additional plugin .dlls at runtime
            _eyeTrackingModule = new EyeTrackingModule(_messageDistributor);
            
            //Initialize modules
            _isRunning &= _OSCNetworkModule.Initialize();
            _isRunning &= _hapticsModule.Initialize();
            _isRunning &= _eyeTrackingModule.Initialize();

            //Create task list
            _tasks = new List<Task>();

            //Main thread
            if (_isRunning)
            {
                Console.WriteLine("All modules loaded - controller running");
                while (_isRunning)
                {
                    using(var cts = new CancellationTokenSource())
                    {
                        _tasks.Clear();

                        //Start each modules update tasks
                        _tasks.Add(_OSCNetworkModule.UpdateAsync(cts.Token));
                        _tasks.Add(_hapticsModule.UpdateAsync(cts.Token));
                        _tasks.Add(_eyeTrackingModule.UpdateAsync(cts.Token));

                        //Process input from command line
                        ProcessInput(cts);

                        //Run the synchronous updates for each module
                        _OSCNetworkModule.Update();
                        _hapticsModule.Update();
                        _eyeTrackingModule.Update();

                        //Await all tasks to complete
                        await Task.WhenAll(_tasks);

                        //Manual delay for the next update
                        Thread.Sleep(100);
                    }
                }
            }

            Exit();
        }

        //TODO: Maybe we should allow each module to process its own input and just forward the key pressed along to it?
        /// <summary>
        /// Processes command line input
        /// </summary>
        /// <param name="cts"></param>
        private static void ProcessInput(CancellationTokenSource cts)
        {
            //DEBUG: Console.WriteLine("Processing input...");
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.G:
                        (_eyeTrackingModule as EyeTrackingModule)?.RecalibrateEyeTracking();
                        break;
                    case ConsoleKey.Escape:
                        //Pause
                        cts.Cancel();           //Stop any running tasks
                        _messageDistributor.ToggleReceiving();  //Stop receiving messages
                        Task.WhenAll(_tasks).Wait();
                        Console.WriteLine("Press any key to resume...");
                        Console.ReadKey();
                        _messageDistributor.ToggleReceiving();  //Start receiving messages
                        break;
                    default:
                        //Do nothing
                        break;
                }
            }
        }

        private static void Exit()
        {
            Console.WriteLine("Exiting...");
            _isRunning = false;
            _hapticsModule.Dispose();
            _eyeTrackingModule.Dispose();
            _OSCNetworkModule.Dispose();
        }
    }
}
