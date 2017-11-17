// VR Simulator|Prefabs|0005

// ReSharper disable once RedundantUsingDirective
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_PS4
using UnityEngine.PS4;
using UnityEngine.PS4.VR;
#endif
using UnityEngine.VR;

namespace VRTK
{
    //    [SerializeField]
    //    public class PSVR_KeyMapper
    //    {
    //        public PlayStationKeys PSKey;
    //#if UNITY_EDITOR
    //        public KeyCode SimCode;
    //#endif
    //    }
    /// <remarks>
    /// The Input tracking extension allows the rig to register the PlayStation Controllers so that they can be tracked
    /// </remarks>
    public partial class SDK_PlayStationVRInput
    {
        [Header("PlayStation Input Adjustments")]
        [Range(0, 100)]
        public int VibrationAmount = 100;

        public PlayStationKeys Trigger = PlayStationKeys.Trigger;
        public PlayStationKeys GripButton = PlayStationKeys.Cross;
        public PlayStationKeys ButtonOne = PlayStationKeys.Square;
        public PlayStationKeys ButtonTwo = PlayStationKeys.Triangle;
        public PlayStationKeys ButtonStart = PlayStationKeys.Start;
        public PlayStationKeys TouchPad = PlayStationKeys.Middle;

        private bool trackingStarted;
        private static bool loaded;
        private static bool centered;
        private Vector3 startHeadTrackingLocation;
        private Transform headsetRoot;

        private void InitDeviceTracking()
        {
#if UNITY_PS4
            var controllerSDK = (SDK_PlayStationVRController)VRTK_SDK_Bridge.GetControllerSDK();
            controllerSDK.SetvibrationStrength(VibrationAmount);

            var mapper = new Dictionary<SDK_BaseController.ButtonTypes, PlayStationKeys>()
            {
                {SDK_BaseController.ButtonTypes.Trigger, Trigger},
                {SDK_BaseController.ButtonTypes.Grip, GripButton},
                {SDK_BaseController.ButtonTypes.ButtonOne, ButtonOne},
                {SDK_BaseController.ButtonTypes.ButtonTwo, ButtonTwo},
                {SDK_BaseController.ButtonTypes.StartMenu, ButtonStart},
                {SDK_BaseController.ButtonTypes.Touchpad, TouchPad},
            };
            controllerSDK.InitKeyMapper(mapper);

#endif
            headsetRoot = myCamera.parent;
            startHeadTrackingLocation = headsetRoot.localPosition;


            rightHand.gameObject.SetActive(false);
            leftHand.gameObject.SetActive(false);

            if (!trackingStarted)
            {
                StartCoroutine(DeviceTracking());
            }
        }

        private IEnumerator DeviceTracking()
        {

            // Keep waiting until we have a VR Device available
            while (!UnityEngine.XR.XRDevice.isPresent)
            {
                yield return new WaitForSeconds(1.0f);
            }



            if (loaded == false)
            {
                if (UnityEngine.XR.XRSettings.loadedDeviceName != "PlayStationVR")
                {
                    UnityEngine.XR.XRSettings.LoadDeviceByName("PlayStationVR");
                }
                yield return null;
                UnityEngine.XR.XRSettings.enabled = true;
                UnityEngine.XR.XRSettings.eyeTextureResolutionScale = 1.4f;
                loaded = true;
            }
#if UNITY_PS4 && !UNITY_EDITOR
            hintCanvas =  Instantiate(Resources.Load("Game Setup Hints", typeof(GameObject))) as GameObject;
            Canvas canvas = hintCanvas.GetComponentInChildren<Canvas>();
            canvas.worldCamera = Camera.main;

            canvas.planeDistance = 0.1f;


            Text controllerText = hintCanvas.GetComponentInChildren<Text>();
            controllerText.enabled = false;
            while (!ControllersConnected())
            {

                controllerText.enabled = true;
                canvas.planeDistance = 0.5f;

                controllerText.text = String.Format("Please Connect Both Controllers\n<b>Primary Controller</b>: <i>{0}</i>\n<b>Secondary Controller</b>: <i>{1}</i>",
                                                    PS4Input.MoveIsConnected(0, 0) ? "Connected":"Not Connected",
                                                    PS4Input.MoveIsConnected(0, 1) ? "Connected" : "Not Connected");

                yield return new WaitForEndOfFrame();
            }
             canvas.planeDistance = 0.1f;
            yield return new WaitUntil(ControllersConnected);
#endif

            Recenter();

#if UNITY_PS4
            // Register the callbacks needed to detect resetting the HMD
            Utility.onSystemServiceEvent += OnSystemServiceEvent;
#endif
            ResetDeviceTracking();
        }

