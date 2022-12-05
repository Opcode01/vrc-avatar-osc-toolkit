namespace HapticsModule
{
    using global::HapticsModule.Interfaces;
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
            Console.WriteLine($"{this.GetType().Name} -- Initializing for {controllerType}...");      //TODO: Better logging

            try
            {
                //Initialize controller
                _controllerIndex = controllerType == ControllerType.LEFTHAND ?
                    OpenVR.System.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand) :
                    OpenVR.System.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);
                if (_controllerIndex == OpenVR.k_unTrackedDeviceIndexInvalid)
                    throw new Exception($"No controller found for controller type {controllerType}");

                _isInitialized = true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"{this.GetType().Name} -- Error with initialization - {e.Message}, \n {e.StackTrace}");      //TODO: Better logging
                _isInitialized = false;  
            }

            Console.WriteLine($"{this.GetType().Name} -- Init result: {_isInitialized}");    //TODO: Better logging
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
    }
}
