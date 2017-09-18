using UnityEngine;

using UnityEngine.VR;
#if UNITY_PS4
using UnityEngine.PS4.VR;
using UnityEngine.PS4;
#endif

namespace VRTK
{
    /// <remarks>
    /// The PlayStation Frustum Tracking extension allows the frustum to use the Playstation Camera to accurately depict the players play-space
    /// </remarks>
    public partial class SDK_PlayStationFrustum
    {
#if UNITY_PS4
        private PlayStationVRPlayAreaWarningInfo info;
        private PlayStationVRTrackingStatus status;
#endif

        public void UpdateFrustumTracking()
        {
#if UNITY_PS4
            if (!VRSettings.enabled)
            {
                return;
            }
            PlayStationVR.GetPlayAreaWarningInfo(out info);

            // Show/hide the frustum if the HMD is too close to the edge of the play space
            if (info.distanceFromHorizontalBoundary < safeDistance || info.distanceFromVerticalBoundary < safeDistance)
            {
                if (ShowFrustum == false)
                {
                    UpdateFrustumTransform();
                    ShowFrustum = true;
                }
            }
            else if (ShowFrustum)
            {
                ShowFrustum = false;
            }

            UpdateFrustumDisplay();
#endif
        }

        public void Register()
        {

            foreach (Renderer fR in frustumRenderers)
            {
                fR.material.color = hideColor;
            }
        }


        




        private void UpdateFrustumTransform()
        {
#if UNITY_PS4
            int hmdHandle = PlayStationVR.GetHmdHandle();

            Tracker.GetTrackedDevicePosition(hmdHandle, PlayStationVRSpace.Raw, out hmdPositionRaw);

            // Convert from RAW device space into Unity Left handed space.
            hmdPositionRaw.z = -hmdPositionRaw.z;
            Tracker.GetTrackedDeviceOrientation(hmdHandle, PlayStationVRSpace.Unity, out hmdRotationUnity);
            Tracker.GetTrackedDeviceOrientation(hmdHandle, PlayStationVRSpace.Raw, out hmdRotationRaw);

            Quaternion hmdRotationRawInUnity = hmdRotationRaw;
            hmdRotationRawInUnity.z = -hmdRotationRawInUnity.z;
            hmdRotationRawInUnity.w = -hmdRotationRawInUnity.w;
            Quaternion q =Quaternion.Euler( (Quaternion.Inverse(hmdRotationRawInUnity * Quaternion.Inverse(hmdRotationUnity))).eulerAngles
                                            + transform.root.eulerAngles);

            frustumTransform.position =  (Camera.main.transform.position + q * -hmdPositionRaw) ;

            PlayStationVR.GetCameraAccelerationVector(out camAcceleration);
            Quaternion cameraOrientation =
                Quaternion.FromToRotation(new Vector3(-camAcceleration.x, camAcceleration.y, -camAcceleration.z),
                                          new Vector3(0, 1, 0));
            frustumTransform.eulerAngles=  (q * cameraOrientation).eulerAngles ;
#endif
        }

   


    }
}