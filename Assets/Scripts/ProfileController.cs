using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Fusion;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using WebSocketSharp;
using Application = UnityEngine.Device.Application;
using Random = UnityEngine.Random;
using SystemInfo = UnityEngine.Device.SystemInfo;

public class ProfileController : NetworkBehaviour
{
    public static ProfileController Instance;
    
    public Boolean _loginComplete;
    public Boolean _dataIsSynchronized;
    public Boolean _errorOccurredWhileLogging;
    public Boolean _noInternetConnection;
        
    [SerializeField] public String _playFabId;

    public Profile _profile;
    
    [SerializeField] public List<Sprite> _avatarList;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        StartCoroutine(LoginAndGet());
    }

    private IEnumerator LoginAndGet()
    {  
        CheckForExistence();
        _profile = JsonConvert.DeserializeObject<Profile>(ReadFromFile());
        ProfileViewmodel.Instance.Setup();
        
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            _loginComplete = true;
            _noInternetConnection = true;
        }
        else
        {
            Login();
        }

        while (_loginComplete == false)
        {
            yield return new WaitForEndOfFrame();
        }
        
        GetCurrentUserData(_playFabId);
        


        while (_dataIsSynchronized == false)
        {
            yield return new WaitForEndOfFrame();
        }
        
        ProfileViewmodel.Instance.Setup();
    }
    
    private void Login()
    {
        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
        {
            CustomId = Application.isEditor?"editor_":"" + SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        }, result => {
            _loginComplete = true;
        }, error => {   
            _loginComplete = true;
            _errorOccurredWhileLogging = true;
            _dataIsSynchronized = true;
        });    
    }
    
    public void GetCurrentUserData(String playFabId)
    {
        if (_noInternetConnection || _errorOccurredWhileLogging)
        {
            _profile.LastTimeWasInGame = DateTime.Now;
            SaveData();
            return;    
        }

        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = playFabId,
            Keys = null
        }, result =>
        {
            if (result.Data.TryGetValue("Profile", out var record) && !String.IsNullOrEmpty(record.Value))
            {
                if (_profile.LastTimeWasInGame > JsonConvert.DeserializeObject<Profile>(record.Value).LastTimeWasInGame)
                {
                    _profile.LastTimeWasInGame = DateTime.Now;
                    SaveData();
                }
                else
                {
                    _profile = JsonConvert.DeserializeObject<Profile>(record.Value);
                    _profile.LastTimeWasInGame = DateTime.Now;
                    SaveData();
                }
            }
            else
            {
                SetUserData();
            }
            _dataIsSynchronized = true;
            ProfileViewmodel.Instance.Setup();
        }, error =>
        {
            _dataIsSynchronized = true;
        });
    }

    public void UpdatePlayerDisplayName()
    {
        if (_noInternetConnection) return;

        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = _profile.Name
        }, result => {
        }, error => { 
        });
    }

    public void SaveData()
    {
        SetUserData();
        WriteToFile(JsonConvert.SerializeObject(_profile));
    }
    
    public void SetUserData()
    {
        if (_noInternetConnection) return;

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>
            {
                { "Profile", JsonConvert.SerializeObject(_profile) },
            },
        }, result => {
        }, error =>
        {
        });
    }
    
    private void WriteToFile(String str)
    {
        File.WriteAllText(Application.persistentDataPath + $"\\{(Application.isEditor?"editor_":"")}profile.txt", str);
    }

    private String ReadFromFile()
    {
        return File.ReadAllText(Application.persistentDataPath + $"\\{(Application.isEditor?"editor_":"")}profile.txt");
    }

    private void CheckForExistence()
    {
        if (!File.Exists(Application.persistentDataPath + $"\\{(Application.isEditor?"editor_":"")}profile.txt"))
        {
            _profile.Name = GeneratePlayerNickname();
            WriteToFile(JsonConvert.SerializeObject(_profile));
        }
    }
    
    private String GeneratePlayerNickname()
    {
        Int32 randomInt32 = Random.Range(10000000, 99999999);
        return "Player#" + randomInt32;
    }
}

[Serializable]
public class Profile
{
    public String Name;
    public Int32 WinCount;
    public Int32 AvatarId;
    public DateTime LastTimeWasInGame;
}