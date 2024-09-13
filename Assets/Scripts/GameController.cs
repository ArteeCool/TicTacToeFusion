using System;
using System.Collections.Generic;
using System.Text;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using Fusion.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : NetworkBehaviour
{
    public static GameController Instance;

    private Int32 _crossWins;
    private Int32 _circleWins;
    
    [SerializeField] private TextMeshProUGUI _crossCounterText;
    [SerializeField] private TextMeshProUGUI _circleCounterText;
    
    [SerializeField] private List<GameObject> _buttons; 
    
    [SerializeField] private List<String> _buttonsMarks; 
    
    [SerializeField] private GameObject _restartButton;
    
    private Boolean _state;
    
    public Boolean _playerTurn;
    
    public Boolean _gamestarted;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        ChangeButtonState(false);
    }

    public void StartGame()
    {
        _gamestarted = true;
        _playerTurn = false;
        OnButtonClicked();
        
        GetInfo();
        
    }

    public void Disconnect()
    {
        NetworkController.Instance.Disconnect();
    }

    public void GetInfo()
    {
        List<byte> Name = new List<byte>();
        Name.AddRange(Encoding.UTF8.GetBytes(ProfileController.Instance._profile.Name));
        
        
        List<byte> WinsCount = new List<byte>();
        WinsCount.AddRange(Encoding.UTF8.GetBytes(ProfileController.Instance._profile.WinCount.ToString()));
        
        List<byte> AvatarId = new List<byte>();
        AvatarId.AddRange(Encoding.UTF8.GetBytes(ProfileController.Instance._profile.AvatarId.ToString()));
        
        
        if (NetworkController.Instance._runner.IsServer)
        {
            foreach (var playerRef in NetworkController.Instance._runner.ActivePlayers)
            {
                if (playerRef.PlayerId == NetworkController.Instance._runner.LocalPlayer.PlayerId) continue;
                
                NetworkController.Instance._runner.SendReliableDataToPlayer(playerRef, ReliableKey.FromInts(0), Name.ToArray()); 
                NetworkController.Instance._runner.SendReliableDataToPlayer(playerRef, ReliableKey.FromInts(1), WinsCount.ToArray());
                NetworkController.Instance._runner.SendReliableDataToPlayer(playerRef, ReliableKey.FromInts(2), AvatarId.ToArray());
            }
        }
        else
        {
            NetworkController.Instance._runner.SendReliableDataToServer(ReliableKey.FromInts(0), Name.ToArray());
            NetworkController.Instance._runner.SendReliableDataToServer(ReliableKey.FromInts(1), WinsCount.ToArray());
            NetworkController.Instance._runner.SendReliableDataToServer(ReliableKey.FromInts(2), AvatarId.ToArray());
        }
    }
    
    public void OnButtonClicked()
    {

        if (NetworkController.Instance._runner.LocalPlayer.PlayerId - 1 == Convert.ToInt32(_playerTurn))
        {
            ChangeButtonState(true);
        } 
        if (NetworkController.Instance._runner.LocalPlayer.PlayerId - 1 != Convert.ToInt32(_playerTurn))
        {
            ChangeButtonState(false);
        }

        if (CheckWin())
        {
            if (NetworkController.Instance._runner.LocalPlayer.PlayerId - 1 != Convert.ToInt32(_playerTurn))
            {
                ProfileController.Instance._profile.WinCount++;
                ProfileController.Instance.SaveData();
            }
            GetInfo();
            GameProfilesViewmodel.Instance.UpdateProfile();
            if (_playerTurn)
            {
                _circleWins++;
                _circleCounterText.text = _circleWins.ToString();
            }
            else if (!_playerTurn)
            {
                _crossWins++;
                _crossCounterText.text = _crossWins.ToString();
            }
            
            _restartButton.SetActive(true);
            ChangeButtonState(false);
        }

        _playerTurn = !_playerTurn;
    }
    
    public void ChangeButtonState(Boolean state)
    {
        Boolean wasChanged = false;
        foreach (var button in _buttons)
        {
            if (button.GetComponent<ButtonController>()._wasClicked) continue;
            wasChanged = true;
            Button buttonComponent = button.GetComponent<Button>();
            buttonComponent.interactable = state;
        }

        if (!wasChanged)
        {
            _restartButton.SetActive(true);
        }
    }

    [Rpc]
    public void RPC_Restart()
    {
        foreach (var button in _buttons)
        {
            ButtonController buttonComponent = button.GetComponent<ButtonController>();
            
            buttonComponent._wasClicked = false;
            buttonComponent.GetComponentInChildren<TextMeshProUGUI>().text = String.Empty;
        }
        OnButtonClicked();
        _restartButton.SetActive(false);
    }
    
    private Boolean CheckWin()
    {
        String _mark = String.Empty;
        if (_playerTurn)
        {
            _mark = "O";
        }
        else if (!_playerTurn)
        {
            _mark = "X";
        }
        
        _buttonsMarks.Clear();
            
        for (int i = 0; i < _buttons.Count; i++)
        {
            _buttonsMarks.Add(_buttons[i].GetComponentInChildren<TextMeshProUGUI>().text);
        }
        
        // Check rows
        for (int row = 0; row < 3; row++)
        {
            int startIndex = row * 3;
            if (_buttonsMarks[startIndex] == _mark && _buttonsMarks[startIndex + 1] == _mark && _buttonsMarks[startIndex + 2] == _mark)
                return true; // Row
        }

        // Check columns
        for (int col = 0; col < 3; col++)
        {
            if (_buttonsMarks[col] == _mark && _buttonsMarks[col + 3] == _mark && _buttonsMarks[col + 6] == _mark)
                return true; // Column
        }

        // Check diagonals
        if (_buttonsMarks[0] == _mark && _buttonsMarks[4] == _mark && _buttonsMarks[8] == _mark)
            return true; // Diagonal (top-left to bottom-right)
        if (_buttonsMarks[2] == _mark && _buttonsMarks[4] == _mark && _buttonsMarks[6] == _mark)
            return true; // Diagonal (top-right to bottom-left)

        return false;
    }
}
