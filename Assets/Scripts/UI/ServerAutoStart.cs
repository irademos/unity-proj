using FishNet.Managing;
using UnityEngine;

public class ServerAutoStart : MonoBehaviour
{
    public NetworkManager networkManager;

    void Start()
    {
        if (networkManager == null)
            networkManager = FindAnyObjectByType<NetworkManager>();

        // Only start server in server builds
#if UNITY_SERVER
        Debug.Log("Starting server...");
        networkManager.ServerManager.StartConnection();
        Debug.Log("Server started? " + networkManager.ServerManager.Started);
#endif
    }
}
