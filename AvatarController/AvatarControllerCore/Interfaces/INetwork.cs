namespace AvatarController.Infrastructure.Interfaces
{
    /// <summary>
    /// Receives messages through some mechanism and hands them off to receivers
    /// </summary>
    public interface INetwork
    {
        /// <summary>
        /// Initializes the network
        /// </summary>
        /// <returns></returns>
        bool Initialize();

        /// <summary>
        /// Send a message to the specified address
        /// </summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        void SendMessage(string address, object value);

        /// <summary>
        /// Event for when a message is received by this network
        /// </summary>
        event EventHandler<MsgReceivedEventArgs> MessageReceived;
    }
}
