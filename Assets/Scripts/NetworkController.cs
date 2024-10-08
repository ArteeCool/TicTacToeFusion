using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
        if (GameController.Instance._gamestarted) return;

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
        if (!_runner.LobbyInfo.IsValid) return;
        _roomName = _inputFieldText.text;
        
        if (String.IsNullOrEmpty(_roomName))
            return;
        
        _sessionName = Guid.NewGuid().ToString();
        StartGame(GameMode.Host);
    }
    
    public void JoinSession(String sessionName)
    {
        if (_runner.State == NetworkRunner.States.Shutdown) return;
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

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        key.GetInts(out var a, out var b, out var c, out var d);
        StartCoroutine(Fill(a, b, c, data));

    }

    public IEnumerator Fill(int a, int b, int c, ArraySegment<byte> data)
    {
        while (GameProfilesViewmodel.Instance == null)
        {
            yield return new WaitForEndOfFrame();
        }
        
        if (a == 0)
        {
            GameProfilesViewmodel.Instance.Setup(Encoding.UTF8.GetString(data),-1,-1);
        }
        else if (a == 1)
        {
            GameProfilesViewmodel.Instance.Setup(null,Convert.ToInt32(Encoding.UTF8.GetString(data)),-1);
        }        
        else if (a == 2)
        {
            GameProfilesViewmodel.Instance.Setup(null,-1,Convert.ToInt32(Encoding.UTF8.GetString(data)));
        }
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
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
    
    public void OnConnectedToServer(NetworkRunner runner)
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

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }
    #endregion
}
