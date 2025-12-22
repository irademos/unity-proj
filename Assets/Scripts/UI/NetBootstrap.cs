using FishNet.Managing;
using UnityEngine;
using UnityEngine.SceneManagement;
using FishNet.Transporting.Tugboat;

public class NetBootstrap : MonoBehaviour
{
    public NetworkManager networkManager;
    public string gameSceneName = "Game";

    public void StartHost()
    {
        Debug.Log("Connect clicked");

        if (networkManager == null)
        {
            Debug.LogError("NetworkManager not assigned");
            return;
        }

        // WebGL should be client-only
        networkManager.ClientManager.StartConnection();

        Debug.Log($"Client started? {networkManager.ClientManager.Started}");

        var gps = FindAnyObjectByType<GpsBridge>();
        if (gps != null) gps.StartGps();

        SceneManager.LoadScene(gameSceneName);
    }

    public void StartClient()
    {
        networkManager.ClientManager.StartConnection();
        SceneManager.LoadScene(gameSceneName);
    }

    public void Connect()
    {
        Debug.Log("Connect clicked");

        if (networkManager == null)
        {
            Debug.LogError("NetworkManager not assigned");
            return;
        }

        // Force assign Tugboat transport
        var tugboat = networkManager.GetComponent<Tugboat>();
        Debug.Log("Tugboat component present: " + (tugboat != null));

        if (tugboat != null && networkManager.TransportManager.Transport == null)
        {
            networkManager.TransportManager.Transport = tugboat;
            Debug.Log("Assigned Tugboat as active transport.");
        }

        Debug.Log("Active transport: " + networkManager.TransportManager.Transport);

        networkManager.ClientManager.StartConnection();
        Debug.Log($"Client started? {networkManager.ClientManager.Started}");
    }

}
