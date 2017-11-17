using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;
#if UNITY_PS4
using UnityEngine.PS4;
using UnityEngine.PS4.VR;
#endif

namespace VRTK
{
    /// <remarks>
    /// The PlayStation Move Controller Tracking extension allows the controllers to use the PlayStation SDK to register 
    /// the controle
    /// and update on system events.
    /// </remarks>
    public partial class SDK_PlayStationMoveController
    {
        private  TrackedDevice device;
        public TrackedDevice Device {
            get { return device ?? (device = CreateDevice()); }
        }

       
        public TrackedDevice CreateDevice()
        {
         
                 Transform lightEffect = transform.Find("Hand/Controller/motion_controller_lod01_top/sphere_lod01_mesh");
                Renderer lightRenderer = null;
                if (lightEffect)
                {
                    lightRenderer = lightEffect.GetComponent<MeshRenderer>();
                }

              return new TrackedDevice
                {
                    transform = transform,
                    light = lightRenderer,
                    Primary = ControllerType == Controller.Primary
                };
            
        }



        public void OnUnregisterMoveController()
        {
            Device.handle = -1;
          //  transform.localPosition = Vector3.zero;
           // transform.localRotation = Quaternion.identity;
            gameObject.SetActive(false);
        }

        public Vector2 Trigger()
        {
            int index = ControllerType == Controller.Primary ? 0 : 1;

#if UNITY_PS4 && !UNITY_EDITOR
            return new Vector2(0, PS4Input.MoveGetAnalogButton(0, index));
#endif
            return Vector2.zero;
        }

     

        public Vector3 DeviceVelocity()
        {
            return Device.Velocity;
        }

        public Vector3 DeviceAngularVelocity()
        {
            return Device.AngularVelocity;
        }


    

        private void UpdateMoveTransforms()
        {



            if (!UnityEngine.XR.XRDevice.isPresent)
            {
                return;
            }

#if UNITY_PS4
            // Perform tracking for the primary controller, if we've got a handle
            Vector3 velocity;
            Vector3 angularVelocity;

            if (Device.transform == null || Device.handle < 0)
            {
                return;
            }

     
            if (Tracker.GetTrackedDevicePosition(Device.handle, out Device.position) == PlayStationVRResult.Ok)
            {
                Device.transform.localPosition = Device.position;
            }

            if (Tracker.GetTrackedDeviceOrientation(Device.handle, out Device.orientation) == PlayStationVRResult.Ok)
            {
                Device.transform.localRotation = Device.orientation;
            }

            if (Tracker.GetTrackedDeviceVelocity(Device.handle, out velocity) == PlayStationVRResult.Ok)
            {
            
                Device.Velocity = Device.transform.root.rotation * Device.transform.parent.InverseTransformDirection(velocity);
            }

            if (Tracker.GetTrackedDeviceAngularVelocity(Device.handle, out angularVelocity) == PlayStationVRResult.Ok)
            {
             
                Device.AngularVelocity =angularVelocity;
            }





#endif
        }

        #region Nested type: TrackedDevice

        public class TrackedDevice
        {
            public Vector3 AngularVelocity;
            public int handle = -1;
            public Renderer light;
            public Quaternion orientation = Quaternion.identity;
            public Vector3 position = Vector3.zero;
            public bool Primary;
            public Transform transform;
            public Vector3 Velocity;

#if UNITY_PS4

            public void SetLightColor(PlayStationVRTrackingColor trackingColor)
            {
               
                if (light)
                {
                    Color newColor = GetUnityColor(trackingColor);
                    light.material.color = newColor;
                }
            }

            private Color GetUnityColor(PlayStationVRTrackingColor trackingColor)
            {
                switch (trackingColor)
                {
                    case PlayStationVRTrackingColor.Blue:
                        return Color.blue;
                    case PlayStationVRTrackingColor.Green:
                        return Color.green;
                    case PlayStationVRTrackingColor.Magenta:
                        return Color.magenta;
                    case PlayStationVRTrackingColor.Red:
                        return Color.red;
                    case PlayStationVRTrackingColor.Yellow:
                        return Color.yellow;
                    default:
                        return Color.black;
                }
            }

   

#endif
        }

        #endregion
    }
}