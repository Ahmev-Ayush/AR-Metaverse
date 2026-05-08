using UnityEngine;
using UnityEngine.InputSystem;

public class RemoteSensorEnablerScript : MonoBehaviour
{
    void OnEnable()
    {
        // Listen for when Render Streaming's InputReceiver adds network devices
        InputSystem.onDeviceChange += OnDeviceChange;
        
        // Just in case the device was added right before this script ran
        foreach (var device in InputSystem.devices)
        {
            if (device is AttitudeSensor && device.remote)
            {
                InputSystem.EnableDevice(device);
                Debug.Log($"[RemoteSensorEnablerScript] Enabled existing remote {device.name}");
            }
        }
    }

    void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        // As soon as the Android App connects and sends its AttitudeSensor layout,
        // enable it so the Windows Input System processes the incoming gyroscope events
        if (change == InputDeviceChange.Added && device is AttitudeSensor)
        {
            Debug.Log($"[RemoteSensorEnablerScript] Remote {device.name} network device added! Enabling...");
            InputSystem.EnableDevice(device);
        }
    }
}