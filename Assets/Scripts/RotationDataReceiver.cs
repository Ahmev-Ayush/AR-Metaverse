using UnityEngine;
using Unity.RenderStreaming;
using System.Text;

[RequireComponent(typeof(SingleConnection))]
public class RotationDataReceiver : DataChannelBase
{
    [Header("Target Camera Object")]
    public Transform stereoCameraTarget;

    [Header("Calibration Settings")]
    [Tooltip("Adjust the starting direction.")]  //  For horizontal phone, try setting X to 90 or -90.
    public Vector3 initialRotationOffset = new Vector3(90, 0, 0); // default is 90 on X to align with typical phone orientation, but adjust as needed for your setup.
    [Tooltip("Check this if Up/Down rotation is inverted.")]
    public bool invertPitch = true;
    [Tooltip("Check this if Left/Right rotation is inverted.")]
    public bool invertYaw = true;
    [Tooltip("Check this if Tilt/Roll is inverted.")]
    public bool invertRoll = false;

    [Header("Connection Settings")]
    public string connectionId = "InputStream";

    public SingleConnection _singleConnection;

    private Quaternion _newRot = Quaternion.identity;
    private Quaternion _lastLogRot = Quaternion.identity;
    private float _nextLogTime;

  

    void Update()
    {
        if (!IsConnected)
        {
            if (Time.time > _nextLogTime)
            {
                Debug.Log("[RotationDataReceiver] Waiting for DataChannel to connect...");
                _nextLogTime = Time.time + 2f;
            }
            return;
        }

        // Apply it safely to the camera
        if (Mathf.Abs(_newRot.x) > 0.01f || Mathf.Abs(_newRot.y) > 0.01f || Mathf.Abs(_newRot.z) > 0.01f || Mathf.Abs(_newRot.w) > 0.01f)
        {
            // Apply the initial rotation offset over the received rotation
            Quaternion finalRotation = Quaternion.Euler(initialRotationOffset) * _newRot;

            if (stereoCameraTarget != null)
            {
                stereoCameraTarget.localRotation = finalRotation;
            }
            else
            {
                transform.localRotation = finalRotation;
            }
        }
    }

    // Called automatically by Unity Render Streaming when the string message arrives
    protected override void OnMessage(byte[] bytes)
    {
        base.OnMessage(bytes);
        
        string msg = Encoding.UTF8.GetString(bytes);

        // Deserializing the string back into a Quaternion
        string[] parts = msg.Split(',');
        if (parts.Length == 4)
        {
            if (float.TryParse(parts[0], out float x) &&
                float.TryParse(parts[1], out float y) &&
                float.TryParse(parts[2], out float z) &&
                float.TryParse(parts[3], out float w))
            {
                Quaternion rawRot = new Quaternion(-x, -y, -z, -w);
                Vector3 euler = rawRot.eulerAngles;

                // Normalize angles to -180 to 180 for reliable axis inversion
                float pitch = euler.x;
                float yaw = euler.y;
                float roll = euler.z;

                if (pitch > 180f) pitch -= 360f;
                if (yaw > 180f) yaw -= 360f;
                if (roll > 180f) roll -= 360f;

                // Invert specific axes based on inspector settings
                if (invertPitch) pitch = -pitch;
                if (invertYaw) yaw = -yaw;
                if (invertRoll) roll = -roll;

                _newRot = Quaternion.Euler(pitch, yaw, roll);
            }
        }
    }

    // Called when the DataChannel is opened
    protected override void OnOpen(string connectionId)
    {
        base.OnOpen(connectionId);
        Debug.Log($"[RotationDataReceiver] WebRTC DataChannel Opened for {connectionId}");
    }
}