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
        private ActionSet _actionSet;
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
                //Get number of extensions
                UInt32 extensionCount = 0;
                _xrApi.EnumerateInstanceExtensionProperties("", 0, &extensionCount, null);

                //Get extensions
                ExtensionProperties[] extensionProperties = new ExtensionProperties[extensionCount];
                for (int i = 0; i < extensionCount; i++)
                {
                    extensionProperties[i] = new ExtensionProperties() { Type = StructureType.TypeExtensionProperties };
                }
                var result = _xrApi.EnumerateInstanceExtensionProperties("", extensionCount, &extensionCount, extensionProperties);
                if(result == Result.Success && extensionProperties != null)
                {
                    foreach (var extensionProperty in extensionProperties)
                    {
                        string extensionname = Encoding.UTF8.GetString(extensionProperty.ExtensionName, 128);
                        Console.WriteLine($"Found Extension - {extensionname} : {extensionProperty.ExtensionVersion}");
                    }
                }
                else
                {
                    Console.WriteLine($"Could not retrieve extension properties! Result: {result}");
                    return _isInitialized;
                }

                //Create AppInfo
                ApplicationInfo appInfo = new ApplicationInfo() { ApplicationVersion = 1, ApiVersion = XR_MAKE_VERSION(1, 0, 22) }; //TODO: We should actually get the version from the openxr_loader.dll if possible
                Encoding.UTF8.GetBytes("AppName", new Span<byte>(appInfo.ApplicationName, 128));

                //Initialize XR instance
                InstanceCreateInfo instanceInfo = new InstanceCreateInfo()
                {
                    Type = StructureType.TypeInstanceCreateInfo,
                    ApplicationInfo = appInfo
                };
                result = _xrApi.CreateInstance(instanceInfo, ref _xrInstance);
                if (result != Result.Success)
                {
                    Console.WriteLine($"XrInstance failed to initialize! Result: {result}");
                    return _isInitialized;
                }

                //Get XR System
                ulong systemId = 0;
                SystemGetInfo systemInfo = new SystemGetInfo() { Type = StructureType.TypeSystemGetInfo, FormFactor = FormFactor.HeadMountedDisplay };
                result = _xrApi.GetSystem(_xrInstance, systemInfo, ref systemId);
                if(result != Result.Success)
                {
                    Console.WriteLine($"Failed to get XR system! Result: {result}");
                    return _isInitialized;
                }

                //Initialize XR session
                SessionCreateInfo sessionInfo = new SessionCreateInfo() { Type = StructureType.TypeSessionCreateInfo, SystemId = systemId };      
                result = _xrApi.CreateSession(_xrInstance, sessionInfo, ref _session);
                if (result != Result.Success)
                {
                    Console.WriteLine($"XrSession failed to initialize! Result: {result}");
                    return _isInitialized;
                }

                //Create an action set
                ActionSetCreateInfo actionSetInfo = new ActionSetCreateInfo() { Type = StructureType.TypeActionSetCreateInfo, Priority = 0 };
                Encoding.UTF8.GetBytes("ControllerHapticsActionSet", new Span<byte>(actionSetInfo.ActionSetName, 128));
                result = _xrApi.CreateActionSet(_xrInstance, actionSetInfo, ref _actionSet);
                if(result != Result.Success)
                {
                    Console.WriteLine($"Could not create action set. Result: {result}");
                    return _isInitialized;
                }

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