using FishNet.Managing;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetBootstrap : MonoBehaviour
{
    public NetworkManager networkManager;
    public string gameSceneName = "Game";

    public void StartHost()
    {
        Debug.Log("StartHost clicked");

        if (networkManager == null)
        {
            Debug.LogError("NetworkManager not assigned!");
            return;
        }

        networkManager.ServerManager.StartConnection();
        networkManager.ClientManager.StartConnection();

        Debug.Log($"Server started? {networkManager.ServerManager.Started}");
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
}
