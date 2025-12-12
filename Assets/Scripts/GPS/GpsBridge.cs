using UnityEngine;
using System.Globalization;
using System.Runtime.InteropServices;

public class GpsBridge : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void StartBrowserLocationWatch();
#endif

    public Vector2 LatLon { get; private set; }
    public bool IsReady { get; private set; }
    public string LastError { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        StartBrowserLocationWatch();
#endif
    }

    // Called from JS
    public void OnLocation(string csv)
    {
        var parts = csv.Split(',');
        if (parts.Length != 2) return;

        LatLon = new Vector2(
            float.Parse(parts[0], CultureInfo.InvariantCulture),
            float.Parse(parts[1], CultureInfo.InvariantCulture)
        );

        IsReady = true;
    }

    // Called from JS on error
    public void OnLocationError(string msg)
    {
        LastError = msg;
        Debug.LogWarning($"GPS error: {msg}");
    }
}
