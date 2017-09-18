// Simulator Controller|SDK_Simulator|003

using System.Collections;
using UnityEngine;
#if UNITY_PS4
using UnityEngine.PS4;

#endif

namespace VRTK
{
    /// <remarks>
    /// The PlayStation VR Controller Tracking extension allows the use of the Playstation Controller vibration
    /// </remarks>
    public partial class SDK_PlayStationVRController
    {
        private Coroutine vibrateCoroutine;
        private float vibrationStrength = 1;

        public void SetvibrationStrength(float value)
        {
            vibrationStrength = Mathf.Clamp01(value);
        }

        public void SetvibrationStrength(int value, int max = 100)
        {
            float strength = value / max;
            vibrationStrength = Mathf.Clamp01(strength);
        }

        /// <summary>
        /// The HapticPulse method is used to initiate a simple haptic pulse on the tracked object of the given index.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to initiate the haptic pulse on.</param>
        /// <param name="strength">The intensity of the rumble of the controller motor. `0` to `1`.</param>
        public override void HapticPulse(VRTK_ControllerReference controllerReference, float strength = 0.5f)
        {
            PlayStationHapticPulseOnIndex(VRTK_ControllerReference.GetRealIndex(controllerReference), strength);
        }

        /// <summary>
        ///     The HapticPulseOnIndex method is used to initiate a simple haptic pulse on the tracked object of the given index.
        /// </summary>
        /// <param name="index">The index of the tracked object to initiate the haptic pulse on.</param>
        /// <param name="strength">The intensity of the rumble of the controller motor. `0` to `1`.</param>
        public virtual void PlayStationHapticPulseOnIndex(uint index, float strength = 0.5f)
        {
            int vibrate = Mathf.RoundToInt((62 + 192 * strength) * vibrationStrength);

            switch (index)
            {
                case 1:
                    TriggerHapticPulse(0, vibrate);
                    break;
                case 2:
                    TriggerHapticPulse(1, vibrate);
                    break;
            }
        }

        public virtual void TriggerHapticPulse(int index, int strength = 128)
        {
            if (vibrateCoroutine != null)
            {
                return;
            }
            vibrateCoroutine = index == 0
                ? rightController.StartCoroutine(Vibrate(index, strength))
                : leftController.StartCoroutine(Vibrate(index, strength));
        }

        public virtual void VibrateController(int index, int strength = 128, float time = .1f)
        {
            if (vibrateCoroutine != null)
            {
                return;
            }

            vibrateCoroutine = index == 0
                ? rightController.StartCoroutine(Vibrate(index, strength))
                : leftController.StartCoroutine(Vibrate(index, strength));
        }


        private IEnumerator Vibrate(int index, int strength = 128, float time = .1f)
        {
#if UNITY_PS4
            PS4Input.MoveSetVibration(0, index, strength);

#endif
            yield return new WaitForSeconds(time);

#if UNITY_PS4

            PS4Input.MoveSetVibration(0, index, 0);

#endif
            vibrateCoroutine = null;
            yield return null;
        }
    }
}