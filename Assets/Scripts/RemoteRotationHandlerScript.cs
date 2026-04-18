using UnityEngine;
using UnityEngine.InputSystem;

public class RemoteRotationHandlerScript : MonoBehaviour
{
    // Drag the Quaternion-producing Input Action from your Input Actions asset here.
    public InputActionReference rotationAction;

    // Cached action reference to avoid repeated property lookups and simplify unsubscribe.
    InputAction _rotationInput;

    void OnEnable()
    {
        // Resolve the runtime action from the serialized action reference.
        _rotationInput = rotationAction?.action;
        if (_rotationInput == null)
            return;

        // Event-driven input: update rotation only when input changes.
        _rotationInput.performed += OnRotationChanged;
        _rotationInput.canceled += OnRotationChanged;
        _rotationInput.Enable();
    }

    void OnDisable()
    {
        if (_rotationInput == null)
            return;

        // Always unsubscribe to prevent duplicate callbacks or leaks after re-enable.
        _rotationInput.performed -= OnRotationChanged;
        _rotationInput.canceled -= OnRotationChanged;
        _rotationInput.Disable();
    }

    void OnRotationChanged(InputAction.CallbackContext context)
    {
        // Apply incoming remote/device quaternion directly to this transform.
        transform.localRotation = context.ReadValue<Quaternion>();
    }

    /*
    Legacy polling approach (uses more CPU because it runs every frame):

    void Update()
    {
        if (rotationAction?.action == null)
            return;

        Quaternion remoteRot = rotationAction.action.ReadValue<Quaternion>();
        transform.localRotation = remoteRot;
    }

    OR

    void Update()
    {
        // Read the Quaternion rotation sent from the Android phone
        Quaternion remoteRot = rotationAction.action.ReadValue<Quaternion>();

        if (remoteRot != Quaternion.identity)
        {
            // Apply it to the camera
            transform.localRotation = remoteRot;
        }
    }
    */
}