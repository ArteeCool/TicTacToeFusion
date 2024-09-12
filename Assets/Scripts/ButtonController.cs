using System;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : NetworkBehaviour
{
    private Char _mark;

    private TextMeshProUGUI _textMeshProUGUI;

    public Boolean _wasClicked;

    private void Start()
    {
        _textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void ChangeMark()
    {
        if (GameController.Instance._playerTurn)
        {
            _mark = 'O';
        }        
        else if(!GameController.Instance._playerTurn)
        {
            _mark = 'X';
        }
    }

    private void ChangeMarkColor()
    {
        if (_mark == 'X')
        {
            _textMeshProUGUI.color = Color.red;
        }
        else if (_mark == 'O')
        {
            _textMeshProUGUI.color = Color.cyan;
        }
    }

    [Rpc]
    public void RPC_SetMark()
    {
        ChangeMark();
        ChangeMarkColor();

        _textMeshProUGUI.text = _mark.ToString();

        GetComponent<Button>().interactable = false;
        _wasClicked = true;
        
        GameController.Instance.OnButtonClicked();
    }
}
