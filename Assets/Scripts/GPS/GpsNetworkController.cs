using FishNet.Object;
using UnityEngine;

#if MAPBOX_PRESENT
using Mapbox.Unity.Map;
using Mapbox.Utils;
#endif

public class GpsNetworkController : NetworkBehaviour
{
    public bool useGpsToDrivePosition = false;

    private GpsBridge gps;

#if MAPBOX_PRESENT
    private AbstractMap map;
#endif

    private void Awake()
    {
        gps = Object.FindAnyObjectByType<GpsBridge>();

#if MAPBOX_PRESENT
        map = Object.FindAnyObjectByType<AbstractMap>();
#endif
    }

    private void Update()
    {
        if (!useGpsToDrivePosition) return;
        if (!IsServerInitialized) return;
        if (gps == null || !gps.IsReady) return;

#if MAPBOX_PRESENT
        if (map == null) return;

        var latLon = gps.LatLon;
        var worldPos = map.GeoToWorldPosition(new Vector2d(latLon.x, latLon.y), true);
        transform.position = worldPos;
#else
        // No Mapbox yet: just keep the player at origin while you validate GPS is working.
        // You can log the GPS so you know it’s alive:
        // Debug.Log($"GPS: {gps.LatLon}");
#endif
    }
}
