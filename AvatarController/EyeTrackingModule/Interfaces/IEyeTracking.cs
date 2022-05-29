namespace EyeTrackingModule.Interfaces
{
    internal interface IEyeTracking : IDisposable
    {
        /// <summary>
        /// Initializes EyeTracking
        /// </summary>
        /// <returns></returns>
        bool Initialize();

        /// <summary>
        /// Asynchronous update loop to get next eye data
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task UpdateAsync(CancellationToken token = default);

        /// <summary>
        /// Request recalibration of eye tracking
        /// </summary>
        void Recalibrate();

    }
}
