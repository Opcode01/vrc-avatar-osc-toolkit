namespace AvatarController.Infrastructure.Interfaces
{
    /// <summary>
    /// Receives messages through some mechanism and hands them off to receivers
    /// </summary>
    public interface INetwork
    {
        bool Initialize();

        event EventHandler<MsgReceivedEventArgs> MessageReceived;
    }
}
