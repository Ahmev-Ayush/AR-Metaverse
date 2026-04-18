using UnityEngine;
using Unity.RenderStreaming;
using Unity.WebRTC;

[RequireComponent(typeof(VideoStreamSender))]
public class highSpeedVideoStreamingScript : MonoBehaviour
{
    public VideoStreamSender videoStreamSender;

    // Minimum and Maximum Bitrate in Kbps
    public uint minBitrateKbps = 10000;  // 10 Mbps
    public uint maxBitrateKbps = 15000;  // 15 Mbps

    void Start()
    {
        if (videoStreamSender == null)
            videoStreamSender = GetComponent<VideoStreamSender>();
        
        // METHOD 1: The Native Unity WebRTC Way (Best Practice)
        // Unity's VideoStreamSender component actually has a built-in method to handle bitrate.
        // Doing this here guarantees that any video track sent from here respects these limits,
        // without needing to hack the SDP string!
        
        try 
        {
            videoStreamSender.SetBitrate(minBitrateKbps, maxBitrateKbps);
            Debug.Log($"[HighSpeedScript] Bitrate set to: Min {minBitrateKbps}Kbps, Max {maxBitrateKbps}Kbps.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[HighSpeedScript] Failed to set native bitrate: {e.Message}");
        }
    }
}
