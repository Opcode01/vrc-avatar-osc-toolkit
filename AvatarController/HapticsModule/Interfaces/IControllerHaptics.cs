namespace HapticsModule.Interfaces
{
    public interface IControllerHaptics
    {
        bool Initialize(ControllerType controllerType);
        void TriggerHapticPulse(ushort intensity);
    }
}
