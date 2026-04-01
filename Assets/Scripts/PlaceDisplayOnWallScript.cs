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
    // private bool isPlaced = false;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>(); // Reusable list to avoid allocations

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        Touch.onFingerDown += OnFingerDown; // Subscribe to finger down events
    }

    void OnDisable()
    {
        Touch.onFingerDown -= OnFingerDown; // Unsubscribe from finger down events
        EnhancedTouchSupport.Disable();
    }

    void Awake()
    {
        // Use FindAnyObjectByType so it finds them wherever they are in the scene (usually on XR Origin)
        raycastManager = FindAnyObjectByType<ARRaycastManager>();
        planeManager   = FindAnyObjectByType<ARPlaneManager>();
        
        // if (raycastManager == null || planeManager == null)
        // {
        //     // Debug.LogError("Could not find ARRaycastManager or ARPlaneManager in the scene! Ensure they are on your XR Origin.");
        // }
    }

    /* // Avoid using Update and raycasting every frame for better performance. Instead, we react to touch events directly.
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

    */

    private void OnFingerDown(Finger finger)
    {
        PerformRaycast(finger.currentTouch.screenPosition);
    }

#if UNITY_EDITOR
    void Update()
    {
        if (UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
        {
            PerformRaycast(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
        }
    }
#endif

    private void PerformRaycast(Vector2 screenPosition)
    {
        if(raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            var hitTrackableId = hits[0].trackableId;
            var plane = planeManager.GetPlane(hitTrackableId);

            if (plane != null && plane.alignment == PlaneAlignment.Vertical)
            {
                PlaceQuad(hitPose, plane);
            }
        }
    }

    void PlaceQuad(Pose hitPose, ARPlane selectedPlane)
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

        // isPlaced = true;

        // 5. Turn off AR Plane Manager and delete existing unused planes
        StopTrackingPlanes(selectedPlane);

        // turn off update of this script
        // this.enabled = false; // disable this script to stop further raycasts and updates after placement
    }

    void StopTrackingPlanes(ARPlane selectedPlane)
    {
        // Disable the AR Plane Manager to stop detecting new planes
        planeManager.enabled = false; 

        // Delete all planes currently visible in the scene that are not the selected one
        foreach (var plane in planeManager.trackables)
        {
            if (plane != selectedPlane)
            {
                Destroy(plane.gameObject);
            }
        }

        // foreach (var plane in planeManager.trackables)
        // {
        //     plane.gameObject.SetActive(false); // hide the selected plane as well to avoid visual clutter
        // }

        selectedPlane.gameObject.SetActive(false); // hide the selected plane as well to avoid visual clutter

        raycastManager.enabled = false; // disable raycasting if you no longer need it after placement (optimization purpose)
        
        // Debug.Log("Object placed. Plane tracking disabled.");
    }
}