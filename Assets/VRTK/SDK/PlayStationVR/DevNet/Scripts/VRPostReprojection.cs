using UnityEngine;
#if UNITY_PS4
using UnityEngine.PS4.VR;
#endif

public class VRPostReprojection : MonoBehaviour
{
    public GameObject busySpinner;

#if UNITY_PS4
    private int currentEye = 0;
    private RenderTexture postReprojectionTexture;
    private Camera cam;

    void Update()
    {
        // Reset which eye we're adjusting at the start of every frame
        currentEye = 0;
	}

    void OnPostRender()
    {
        if (cam == null)
            cam = GetComponent<Camera>();

        if (UnityEngine.VR.VRSettings.loadedDeviceName == "PlayStationVR")
        {
            if (PlayStationVRSettings.postReprojectionType == PlayStationVRPostReprojectionType.None)
            {
                // If post-reprojection isn't supported (either because it wasn't turned on, or else we're in
                // Deferred) then disable this script and re-parent the reticle to the main camera instead     
                gameObject.SetActive(false);
                busySpinner.SetActive(false);
            }
            else
            {
                if (currentEye == 0)
                    postReprojectionTexture = PlayStationVR.GetCurrentFramePostReprojectionEyeTexture(UnityEngine.VR.VRNode.LeftEye);
                else if (currentEye == 1)
                    postReprojectionTexture = PlayStationVR.GetCurrentFramePostReprojectionEyeTexture(UnityEngine.VR.VRNode.RightEye);

#if UNITY_5_6_OR_NEWER
				Graphics.Blit(RenderTexture.active, postReprojectionTexture);
#else
                Graphics.Blit(cam.targetTexture, postReprojectionTexture);
#endif
                currentEye++;
            }
        }
    }
#endif
}
