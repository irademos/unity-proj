using FishNet.Managing;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetBootstrap : MonoBehaviour
{
    public NetworkManager networkManager;
    public string gameSceneName = "Game";

    public void StartHost()
    {
        networkManager.ServerManager.StartConnection();
        networkManager.ClientManager.StartConnection();
        Debug.Log("StartHost clicked");
        Debug.Log($"NM present: {networkManager != null}");
        Debug.Log($"Server started? {networkManager.ServerManager.Started}");
        Debug.Log($"Client started? {networkManager.ClientManager.Started}");

        FindAnyObjectByType<GpsBridge>().StartGps();
        SceneManager.LoadScene(gameSceneName);
    }

    public void StartClient()
    {
        networkManager.ClientManager.StartConnection();
        SceneManager.LoadScene(gameSceneName);
    }
}
