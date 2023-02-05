using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using TMPro;
using UnityEngine;

public class PlayerGameIdentity : PlayerIdentity
{
    public MovementReceiver MovementReceiver { private set; get; }

    public RemoteWeaponSelection RemoteSelection { private set; get; }

    [SerializeField] private TMP_Text _nameText;
    
    
    private void Awake()
    {
        MovementReceiver = GetComponent<MovementReceiver>();
        RemoteSelection = GetComponent<RemoteWeaponSelection>();
    }

    public void SetName(string name)
    {
        _nameText.text = name;
    }
}
