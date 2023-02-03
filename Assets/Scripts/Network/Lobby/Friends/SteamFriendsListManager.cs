using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class SteamFriendsListManager : MonoBehaviour
{
    [SerializeField] private RectTransform _content;
    [SerializeField] private SteamFriendProfile _friendProfilePrefab;
    [SerializeField] private Button _refreshFriendsListButton;

    private float _yOffset = -25f;
    private List<CSteamID> _friends = new List<CSteamID>();
    private void Start()
    {
        if(NetworkManager.Instance.UseSteam) StartCoroutine(RefreshFriendsList());
    }

    private void GrabFriends()
    {
        int friendsCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll);
        
        for (int i = 0; i < friendsCount; i++)
        {
            CSteamID friendId = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagAll);

            _friends.Add(friendId);
        }
        
        for (int i = 0; i < _friends.Count; i++)
        {
            if (SteamFriends.GetFriendGamePlayed(_friends[i], out FriendGameInfo_t friendGameInfo))
            {
                if (friendGameInfo.m_gameID == new CGameID((ulong) 480))
                {
                    AddFriendToList().Initialize(true, _friends[i]);
                    _friends.RemoveAt(i);
                    i--;
                }
            }
        }
        
        for (int i = 0; i < _friends.Count; i++)
        {
            if (SteamFriends.GetFriendPersonaName(_friends[i]) != "[unknown]")
            {
                AddFriendToList().Initialize(false, _friends[i]);
            }
        }
    }
    
    private SteamFriendProfile AddFriendToList()
    {
        SteamFriendProfile friendTemp = Instantiate(_friendProfilePrefab, _content);

        Vector3 localPosition = friendTemp.transform.localPosition;
        localPosition = new Vector3(localPosition.x, _yOffset, localPosition.z);
        friendTemp.transform.localPosition = localPosition;

        Vector2 sizeDelta = _content.sizeDelta;
        sizeDelta = new Vector2(sizeDelta.x, sizeDelta.y + 75f);
        _content.sizeDelta = sizeDelta;
        
        _yOffset -= 75f;
        return friendTemp;
    }

    private IEnumerator RefreshFriendsList()
    {
        _yOffset = -25f;

        _content.sizeDelta = new Vector2(_content.sizeDelta.x, 0);
        
        int friendsDisplaysCount = _content.childCount;

        for (int i = 0; i < friendsDisplaysCount; i++)
        {
            Destroy(_content.GetChild(i).gameObject);
        }
        
        _friends.Clear();
        GrabFriends();
        yield return new WaitForSeconds(5);
        StartCoroutine(RefreshFriendsList());
    }
}
