using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace YVR.Interaction.Runtime
{
    public class EyeGazeInput : MonoBehaviour
    {
        public Transform eyeRayOriginTransform;
        public float dragSensitivity = 0.25f;
        public Vector3 pinchingHandPosition { get; set; }
        public InteractorHandedness dragInputHand { get; set; }
        private bool m_WasPressed = default;
        private int m_PressedDeviceId;
        private YVRInputActions m_YvrInputActions;
        private Vector3 m_EyeRayOriginPosition;
        private Quaternion m_EyeRayQuaternion;
        private Vector3 m_StartEyeRayForward;
        private Vector3 m_StartPinchEyePosition;
        private Vector3 m_StartPinchHandPosition;

        void Start()
        {
            m_YvrInputActions = new YVRInputActions();
            m_YvrInputActions.asset.Enable();
            if (eyeRayOriginTransform == null)
            {
                eyeRayOriginTransform = GetComponent<XRGazeInteractor>().transform;
            }

            //m_YvrInputActions.YVRLeft.IndexPressed.started += (context) =>
            //{
            //    if (!m_WasPressed)
            //    {
            //        dragInputHand = InteractorHandedness.Left;
            //        m_StartPinchHandPosition = m_YvrInputActions.YVRLeft.AimPosition.ReadValue<Vector3>();
            //        pinchingHandPosition = m_StartPinchHandPosition;
            //    }
            //    PressedStartedCallback(context);
            //};
            //m_YvrInputActions.YVRLeft.IndexPressed.canceled += PressedCanceledCallback;
            m_YvrInputActions.YVRRight.IndexPressed.started += (context) =>
            {
                if (!m_WasPressed)
                {
                    dragInputHand = InteractorHandedness.Right;
                    m_StartPinchHandPosition = m_YvrInputActions.YVRRight.AimPosition.ReadValue<Vector3>();
                    pinchingHandPosition = m_StartPinchHandPosition;
                }
                PressedStartedCallback(context);
            };
            m_YvrInputActions.YVRRight.IndexPressed.canceled += PressedCanceledCallback;
        }

        private void PressedStartedCallback(InputAction.CallbackContext context)
        {
            if (m_WasPressed) return;

            m_PressedDeviceId = context.control.device.deviceId;
            m_StartPinchEyePosition = m_YvrInputActions.YVREye.devicePosition.ReadValue<Vector3>();
            m_StartEyeRayForward = eyeRayOriginTransform.transform.forward;
            m_WasPressed = true;
        }

        private void PressedCanceledCallback(InputAction.CallbackContext context)
        {
            if (m_PressedDeviceId != context.control.device.deviceId) return;
            m_WasPressed = false;
            dragInputHand = InteractorHandedness.None;
        }

        private void Update()
        {
            m_EyeRayOriginPosition = m_YvrInputActions.YVREye.devicePosition.ReadValue<Vector3>();
            m_EyeRayQuaternion = m_YvrInputActions.YVREye.deviceRotation.ReadValue<Quaternion>();
            switch (dragInputHand)
            {
                case InteractorHandedness.Left:
                    pinchingHandPosition = m_YvrInputActions.YVRLeft.AimPosition.ReadValue<Vector3>();
                    break;
                case InteractorHandedness.Right:
                    pinchingHandPosition = m_YvrInputActions.YVRRight.AimPosition.ReadValue<Vector3>();
                    break;
            }

            if (m_WasPressed)
            {
                eyeRayOriginTransform.position = m_StartPinchEyePosition;
                CalcPressRayOriginRotation();
            }
            else
            {
                eyeRayOriginTransform.position = m_EyeRayOriginPosition;
                eyeRayOriginTransform.rotation = m_EyeRayQuaternion;
            }
        }

        private void CalcPressRayOriginRotation()
        {
            Vector3 virtualPosition = m_StartPinchEyePosition + m_StartEyeRayForward * dragSensitivity;
            Vector3 offsetProjectOnPlanePinch = Vector3.ProjectOnPlane(pinchingHandPosition, (virtualPosition - m_StartPinchEyePosition).normalized);
            Vector3 offsetProjectOnPlaneStart = Vector3.ProjectOnPlane(m_StartPinchHandPosition, (virtualPosition - m_StartPinchEyePosition).normalized);
            Vector3 offsetProjectOnPlane = offsetProjectOnPlanePinch - offsetProjectOnPlaneStart;
            virtualPosition += offsetProjectOnPlane;
            eyeRayOriginTransform.LookAt(virtualPosition);
        }
    }
}