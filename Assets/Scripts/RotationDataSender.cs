using UnityEngine;
using UnityEngine.InputSystem;
using Unity.RenderStreaming;

[RequireComponent(typeof(SingleConnection))]
public class RotationDataSender : DataChannelBase
{
    private float _nextLogTime;

    void Start()
    {
        if (AttitudeSensor.current != null)
        {
            InputSystem.EnableDevice(AttitudeSensor.current);
            Debug.Log("[RotationDataSender] AttitudeSensor enabled.");
        }
        else
        {
            Debug.LogWarning("[RotationDataSender] No AttitudeSensor found!");
        }

        if (UnityEngine.InputSystem.Gyroscope.current != null)
            InputSystem.EnableDevice(UnityEngine.InputSystem.Gyroscope.current);
    }

    void Update()
    {
        if (AttitudeSensor.current == null) return;

        // Ensure the WebRTC channel is actually open to send data
        if (!IsConnected)
        {
            if (Time.time > _nextLogTime)
            {
                Debug.Log("[RotationDataSender] Waiting for DataChannel to connect...");
                _nextLogTime = Time.time + 2f;
            }
            return;
        }

        Quaternion q = AttitudeSensor.current.attitude.ReadValue();

        if (Time.time > _nextLogTime)
        {
            Debug.Log($"[RotationDataSender] Sending over WebRTC DataChannel: {q}");
            _nextLogTime = Time.time + 2f;
        }

        // Just serialize the 4 floats as a small string
        string msg = $"{q.x:F5},{q.y:F5},{q.z:F5},{q.w:F5}";
        Send(msg);
    }

    // When the channel connects, we get this callback natively.
    protected override void OnOpen(string connectionId)
    {
        base.OnOpen(connectionId);
        Debug.Log($"[RotationDataSender] WebRTC DataChannel Opened for {connectionId}");
    }
}