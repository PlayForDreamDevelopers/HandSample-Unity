using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using YVR.Core;
using YVR.Utilities;

namespace YVR.Interaction.Runtime
{
    public class HandLaserBeam : LaserBeamBase
    {
        public InputActionProperty aimState;
        protected override void UpdateEffect()
        {
            if (InputModalityManager.instance.currentInputMode != InputMode.HandTracking || rayInteractor.IsBlockedByInteractionWithinGroup())
            {
                cursor.SetActive(false);
                return;
            }

            rayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult raycastResult);
            bool aimValid = ((HandStatus)aimState.action.ReadValue<int>() & HandStatus.InputStateValid) != 0;
            cursor.SetActive(aimValid && raycastResult.gameObject);
            if (aimValid && raycastResult.gameObject)
            {
                cursor.UpdateEffect(rayInteractor);
            }
        }
    }
}