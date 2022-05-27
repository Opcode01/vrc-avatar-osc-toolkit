namespace HapticsModule
{
    using Valve.VR;

    public class OVRControllerHaptics : IControllerHaptics
    {
        private bool _isInitialized = false;
        private uint _controllerIndex = 0;      //Index of the controller this is class is responsible for

        public OVRControllerHaptics()
        {

        }

        public bool Initialize(ControllerType controllerType)
        {
            Console.WriteLine($"{this.GetType().Name} - Initializing...");      //TODO: Better logging

            EVRInitError error = EVRInitError.Unknown;
            try
            {
                //Initialize OVR
                OpenVR.Init(ref error, EVRApplicationType.VRApplication_Background);
                if (error != EVRInitError.None)
                    throw new Exception($"OVR Initialization failed with error: {error}");

                //Initialize controller
                _controllerIndex = controllerType == ControllerType.LEFTHAND ?
                    OpenVR.System.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand) :
                    OpenVR.System.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);
                if (_controllerIndex == OpenVR.k_unTrackedDeviceIndexInvalid)
                    throw new Exception($"No controller found for controller type {controllerType}");

                _isInitialized = (error == EVRInitError.None);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error with {this.GetType().Name} initialization - \n{e.Message}");      //TODO: Better logging
                _isInitialized = false;  
            }

            Console.WriteLine($"{this.GetType().Name} - Init result: {_isInitialized}, OVRError: {error}");    //TODO: Better logging
            return _isInitialized;
        }

        public void TriggerHapticPulse(ushort intensity)
        {
            if (_isInitialized)
            {
                OpenVR.System.TriggerHapticPulse(_controllerIndex, (uint)EVRControllerAxisType.k_eControllerAxis_None, intensity);
            }
            else
            {
                Console.WriteLine($"Error - {this.GetType().Name} has not been initialized!");
            }
        }

        public void Dispose()
        {
            OpenVR.Shutdown();
        }
    }

    public enum ControllerType
    {
        LEFTHAND = 0,
        RIGHTHAND = 1
    }
}
