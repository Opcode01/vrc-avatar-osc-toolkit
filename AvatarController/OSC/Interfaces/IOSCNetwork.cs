/// <summary>
/// Namespace OSC - A wrapper library to provide interoperability across various OSC communication implementations
/// </summary>
namespace OSC
{
    internal interface IOSCNetwork
    {
        public void SendMessage(string address, float value);
    }
}
