using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class SteamLobbyManager : Singleton<SteamLobbyManager>
{
    #region Callbacks
    protected Callback<LobbyCreated_t> _lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> _gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> _lobbyEnter;
    #endregion
    
    #region Fields
    private CSteamID _lobbyId;
    private const string _hostAddressKey = "HostAddress";
    #endregion

    #region Getters
    public CSteamID GetLobbyId => _lobbyId;
    #endregion
    
    private void Start()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steam is not initialized !");
            return;
        }

        _lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        _lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
        _gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
    }

    public void CreateLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, NetworkManager.Instance.MaxPlayer);
    }
    
    public void LeaveLobby()
    {
        SteamMatchmaking.LeaveLobby(_lobbyId);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) return;

        _lobbyId = new CSteamID(callback.m_ulSteamIDLobby);
        SteamMatchmaking.SetLobbyData(_lobbyId, _hostAddressKey, SteamUser.GetSteamID().ToString());
        
        NetworkManager.Instance.Server.Start(0,NetworkManager.Instance.MaxPlayer);
        NetworkManager.Instance.Client.Connect("127.0.0.1");
    }

    public void JoinLobby(ulong lobbyId)
    {
        SteamMatchmaking.JoinLobby(new CSteamID(lobbyId));
    }
    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEnter(LobbyEnter_t callback)
    {
        if (NetworkManager.Instance.Server.IsRunning) return;

        _lobbyId = new CSteamID(callback.m_ulSteamIDLobby);
        string hostAddress = SteamMatchmaking.GetLobbyData(_lobbyId, _hostAddressKey);

        NetworkManager.Instance.Client.Connect(hostAddress);
    }
}