using System;

namespace AvatarController
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Exit on Ctrl+C
            Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs e) =>
            {
                e.Cancel = true;
                Exit();
            };
        }

        private static void Exit()
        {
            Console.WriteLine("Exiting...");
        }
    }
}
