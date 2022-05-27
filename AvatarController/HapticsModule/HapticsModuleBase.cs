namespace HapticsModule
{
    using AvatarController.Infrastructure;
    using AvatarController.Infrastructure.Interfaces;
    using Valve.VR;

    /// <summary>
    /// External Dependencies - 
    ///     Valve.VR (openvr_api.dll)    
    /// </summary>
    public class HapticsModuleBase : IModule
    {
        private bool _isInitialized = false;
        private readonly IControllerHaptics _leftControllerHaptics;
        private readonly IControllerHaptics _rightControllerHaptics;
        private readonly IMessageDistributor _messageDistributor;

        public HapticsModuleBase(IMessageDistributor messageDistributor)
        {
            _messageDistributor = messageDistributor;

            //Using OVR controller haptics for now - eventually we want to construct these with a factory that can 
            //choose the correct IControllerHaptics implementation based on some options that are set up at runtime
            _leftControllerHaptics = new OVRControllerHaptics();
            _rightControllerHaptics = new OVRControllerHaptics(); 
        }

        //Load and initialize dependencies
        public bool Initialize()
        {
            if (_isInitialized)
            {
                Console.WriteLine($"Error - {this.GetType().Name} is already initialized");
                return false;
            }

            Console.WriteLine($"{this.GetType().Name} - Initializing...");      //TODO: Better logging
            EVRInitError error = EVRInitError.Unknown;
            try
            {
                //Initialize OVR
                OpenVR.Init(ref error, EVRApplicationType.VRApplication_Background);
                if (error != EVRInitError.None)
                    throw new Exception($"OVR Initialization failed with error: {error}");

                _isInitialized = (error == EVRInitError.None);

                //Initialize left and right hand controllers
                _isInitialized &= _leftControllerHaptics.Initialize(ControllerType.LEFTHAND);
                _isInitialized &= _rightControllerHaptics.Initialize(ControllerType.RIGHTHAND);

                //TODO: Initialize other haptics devices

                //Add bindings to MessageDistributor
                _messageDistributor.AddBinding("/avatar/parameters/RightHandTouch", (object? sender, MsgReceivedEventArgs eventArgs) => TriggerHaptics(ControllerType.RIGHTHAND, (float)eventArgs.Contents[0]));
                _messageDistributor.AddBinding("/avatar/parameters/LeftHandTouch", (object? sender, MsgReceivedEventArgs eventArgs) => TriggerHaptics(ControllerType.LEFTHAND, (float)eventArgs.Contents[0]));

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error with {this.GetType().Name} initialization - {ex.Message}, \n{ex.StackTrace}");      //TODO: Better logging
                _isInitialized = false;
            }

            Console.WriteLine($"{this.GetType().Name} - Init result: {_isInitialized}, OVRError: {error}");    //TODO: Better logging
            return _isInitialized;
        }

        private void TriggerHaptics(ControllerType controllerType, float proximity)
        {
            ushort intensity = (ushort)Math.Normalize(proximity, 0, 1, 100, 1000);
            switch (controllerType)
            {
                case ControllerType.RIGHTHAND:
                    _rightControllerHaptics.TriggerHapticPulse(intensity);
                    break;
                case ControllerType.LEFTHAND:
                    _leftControllerHaptics.TriggerHapticPulse(intensity);
                    break;
                default:
                    //Do nothing
                    break;
            }
            Console.WriteLine($"Triggered {controllerType} \t Proximity value: {proximity} \t Normalized Intensity: {intensity}");
        }

        public void Dispose()
        {
            OpenVR.Shutdown();
            _isInitialized = false;
        }
    }

    public enum ControllerType
    {
        LEFTHAND = 0,
        RIGHTHAND = 1
    }
}
