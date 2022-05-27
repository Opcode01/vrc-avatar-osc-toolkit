/// <summary>
/// Namespace OSC - A wrapper library to provide interoperability across various OSC communication implementations
/// </summary>
namespace OSC
{
    public interface IOSCNetwork
    {
        bool Initialize(ICollection<EventHandler<OSCMsgReceivedEventArgs>>? eventHandlers = null);

        void SendMessage(string address, object value);

        event EventHandler<OSCMsgReceivedEventArgs> MessageReceived;
    }
}
