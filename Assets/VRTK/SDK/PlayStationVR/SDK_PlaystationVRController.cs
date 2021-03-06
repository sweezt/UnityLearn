﻿// Simulator Controller|SDK_Simulator|003

using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRTK
{
    /// <summary>
    ///     The PlayStation SDK script provides functions to set up the PlayStation Move Controller with VRTK.
    /// </summary>
    [SDK_Description(typeof(SDK_PlayStationVRSystem))]
    public partial class SDK_PlayStationVRController : SDK_BaseController
    {
        //public PlayStationKeys Trigger = PlayStationKeys.Trigger;
        //public PlayStationKeys Grip = PlayStationKeys.Square;
        //public PlayStationKeys ButtonOne = PlayStationKeys.Cross;
        //public PlayStationKeys ButtonTwo = PlayStationKeys.Circle;
        //public PlayStationKeys StartMenu = PlayStationKeys.Start;

        private const string HandName = "Hand";
        private SDK_PlayStationMoveController leftController;
        private SDK_PlayStationMoveController rightController;

        //private bool stick;

        //[Tooltip("sticky - the activator remains on until pressed again")]
        //public bool Sticky;

        //[Tooltip("the key used to access the touch-pad features inside of VRTK")]
        //public PlayStationKeys TouchPadSimulationActivator =
        //    PlayStationKeys.Trigger;

        //[Tooltip("the button that will simulation the touch-pad when the activator is active ")]
        //public PlayStationKeys TouchPadSimulationKey =
        //    PlayStationKeys.Cross;

        /// <summary>
        ///     set the key mapping for both controllers
        /// </summary>
        /// <param name="givenKeyMappings"> key mapping </param>
        public virtual void SetKeyMappings(
            Dictionary<PlayStationKeys, KeyCode> givenKeyMappings)
        {
            rightController.SetKeyMappings(givenKeyMappings);
            leftController.SetKeyMappings(givenKeyMappings);
        }

        ///// <summary>
        /////     set the key mapping for a specific controller
        ///// </summary>
        ///// <param name="givenKeyMappings"></param>
        ///// <param name="controller"></param>
        //public virtual void SetKeyMappings(
        //    Dictionary<PlayStationKeys, KeyCode> givenKeyMappings,
        //    SDK_PlayStationMoveController controller)
        //{
        //    controller.SetKeyMappings(givenKeyMappings);
        //}

        /// <summary>
        ///     The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="index">The index of the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public override void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options) { }

        /// <summary>
        ///     The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate
        /// </summary>
        /// <param name="index">The index of the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the fixed update.</param>
        public override void ProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options) { }

        /// <summary>
        ///     The GetControllerDefaultColliderPath returns the path to the prefab that contains the collider objects for the
        ///     default controller of this SDK.
        /// </summary>
        /// <param name="hand">The controller hand to check for</param>
        /// <returns>A path to the resource that contains the collider GameObject.</returns>
        public override string GetControllerDefaultColliderPath(ControllerHand hand)
        {
            return "ControllerColliders/Simulator";
        }

        /// <summary>
        ///     The GetControllerElementPath returns the path to the game object that the given controller element for the given
        ///     hand resides in.
        /// </summary>
        /// <param name="element">The controller element to look up.</param>
        /// <param name="hand">The controller hand to look up.</param>
        /// <param name="fullPath">Whether to get the initial path or the full path to the element.</param>
        /// <returns>A string containing the path to the game object that the controller element resides in.</returns>
        public override string GetControllerElementPath(ControllerElements element, ControllerHand hand,
                                                        bool fullPath = false)
        {
            string suffix = fullPath ? "/attach" : "";
            switch (element)
            {
                case ControllerElements.AttachPoint:
                    return "";
                case ControllerElements.Trigger:
                    return "" + suffix;
                case ControllerElements.GripLeft:
                    return "" + suffix;
                case ControllerElements.GripRight:
                    return "" + suffix;
                case ControllerElements.Touchpad:
                    return "" + suffix;
                case ControllerElements.ButtonOne:
                    return "" + suffix;
                case ControllerElements.SystemMenu:
                    return "" + suffix;
                case ControllerElements.Body:
                    return "";
            }
            return null;
        }

        /// <summary>
        ///     The GetControllerIndex method returns the index of the given controller.
        /// </summary>
        /// <param name="controller">The GameObject containing the controller.</param>
        /// <returns>The index of the given controller.</returns>
        public override uint GetControllerIndex(GameObject controller)
        {
            uint index = 0;

            switch (controller.name)
            {
                case "Camera":
                    index = 0;
                    break;
                case "RightController":
                    index = 1;
                    break;
                case "LeftController":
                    index = 2;
                    break;
            }
            return index;
        }

        /// <summary>
        ///     The GetControllerByIndex method returns the GameObject of a controller with a specific index.
        /// </summary>
        /// <param name="index">The index of the controller to find.</param>
        /// <param name="actual">
        ///     If true it will return the actual controller, if false it will return the script alias controller
        ///     GameObject.
        /// </param>
        /// <returns>The GameObject of the controller</returns>
        public override GameObject GetControllerByIndex(uint index, bool actual = false)
        {
            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                switch (index)
                {
                    case 1:
                        return actual ? sdkManager.actualRightController : sdkManager.scriptAliasRightController;
                    case 2:
                        return actual ? sdkManager.actualLeftController : sdkManager.scriptAliasLeftController;
                    default:
                        return null;
                }
            }

            return null;
        }

        /// <summary>
        ///     The GetControllerOrigin method returns the origin of the given controller.
        /// </summary>
        /// <param name="controller">The controller to retrieve the origin from.</param>
        /// <returns>A Transform containing the origin of the controller.</returns>
        public override Transform GetControllerOrigin(VRTK_ControllerReference controller)
        {
            return controller.actual.transform;
        }

        /// <summary>
        ///     The GenerateControllerPointerOrigin method can create a custom pointer origin Transform to represent the pointer
        ///     position and forward.
        /// </summary>
        /// <param name="parent">
        ///     The GameObject that the origin will become parent of. If it is a controller then it will also be
        ///     used to determine the hand if required.
        /// </param>
        /// <returns>A generated Transform that contains the custom pointer origin.</returns>
        public override Transform GenerateControllerPointerOrigin(GameObject parent)
        {
            return null;
        }

        /// <summary>
        ///     The GetControllerLeftHand method returns the GameObject containing the representation of the left hand controller.
        /// </summary>
        /// <param name="actual">
        ///     If true it will return the actual controller, if false it will return the script alias controller
        ///     GameObject.
        /// </param>
        /// <returns>The GameObject containing the left hand controller.</returns>
        public override GameObject GetControllerLeftHand(bool actual = false)
        {
            // use the basic base functionality to find the left hand controller
            GameObject controller = GetSDKManagerControllerLeftHand(actual);
            // if the controller cannot be found with default settings, try finding it below the InputSimulator by name
            if (!controller && actual)
            {
                controller = GetActualController(ControllerHand.Left);
            }

            return controller;
        }

        /// <summary>
        ///     The GetControllerRightHand method returns the GameObject containing the representation of the right hand
        ///     controller.
        /// </summary>
        /// <param name="actual">
        ///     If true it will return the actual controller, if false it will return the script alias controller
        ///     GameObject.
        /// </param>
        /// <returns>The GameObject containing the right hand controller.</returns>
        public override GameObject GetControllerRightHand(bool actual = false)
        {
            // use the basic base functionality to find the right hand controller
            GameObject controller = GetSDKManagerControllerRightHand(actual);
            // if the controller cannot be found with default settings, try finding it below the InputSimulator by name
            if (!controller && actual)
            {
                controller = GetActualController(ControllerHand.Right);
            }

            return controller;
        }

        /// <summary>
        ///     finds the actual controller for the specified hand (identified by name) and returns it
        /// </summary>
        /// <param name="hand">the for which to find the respective controller gameobject</param>
        /// <returns>the gameobject of the actual controller corresponding to the specified hand</returns>
        private static GameObject GetActualController(ControllerHand hand)
        {
            GameObject simPlayer = SDK_PlayStationVRInput.FindInScene();
            GameObject controller = null;

            if (simPlayer == null)
            {
                return controller;
            }
            List<SDK_PlayStationMoveController> controllers =
                new List<SDK_PlayStationMoveController>(
                    simPlayer.GetComponentsInChildren<SDK_PlayStationMoveController>(true));
            switch (hand)
            {
                case ControllerHand.Right:
                    SDK_PlayStationMoveController rightController =
                        controllers.Find(e => e.ControllerType == SDK_PlayStationMoveController.Controller.Primary);
                    if (rightController)
                    {
                        controller = rightController.gameObject;
                    }
                    break;
                case ControllerHand.Left:
                    SDK_PlayStationMoveController leftController =
                        controllers.Find(e => e.ControllerType == SDK_PlayStationMoveController.Controller.Secondary);
                    if (leftController)
                    {
                        controller = leftController.gameObject;
                    }
                    break;
                case ControllerHand.None:
                    break;
                default:
                    break;
            }

            return controller;
        }

        /// <summary>
        ///     The IsControllerLeftHand/1 method is used to check if the given controller is the the left hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>Returns true if the given controller is the left hand controller.</returns>
        public override bool IsControllerLeftHand(GameObject controller)
        {
            return CheckActualOrScriptAliasControllerIsLeftHand(controller);
        }

        /// <summary>
        ///     The IsControllerRightHand/1 method is used to check if the given controller is the the right hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>Returns true if the given controller is the right hand controller.</returns>
        public override bool IsControllerRightHand(GameObject controller)
        {
            return CheckActualOrScriptAliasControllerIsRightHand(controller);
        }

        /// <summary>
        ///     The IsControllerLeftHand/2 method is used to check if the given controller is the the left hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <param name="actual">If true it will check the actual controller, if false it will check the script alias controller.</param>
        /// <returns>Returns true if the given controller is the left hand controller.</returns>
        public override bool IsControllerLeftHand(GameObject controller, bool actual)
        {
            return CheckControllerLeftHand(controller, actual);
        }

        /// <summary>
        ///     The IsControllerRightHand/2 method is used to check if the given controller is the the right hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <param name="actual">If true it will check the actual controller, if false it will check the script alias controller.</param>
        /// <returns>Returns true if the given controller is the right hand controller.</returns>
        public override bool IsControllerRightHand(GameObject controller, bool actual)
        {
            return CheckControllerRightHand(controller, actual);
        }

        /// <summary>
        ///     The GetControllerModel method returns the model alias for the given GameObject.
        /// </summary>
        /// <param name="controller">The GameObject to get the model alias for.</param>
        /// <returns>The GameObject that has the model alias within it.</returns>
        public override GameObject GetControllerModel(GameObject controller)
        {
            return GetControllerModelFromController(controller);
        }

        /// <summary>
        ///     The GetControllerModel method returns the model alias for the given controller hand.
        /// </summary>
        /// <param name="hand">The hand enum of which controller model to retrieve.</param>
        /// <returns>The GameObject that has the model alias within it.</returns>
        public override GameObject GetControllerModel(ControllerHand hand)
        {
            GameObject model = null;
            GameObject simPlayer = SDK_PlayStationVRInput.FindInScene();
            if (!simPlayer)
            {
                return null;
            }
            List<SDK_PlayStationMoveController> controllers =
                new List<SDK_PlayStationMoveController>(
                    simPlayer.GetComponentsInChildren<SDK_PlayStationMoveController>(true));
            SDK_PlayStationMoveController rightController =
                controllers.Find(e => e.ControllerType == SDK_PlayStationMoveController.Controller.Primary);
            SDK_PlayStationMoveController leftController =
                controllers.Find(e => e.ControllerType == SDK_PlayStationMoveController.Controller.Secondary);
            switch (hand)
            {
                case ControllerHand.Left:
                    model = leftController.transform.Find(HandName).gameObject;
                    break;
                case ControllerHand.Right:
                    model = rightController.transform.Find(HandName).gameObject;
                    break;
                case ControllerHand.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("hand", hand, null);
            }
            return model;
        }

        /// <summary>
        ///     The GetControllerRenderModel method gets the game object that contains the given controller's render model.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>A GameObject containing the object that has a render model for the controller.</returns>
        public override GameObject GetControllerRenderModel(VRTK_ControllerReference controller)
        {
            return controller.actual.transform.Find("Hand").gameObject;
        }

        /// <summary>
        ///     The SetControllerRenderModelWheel method sets the state of the scroll wheel on the controller render model.
        /// </summary>
        /// <param name="renderModel">The GameObject containing the controller render model.</param>
        /// <param name="state">
        ///     If true and the render model has a scroll wheen then it will be displayed, if false then the scroll
        ///     wheel will be hidden.
        /// </param>
        public override void SetControllerRenderModelWheel(GameObject renderModel, bool state) { }


        /// <summary>
        ///     The GetHapticModifiers method is used to return modifiers for the duration and interval if the SDK handles it
        ///     slightly differently.
        /// </summary>
        /// <returns>An SDK_ControllerHapticModifiers object with a given `durationModifier` and an `intervalModifier`.</returns>
        public override SDK_ControllerHapticModifiers GetHapticModifiers()
        {
            return new SDK_ControllerHapticModifiers();
        }

        /// <summary>
        /// The GetVelocity method is used to determine the current velocity of the tracked object on the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current velocity of the tracked object.</returns>
        public override Vector3 GetVelocity(VRTK_ControllerReference controllerReference)
        {
            return GetVelocityOnIndex(VRTK_ControllerReference.GetRealIndex(controllerReference));
        }

        /// <summary>
        ///     The GetVelocityOnIndex method is used to determine the current velocity of the tracked object on the given index.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current velocity of the tracked object.</returns>
        public Vector3 GetVelocityOnIndex(uint index)
        {
            switch (index)
            {
                case 1:
                    return rightController.GetVelocity();
                case 2:
                    return leftController.GetVelocity();
                default:
                    return Vector3.zero;
            }
        }

        /// <summary>
        /// The GetAngularVelocity method is used to determine the current angular velocity of the tracked object on the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current angular velocity of the tracked object.</returns>
        public override Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference)
        {
            return GetAngularVelocityOnIndex(VRTK_ControllerReference.GetRealIndex(controllerReference));
        }

        /// <summary>
        ///     The GetAngularVelocityOnIndex method is used to determine the current angular velocity of the tracked object on the
        ///     given index.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current angular velocity of the tracked object.</returns>
        public Vector3 GetAngularVelocityOnIndex(uint index)
        {
            switch (index)
            {
                case 1:
                    return rightController.GetAngularVelocity();
                case 2:
                    return leftController.GetAngularVelocity();
                default:
                    return Vector3.zero;
            }
        }

        /// <summary>
        ///     The GetTouchpadAxisOnIndex method is used to get the current touch position on the controller touchpad.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector2 containing the current x,y position of where the touchpad is being touched.</returns>
        public Vector2 GetTouchpadAxisOnIndex(uint index)
        {
            //if (TouchPadSimulator(index, ButtonPressTypes.Press))
            //{
            //    return new Vector2(0, 1);
            //}

            return Vector2.zero;
        }

        /// <summary>
        /// The GetButtonAxis method retrieves the current X/Y axis values for the given button type on the given controller reference.
        /// </summary>
        /// <param name="buttonType">The type of button to check for the axis on.</param>
        /// <param name="controllerReference">The reference to the controller to check the button axis on.</param>
        /// <returns>A Vector2 of the X/Y values of the button axis. If no axis values exist for the given button, then a Vector2.Zero is returned.</returns>
        public override Vector2 GetButtonAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);

            switch (buttonType)
            {
                case ButtonTypes.Touchpad:
                    return GetTouchpadAxisOnIndex(index);
                case ButtonTypes.Trigger:
                    return GetTriggerAxisOnIndex(index);
            }
            return Vector2.zero;
        }

        /// <summary>
        /// The GetButtonHairlineDelta method is used to get the difference between the current button press and the previous frame button press.
        /// </summary>
        /// <param name="buttonType">The type of button to get the hairline delta for.</param>
        /// <param name="controllerReference">The reference to the controller to get the hairline delta for.</param>
        /// <returns>The delta between the button presses.</returns>
        public override float GetButtonHairlineDelta(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);

            return (buttonType == ButtonTypes.Trigger || buttonType == ButtonTypes.TriggerHairline ? GetTriggerHairlineDeltaOnIndex(index) : GetGripHairlineDeltaOnIndex(index));
        }

        private Dictionary<ButtonTypes, PlayStationKeys> m_keysMapper = new Dictionary<ButtonTypes, PlayStationKeys>();

        public void InitKeyMapper(Dictionary<ButtonTypes, PlayStationKeys> keymapper)
        {
            m_keysMapper = keymapper;
        }

        /// <summary>
        /// The GetControllerButtonState method is used to determine if the given controller button for the given press type on the given controller reference is currently taking place.
        /// </summary>
        /// <param name="buttonType">The type of button to check for the state of.</param>
        /// <param name="pressType">The button state to check for.</param>
        /// <param name="controllerReference">The reference to the controller to check the button state on.</param>
        /// <returns>Returns true if the given button is in the state of the given press type on the given controller reference.</returns>
        public override bool GetControllerButtonState(ButtonTypes buttonType, ButtonPressTypes pressType, VRTK_ControllerReference controllerReference)
        {
            var index = VRTK_ControllerReference.GetRealIndex(controllerReference);

            //return m_keysMapper.ContainsKey(buttonType) && IsButtonPressed(index, pressType, m_keysMapper[buttonType]);
            switch (buttonType)
            {
                case ButtonTypes.Trigger:
                    return IsButtonPressed(index, pressType, m_keysMapper[ButtonTypes.Trigger]);
                case ButtonTypes.TriggerHairline:
                    return false;
                case ButtonTypes.Grip:
                    return IsButtonPressed(index, pressType, m_keysMapper[ButtonTypes.Grip]);
                case ButtonTypes.GripHairline:
                    return false;
                case ButtonTypes.Touchpad:
                    return IsButtonPressed(index, pressType, m_keysMapper[ButtonTypes.Touchpad]);
                case ButtonTypes.ButtonOne:
                    return IsButtonPressed(index, pressType, m_keysMapper[ButtonTypes.ButtonOne]);
                case ButtonTypes.ButtonTwo:
                    return IsButtonPressed(index, pressType, m_keysMapper[ButtonTypes.ButtonTwo]);
                case ButtonTypes.StartMenu:
                    return IsButtonPressed(index, pressType, m_keysMapper[ButtonTypes.StartMenu]);

                default:
                    return false;
            }
        }

        /// <summary>
        ///     The GetTriggerAxisOnIndex method is used to get the current trigger position on the controller.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector2 containing the current position of the trigger.</returns>
        public Vector2 GetTriggerAxisOnIndex(uint index)
        {

            return Vector3.zero;

        }

        ///// <summary>
        /////     The GetGripAxisOnIndex method is used to get the current grip position on the controller.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>A Vector2 containing the current position of the grip.</returns>
        //public Vector2 GetGripAxisOnIndex(uint index)
        //{

        //    return Vector2.zero;
        //}

        /// <summary>
        ///     The GetTriggerHairlineDeltaOnIndex method is used to get the difference between the current trigger press and the
        ///     previous frame trigger press.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>The delta between the trigger presses.</returns>
        public float GetTriggerHairlineDeltaOnIndex(uint index)
        {
            return 0;
        }

        /// <summary>
        ///     The GetGripHairlineDeltaOnIndex method is used to get the difference between the current grip press and the
        ///     previous frame grip press.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>The delta between the grip presses.</returns>
        public float GetGripHairlineDeltaOnIndex(uint index)
        {
            return 0f;
        }

        ///// <summary>
        /////     The IsTriggerPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button is continually being pressed.</returns>
        //public bool IsTriggerPressedOnIndex(uint index)
        //{
        //    // button presses shall be ignored if the hair touch or touch modifiers are used
        //    return !IsButtonPressIgnored() && IsTriggerTouchedOnIndex(index);
        //}

        ///// <summary>
        /////     The IsTriggerPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been pressed down.</returns>
        //public bool IsTriggerPressedDownOnIndex(uint index)
        //{
        //    // button presses shall be ignored if the hair touch or touch modifiers are used
        //    return !IsButtonPressIgnored() && IsTriggerTouchedDownOnIndex(index);
        //}

        ///// <summary>
        /////     The IsTriggerPressedUpOnIndex method is used to determine if the controller button has just been released.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been released.</returns>
        //public bool IsTriggerPressedUpOnIndex(uint index)
        //{
        //    // button presses shall be ignored if the hair touch or touch modifiers are used
        //    return !IsButtonPressIgnored() && IsTriggerTouchedUpOnIndex(index);
        //}

        ///// <summary>
        /////     The IsTriggerTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button is continually being touched.</returns>
        //public bool IsTriggerTouchedOnIndex(uint index)
        //{
        //    return IsButtonPressed(index, ButtonPressTypes.Press,
        //                           PlayStationKeys.Trigger);
        //}

        ///// <summary>
        /////     The IsTriggerTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been touched down.</returns>
        //public bool IsTriggerTouchedDownOnIndex(uint index)
        //{
        //    return IsButtonPressed(index, ButtonPressTypes.PressDown,
        //                           PlayStationKeys.Trigger);
        //}

        ///// <summary>
        /////     The IsTriggerTouchedUpOnIndex method is used to determine if the controller button has just been released.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been released.</returns>
        //public bool IsTriggerTouchedUpOnIndex(uint index)
        //{
        //    return IsButtonPressed(index, ButtonPressTypes.PressUp,
        //                           PlayStationKeys.Trigger);
        //}

        ///// <summary>
        /////     The IsHairTriggerDownOnIndex method is used to determine if the controller button has passed it's press threshold.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has passed it's press threshold.</returns>
        //public bool IsHairTriggerDownOnIndex(uint index)
        //{
        //    // button hair touches shall be ignored if the only the touch modifier is used
        //    return !IsButtonHairTouchIgnored() && IsTriggerTouchedDownOnIndex(index);
        //}

        ///// <summary>
        /////     The IsHairTriggerUpOnIndex method is used to determine if the controller button has been released from it's press
        /////     threshold.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been released from it's press threshold.</returns>
        //public bool IsHairTriggerUpOnIndex(uint index)
        //{
        //    // button hair touches shall be ignored if the only the touch modifier is used
        //    return !IsButtonHairTouchIgnored() && IsTriggerTouchedUpOnIndex(index);
        //}

        ///// <summary>
        /////     The IsGripPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button is continually being pressed.</returns>
        //public bool IsGripPressedOnIndex(uint index)
        //{
        //    // button presses shall be ignored if the hair touch or touch modifiers are used
        //    return !IsButtonPressIgnored() && IsGripTouchedOnIndex(index);
        //}

        ///// <summary>
        /////     The IsGripPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been pressed down.</returns>
        //public bool IsGripPressedDownOnIndex(uint index)
        //{
        //    // button presses shall be ignored if the hair touch or touch modifiers are used
        //    return !IsButtonPressIgnored() && IsGripTouchedDownOnIndex(index);
        //}

        ///// <summary>
        /////     The IsGripPressedUpOnIndex method is used to determine if the controller button has just been released.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been released.</returns>
        //public bool IsGripPressedUpOnIndex(uint index)
        //{
        //    // button presses shall be ignored if the hair touch or touch modifiers are used
        //    return !IsButtonPressIgnored() && IsGripTouchedUpOnIndex(index);
        //}

        ///// <summary>
        /////     The IsGripTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button is continually being touched.</returns>
        //public bool IsGripTouchedOnIndex(uint index)
        //{
        //    return IsButtonPressed(index, ButtonPressTypes.Press, PlayStationKeys.Middle);
        //}

        ///// <summary>
        /////     The IsGripTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been touched down.</returns>
        //public bool IsGripTouchedDownOnIndex(uint index)
        //{
        //    return IsButtonPressed(index, ButtonPressTypes.PressDown,
        //                           PlayStationKeys.Middle);
        //}

        ///// <summary>
        /////     The IsGripTouchedUpOnIndex method is used to determine if the controller button has just been released.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been released.</returns>
        //public bool IsGripTouchedUpOnIndex(uint index)
        //{
        //    return IsButtonPressed(index, ButtonPressTypes.PressUp,
        //                           PlayStationKeys.Middle);
        //}

        ///// <summary>
        /////     The IsHairGripDownOnIndex method is used to determine if the controller button has passed it's press threshold.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has passed it's press threshold.</returns>
        //public bool IsHairGripDownOnIndex(uint index)
        //{
        //    // button hair touches shall be ignored if the hair touch modifier is used
        //    return !IsButtonHairTouchIgnored() && IsGripTouchedDownOnIndex(index);
        //}

        ///// <summary>
        /////     The IsHairGripUpOnIndex method is used to determine if the controller button has been released from it's press
        /////     threshold.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been released from it's press threshold.</returns>
        //public bool IsHairGripUpOnIndex(uint index)
        //{
        //    // button hair touches shall be ignored if the hair touch modifier is used
        //    return !IsButtonHairTouchIgnored() && IsGripTouchedUpOnIndex(index);
        //}

        /// <summary>
        /// The IsTouchpadStatic method is used to determine if the touchpad is currently not being moved.
        /// </summary>
        /// <param name="isTouched"></param>
        /// <param name="currentAxisValues"></param>
        /// <param name="previousAxisValues"></param>
        /// <param name="compareFidelity"></param>
        /// <returns>Returns true if the touchpad is not currently being touched or moved.</returns>
        public override bool IsTouchpadStatic(bool isTouched, Vector2 currentAxisValues, Vector2 previousAxisValues, int compareFidelity)
        {
            return (!isTouched || VRTK_SharedMethods.Vector2ShallowCompare(currentAxisValues, previousAxisValues, compareFidelity));
        }

        ///// <summary>
        /////     The IsTouchpadPressedOnIndex method is used to determine if the controller button is being pressed down
        /////     continually.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button is continually being pressed.</returns>
        //public bool IsTouchpadPressedOnIndex(uint index)
        //{
        //    // button presses shall be ignored if the hair touch or touch modifiers are used
        //    return !IsButtonPressIgnored() && IsTouchpadTouchedOnIndex(index);
        //}

        ///// <summary>
        /////     The IsTouchpadPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been pressed down.</returns>
        //public bool IsTouchpadPressedDownOnIndex(uint index)
        //{
        //    // button presses shall be ignored if the hair touch or touch modifiers are used
        //    return !IsButtonPressIgnored() && IsTouchpadTouchedDownOnIndex(index);
        //}

        ///// <summary>
        /////     The IsTouchpadPressedUpOnIndex method is used to determine if the controller button has just been released.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been released.</returns>
        //public bool IsTouchpadPressedUpOnIndex(uint index)
        //{
        //    // button presses shall be ignored if the hair touch or touch modifiers are used
        //    return !IsButtonPressIgnored() && IsTouchpadTouchedUpOnIndex(index);
        //}

        ///// <summary>
        /////     The IsTouchpadTouchedOnIndex method is used to determine if the controller button is being touched down
        /////     continually.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button is continually being touched.</returns>
        //public bool IsTouchpadTouchedOnIndex(uint index)
        //{
        //    return TouchPadSimulator(index, ButtonPressTypes.Press);
        //}

        ///// <summary>
        /////     The IsTouchpadTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been touched down.</returns>
        //public bool IsTouchpadTouchedDownOnIndex(uint index)
        //{
        //    return TouchPadSimulator(index, ButtonPressTypes.PressDown);
        //}

        ///// <summary>
        /////     The IsTouchpadTouchedUpOnIndex method is used to determine if the controller button has just been released.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been released.</returns>
        //public bool IsTouchpadTouchedUpOnIndex(uint index)
        //{
        //    return TouchPadSimulator(index, ButtonPressTypes.PressUp);
        //}

        ///// <summary>
        /////     The IsButtonOnePressedOnIndex method is used to determine if the controller button is being pressed down
        /////     continually.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button is continually being pressed.</returns>
        //public bool IsButtonOnePressedOnIndex(uint index)
        //{
        //    // button presses shall be ignored if the hair touch or touch modifiers are used
        //    return !IsButtonPressIgnored() && IsButtonOneTouchedOnIndex(index);
        //}

        ///// <summary>
        /////     The IsButtonOnePressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been pressed down.</returns>
        //public bool IsButtonOnePressedDownOnIndex(uint index)
        //{
        //    // button presses shall be ignored if the hair touch or touch modifiers are used
        //    return !IsButtonPressIgnored() && IsButtonOneTouchedDownOnIndex(index);
        //}

        ///// <summary>
        /////     The IsButtonOnePressedUpOnIndex method is used to determine if the controller button has just been released.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been released.</returns>
        //public bool IsButtonOnePressedUpOnIndex(uint index)
        //{
        //    // button presses shall be ignored if the hair touch or touch modifiers are used
        //    return !IsButtonPressIgnored() && IsButtonOneTouchedUpOnIndex(index);
        //}

        ///// <summary>
        /////     The IsButtonOneTouchedOnIndex method is used to determine if the controller button is being touched down
        /////     continually.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button is continually being touched.</returns>
        //public bool IsButtonOneTouchedOnIndex(uint index)
        //{
        //    return IsButtonPressed(index, ButtonPressTypes.Press, PlayStationKeys.Cross);
        //}

        ///// <summary>
        /////     The IsButtonOneTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been touched down.</returns>
        //public bool IsButtonOneTouchedDownOnIndex(uint index)
        //{
        //    return IsButtonPressed(index, ButtonPressTypes.PressDown,
        //                           PlayStationKeys.Cross);
        //}

        ///// <summary>
        /////     The IsButtonOneTouchedUpOnIndex method is used to determine if the controller button has just been released.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been released.</returns>
        //public bool IsButtonOneTouchedUpOnIndex(uint index)
        //{
        //    return IsButtonPressed(index, ButtonPressTypes.PressUp,
        //                           PlayStationKeys.Cross);
        //}

        ///// <summary>
        /////     The IsButtonTwoPressedOnIndex method is used to determine if the controller button is being pressed down
        /////     continually.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button is continually being pressed.</returns>
        //public bool IsButtonTwoPressedOnIndex(uint index)
        //{
        //    // button presses shall be ignored if the hair touch or touch modifiers are used
        //    return !IsButtonPressIgnored() && IsButtonTwoTouchedOnIndex(index);
        //}

        ///// <summary>
        /////     The IsButtonTwoPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been pressed down.</returns>
        //public bool IsButtonTwoPressedDownOnIndex(uint index)
        //{
        //    // button presses shall be ignored if the hair touch or touch modifiers are used
        //    return !IsButtonPressIgnored() && IsButtonTwoTouchedDownOnIndex(index);
        //}

        ///// <summary>
        /////     The IsButtonTwoPressedUpOnIndex method is used to determine if the controller button has just been released.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been released.</returns>
        //public bool IsButtonTwoPressedUpOnIndex(uint index)
        //{
        //    // button presses shall be ignored if the hair touch or touch modifiers are used
        //    return !IsButtonPressIgnored() && IsButtonTwoTouchedUpOnIndex(index);
        //}

        ///// <summary>
        /////     The IsButtonTwoTouchedOnIndex method is used to determine if the controller button is being touched down
        /////     continually.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button is continually being touched.</returns>
        //public bool IsButtonTwoTouchedOnIndex(uint index)
        //{
        //    return IsButtonPressed(index, ButtonPressTypes.Press, PlayStationKeys.Circle);
        //}

        ///// <summary>
        /////     The IsButtonTwoTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been touched down.</returns>
        //public bool IsButtonTwoTouchedDownOnIndex(uint index)
        //{
        //    return IsButtonPressed(index, ButtonPressTypes.PressDown,
        //                           PlayStationKeys.Circle);
        //}

        ///// <summary>
        /////     The IsButtonTwoTouchedUpOnIndex method is used to determine if the controller button has just been released.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been released.</returns>
        //public bool IsButtonTwoTouchedUpOnIndex(uint index)
        //{
        //    return IsButtonPressed(index, ButtonPressTypes.PressUp,
        //                           PlayStationKeys.Circle);
        //}

        ///// <summary>
        /////     The IsStartMenuPressedOnIndex method is used to determine if the controller button is being pressed down
        /////     continually.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button is continually being pressed.</returns>
        //public bool IsStartMenuPressedOnIndex(uint index)
        //{
        //    // button presses shall be ignored if the hair touch or touch modifiers are used
        //    return !IsButtonPressIgnored() && IsStartMenuTouchedOnIndex(index);
        //}

        ///// <summary>
        /////     The IsStartMenuPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been pressed down.</returns>
        //public bool IsStartMenuPressedDownOnIndex(uint index)
        //{
        //    // button presses shall be ignored if the hair touch or touch modifiers are used
        //    return !IsButtonPressIgnored() && IsStartMenuTouchedDownOnIndex(index);
        //}

        ///// <summary>
        /////     The IsStartMenuPressedUpOnIndex method is used to determine if the controller button has just been released.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been released.</returns>
        //public bool IsStartMenuPressedUpOnIndex(uint index)
        //{
        //    // button presses shall be ignored if the hair touch or touch modifiers are used
        //    return !IsButtonPressIgnored() && IsStartMenuTouchedUpOnIndex(index);
        //}

        ///// <summary>
        /////     The IsStartMenuTouchedOnIndex method is used to determine if the controller button is being touched down
        /////     continually.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button is continually being touched.</returns>
        //public bool IsStartMenuTouchedOnIndex(uint index)
        //{
        //    return IsButtonPressed(index, ButtonPressTypes.Press, PlayStationKeys.Start);
        //}

        ///// <summary>
        /////     The IsStartMenuTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been touched down.</returns>
        //public bool IsStartMenuTouchedDownOnIndex(uint index)
        //{
        //    return IsButtonPressed(index, ButtonPressTypes.PressDown,
        //                           PlayStationKeys.Start);
        //}

        ///// <summary>
        /////     The IsStartMenuTouchedUpOnIndex method is used to determine if the controller button has just been released.
        ///// </summary>
        ///// <param name="index">The index of the tracked object to check for.</param>
        ///// <returns>Returns true if the button has just been released.</returns>
        //public bool IsStartMenuTouchedUpOnIndex(uint index)
        //{
        //    return IsButtonPressed(index, ButtonPressTypes.PressUp,
        //                           PlayStationKeys.Start);
        //}



        ///// <summary>
        /////     whether or not the touch modifier is currently pressed
        /////     if so, pressing a key on the keyboard will only emit touch events,
        /////     but not a real press (or hair touch events).
        ///// </summary>
        ///// <returns>whether or not the TouchModifier is active</returns>
        //protected bool IsTouchModifierPressed()
        //{
        //    return Input.GetKey(
        //               rightController.GetControllerKey(PlayStationKeys.Triangle))
        //           || Input.GetKey(
        //               leftController.GetControllerKey(PlayStationKeys.Triangle));
        //}

        ///// <summary>
        /////     whether or not the hair touch modifier is currently pressed
        /////     if so, pressing a key on the keyboard will only emit touch and hair touch events,
        /////     but not a real press.
        ///// </summary>
        ///// <returns>whether or not the HairTouchModifier is active</returns>
        //protected bool IsHairTouchModifierPressed()
        //{
        //    return Input.GetKey(rightController.GetControllerKey(PlayStationKeys.Square))
        //           || Input.GetKey(
        //               leftController.GetControllerKey(PlayStationKeys.Square));
        //}

        ///// <summary>
        /////     whether or not a button press shall be ignored, e.g. because of the
        /////     use of the touch or hair touch modifier
        ///// </summary>
        ///// <returns>Returns true if the button press is ignored.</returns>
        //protected bool IsButtonPressIgnored()
        //{
        //    // button presses shall be ignored if the hair touch or touch modifiers are used
        //    return IsHairTouchModifierPressed() || IsTouchModifierPressed();
        //}

        ///// <summary>
        /////     whether or not a button press shall be ignored, e.g. because of the
        /////     use of the touch or hair touch modifier
        ///// </summary>
        ///// <returns>Returns true if the hair trigger touch should be ignored.</returns>
        //protected bool IsButtonHairTouchIgnored()
        //{
        //    // button presses shall be ignored if the hair touch or touch modifiers are used
        //    return IsTouchModifierPressed() && !IsHairTouchModifierPressed();
        //}

        //private bool TouchPadSimulator(uint index, ButtonPressTypes type)
        //{
        //    bool activator = IsButtonPressed(index, ButtonPressTypes.Press, TouchPadSimulationActivator);

        //    if (Sticky && IsButtonPressed(index, ButtonPressTypes.PressDown, TouchPadSimulationActivator))
        //    {
        //        stick = !stick;
        //        activator = stick;
        //    }


        //    bool touchPadSimulator = IsButtonPressed(index, type, TouchPadSimulationKey);

        //    return activator && touchPadSimulator;
        //}

        /// <summary>
        ///     checks if the given button (KeyCode) is currently in a specific pressed state (ButtonPressTypes) on the keyboard
        ///     also asserts that button presses are only handled for the currently active controller by comparing the controller
        ///     indices
        /// </summary>
        /// <param name="index">unique index of the controller for which the button press is to be checked</param>
        /// <param name="type">the type of press (up, down, hold)</param>
        /// <param name="playStationKey">the button on the Move Controller</param>
        /// <returns>Returns true if the button is being pressed.</returns>
        private bool IsButtonPressed(uint index, ButtonPressTypes type, PlayStationKeys playStationKey)
        {
            if (index >= uint.MaxValue)
            {
                return false;
            }
            KeyCode button;
            if (index == 1 || index == 0)
            {
                if (!rightController.ActiveController)
                {
                    return false;
                }
                button = rightController.GetControllerKey(playStationKey);
            }
            else if (index == 2)
            {
                if (!leftController.ActiveController)
                {
                    return false;
                }
                button = leftController.GetControllerKey(playStationKey);
            }
            else
            {
                return false;
            }


            switch (type)
            {
                case ButtonPressTypes.Press:
                    return Input.GetKey(button);
                case ButtonPressTypes.PressDown:
                    return Input.GetKeyDown(button);
                case ButtonPressTypes.PressUp:
                    return Input.GetKeyUp(button);
            }
            return false;
        }

        internal void OnEnable()
        {
            var simPlayer = SDK_PlayStationVRInput.FindInScene();
            if (simPlayer == null)
            {
                return;
            }
            var cons = simPlayer.GetComponentsInChildren<SDK_PlayStationMoveController>();
            var controllers = new List<SDK_PlayStationMoveController>(cons);
            rightController = controllers.Find(e => e.ControllerType == SDK_PlayStationMoveController.Controller.Primary);
            leftController = controllers.Find(e => e.ControllerType == SDK_PlayStationMoveController.Controller.Secondary);
        }
    }
}