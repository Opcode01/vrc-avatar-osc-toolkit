namespace EyeTrackingModule
{
    using VarjoNative;
    using AvatarController.Infrastructure;
    using AvatarController.Infrastructure.Interfaces;
    using global::EyeTrackingModule.Interfaces;

    internal class VarjoEyeTracking : IEyeTracking
    {
        private IntPtr _session;
        private ICollection<INetwork> _networks;
        private bool _isInitialized = false;
        private bool _LeftPreviousStatus;
        private bool _RightPreviousStatus;
        private float MAX_LEFTX = 0.0f;
        private float MIN_LEFTX = 0.0f;
        private float MAX_LEFTY = 0.0f;
        private float MIN_LEFTY = 0.0f;
        private float MAX_RIGHTX = 0.0f;
        private float MIN_RIGHTX = 0.0f;
        private float MAX_RIGHTY = 0.0f;
        private float MIN_RIGHTY = 0.0f;

        public VarjoEyeTracking(ICollection<INetwork> networks)
        {
            _networks = networks;
        }

        public bool Initialize()
        {
            //Initialize Varjo
            Console.WriteLine($"{this.GetType().Name} -- Initializing...");
            try
            {
                string varjoVersion = VarjoNative.GetVarjoVersion();
                if (!VarjoNative.varjo_IsAvailable())
                {
                    throw new Exception("Varjo headset not detected!");
                }
                _session = VarjoNative.varjo_SessionInit();
                if (_session == IntPtr.Zero)
                {
                    throw new Exception("Failed to initialize Varjo session");
                }
                if (!VarjoNative.varjo_IsGazeAllowed(_session))
                {
                    throw new Exception("Gaze tracking is not allowed! Please enable it in the Varjo Base!");
                }
            
                VarjoNative.varjo_GazeInit(_session);
                VarjoNative.varjo_SyncProperties(_session);

                _isInitialized = true;
                Console.WriteLine($"{this.GetType().Name} -- Init result: {_isInitialized}, VarjoVersion: {varjoVersion}");    //TODO: Better logging
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{this.GetType().Name} -- Error with initialization - {ex.Message}, \n {ex.StackTrace}");      //TODO: Better logging
                _isInitialized = false; 
            }

            return _isInitialized;
        }

        public async Task UpdateAsync(CancellationToken token = default)
        {
            if (_session == IntPtr.Zero || !_isInitialized)
                return;
            if (token.IsCancellationRequested)
                return;

            var gazeData = VarjoNative.varjo_GetGaze(_session);

            //Do left eye processing
            Task<bool> leftEyeTask = ProcessEyeStatus(gazeData.leftStatus, _LeftPreviousStatus, Eye.LEFT, token);

            //Do right eye processing
            Task<bool> rightEyeTask = ProcessEyeStatus(gazeData.rightStatus, _RightPreviousStatus, Eye.RIGHT, token);

            float rightEyeX = 0.0f;
            float rightEyeY = 0.0f;
            float leftEyeX = 0.0f;
            float leftEyeY = 0.0f;

            bool leftEyeState = await leftEyeTask.ConfigureAwait(false);            
            bool rightEyeState = await rightEyeTask.ConfigureAwait(false);
            _LeftPreviousStatus = leftEyeState;
            _RightPreviousStatus = rightEyeState;   

            if (leftEyeState && rightEyeState)
            {
                //Both eyes good - use combined gaze
                var combinedRay = gazeData.gaze;
                rightEyeX = (float)combinedRay.forward.x;
                rightEyeY = (float)combinedRay.forward.y;
                leftEyeX = (float)combinedRay.forward.x;
                leftEyeY = (float)combinedRay.forward.y;
            }
            else
            {
                //Use independent gaze
                var rightEye = gazeData.rightEye;
                rightEyeX = (float)rightEye.forward.x;
                rightEyeY = (float)rightEye.forward.y;

                var leftEye = gazeData.leftEye;
                leftEyeY = (float)leftEye.forward.y;
                leftEyeX = (float)leftEye.forward.x;
            }

            if (rightEyeX > MAX_RIGHTX)
                MAX_RIGHTX = rightEyeX;
            if (rightEyeX < MIN_RIGHTX)
                MIN_RIGHTX = rightEyeX;
            if (rightEyeY > MAX_RIGHTY)
                MAX_RIGHTY = rightEyeY;
            if (rightEyeY < MIN_RIGHTY)
                MIN_RIGHTY = rightEyeY;

            if (leftEyeX > MAX_LEFTX)
                MAX_LEFTX = leftEyeX;
            if (leftEyeX < MIN_LEFTX)
                MIN_LEFTX = leftEyeX;
            if (leftEyeY > MAX_LEFTY)
                MAX_LEFTY = leftEyeY;
            if (leftEyeY < MIN_LEFTY)
                MIN_LEFTY = leftEyeY;

            //Normalize between -1 and 1
            rightEyeX = Math.Normalize(rightEyeX, MAX_RIGHTX, MIN_RIGHTX);
            rightEyeY = Math.Normalize(rightEyeY, MAX_RIGHTY, MIN_RIGHTY);
            leftEyeX =  Math.Normalize(leftEyeX,  MAX_LEFTX,  MIN_LEFTX);
            leftEyeY =  Math.Normalize(leftEyeY,  MAX_LEFTY,  MIN_LEFTY);

            foreach (var network in _networks)
            {
                if (network != null)
                {
                    network.SendMessage("/avatar/parameters/RightEyeX", rightEyeX);
                    network.SendMessage("/avatar/parameters/RightEyeY", rightEyeY);
                    network.SendMessage("/avatar/parameters/LeftEyeX", leftEyeX);
                    network.SendMessage("/avatar/parameters/LeftEyeY", leftEyeY);
                }
            }
        }

        public void Recalibrate()
        {
            //Recalibrate Varjo
            VarjoNative.varjo_RequestGazeCalibration(_session);
        }

        //TODO: Unit testing framework?
        public async Task TestEyeBlink(CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"{this.GetType().Name} -- Testing Eye Blink...");

            //Setup data to do a blink for each eye
            _LeftPreviousStatus = true;
            _RightPreviousStatus = true;
            GazeData gazeData = new GazeData()
            {
                leftStatus = GazeEyeStatus.Invalid,
                rightStatus = GazeEyeStatus.Invalid
            };

            //Do left eye processing 
            Task<bool> LeftEyeTask = ProcessEyeStatus(gazeData.leftStatus, _LeftPreviousStatus, Eye.LEFT, cancellationToken);

            //Do right eye processing
            Task<bool> RightEyeTask = ProcessEyeStatus(gazeData.rightStatus, _RightPreviousStatus, Eye.RIGHT, cancellationToken);
            
            bool[] results = await Task.WhenAll(LeftEyeTask, RightEyeTask);
            _LeftPreviousStatus = results[0];
            _RightPreviousStatus = results[1];
            Console.WriteLine($"{this.GetType().Name} -- Eye Statuses: {_LeftPreviousStatus},\t{_RightPreviousStatus}");
        }

        public void Dispose()
        {
            VarjoNative.varjo_SessionShutDown(_session);
        }

        private async Task<bool> ProcessEyeStatus(GazeEyeStatus gazeStatus, bool prevStatus, Eye eye, CancellationToken token = default)
        {
            bool shouldClose = false;
            bool gazeStatusGood = false;
            if (gazeStatus != GazeEyeStatus.Invalid)
                gazeStatusGood = true;

            //Is the eye shut or untracked? If so, do blink on that eye
            if (gazeStatusGood != prevStatus)
            {
                if (gazeStatus == GazeEyeStatus.Invalid)
                {
                    shouldClose = true;
                }
                await Task.Run(() => DoBlink(shouldClose, eye, token));
            }

            //Is the eye status good?
            return gazeStatusGood;
        }

        private void DoBlink(bool isClosing, Eye eye, CancellationToken token = default)
        {
            //Blink control
            string address = "";
            if (eye == Eye.LEFT)
            {
                address = "/avatar/parameters/LeftEyeBlink";
            }
            else
            {
                address = "/avatar/parameters/RightEyeBlink";
            }

            if (isClosing)
            {
                //Close the eye
                for (int i = 0; i < 100; i++)
                {
                    if(token.IsCancellationRequested)
                        break;

                    //Create a double value between 0 and 1
                    float value = (i / 100f);
                    foreach (var network in _networks)
                    {
                        network?.SendMessage(address, value);
                    }
                    //Wait before sending next message
                    //Thread.Sleep(1);
                }
            }
            else
            {
                //Open the eye
                for (int i = 100; i > 0; i--)
                {
                    if (token.IsCancellationRequested)
                        break;

                    //Create a double value between 0 and 1
                    float value = (i / 100f);
                    foreach (var network in _networks)
                    {
                        network?.SendMessage(address, value);
                    }
                    //Wait before sending next message
                    //Thread.Sleep(1);
                }
            }
        }
    }
}
