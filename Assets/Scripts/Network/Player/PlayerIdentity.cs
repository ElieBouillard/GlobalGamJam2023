using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class PlayerIdentity : MonoBehaviour
{
    #region Fields
    private ushort _id;
    protected ulong _steamId;
    private bool _isLocalPlayer;
    #endregion
    
    #region Getters
    public ushort GetId => _id;
    public ulong GetSteamId => _steamId;
    public bool IsLocalPlayer => _isLocalPlayer;
    #endregion

    public virtual void Initialize(ushort id, string newName)
    {
        _id = id;

        if (_id == NetworkManager.Instance.Client.Id) { _isLocalPlayer = true; }

        gameObject.name = newName;
    }
    
    public virtual void Initialize(ushort id, ulong steamId)
    {
        Initialize(id, SteamFriends.GetFriendPersonaName((CSteamID)steamId));
        
        _steamId = steamId;
    }
}