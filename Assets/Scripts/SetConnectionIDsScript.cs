using UnityEngine;
using Unity.RenderStreaming;

public class SetConnectionIDs : MonoBehaviour
{
    [Header("PC to Android Settings")]
    public SignalingHandlerBase androidConnectionHandler;
    public string androidConnectionId = "windowsStream";

    [Header("Browser to PC Settings")]
    public SignalingHandlerBase browserConnectionHandler;
    public string browserConnectionId = "ScreenStream";

    void Start()
    {
        if (androidConnectionHandler == null || browserConnectionHandler == null)
        {
            Debug.LogError("[SetConnectionIDs] Handlers are missing! Please assign Android (SingleConnection) and Browser (SingleConnection) in the Inspector.");
            return;
        }

        // Connection separation is done at the signaling handler level.
        CreateConnectionIfAssigned(androidConnectionHandler, androidConnectionId, "Android");
        CreateConnectionIfAssigned(browserConnectionHandler, browserConnectionId, "Browser");

        Debug.Log($"Connection IDs Set: Android={androidConnectionId}, Browser={browserConnectionId}");
    }

    static void CreateConnectionIfAssigned(SignalingHandlerBase handler, string connectionId, string label)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
        {
            Debug.LogWarning($"[SetConnectionIDs] {label} connection ID is empty.");
            return;
        }

        // Both Android and Browser handlers need to explicitly request their connection IDs 
        // from the signaling server, otherwise the server won't know which peer is which.
        // Worked.....but it was a bit of a mystery why the connection IDs needed to be set on both sides.
        handler.CreateConnection(connectionId);
        Debug.Log($"[SetConnectionIDs] {label} is initiating connection to: {connectionId}");
    }
}
