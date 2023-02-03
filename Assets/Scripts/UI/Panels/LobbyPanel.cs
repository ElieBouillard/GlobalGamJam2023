using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : Panel
{
    [Header("References")] 
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _leaveLobbyButton;
    [SerializeField] private Button _openFriendsListButton;
    [SerializeField] private Button _closeFriendsListButton;
    [SerializeField] private Transform _friendsListPanel;

    protected override void AssignButtonsReference()
    {
        _startGameButton.onClick.AddListener(OnClickStartGame);
        _leaveLobbyButton.onClick.AddListener(OnClickLeaveLobby);
        
        _openFriendsListButton.onClick.AddListener(delegate { EnableFriendsList(true); });
        _closeFriendsListButton.onClick.AddListener(delegate { EnableFriendsList(false); });
    }

    public void EnableStartButton(bool value)
    {
        _startGameButton.gameObject.SetActive(value);
    }
    
    private void OnClickStartGame()
    {
        NetworkManager.Instance.StartGame();
    }

    private void OnClickLeaveLobby()
    {
        NetworkManager.Instance.Leave();
    }

    private void EnableFriendsList(bool value)
    {
        _openFriendsListButton.gameObject.SetActive(!value);
        _friendsListPanel.DOLocalMoveY(value ? 215 : 875, 0.2f);
    }
}