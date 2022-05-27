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
                _networks.Add(network);
                network.MessageReceived += OnMessageReceived;
            }
        }

        public void AddBinding(string address, EventHandler<MsgReceivedEventArgs> eventHandler)
        {
            _bindings.Add(address, eventHandler);
        }

        private void OnMessageReceived(object? sender, MsgReceivedEventArgs eventArgs)
        {
            Console.WriteLine($"{this.GetType().Name} -- Received message From: \t {sender?.GetType().Name} \t Address: {eventArgs.Address} \t Data: {eventArgs.Contents}"); //TODO: Better logging
            foreach (var binding in _bindings)
            {
                if(eventArgs.Address == binding.Key)
                {
                    Console.WriteLine($"Found binding for {eventArgs.Address}!");
                    binding.Value?.Invoke(this, eventArgs);
                }
            }
        }
    }
}
