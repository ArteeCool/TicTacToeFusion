using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class GameProfilesViewmodel : MonoBehaviour
{
    public static GameProfilesViewmodel Instance;
    
    [SerializeField] private TextMeshProUGUI _nicknameTextMeshPro;

    [SerializeField] private TextMeshProUGUI _winCountsTextMeshPro;
    
    [SerializeField] private Image _avatarImage;   
    
    [SerializeField] private TextMeshProUGUI _enemyNicknameTextMeshPro;

    [SerializeField] private TextMeshProUGUI _enemyWinCountsTextMeshPro;
    
    [SerializeField] private Image _enemyAvatarImage;

    public void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        _nicknameTextMeshPro.text = ProfileController.Instance._profile.Name;
        _winCountsTextMeshPro.text = ProfileController.Instance._profile.WinCount.ToString();
        _avatarImage.sprite = ProfileController.Instance._avatarList[ProfileController.Instance._profile.AvatarId];
    }  
    
    public void Setup(String name, Int32 winCounts, Int32 avatarId)
    {
        if(name != null) _enemyNicknameTextMeshPro.text = name;
        if(winCounts != -1) _enemyWinCountsTextMeshPro.text = winCounts.ToString();
        if(avatarId != -1) _enemyAvatarImage.sprite = ProfileController.Instance._avatarList[avatarId];
    }

    public void UpdateProfile()
    {
        _nicknameTextMeshPro.text = ProfileController.Instance._profile.Name;
        _winCountsTextMeshPro.text = ProfileController.Instance._profile.WinCount.ToString();
        _avatarImage.sprite = ProfileController.Instance._avatarList[ProfileController.Instance._profile.AvatarId];
    }
}
