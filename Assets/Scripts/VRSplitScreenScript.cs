using UnityEngine;

public class VRSplitScreen : MonoBehaviour
{
    [Tooltip("Drag your LeftEyeCamera (the one controlled by AR Foundation) here.")]
    public Transform leftEyeCamera;
    
    [Tooltip("Average human eye distance is 0.064 meters.")]
    public float ipd = 0.064f; 

    void LateUpdate()
    {
        if (leftEyeCamera != null)
        {
            // Match the rotation of the AR tracked camera
            transform.rotation = leftEyeCamera.rotation;
            
            // Match the position, but shift it to the right by the IPD distance
            transform.position = leftEyeCamera.position + (leftEyeCamera.right * ipd);
        }
    }
}