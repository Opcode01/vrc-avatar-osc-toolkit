/// <summary>
/// Namespace OSC - A wrapper library to provide interoperability across various OSC communication implementations
/// </summary>
namespace OSCModule
{
    using AvatarController.Infrastructure.Interfaces;

    public interface IOSCNetwork : INetwork, IDisposable
    {
        void SendMessage(string address, object value);
    }
}
