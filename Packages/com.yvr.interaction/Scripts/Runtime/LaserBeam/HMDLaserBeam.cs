using UnityEngine;
using UnityEngine.EventSystems;
using YVR.Utilities;

namespace YVR.Interaction.Runtime
{
    public class HMDLaserBeam : LaserBeamBase
    {
        protected override void UpdateEffect()
        {
            rayInteractor.TryGetCurrentUIRaycastResult(out RaycastResult raycastResult);
            cursor.SetActive(raycastResult.gameObject);
            if (raycastResult.gameObject)
            {
                cursor.UpdateEffect(rayInteractor);
            }
        }
    }
}