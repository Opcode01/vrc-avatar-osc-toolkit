namespace AvatarController.Infrastructure
{
    using AvatarController.Infrastructure.Interfaces;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class ModuleBase : IModule
    {
        protected bool _isInitialized = false;

        public virtual bool Initialize()
        {
            return _isInitialized;
        }
        public virtual void Update()
        {
            //Do nothing
        }
        public virtual Task UpdateAsync(CancellationToken token = default)
        {
            return Task.CompletedTask;
        }
        public virtual void Dispose()
        {
            _isInitialized = false;
        }
    }
}
