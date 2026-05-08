using UnityEngine;
using Unity.RenderStreaming;
using System.Text;

[RequireComponent(typeof(SingleConnection))]
public class RotationDataReceiver : DataChannelBase
{
    [Header("Camera To Rotate")]
    public Transform stereoCameraTarget;

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
            if (stereoCameraTarget != null)
            {
                stereoCameraTarget.localRotation = _newRot;
            }
            else
            {
                transform.localRotation = _newRot;
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
                _newRot = new Quaternion(x, y, z, w);

                if (Quaternion.Angle(_newRot, _lastLogRot) > 0.5f && Time.time > _nextLogTime)
                {
                    Debug.Log($"[RotationDataReceiver] Received Quat: {_newRot} | Euler: {_newRot.eulerAngles}");
                    _lastLogRot = _newRot;
                    _nextLogTime = Time.time + 2f;
                }
            }
        }
    }

    protected override void OnOpen(string connectionId)
    {
        base.OnOpen(connectionId);
        Debug.Log($"[RotationDataReceiver] WebRTC DataChannel Opened for {connectionId}");
    }
}