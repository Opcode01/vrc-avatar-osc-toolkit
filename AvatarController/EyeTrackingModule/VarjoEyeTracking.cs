namespace EyeTrackingModule
{
    using VarjoNative;
    using AvatarController.Infrastructure;
    using AvatarController.Infrastructure.Interfaces;
    using global::EyeTrackingModule.Interfaces;
    using System.Net;

    internal class VarjoEyeTracking : IEyeTracking
    {
        private IntPtr _session;
        private ICollection<INetwork> _networks;
        private bool _isInitialized = false;
        private bool _LeftPreviousStatus;
        private bool _RightPreviousStatus;

        // 999 for min and -999 for max, to ensure these Values get overwritten the first runthrough
        private float _min_left_x = 999.0f, _min_left_y = 999.0f, _min_right_x = 999.0f, _min_right_y = 999.0f;
        private float _max_left_x = -999.0f, _max_left_y = -999.0f, _max_right_x = -999.0f, _max_right_y = -999.0f;
        private float _min_pupil_size = 999.0f, _max_pupil_size = -999.0f;

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

            GazeData gazeData;
            EyeMeasurements eyeMeasurements;
            bool success = VarjoNative.varjo_GetGazeData(_session, out gazeData, out eyeMeasurements);

            //Skip this update if the gaze data isn't good
            if (!success || gazeData.status != GazeStatus.Valid)
                return;

            float rightEyeX = 0.0f;
            float rightEyeY = 0.0f;
            float leftEyeX = 0.0f;
            float leftEyeY = 0.0f;
            float eyesY = 0.0f;
            float pupilSize = 0.0f;
            float eyeDilation = 0.0f;

            bool leftEyeState = (gazeData.leftStatus == GazeEyeStatus.Invalid);
            bool rightEyeState = (gazeData.rightStatus == GazeEyeStatus.Invalid);

            if (leftEyeState && rightEyeState)
            {
                //Both eyes good - use combined gaze
                var combinedRay = gazeData.gaze;
                rightEyeX = (float)combinedRay.forward.x;
                rightEyeY = (float)combinedRay.forward.y;
                leftEyeX = (float)combinedRay.forward.x;
                leftEyeY = (float)combinedRay.forward.y;
                eyesY = (float)combinedRay.forward.y;

                //Process pupil size
                pupilSize = (eyeMeasurements.leftPupilDiameterInMM + eyeMeasurements.rightPupilDiameterInMM) / 2;
            }
            else
            {
                //Use independent gaze
                if (rightEyeState)
                {
                    var rightEye = gazeData.rightEye;
                    rightEyeX = (float)rightEye.forward.x;
                    rightEyeY = (float)rightEye.forward.y;
                    pupilSize = eyeMeasurements.rightPupilDiameterInMM;
                }
                if (leftEyeState)
                {
                    var leftEye = gazeData.leftEye;
                    leftEyeY = (float)leftEye.forward.y;
                    leftEyeX = (float)leftEye.forward.x;
                    pupilSize = eyeMeasurements.leftPupilDiameterInMM;
                }
            }

            //Set Min/Maxs
            if (rightEyeX > _max_right_x)
                _max_right_x = rightEyeX;
            if (rightEyeX < _min_right_x)
                _min_right_x = rightEyeX;
            if (rightEyeY > _max_right_y)
                _max_right_y = rightEyeY;
            if (rightEyeY < _min_right_y)
                _min_right_y = rightEyeY;

            if (leftEyeX > _max_left_x)
                _max_left_x = leftEyeX;
            if (leftEyeX < _min_left_x)
                _min_left_x = leftEyeX;
            if (leftEyeY > _max_left_y)
                _max_left_y = leftEyeY;
            if (leftEyeY < _min_left_y)
                _min_left_y = leftEyeY;

            if(pupilSize > _max_pupil_size)
                _max_pupil_size = pupilSize;
            if (pupilSize < _min_pupil_size)
                _min_pupil_size = pupilSize;

            //Normalize between -1 and 1
            rightEyeX = Math.Normalize(rightEyeX, _max_right_x, _min_right_x);
            rightEyeY = Math.Normalize(rightEyeY, _max_right_y, _min_right_y);
            leftEyeX =  Math.Normalize(leftEyeX,  _max_left_x,  _min_left_x);
            leftEyeY =  Math.Normalize(leftEyeY,  _max_left_y,  _min_left_y);
            eyeDilation = Math.Normalize(pupilSize, _max_pupil_size, _min_pupil_size, 1.0f, 0.0f);

            foreach (var network in _networks)
            {
                if (network != null)
                {
                    //For use if the avatar uses the EyesY parameter instead of independent eye tracking
                    //https://github.com/benaclejames/VRCFaceTracking/wiki/Eye-Tracking-Setup
                    network.SendMessage("/avatar/parameters/EyesY", eyesY);

                    //Tracking eyes independently
                    network.SendMessage("/avatar/parameters/RightEyeY", rightEyeY);
                    network.SendMessage("/avatar/parameters/LeftEyeY", leftEyeY);                    
                    network.SendMessage("/avatar/parameters/RightEyeX", rightEyeX);
                    network.SendMessage("/avatar/parameters/LeftEyeX", leftEyeX);

                    //Tracking pupil diameter and size
                    network.SendMessage("/avatar/parameters/EyesPupilDiameter", (pupilSize > 10.0f || pupilSize == 0.0f) ? 1 : pupilSize / 10.0f); //NOTE: PupilDiameter for VRCFT is in CM not MM
                    network.SendMessage("/avatar/parameters/EyesDilation", eyeDilation);
                    
                    //Tracking eye lids
                    network.SendMessage("/avatar/parameters/LeftEyeLidExpanded", eyeMeasurements.leftEyeOpenness);
                    network.SendMessage("/avatar/parameters/RightEyeLidExpanded", eyeMeasurements.rightEyeOpenness);
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

        [Obsolete]
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

        [Obsolete]
        private void DoBlink(bool isClosing, Eye eye, CancellationToken token = default)
        {
            //Blink control
            string address = "";
            if (eye == Eye.LEFT)
            {
                address = "/avatar/parameters/LeftEyeLidExpanded";
            }
            else
            {
                address = "/avatar/parameters/RightEyeLidExpanded";
            }

            if (isClosing)
            {
                //Close the eye
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
            else
            {
                //Open the eye
                for (int i = 0; i < 100; i++)
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
