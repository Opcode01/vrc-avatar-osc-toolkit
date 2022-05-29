namespace OSCModule
{
    using AvatarController.Infrastructure.Interfaces;

    /// <summary>
    /// External Dependencies - 
    ///     SharpOSC  
    /// </summary>
    public class OSCModuleBase : IModule
    {
        private bool _isInitialized = false;
        private readonly IOSCNetwork _network;
        private readonly IMessageDistributor _messageDistributor;   

        public OSCModuleBase(IMessageDistributor messageDistributor)
        {
            _network = new SharpOSCNetwork();
            _messageDistributor = messageDistributor;   
        }

        public bool Initialize()
        {
            if (_isInitialized)
            {
                Console.WriteLine($"Error -- {this.GetType().Name} is already initialized");
                return false;
            }

            Console.WriteLine($"{this.GetType().Name} -- Initializing...");      //TODO: Better logging
            try
            {
                _isInitialized = _network.Initialize();
                _messageDistributor.AddNetwork(_network);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{this.GetType().Name} -- Error with initialization - {ex.Message}, \n{ex.StackTrace}"); //TODO: Better logging
                _isInitialized = false;
            }

            Console.WriteLine($"{this.GetType().Name} -- Init result: {_isInitialized}"); //TODO: Better logging
            return _isInitialized;
        }

        public void Dispose()
        {
            _network.Dispose();
        }
    }
}
