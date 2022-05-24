namespace HapticsModule
{
    using System;
    using System.Text;
    using Silk.NET.Core.Native;
    using Silk.NET.OpenXR;

    public class ControllerHaptics : IControllerHaptics
    {
        private XR _xrApi;
        private Instance _xrInstance;
        private Session _session;
        private bool _isInitialized = false;

        public ControllerHaptics()
        {
            //Get XRApi
            _xrApi = XR.GetApi();
        }

        public unsafe bool Initialize()
        {
            if (_xrApi != null)
            {
                ApplicationInfo appInfo = new ApplicationInfo() { ApplicationVersion = 1, ApiVersion = XR_MAKE_VERSION(1, 0, 22) };
                Encoding.UTF8.GetBytes("AppName", new Span<byte>(appInfo.ApplicationName, 128));

                InstanceCreateInfo instanceInfo = new InstanceCreateInfo()
                {
                    Type = StructureType.TypeInstanceCreateInfo,
                    ApplicationInfo = appInfo
                };

                var result = _xrApi.CreateInstance(instanceInfo, ref _xrInstance);
                if (result != Result.Success)
                {
                    Console.WriteLine($"XrInstance failed to initialize! Result: {result}");
                    return _isInitialized;
                }

                //Initialize XR session
                SessionCreateInfo sessionInfo = new SessionCreateInfo();
                if (_xrApi.CreateSession(_xrInstance, sessionInfo, ref _session) != Result.Success)
                {
                    Console.WriteLine("XrSession failed to initialize!");
                    return _isInitialized;
                }

                //Create an action set
                ActionSetCreateInfo actionSetInfo = new ActionSetCreateInfo();

                //Create an output haptic action

                //Create XrPath to the controller haptic device

                //Create an XrPath to the vendor interaction profile

                //Bind the haptic action to the haptic path

                //Bind the bindings to the interaction profile

                //Attach the action set to the session

                _isInitialized = true;
            }

            return _isInitialized;
        }

        public void VibrateController(float intensity)
        {
            if (_isInitialized)
            {
                //Sync action data
                //(As far as I can tell, this tells the OpenXR runtime what actions we want to listen for. This is allowed to change at any point during runtime)

                //Fire haptics using output action
                var hapticVibration = new HapticVibration
                {
                    Type = StructureType.TypeHapticVibration,
                    Amplitude = intensity,
                    Duration = 300,
                    Frequency = 3000
                };

                var hapticActionInfo = new HapticActionInfo();

                //_xrApi.ApplyHapticFeedback(_session, hapticActionInfo, hapticVibration);
            }
            else
            {
                Console.WriteLine("Error - ControllerHaptics is not initialized!");
            }
        }

        private ulong XR_MAKE_VERSION(ushort major, ushort minor, ushort patch)
        {
            return (((major) & 0xffffUL) << 48) | (((minor) & 0xffffUL) << 32) | ((patch) & 0xffffffffUL);
        }
    }
}