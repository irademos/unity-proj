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
        SceneManager.LoadScene(gameSceneName);
    }

    public void StartClient()
    {
        networkManager.ClientManager.StartConnection();
        SceneManager.LoadScene(gameSceneName);
    }
}
