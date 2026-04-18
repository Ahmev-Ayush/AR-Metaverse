using UnityEngine;
using UnityEngine.InputSystem;

public class AndroidSensorActivatorScript : MonoBehaviour
{
    void Start()
    {
        // Enable the Gyroscope on the Android device
        if (AttitudeSensor.current != null)
            InputSystem.EnableDevice(AttitudeSensor.current);
            
        // Also enable normal Accelerometer just in case
        if (Accelerometer.current != null)
            InputSystem.EnableDevice(Accelerometer.current);
    }
}