using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using TMPro;
using UnityEngine;

public class PanelManager : Singleton<PanelManager>
{
    [SerializeField] private Panel[] _panels;

    private NetworkManager _networkManager;
    private bool _isPause;
    protected override void Awake()
    {
        base.Awake();

        EnablePanel(PanelType.MainMenu);
    }

    private void Start()
    {
        _networkManager = NetworkManager.Instance;
    }

    private void Update()
    {
        if (_networkManager.GameState == GameState.Gameplay)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                foreach (var panel in _panels)
                {
                    if (panel.PanelType == PanelType.Pause) panel.gameObject.SetActive(!_isPause);
                    _isPause = !_isPause;
                }
            }
        }
    }
    
    public void EnablePanel(PanelType panelType)
    {
        foreach (var panel in _panels)
        {
            panel.gameObject.SetActive(panel.PanelType == panelType);
        }
    }
}

public enum PanelType
{
    MainMenu = 1,
    Options,
    Lobby,
    Pause,
}