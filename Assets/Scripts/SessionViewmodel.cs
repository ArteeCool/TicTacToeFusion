using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionViewmodel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _sessionName;
    [SerializeField] private TextMeshProUGUI _playerCount;

    [SerializeField] private Button _joinButton;

    public void Setup(SessionInfo sessionInfo)
    {
        _sessionName.text = sessionInfo.Properties["RoomName"];
        _playerCount.text = sessionInfo.PlayerCount + "/" + sessionInfo.MaxPlayers;

        _joinButton.onClick.AddListener(() =>
        {
            if (sessionInfo.PlayerCount == sessionInfo.MaxPlayers) return;
            if (!ProfileController.Instance._loginComplete && !ProfileController.Instance._dataIsSynchronized) return;

            NetworkController.Instance.JoinSession(sessionInfo.Name);
        });
    }
}