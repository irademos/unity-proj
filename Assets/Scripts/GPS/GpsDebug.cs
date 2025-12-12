using UnityEngine;

public class GpsDebug : MonoBehaviour
{
    public GpsBridge gps;

    void Update()
    {
        if (gps != null && gps.IsReady)
            Debug.Log($"GPS: {gps.LatLon}");
    }
}
