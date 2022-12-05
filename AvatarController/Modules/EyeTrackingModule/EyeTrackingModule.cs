namespace EyeTrackingModule
{
    using AvatarController.Infrastructure;
    using AvatarController.Infrastructure.Interfaces;
    using global::EyeTrackingModule.Interfaces;
    using System.Threading;
    using System.Threading.Tasks;

    public class EyeTrackingModule : ModuleBase
    {
        private readonly IMessageDistributor _messageDistributor;
        private IEyeTracking? _eyeTracking;
        private Task? _updateTask;

        public EyeTrackingModule(IMessageDistributor messageDistributor)
        {
            _messageDistributor = messageDistributor;
        }

        public override bool Initialize()
        {
            if (_isInitialized)
            {
                Console.WriteLine($"Error - {this.GetType().Name} is already initialized");
                return false;
            }

            Console.WriteLine($"{this.GetType().Name} -- Initializing...");      //TODO: Better logging
            _eyeTracking = new VarjoEyeTracking(_messageDistributor.GetNetworks());     //TODO: Config to allow for EyeTracking solutions other than Varjo
            _isInitialized = _eyeTracking.Initialize();

            Console.WriteLine($"{this.GetType().Name} -- Init result: {_isInitialized}");    //TODO: Better logging
            return _isInitialized;
        }

        public override async Task UpdateAsync(CancellationToken token = default)
        {
            if (_isInitialized)
            {
                _updateTask = _eyeTracking.UpdateAsync(token);

                //Wait until the update finishes before updating again
                await _updateTask;
            }
        }

        public void RecalibrateEyeTracking()
        {
            if(_isInitialized)
                _eyeTracking.Recalibrate();
        }

        public override void Dispose()
        {
            _eyeTracking?.Dispose();
        }
    }

    public enum Eye
    {
        LEFT = 0,
        RIGHT = 1
    }
}