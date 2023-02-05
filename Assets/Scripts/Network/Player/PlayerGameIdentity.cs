using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class PlayerGameIdentity : PlayerIdentity
{
    public MovementReceiver MovementReceiver { private set; get; }

    public RemoteWeaponSelection RemoteSelection { private set; get; }
    
    private void Awake()
    {
        MovementReceiver = GetComponent<MovementReceiver>();
        RemoteSelection = GetComponent<RemoteWeaponSelection>();
    }
}
