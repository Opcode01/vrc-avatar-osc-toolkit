namespace HapticsModule.Interfaces
{
    internal interface IControllerHaptics
    {
        bool Initialize(ControllerType controllerType);
        void TriggerHapticPulse(ushort intensity);
    }
}
