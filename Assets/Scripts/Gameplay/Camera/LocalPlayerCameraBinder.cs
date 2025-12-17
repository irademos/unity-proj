using FishNet.Object;
using UnityEngine;

public class LocalPlayerCameraBinder : NetworkBehaviour
{
    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!IsOwner) return;

        var cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("No Camera.main found.");
            return;
        }

        // Grab CameraOrbit without referencing its type
        var orbit = cam.GetComponent("CameraOrbit");
        if (orbit == null)
        {
            Debug.LogWarning("Main Camera has no CameraOrbit component.");
            return;
        }

        // Set the public field "target" on CameraOrbit via reflection
        var field = orbit.GetType().GetField("target");
        if (field == null)
        {
            Debug.LogWarning("CameraOrbit has no public field named 'target'.");
            return;
        }

        field.SetValue(orbit, transform);

        Debug.Log("Bound Main Camera to local player.");
    }
}