        private bool ControllersConnected()
        {
#if UNITY_PS4
            return (PS4Input.MoveIsConnected(0, 0) && PS4Input.MoveIsConnected(0, 1));
#endif
            return false;
        }

        private void OnDisable()
        {
#if UNITY_PS4
            Utility.onSystemServiceEvent -= OnSystemServiceEvent;
#endif
            if (trackingStarted)
            {
                UnregisterDevices();
            }
        }

        private void OnEnable()
        {
#if UNITY_PS4
            if (VRSettings.enabled && trackingStarted)
            {
                ResetDeviceTracking();
            }
#endif
        }

        private void OnDestroy()
        {
#if UNITY_PS4
            Utility.onSystemServiceEvent -= OnSystemServiceEvent;
#endif
            UnregisterDevices();
        }

        // Unregister and re-register the controllers to reset them
        private void ResetDeviceTracking()
        {

            UnregisterDevices();
            RegisterDevices();
            Recenter();
        }



        private void RegisterDevices()
        {
            StartCoroutine(RegisterMoveControllers());
        }


        // Register Move device(s) to track
        private IEnumerator RegisterMoveControllers()
        {
            rightHand.gameObject.SetActive(false);
            leftHand.gameObject.SetActive(false);
#if UNITY_PS4
            //     yield return new WaitForSeconds(1);

            Debug.Log("register Controllers");
            int i = 0;
            string controllerName = "NOT SET";
            while (i < 15 && (!PS4Input.MoveIsConnected(0, 1) || !PS4Input.MoveIsConnected(0, 0)))
            {
                if (PS4Input.MoveIsConnected(0, 0) == false)
                {
                    controllerName = "[primary] ";
                }

                if (PS4Input.MoveIsConnected(0, 1) == false)
                {
                    controllerName = "[secondary] ";
                }
                Debug.LogWarning(string.Format(
                                     "Trying to register the {0}Move device, but it is not connected! \n waited: {1}s",
                                     controllerName, i));
                i++;
                yield return new WaitForSeconds(1);
            }
            if (i >= 15 && (!PS4Input.MoveIsConnected(0, 0) || !PS4Input.MoveIsConnected(0, 1)))
            {
                Debug.LogError(string.Format(
                                   "Trying to register the {0} Move device, but it is not connected!  Timed out at 15s!",
                                   controllerName));
                yield break;
            }
            //      yield return new WaitForSeconds(1);
            int[] primaryHandles = new int[1];
            int[] secondaryHandles = new int[1];
            PS4Input.MoveGetUsersMoveHandles(1, primaryHandles, secondaryHandles);

            rightController.Device.handle = primaryHandles[0];
            leftController.Device.handle = secondaryHandles[0];
            PlayStationVRTrackingColor trackedColor;

            // Get the tracking for the primary Move device, and wait for it to start
            PlayStationVRResult resultRight = Tracker.RegisterTrackedDevice(PlayStationVRTrackedDevice.DeviceMove, rightController.Device.handle,
                                                                       PlayStationVRTrackingType.Absolute,
                                                                       PlayStationVRTrackerUsage.OptimizedForHmdUser);

            if (resultRight == PlayStationVRResult.Ok)
            {
                PlayStationVRTrackingStatus trackingStatusPrimary = new PlayStationVRTrackingStatus();

                while (trackingStatusPrimary == PlayStationVRTrackingStatus.NotStarted)
                {
                    Tracker.GetTrackedDeviceStatus(rightController.Device.handle, out trackingStatusPrimary);
                    yield return null;
                }
                yield return new WaitUntil(
                    () => Tracker.GetTrackedDeviceLedColor(rightController.Device.handle, out trackedColor)
                          == PlayStationVRResult.Ok);
                Tracker.GetTrackedDeviceLedColor(rightController.Device.handle, out trackedColor);
                rightController.Device.SetLightColor(trackedColor);
                Debug.Log("Registered Primary Device");

            }
            else
            {
                Debug.LogError(
                    "Tracking failed for primary Move Controller! This may be because you're trying to register too many devices at once.");
            }
            yield return new WaitForSeconds(.2f);

            PlayStationVRResult resultLeft = Tracker.RegisterTrackedDevice(PlayStationVRTrackedDevice.DeviceMove, leftController.Device.handle,
                                                   PlayStationVRTrackingType.Absolute,
                                                   PlayStationVRTrackerUsage.OptimizedForHmdUser);

            if (resultLeft == PlayStationVRResult.Ok)
            {
                PlayStationVRTrackingStatus trackingStatusSecondary = new PlayStationVRTrackingStatus();

                while (trackingStatusSecondary == PlayStationVRTrackingStatus.NotStarted)
                {
                    Tracker.GetTrackedDeviceStatus(leftController.Device.handle, out trackingStatusSecondary);
                    yield return null;
                }
                yield return new WaitUntil(() => Tracker.GetTrackedDeviceLedColor(leftController.Device.handle, out trackedColor) == PlayStationVRResult.Ok);
                Tracker.GetTrackedDeviceLedColor(leftController.Device.handle, out trackedColor);
                leftController.Device.SetLightColor(trackedColor);
                Debug.Log("Registered Secondary Device");

            }
            else
            {
                Debug.LogError(
                    "Tracking failed for DeviceMove! This may be because you're trying to register too many devices at once.");
            }

            while (leftController.Device.handle < 0 || rightController.Device.handle < 0)
            {
                yield return null;
            }
            rightHand.gameObject.SetActive(true);
            leftHand.gameObject.SetActive(true);
            while (centered == false && hintCanvas != null)
            {

                Text controllerText = hintCanvas.GetComponentInChildren<Text>(true);
                controllerText.enabled = true;
                Canvas canvas = hintCanvas.GetComponentInChildren<Canvas>(true);
                canvas.planeDistance = 0.5f;
                controllerText.text = "         Please <b>press and hold START</b> to recenter your VR headset before continuing game play.";
                yield return new WaitForSeconds(1);

            }
            if (hintCanvas != null)
            {
                Destroy(hintCanvas);
            }


            trackingStarted = true;
#else
        yield return null;
#endif
        }


