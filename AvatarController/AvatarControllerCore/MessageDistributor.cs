namespace AvatarController.Infrastructure
{
    using AvatarController.Infrastructure.Interfaces;

    public class MessageDistributor : IMessageDistributor
    {
        private readonly Dictionary<string, EventHandler<MsgReceivedEventArgs>> _bindings;
        private readonly ICollection<INetwork> _networks;

        public MessageDistributor()
        {
            _bindings = new Dictionary<string, EventHandler<MsgReceivedEventArgs>>(); 
            _networks = new List<INetwork>();
        }

        public void AddNetwork(INetwork network)
        {
            if(network != null)
            {
                Console.WriteLine($"{this.GetType().Name} -- Adding Network provider {network.GetType().Name}");
                _networks.Add(network);
                network.MessageReceived += OnMessageReceived;
            }
        }

        public void AddBinding(string address, EventHandler<MsgReceivedEventArgs> eventHandler)
        {
            Console.WriteLine($"{this.GetType().Name} -- Creating binding for {address}");
            _bindings.Add(address, eventHandler);
        }

        private void OnMessageReceived(object? sender, MsgReceivedEventArgs eventArgs)
        {
            //DEBUG: Console.WriteLine($"{this.GetType().Name} -- Received message From: \t {sender?.GetType().Name} \t Address: {eventArgs.Address}"); //TODO: Better logging
            foreach (var binding in _bindings)
            {
                if(eventArgs.Address == binding.Key)
                {
                    //DEBUG: Console.WriteLine($"Found binding for {eventArgs.Address}!");
                    binding.Value?.Invoke(this, eventArgs);
                }
            }
        }
    }
}
