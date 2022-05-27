namespace AvatarController.Infrastructure.Interfaces
{
    /// <summary>
    /// The interface that all modules should implement
    /// </summary>
    public interface IModule : IDisposable
    {
        /// <summary>
        /// Sets up all the dependencies for the module
        /// </summary>
        /// <returns></returns>
        public bool Initialize();
    }
}
