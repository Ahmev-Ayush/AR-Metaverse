using UnityEngine;
using UnityEngine.InputSystem;
using Unity.RenderStreaming;

[RequireComponent(typeof(SingleConnection))]
public class RotationDataSender : DataChannelBase
{
    [Header("Connection Settings")]
    [Tooltip("The Connection ID to send data to")]
    public string connectionId = "InputStream";

    public SingleConnection _singleConnection;
    private float _nextLogTime;

    void Start()
    {
        // _singleConnection = GetComponent<SingleConnection>();
        if (_singleConnection != null && !string.IsNullOrEmpty(connectionId))
        {
            _singleConnection.CreateConnection(connectionId);
            Debug.Log($"[RotationDataSender] Requested connectionId: {connectionId}");
        }

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
        if (AttitudeSensor.current == null || !AttitudeSensor.current.enabled) return;

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

        // Read the Quaternion directly from the valid attitude sensor
        Quaternion q = AttitudeSensor.current.attitude.ReadValue();

        // Serialize the 4 floats into a compact comma-separated string
        string msg = $"{q.x:F5},{q.y:F5},{q.z:F5},{q.w:F5}";
        
        // Send the string over the WebRTC DataChannel
        Send(msg);
    }

    // When the channel connects, we get this callback natively.
    protected override void OnOpen(string connectionId)
    {
        base.OnOpen(connectionId);
        Debug.Log($"[RotationDataSender] WebRTC DataChannel Opened for {connectionId}");
    }
}