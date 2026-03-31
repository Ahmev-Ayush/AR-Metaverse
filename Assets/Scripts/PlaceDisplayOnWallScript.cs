using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class PlaceDisplayOnWallScript : MonoBehaviour
{
    [SerializeField] private GameObject targetQuad; // Assign the existing Quad from your scene here
    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private bool isPlaced = false;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>(); // Reusable list to avoid allocations

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Awake()
    {
        // Use FindAnyObjectByType so it finds them wherever they are in the scene (usually on XR Origin)
        raycastManager = FindAnyObjectByType<ARRaycastManager>();
        planeManager   = FindAnyObjectByType<ARPlaneManager>();
        
        if (raycastManager == null || planeManager == null)
        {
            Debug.LogError("Could not find ARRaycastManager or ARPlaneManager in the scene! Ensure they are on your XR Origin.");
        }
    }

    void Update()
    {
        // 1. If already placed, do nothing (stops further raycasts)
        if (isPlaced) return;

        // 2. Check for touch input 
        if (Touch.activeTouches.Count > 0 && Touch.activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            Vector2 touchPosition = Touch.activeTouches[0].screenPosition;

            // 3. Shoot Raycast looking specifically for Planes
            if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                // Get the hit point
                var hitPose = hits[0].pose;               // first hit is the closest one hit[0]
                var hitTrackableId = hits[0].trackableId; // get the trackable ID of the hit plane
                var plane = planeManager.GetPlane(hitTrackableId); // retrieve the ARPlane using the trackable ID

                // 4. Check if the plane is Vertical
                if (plane != null && plane.alignment == PlaneAlignment.Vertical)
                {
                    PlaceQuad(hitPose);
                }
            }
        }
        // mouse input for editor testing as per new input system
#if UNITY_EDITOR
        else if (UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = UnityEngine.InputSystem.Mouse.current.position.ReadValue();

            // Shoot Raycast looking for Planes
            if (raycastManager.Raycast(mousePosition, hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = hits[0].pose;
                var hitTrackableId = hits[0].trackableId;
                var plane = planeManager.GetPlane(hitTrackableId);

                // Check if plane is not null and is Vertical
                if (plane != null && plane.alignment == PlaneAlignment.Vertical)
                {
                    PlaceQuad(hitPose);
                }
            }
        }
#endif
    }

    void PlaceQuad(Pose hitPose)
    {
        // Move and activate the existing Quad
        targetQuad.transform.position = hitPose.position;

        // Use the wall's normal (hitPose.up) to orient the Quad.
        // We use Quaternion.LookRotation to ensure its 'up' vector remains perfectly parallel to the world's gravity (Vector3.up).
        // A Unity default Quad faces the -Z axis. Pointing its +Z axis INTO the wall (-hitPose.up)
        // makes the viewable side face outward comfortably towards the user, removing any tilted angle.
        targetQuad.transform.rotation = Quaternion.LookRotation(-hitPose.up, Vector3.up);
        
        // Depending on your prefab, you may need a slight adjustment so it doesn't z-fight with the wall:
        // targetQuad.transform.position += hitPose.up * 0.01f;

        targetQuad.SetActive(true);

        isPlaced = true;

        // 5. Turn off AR Plane Manager and hide existing planes
        StopTrackingPlanes();
    }

    void StopTrackingPlanes()
    {
        // Disable the AR Plane Manager to stop detecting new planes
        planeManager.enabled = false; 

        // Hide all planes currently visible in the scene
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
        
        Debug.Log("Object placed. Plane tracking disabled.");
    }
}