namespace AvatarController.Infrastructure.Interfaces
{
    /// <summary>
    /// Responsible for receiving messages from INetwork(s) and handing them off to IModule(s)
    /// </summary>
    public interface IMessageDistributor
    {
        /// <summary>
        /// Networks receive messages
        /// </summary>
        /// <param name="network"></param>
        public void AddNetwork(INetwork network);

        /// <summary>
        /// A Module will assign an action to perform when a message is received from the address provided
        /// </summary>
        /// <param name="address">The message address to listen for</param>
        /// <param name="eventHandler">The action to perform</param>
        public void AddBinding(string address, EventHandler<MsgReceivedEventArgs> eventHandler);

        /// <summary>
        /// Returns the INetworks attached to this instance
        /// </summary>
        /// <returns></returns>
        public ICollection<INetwork> GetNetworks();

        /// <summary>
        /// Toggles the receiving of messages
        /// </summary>
        public void ToggleReceiving();
    }
}
