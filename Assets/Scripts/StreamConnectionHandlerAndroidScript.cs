using UnityEngine;
using Unity.RenderStreaming;
using UnityEngine.UI;

/// <summary>
/// In newer versions of Unity Render Streaming, handshaking (WebRTC Offers/Answers)
/// is automatically handled by the `SingleConnection` or `Broadcast` components.
/// 
/// Below is how you receive the actual Video Texture and apply it.
/// </summary>
public class StreamConnectionHandlerAndroidScript : MonoBehaviour
{
    [Tooltip("Drag the Video Stream Receiver component here")]
    public VideoStreamReceiver videoReceiver;

    [Tooltip("Drag the MeshRenderer (e.g., Quad) that will display the video")]
    public RawImage targetRawImage;

    [Tooltip("Drag the SingleConnection component here")]
    public SingleConnection singleConnection;

    [Tooltip("The Connection ID to receive from")]
    public string connectionId = "windowsStream";

    void Start()
    {
        if (singleConnection != null && !string.IsNullOrEmpty(connectionId))
        {
            singleConnection.CreateConnection(connectionId);
            Debug.Log($"[StreamConnectionHandler] Requested connectionId: {connectionId}");
        }

        if (videoReceiver != null)
        {
            // Subscribe to the event when the video texture updates
            videoReceiver.OnUpdateReceiveTexture += OnUpdateReceiveTexture;
        }
    }

    private bool streamReceivedLogged = false;

    private void OnUpdateReceiveTexture(Texture receiveTexture)
    {
        if (!streamReceivedLogged && receiveTexture != null)
        {
            // Debug.Log($"[StreamConnectionHandler] Stream successfully received! Texture Size: {receiveTexture.width}x{receiveTexture.height}");
            streamReceivedLogged = true;
        }

        // Apply the received video texture to the material of the target renderer
        if (targetRawImage != null && receiveTexture != null)
        {
            targetRawImage.texture = receiveTexture;
        }
    }

    void OnDestroy()
    {
        if (videoReceiver != null)
        {
            // Always unsubscribe to prevent memory leaks when the behavior is destroyed
            videoReceiver.OnUpdateReceiveTexture -= OnUpdateReceiveTexture;
        }
    }
}