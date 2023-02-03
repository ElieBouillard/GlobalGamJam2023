using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Steamworks;
using UnityEngine;

public class SteamInvitationsManager : Singleton<SteamInvitationsManager>
{
    [SerializeField] private RectTransform _content;
    [SerializeField] private SteamInvitation _invitationPrefab;
    private RectTransform _rectTransform;

    private float _offSetY = -25f;

    protected Callback<LobbyInvite_t> InviteLobby;
    
    private readonly List<SteamInvitation> _invitations = new List<SteamInvitation>();
    protected override void Awake()
    {
        base.Awake();
        
        _rectTransform = GetComponent<RectTransform>();
        InviteLobby = Callback<LobbyInvite_t>.Create(OnInvitationReceived);
    }

    private void OnInvitationReceived(LobbyInvite_t lobbyInfo)
    {
        if (_invitations.Count == 0) EnablePanel(true);
        
        SteamInvitation invitationTemp = AddInvitation();
        invitationTemp.Initialize(lobbyInfo);
        _invitations.Add(invitationTemp);
    }
    
    private SteamInvitation AddInvitation()
    {
        _rectTransform = GetComponent<RectTransform>();
        SteamInvitation invitationTemp = Instantiate(_invitationPrefab, _content);

        if (_invitations.Count < 3)
        {
            Vector2 rectTransformDeltaSize = _rectTransform.sizeDelta;
            rectTransformDeltaSize = new Vector2(rectTransformDeltaSize.x, rectTransformDeltaSize.y + 125f);
            _rectTransform.DOSizeDelta(rectTransformDeltaSize, 0.15f);
        }

        Vector2 contentDeltaSize = _content.sizeDelta;
        contentDeltaSize = new Vector2(contentDeltaSize.x, contentDeltaSize.y + 125f);
        _content.sizeDelta = contentDeltaSize;
        
        Vector3 invitationPos =  invitationTemp.transform.localPosition;
        invitationPos = new Vector3(invitationPos.x, _offSetY, invitationPos.y);
        invitationTemp.transform.localPosition = invitationPos;
        
        _offSetY -= 125f;
        return invitationTemp;
    }

    public void RemoveInvitation(SteamInvitation thisInvitation)
    {
        if (!_invitations.Contains(thisInvitation)) return;
        
        Destroy(thisInvitation.gameObject);
        _invitations.Remove(thisInvitation);
        if(_invitations.Count == 0) EnablePanel(false);
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        int invitationsCount = _invitations.Count;
        
        if (invitationsCount < 3)
        {
            Vector2 rectTransformDeltaSize = _rectTransform.sizeDelta;
            rectTransformDeltaSize = new Vector2(rectTransformDeltaSize.x, 125 * invitationsCount + 50);
            _rectTransform.DOSizeDelta(rectTransformDeltaSize, 0.15f);
        }
        
        Vector2 contentDeltaSize = _content.sizeDelta;
        contentDeltaSize = new Vector2(contentDeltaSize.x, 125 * invitationsCount);
        _content.sizeDelta = contentDeltaSize;

        _offSetY = -25f - 125f * invitationsCount;
        
        for (int i = 0; i < _invitations.Count; i++)
        {
            _invitations[i].gameObject.transform.DOLocalMoveY(-125 * i + -25f , 0.1f);
        }
    }

    private void EnablePanel(bool value)
    {
        transform.DOMoveX(value ? 0f : -500f, 0.2f);
    }
}
