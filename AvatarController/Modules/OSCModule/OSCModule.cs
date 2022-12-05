namespace OSCModule
{
    using AvatarController.Infrastructure.Interfaces;
    using global::OSCModule.Interfaces;
    using AvatarController.Infrastructure;

    /// <summary>
    /// External Dependencies - 
    ///     SharpOSC  
    /// </summary>
    public class OSCModule : ModuleBase
    {
        private readonly IOSCNetwork _network;
        private readonly IMessageDistributor _messageDistributor;   

        public OSCModule(IMessageDistributor messageDistributor)
        {
            _network = new SharpOSCNetwork();
            _messageDistributor = messageDistributor;   
        }

        public override bool Initialize()
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

        public override void Dispose()
        {
            _network.Dispose();
        }
    }
}
