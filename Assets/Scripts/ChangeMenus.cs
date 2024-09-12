using System.Collections.Generic;
using UnityEngine;

public class ChangeMenus : MonoBehaviour
{
    [SerializeField] private List<GameObject> _lobbyMenu;
    [SerializeField] private List<GameObject> _profileMenu;

    public void SetActiveProfile()
    {
        if (!ProfileController.Instance._dataIsSynchronized) return;
        
        foreach (var lobbyObject in _lobbyMenu)
        {
            lobbyObject.SetActive(false);
        }
        
        foreach (var profileObject in _profileMenu)
        {
            profileObject.SetActive(true);
        }
        ProfileViewmodel.Instance.Setup();
    }
    
    public void SetActiveLobby()
    {
        if (!ProfileController.Instance._dataIsSynchronized) return;
        
        foreach (var lobbyObject in _lobbyMenu)
        {
            lobbyObject.SetActive(true);
        }
        
        foreach (var profileObject in _profileMenu)
        {
            profileObject.SetActive(false);
        }
        ProfileController.Instance.GetCurrentUserData(ProfileController.Instance._playFabId);
        ProfileViewmodel.Instance.Setup();
    }
}
