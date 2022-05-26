namespace HapticsModule
{
    public interface IControllerHaptics : IDisposable
    {
        bool Initialize(ControllerType controllerType);
        void TriggerHapticPulse();
    }
}
