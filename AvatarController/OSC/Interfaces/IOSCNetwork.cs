/// <summary>
/// Namespace OSC - A wrapper library to provide interoperability across various OSC communication implementations
/// </summary>
namespace OSC
{
    public interface IOSCNetwork
    {
        public void SendMessage(string address, object value);

        public event EventHandler<OSCMsgReceivedEventArgs> MessageReceived;
    }
}
