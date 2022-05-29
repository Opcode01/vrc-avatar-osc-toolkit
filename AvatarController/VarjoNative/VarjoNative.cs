using System;
using System.Runtime.InteropServices;

namespace VarjoNative
{
    #region StructDefinitions
    //Varjo's structs used with both native library and companion
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector
    {

        public double x;
        public double y;
        public double z;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GazeRay
    {
        public Vector origin;   //!< Origin of the ray.
        public Vector forward;  //!< Direction of the ray.
    }

    public enum GazeStatus : long
    {
        Invalid = 0,
        Adjust = 1,
        Valid = 2
    }

    public enum GazeEyeStatus : long
    {
        Invalid = 0,
        Visible = 1,
        Compensated = 2,
        Tracked = 3
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct GazeData
    {
        public GazeRay leftEye;                 //!< Left eye gaze ray.
        public GazeRay rightEye;                //!< Right eye gaze ray.
        public GazeRay gaze;                    //!< Normalized gaze direction ray.
        public double focusDistance;            //!< Estimated gaze direction focus point distance.
        public double stability;                //!< Focus point stability.
        public long captureTime;                //!< Varjo time when this data was captured, see varjo_GetCurrentTime()
        public GazeEyeStatus leftStatus;        //!< Status of left eye data.
        public GazeEyeStatus rightStatus;       //!< Status of right eye data.
        public GazeStatus status;               //!< Tracking main status.
        public long frameNumber;                //!< Frame number, increases monotonically.
        public double leftPupilSize;            //!< Normalized [0..1] left eye pupil size.
        public double rightPupilSize;           //!< Normalized [0..1] right eye pupil size.
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct GazeCalibrationParameter
    {
        [MarshalAs(UnmanagedType.LPStr)] public string key;
        [MarshalAs(UnmanagedType.LPStr)] public string value;
    }

    public enum GazeCalibrationMode
    {
        Legacy,
        Fast
    };

    public enum GazeOutputFilterType
    {
        None,
        Standard
    }

    public enum GazeOutputFrequency
    {
        MaximumSupported,
        Frequency100Hz,
        Frequency200Hz
    }

    public enum GazeEyeCalibrationQuality
    {
        Invalid = 0,
        Low = 1,
        Medium = 2,
        High = 3
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GazeCalibrationQuality
    {
        public GazeEyeCalibrationQuality left;
        public GazeEyeCalibrationQuality right;
    }
    #endregion

    public static class VarjoNative
    {        
        #region PublicMethods
        public static string GetVarjoVersion()
        {
            return GetString(varjo_GetVersionString());
        }

        #endregion
        #region DLLImports
        private static string GetString(IntPtr ptr) => new string(Marshal.PtrToStringAnsi(ptr));

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]        
        private extern static IntPtr varjo_GetVersionString();

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        public static extern bool varjo_IsAvailable();

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        public static extern IntPtr varjo_SessionInit();

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        public static extern void varjo_SessionShutDown(IntPtr session);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        public static extern void varjo_GazeInit(IntPtr session);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        public static extern int varjo_GetError(IntPtr session);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        public static extern string varjo_GetErrorDesc(int errorCode);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        public static extern bool varjo_IsGazeAllowed(IntPtr session);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        public static extern GazeData varjo_GetGaze(IntPtr session);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        public static extern void varjo_RequestGazeCalibration(IntPtr session);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        public static extern bool varjo_GetPropertyBool(IntPtr session, int propertyKey);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        public static extern int varjo_GetPropertyInt(IntPtr session, int propertyKey);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        public static extern void varjo_SyncProperties(IntPtr session);
        #endregion
    }
}