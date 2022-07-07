using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
using Unity.Netcode.Transports.UNET;

public class NetworkConnectionHelper : MonoBehaviour
{
    [SerializeField] private UnityEvent OnConnected;
    [SerializeField] private UnityEvent OnServerStarted;

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += ((ulong obj) =>
        {
            if (OnConnected != null)
            {
                OnConnected.Invoke();
            }
        });

        NetworkManager.Singleton.OnServerStarted += (() =>
        {
            if (OnServerStarted != null)
            {
                OnServerStarted.Invoke();
            }
        });

        var unetTransport = GetComponent<UNetTransport>();
        if (ConfigurationManager.Instance != null && unetTransport != null)
        {
            unetTransport.ConnectAddress = ConfigurationManager.Instance.serverAddr;
            unetTransport.ConnectPort = ConfigurationManager.Instance.serverPort;
            unetTransport.ServerListenPort = ConfigurationManager.Instance.serverPort;
        }
    }
    private void Singleton_OnServerStarted()
    {
        throw new System.NotImplementedException();
    }

    private void OnServerInitialized()
    {
        if (OnConnected != null)
        {
            OnConnected.Invoke();
        }
    }

    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
