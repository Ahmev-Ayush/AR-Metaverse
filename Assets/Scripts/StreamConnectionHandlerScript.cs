using UnityEngine;
using Unity.RenderStreaming;

/// <summary>
/// In newer versions of Unity Render Streaming, handshaking (WebRTC Offers/Answers)
/// is automatically handled by the `SingleConnection` or `Broadcast` components.
/// 
/// Below is how you receive the actual Video Texture and apply it.
/// </summary>
public class StreamConnectionHandler : MonoBehaviour
{
    [Tooltip("Drag the Video Stream Receiver component here")]
    public VideoStreamReceiver videoReceiver;

    [Tooltip("Drag the MeshRenderer (e.g., Quad) that will display the video")]
    public MeshRenderer targetRenderer;

    [Tooltip("Print periodic health logs while waiting for first texture.")]
    public bool verboseLogs = true;

    [Tooltip("Seconds between waiting diagnostics before first frame arrives.")]
    public float healthLogIntervalSeconds = 5f;

    private float nextHealthLogAt;
    private float firstTextureReceivedAt = -1f;
    private int receivedTextureCount;
    private bool subscribed;
/*
    private void Awake()
    {
        if (verboseLogs)
        {
            // Debug.Log($"[StreamConnectionHandler] Awake. activeInHierarchy={gameObject.activeInHierarchy}, enabled={enabled}");
        }
    }

    private void OnEnable()
    {
        if (verboseLogs)
        {
            // Debug.Log("[StreamConnectionHandler] OnEnable.");
        }
    }

    void Start()
    {
        if (verboseLogs)
        {
            // Debug.Log($"[StreamConnectionHandler] Start on platform={Application.platform}. videoReceiver={(videoReceiver != null)}, targetRenderer={(targetRenderer != null)}");
        }

        if (videoReceiver != null)
        {
            // Subscribe to the event when the video texture updates
            videoReceiver.OnUpdateReceiveTexture += OnUpdateReceiveTexture;
            subscribed = true;
            if (verboseLogs)
            {
                // Debug.Log("[StreamConnectionHandler] Subscribed to OnUpdateReceiveTexture.");
            }
        }
        else
        {
            // Debug.LogWarning("[StreamConnectionHandler] VideoStreamReceiver is not assigned.");
        }

        if (targetRenderer == null)
        {
            // Debug.LogWarning("[StreamConnectionHandler] Target renderer is not assigned.");
        }
        else if (verboseLogs)
        {
            var hasMaterial = targetRenderer.sharedMaterial != null;
            // Debug.Log($"[StreamConnectionHandler] Target renderer '{targetRenderer.name}' active={targetRenderer.gameObject.activeInHierarchy}, hasMaterial={hasMaterial}");
        }

        nextHealthLogAt = Time.time + Mathf.Max(1f, healthLogIntervalSeconds);
    }
*/
    private bool streamReceivedLogged = false;

    private void Update()
    {
        if (!verboseLogs || streamReceivedLogged || Time.time < nextHealthLogAt)
        {
            return;
        }

        string receiverState = videoReceiver == null
            ? "null"
            : $"enabled={videoReceiver.enabled}, active={videoReceiver.gameObject.activeInHierarchy}";
        string rendererState = targetRenderer == null
            ? "null"
            : $"enabled={targetRenderer.enabled}, active={targetRenderer.gameObject.activeInHierarchy}";

        // Debug.Log($"[StreamConnectionHandler] Waiting for first video texture... receiver({receiverState}) renderer({rendererState}) subscribed={subscribed}");
        nextHealthLogAt = Time.time + Mathf.Max(1f, healthLogIntervalSeconds);
    }

    private void OnUpdateReceiveTexture(Texture receiveTexture)
    {
        receivedTextureCount++;

        if (receiveTexture == null)
        {
            if (verboseLogs)
            {
                // Debug.LogWarning("[StreamConnectionHandler] OnUpdateReceiveTexture invoked with null texture.");
            }
            return;
        }

        if (!streamReceivedLogged && receiveTexture != null)
        {
            // Debug.Log($"[StreamConnectionHandler] Stream successfully received! Texture Size: {receiveTexture.width}x{receiveTexture.height}");
            streamReceivedLogged = true;
            firstTextureReceivedAt = Time.time;
        }
        else if (verboseLogs)
        {
            // Debug.Log($"[StreamConnectionHandler] Texture update #{receivedTextureCount}: {receiveTexture.width}x{receiveTexture.height}");
        }

        // Apply the received video texture to the material of the target renderer
        if (targetRenderer != null && receiveTexture != null)
        {
            targetRenderer.material.mainTexture = receiveTexture;

            if (verboseLogs && firstTextureReceivedAt > 0f)
            {
                // Debug.Log($"[StreamConnectionHandler] Texture applied to renderer '{targetRenderer.name}'.");
                firstTextureReceivedAt = -1f;
            }
        }
        else if (verboseLogs)
        {
            // Debug.LogWarning("[StreamConnectionHandler] Received texture but targetRenderer is missing.");
        }
    }

    private void OnDisable()
    {
        if (verboseLogs)
        {
            // Debug.Log("[StreamConnectionHandler] OnDisable.");
        }
    }

    void OnDestroy()
    {
        if (verboseLogs)
        {
            // Debug.Log($"[StreamConnectionHandler] OnDestroy. receivedTextureCount={receivedTextureCount}, streamReceivedLogged={streamReceivedLogged}");
        }

        if (videoReceiver != null)
        {
            // Always unsubscribe to prevent memory leaks when the behavior is destroyed
            videoReceiver.OnUpdateReceiveTexture -= OnUpdateReceiveTexture;
            subscribed = false;
            if (verboseLogs)
            {
                // Debug.Log("[StreamConnectionHandler] Unsubscribed from OnUpdateReceiveTexture.");
            }
        }
    }
}