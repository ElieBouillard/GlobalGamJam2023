using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSender : MonoBehaviour
{
    [SerializeField] private Transform _rotationRoot;

    private Vector3 _lastPos;
    private float _lastY;
    
    private NetworkManager _networkManager;
    private PlayerController _controller;
    
    private void Awake()
    {
        _networkManager = NetworkManager.Instance;
        _controller = GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        if (_lastPos != transform.position || _lastY != _rotationRoot.eulerAngles.y)
        {
            _networkManager.ClientMessages.SendMovements(transform.position, _rotationRoot.eulerAngles.y);
            _lastPos = transform.position;
            _lastY = _rotationRoot.eulerAngles.y;
        }
        
        _networkManager.ClientMessages.SendAnimations(_controller.GetMovementDirection());
    }
}