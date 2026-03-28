using Unity.RenderStreaming;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;


public class ManualStart : MonoBehaviour
{
    [SerializeField] private SignalingManager signalingManager;
    [SerializeField] private bool useEnhancedTouchInput = true;

    private bool hasWarned;
    private bool ownsEnhancedTouchSupport;
    private bool ownsTouchSimulation;

    private void OnEnable()
    {
        if (useEnhancedTouchInput && !EnhancedTouchSupport.enabled)
        {
            EnhancedTouchSupport.Enable();
            ownsEnhancedTouchSupport = true;
        }
    }

    private void OnDisable()
    {
        if (ownsEnhancedTouchSupport && EnhancedTouchSupport.enabled)
        {
            EnhancedTouchSupport.Disable();
            ownsEnhancedTouchSupport = false;
        }
    }

    private void Update()
    {
        bool keyboardPressed = false;
        
#if UNITY_EDITOR || UNITY_STANDALONE
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            keyboardPressed = keyboard.spaceKey.wasPressedThisFrame;
        }
#endif

        if (keyboardPressed || DidTouchBeginThisFrame())
        {
            TriggerSignaling();
        }
    }

    private void TriggerSignaling()
    {
        if (signalingManager == null)
        {
            Debug.LogWarning("ManualStart has no SignalingManager assigned.");
            return;
        }

        if (signalingManager.Running)
        {
            return;
        }

        signalingManager.Run();
        Debug.Log("Signaling Started!");
    }

    private bool DidTouchBeginThisFrame()
    {
        if (useEnhancedTouchInput && EnhancedTouchSupport.enabled)
        {
            var touches = Touch.activeTouches;
            for (int i = 0; i < touches.Count; i++)
            {
                if (touches[i].phase == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    return true;
                }
            }
        }

        var touchscreen = Touchscreen.current;
        if (touchscreen != null && touchscreen.primaryTouch.press.wasPressedThisFrame)
        {
            return true;
        }
        
#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == UnityEngine.TouchPhase.Began)
        {
            return true;
        }
#endif

        return false;
    }
}