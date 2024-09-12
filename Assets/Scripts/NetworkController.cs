using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkController : Fusion.Behaviour, INetworkRunnerCallbacks
{
    public static NetworkController Instance;
    
    [SerializeField] public String _roomName;
    
    [SerializeField] public String _sessionName;

    [SerializeField] private TMP_InputField _inputFieldText;
    
    public NetworkRunner _runner;

    [SerializeField] public GameObject _contentGameObject;

    [SerializeField] private GameObject _sessionPrefab;

    private async void Start()
    {
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            
            _runner = gameObject.AddComponent<NetworkRunner>();
            var result = await _runner.JoinSessionLobby(SessionLobby.ClientServer, "MyCustomLobby");
            if (!result.Ok) 
            {
                Debug.LogError($"Failed to Start: {result.ShutdownReason}");
            }
        }
        else
        {
            Destroy(gameObject);        
        }
    }

   

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (_runner.SessionInfo.PlayerCount == 2)
        {
            GameController.Instance.StartGame();
        }
    }
    
    async void StartGame(GameMode mode)
    {
        var customProps = new Dictionary<string, SessionProperty>
        {
            ["RoomName"] = _roomName
        };
        
        var scene = SceneRef.FromIndex(1);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid) {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }
        
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = _sessionName,
            Scene = scene,
            SessionProperties = customProps,
            PlayerCount = 2,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }
    
    public void HostSession()
    {
        _roomName = _inputFieldText.text;
        
        if (String.IsNullOrEmpty(_roomName))
            return;
        
        _sessionName = Guid.NewGuid().ToString();
        StartGame(GameMode.Host);
    }
    
    public void JoinSession(String sessionName)
    {
        _sessionName = sessionName;
        StartGame(GameMode.Client);
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        foreach (Transform child in _contentGameObject.transform)
        {
            Destroy(child.gameObject);
        }
        
        foreach (var session in sessionList)
        {
            var game = Instantiate(_sessionPrefab, _contentGameObject.transform);
            game.GetComponent<SessionViewmodel>().Setup(session);
        }
    }

    public void Disconnect()
    {
        _runner.Shutdown();
        SceneManager.LoadScene("Lobby");
    }
    
    #region UnusedCallbacks

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
       
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        OnConnectFailed(runner, remoteAddress, reason);
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        OnReliableDataReceived(runner, player, key, data);
    }

    void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        OnReliableDataProgress(runner, player, key, progress);
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        throw new NotImplementedException();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        GameController.Instance.RPC_Restart();
        GameController.Instance.ChangeButtonState(false);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Disconnect();
    }

    // ReSharper disable once Unity.IncorrectMethodSignature
    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }
    
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }
    
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }
    #endregion
}
