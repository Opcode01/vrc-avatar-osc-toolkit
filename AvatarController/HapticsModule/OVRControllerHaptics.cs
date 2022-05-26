namespace HapticsModule
{
    using Valve.VR;

    public class OVRControllerHaptics : IControllerHaptics
    {
        private bool _isInitialized = false;

        public OVRControllerHaptics()
        {

        }

        public bool Initialize()
        {
            Console.WriteLine($"{this.GetType().Name} - Initializing...");      //TODO: Better logging

            EVRInitError error = EVRInitError.Unknown;
            try
            {
                OpenVR.Init(ref error, EVRApplicationType.VRApplication_Background);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error with {this.GetType().Name} initialization - \n{e.Message}");      //TODO: Better logging
            }

            _isInitialized = (error == EVRInitError.None);
            Console.WriteLine($"{this.GetType().Name} - Init result: {_isInitialized}, Error: {error}");    //TODO: Better logging
            return _isInitialized;
        }

        public void Dispose()
        {
            OpenVR.Shutdown();
        }
    }
}
