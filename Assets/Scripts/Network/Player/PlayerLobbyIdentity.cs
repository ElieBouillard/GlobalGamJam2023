using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLobbyIdentity : PlayerIdentity
{
    [SerializeField] private TMP_Text _playerPseudoText;
    [SerializeField] private RawImage _profileImage;
    [SerializeField] private GameObject _localSprite;

    protected Callback<AvatarImageLoaded_t> ImageLoaded;
    
    private void Start()
    {
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnPlayerAvatarLoaded);
    }
    
    public override void Initialize(ushort id, string newName)
    {
        base.Initialize(id, newName);

        _playerPseudoText.text = newName;

        NetworkManager networkManager = NetworkManager.Instance;
        
        if (id == networkManager.Client.Id) LobbyManager.Instance.LobbyPanel.EnableStartButton(id == 1);
        
        _localSprite.SetActive(id == networkManager.Client.Id);
    }

    public override void Initialize(ushort id, ulong steamId)
    {
        base.Initialize(id, steamId);
        
        LoadPlayerAvatar();
    }
    
    public void LoadPlayerAvatar()
    {
        int _playerAvatarId = SteamFriends.GetLargeFriendAvatar((CSteamID)_steamId);
        
        if(_playerAvatarId == -1)  {Debug.Log("Error loading image"); return;}

        _profileImage.texture = GetSteamImageAsTexture(_playerAvatarId);
    }
    
    private void OnPlayerAvatarLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID == (CSteamID)_steamId)
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
