using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSender : MonoBehaviour
{
    private Vector3 _lastPos;

    private NetworkManager _networkManager;

    private void Awake()
    {
        _networkManager = NetworkManager.Instance;
    }

    private void FixedUpdate()
    {
        if (_lastPos != transform.position)
        {
            _networkManager.ClientMessages.SendMovements(transform.position);
            _lastPos = transform.position;
        }
    }
}