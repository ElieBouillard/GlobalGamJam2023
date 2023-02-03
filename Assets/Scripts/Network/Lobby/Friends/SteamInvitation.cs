using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SteamInvitation : MonoBehaviour
{
    [SerializeField] private RawImage _profileImage;
    [SerializeField] private TMP_Text _profilePseudoText;
    [SerializeField] private Button _acceptButton;
    [SerializeField] private Button _declineButton;

    private CSteamID _friendSteamId;
    private ulong _lobbyId;
    
    protected Callback<AvatarImageLoaded_t> ImageLoaded;

    private void Awake()
    {
        _acceptButton.onClick.AddListener(AcceptInvite);
        _declineButton.onClick.AddListener(DeclineInvite);
    }

    private void Start()
    {
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnPlayerAvatarLoaded);
    }

    public void Initialize(LobbyInvite_t lobbyInfo)
    {
        _friendSteamId = (CSteamID)lobbyInfo.m_ulSteamIDUser;
        _lobbyId = lobbyInfo.m_ulSteamIDLobby;
        
        string friendName = SteamFriends.GetFriendPersonaName(_friendSteamId);
        _profilePseudoText.text = friendName;

        LoadFriendAvatar(); 
    }

    private void AcceptInvite()
    {
        NetworkManager.Instance.Leave();
        SteamMatchmaking.JoinLobby((CSteamID)_lobbyId);
        DeclineInvite();
    }

    private void DeclineInvite()
    {
        SteamInvitationsManager.Instance.RemoveInvitation(this);
    }
    
    public void LoadFriendAvatar()
    {
        int _playerAvatarId = SteamFriends.GetLargeFriendAvatar(_friendSteamId);
        
        if(_playerAvatarId == -1)  {Debug.Log("Error loading image"); return;}

        _profileImage.texture = GetSteamImageAsTexture(_playerAvatarId);
    }
    
    private void OnPlayerAvatarLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID == _friendSteamId)
        { 
            _profileImage.texture = GetSteamImageAsTexture(callback.m_iImage);
        }
    }
    
    private Texture2D GetSteamImageAsTexture(int image)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(image, out uint width, out uint height);

        if (isValid)
        {
            byte[] imageTemp = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(image, imageTemp,(int)width * (int)height * 4);

            if (isValid)
            {
                texture = new Texture2D((int) width, (int) height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(imageTemp);
                texture.Apply();
            }
        }
        return texture;
    } 
}