        // Remove the registered devices from tracking and reset the transform
        private void UnregisterDevices()
        {
            // We can only unregister tracked devices while in VR, or else a crash may occur
            if (UnityEngine.XR.XRSettings.enabled)
            {
                UnregisterMoveControllers();

            }
        }


        // Remove the registered Move devices from tracking and reset the transform
        private void UnregisterMoveControllers()
        {
#if UNITY_PS4
            if (rightController && rightController.Device != null && rightController.Device.handle >= 0)
            {
                Tracker.UnregisterTrackedDevice(rightController.Device.handle);
                rightController.OnUnregisterMoveController();
                Debug.Log("Unregister right");
            }

            if (leftController && leftController.Device != null && leftController.Device.handle >= 0)
            {
                Tracker.UnregisterTrackedDevice(leftController.Device.handle);

                leftController.OnUnregisterMoveController();
                Debug.Log("Unregister left");
            }
#endif
        }

        private void Recenter()
        {
            rightHand.localPosition = Vector3.zero;
            leftHand.localPosition = Vector3.zero;
            Vector3 recenterLocation = startHeadTrackingLocation - myCamera.localPosition;
            headsetRoot.localPosition = recenterLocation;
            trackedDevices.position = headsetRoot.position;
        }

#if UNITY_PS4


        // HMD recenter happens in this event, which we will also use for tracked devices reset
        private void OnSystemServiceEvent(Utility.sceSystemServiceEventType eventType)
        {
            if (eventType == Utility.sceSystemServiceEventType.RESET_VR_POSITION)
            {
                centered = true;
                ResetDeviceTracking();

            }
        }

#endif
    }
}