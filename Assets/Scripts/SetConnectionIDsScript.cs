using UnityEngine;
using Unity.RenderStreaming;

public class SetConnectionIDs : MonoBehaviour
{
    // [Header("PC to Android Settings")]
    // public SignalingHandlerBase androidConnectionHandler;
    // public string androidConnectionId = "android_vr";

    [Header("Browser to PC Settings")]
    public SignalingHandlerBase browserConnectionHandler;
    public string browserConnectionId = "ScreenStream";

    void Start()
    {
        if (/*androidConnectionHandler == null || */ browserConnectionHandler == null)
        {
            Debug.LogError("[SetConnectionIDs] Handlers are missing! Please assign Android (SingleConnection) and Browser (SingleConnection) in the Inspector.");
            return;
        }

        // Connection separation is done at the signaling handler level.
        // CreateConnectionIfAssigned(androidConnectionHandler, androidConnectionId, "Android");
        CreateConnectionIfAssigned(browserConnectionHandler, browserConnectionId, "Browser");

        // Debug.Log($"Connection IDs Set: Android={androidConnectionId}, Browser={browserConnectionId}");
    }

    static void CreateConnectionIfAssigned(SignalingHandlerBase handler, string connectionId, string label)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
        {
            Debug.LogWarning($"[SetConnectionIDs] {label} connection ID is empty.");
            return;
        }

        // If it's the browser, let Unity initiate the connection to fixing the blank "to" ID issue.
        if (label == "Browser")
        {
            handler.CreateConnection(connectionId);
            Debug.Log($"[SetConnectionIDs] {label} is initiating connection to: {connectionId}");
        }
        else
        {
            Debug.Log($"[SetConnectionIDs] {label} is ready and waiting for incoming connection: {connectionId}");
        }
    }
}
