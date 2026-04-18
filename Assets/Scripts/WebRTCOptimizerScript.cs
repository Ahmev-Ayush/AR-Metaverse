using UnityEngine;
using Unity.RenderStreaming;

public class WebRTCOptimizerScript : MonoBehaviour
{
    [Header("Bitrate (kbps)")]
    [Tooltip("Applied as both min and max bitrate to lock stream at this target value.")]
    [Min(1)]
    public uint targetBitrateKbps = 15000;

    [Header("Optional")]
    [Tooltip("Assign specific senders, or leave empty to auto-find all VideoStreamSender components in the scene.")]
    [SerializeField] private VideoStreamSender[] videoSenders;

    void Start()
    {
        // Newer Render Streaming versions do not expose OnCreateSessionDescription on SignalingManager.
        // Configure bitrate directly on VideoStreamSender, which updates current and future transceivers.
        // if (videoSenders == null || videoSenders.Length == 0)
        // {
        //     videoSenders = FindObjectsByType<VideoStreamSender>(FindObjectsSortMode.None);
        // }

        if (videoSenders == null || videoSenders.Length == 0)
        {
            Debug.LogWarning("[WebRTCOptimizerScript] No VideoStreamSender found. Assign sender(s) in Inspector or add VideoStreamSender to the scene.");
            return;
        }

        for (int i = 0; i < videoSenders.Length; i++)
        {
            var sender = videoSenders[i];
            if (sender == null)
                continue;

            sender.SetBitrate(targetBitrateKbps, targetBitrateKbps);
        }

        Debug.Log("[WebRTCOptimizerScript] Applied target bitrate: " + targetBitrateKbps + " kbps");
    }
}