using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileViewmodel : MonoBehaviour
{
    public static ProfileViewmodel Instance;
    
    [SerializeField] private TMP_InputField _nicknameInputField;
    [SerializeField] private TextMeshProUGUI _nicknameTextMeshPro;

    [SerializeField] private TextMeshProUGUI _winCounts;
    
    [SerializeField] private Image _avatarLobby;
    [SerializeField] private Image _avatarProfile;

    private Profile _profile;

    private void Start()
    {
        Setup();
    }

    private void Awake()
    {        
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    public void ChangeAvatar(Int32 numberToAdd)
    {
        ProfileController.Instance._profile.AvatarId += numberToAdd;
        if (ProfileController.Instance._profile.AvatarId < 0)
        {
            ProfileController.Instance._profile.AvatarId = ProfileController.Instance._avatarList.Count - 1;
        }
        else if (ProfileController.Instance._profile.AvatarId > ProfileController.Instance._avatarList.Count - 1)
        {
            ProfileController.Instance._profile.AvatarId = 0;
        }

        if(_avatarProfile != null) _avatarProfile.sprite = ProfileController.Instance._avatarList[ProfileController.Instance._profile.AvatarId];
    }

    public void Reset()
    {
        ProfileController.Instance.GetCurrentUserData(ProfileController.Instance._playFabId);
    }

    public void SaveData()
    {
        if (_nicknameInputField != null)
        {
            Debug.Log("SaveData");
            if (_nicknameInputField.text != ProfileController.Instance._profile.Name)
            {
                ProfileController.Instance._profile.Name = _nicknameInputField.text;
            }           
            ProfileController.Instance.UpdatePlayerDisplayName();
        }
        
        ProfileController.Instance.SaveData();
        Setup();
    }
    
    public void Setup()
    {
        if(_nicknameTextMeshPro != null) _nicknameTextMeshPro.text = ProfileController.Instance._profile.Name;
        if(_nicknameInputField != null) _nicknameInputField.text = ProfileController.Instance._profile.Name;
        if(_winCounts != null) _winCounts.text = ProfileController.Instance._profile.WinCount.ToString();
        if(_avatarProfile != null) _avatarProfile.sprite = ProfileController.Instance._avatarList[ProfileController.Instance._profile.AvatarId];
        if(_avatarLobby != null) _avatarLobby.sprite = ProfileController.Instance._avatarList[ProfileController.Instance._profile.AvatarId];
    }
}
