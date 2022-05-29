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

        /// <summary>
        /// Asynchronous Update from the main thread loop
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task UpdateAsync(CancellationToken token = default);

        /// <summary>
        /// Synchronous Update from the main thread loop
        /// </summary>
        void Update();
    }
}
