using System;
using System.Linq;
using UnityEngine;
using TMPro;
using Unity.RenderStreaming;

public class WebServerIPScript : MonoBehaviour
{
    [Header("Default Server Address")]
    [SerializeField] private string defaultWebSocketUrl = "ws://192.170.2.137:80";

    [Header("Optional UI Input")]
    [SerializeField] private string playerPrefsKey = "WebSocketServerUrl";

    [SerializeField] private TMP_InputField serverAddressInputField;
    [SerializeField] private SignalingManager signalingManager;
    [SerializeField] private bool restartSignalingWhenAddressChanges = true;

    [Tooltip("If true, the script loads the previously saved URL from PlayerPrefs at startup.")]
    [SerializeField] private bool loadSavedUrlOnStart = true;

    [Tooltip("Current WebSocket URL used by the app.")]
    public string webServerIP;

    private void Start()
    {
        InitializeServerUrl();
    }

    private void InitializeServerUrl()
    {
        string candidateUrl = defaultWebSocketUrl;

        if (loadSavedUrlOnStart && PlayerPrefs.HasKey(playerPrefsKey))
        {
            candidateUrl = PlayerPrefs.GetString(playerPrefsKey);
        }

        if (!TryNormalizeWebSocketUrl(candidateUrl, out webServerIP))
        {
            webServerIP = defaultWebSocketUrl;
            Debug.LogWarning($"Invalid saved/default URL. Falling back to '{webServerIP}'.");
        }

        Debug.Log($"WebSocket Server URL: {webServerIP}");

        ApplyUrlToSignalingManager();

// #if TMP_PRESENT
        if (serverAddressInputField != null)
        {
            serverAddressInputField.text = ToHostPortDisplay(webServerIP);
        }
// #endif
    }

    public bool SetWebSocketAddress(string userInput)
    {
        if (!TryNormalizeHostPortInput(userInput, out string normalizedUrl))
        {
            Debug.LogWarning($"Invalid input. Please enter only host:port like 192.170.3.208:80.");
            return false;
        }

        webServerIP = normalizedUrl;
        PlayerPrefs.SetString(playerPrefsKey, webServerIP);
        PlayerPrefs.Save();

        Debug.Log($"Updated WebSocket Server URL: {webServerIP}");

        ApplyUrlToSignalingManager();

// #if TMP_PRESENT
        string displayAddress = ToHostPortDisplay(webServerIP);
        if (serverAddressInputField != null && serverAddressInputField.text != displayAddress)
        {
            serverAddressInputField.text = displayAddress;
        }
// #endif

        return true;
    }

// #if TMP_PRESENT
    public void ApplyAddressFromInputField()
    {
        if (serverAddressInputField == null)
        {
            Debug.LogWarning("Server address input field is not assigned.");
            return;
        }

        SetWebSocketAddress(serverAddressInputField.text);
    }
// #endif

    public void ResetToDefaultAddress()
    {
        if (TryNormalizeWebSocketUrl(defaultWebSocketUrl, out string normalizedDefault))
        {
            webServerIP = normalizedDefault;
            PlayerPrefs.SetString(playerPrefsKey, webServerIP);
            PlayerPrefs.Save();
            ApplyUrlToSignalingManager();
            Debug.Log($"Reset WebSocket Server URL to default: {webServerIP}");
        }
        else
        {
            Debug.LogError($"Default WebSocket URL is invalid: '{defaultWebSocketUrl}'");
        }
    }

    private static bool TryNormalizeWebSocketUrl(string input, out string normalizedUrl)
    {
        normalizedUrl = string.Empty;

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        string candidate = input.Trim();

        if (!candidate.Contains("://"))
        {
            candidate = "ws://" + candidate;
        }

        if (!Uri.TryCreate(candidate, UriKind.Absolute, out Uri uri))
        {
            return false;
        }

        string scheme = uri.Scheme;

        if (scheme == "http")
        {
            scheme = "ws";
        }
        else if (scheme == "https")
        {
            scheme = "wss";
        }

        if (scheme != "ws" && scheme != "wss")
        {
            return false;
        }

        var builder = new UriBuilder(uri)
        {
            Scheme = scheme,
            Port = uri.IsDefaultPort ? -1 : uri.Port
        };

        normalizedUrl = builder.Uri.ToString().TrimEnd('/');
        return true;
    }

    private static bool TryNormalizeHostPortInput(string input, out string normalizedUrl)
    {
        normalizedUrl = string.Empty;

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        string candidate = input.Trim();
        if (candidate.Contains("://"))
        {
            return false;
        }

        return TryNormalizeWebSocketUrl("ws://" + candidate, out normalizedUrl);
    }

    private static string ToHostPortDisplay(string wsUrl)
    {
        if (string.IsNullOrWhiteSpace(wsUrl))
        {
            return string.Empty;
        }

        string value = wsUrl.Trim();
        if (value.StartsWith("ws://", StringComparison.OrdinalIgnoreCase))
        {
            return value.Substring(5).TrimEnd('/');
        }

        if (value.StartsWith("wss://", StringComparison.OrdinalIgnoreCase))
        {
            return value.Substring(6).TrimEnd('/');
        }

        return value.TrimEnd('/');
    }

    private void ApplyUrlToSignalingManager()
    {
        if (signalingManager == null)
        {
            return;
        }

        IceServer[] clonedIceServers = null;
        SignalingSettings currentSettings = signalingManager.GetSignalingSettings();
        if (currentSettings is WebSocketSignalingSettings wsSettings)
        {
            if (AreUrlsEquivalent(wsSettings.url, webServerIP))
            {
                return;
            }

            clonedIceServers = wsSettings.iceServers.Select(server => server.Clone()).ToArray();
        }

        bool wasRunning = signalingManager.Running;

        if (wasRunning && restartSignalingWhenAddressChanges)
        {
            signalingManager.Stop();
        }

        try
        {
            signalingManager.SetSignalingSettings(new WebSocketSignalingSettings(webServerIP, clonedIceServers));
        }
        catch (InvalidOperationException)
        {
            Debug.LogWarning("Cannot update signaling URL while SignalingManager is running. Enable restartSignalingWhenAddressChanges or stop signaling first.");
            return;
        }

        if (wasRunning && restartSignalingWhenAddressChanges)
        {
            signalingManager.Run();
        }
    }

    private static bool AreUrlsEquivalent(string left, string right)
    {
        if (!TryNormalizeWebSocketUrl(left, out string leftNormalized))
        {
            return false;
        }

        if (!TryNormalizeWebSocketUrl(right, out string rightNormalized))
        {
            return false;
        }

        return string.Equals(leftNormalized, rightNormalized, StringComparison.OrdinalIgnoreCase);
    }
}
