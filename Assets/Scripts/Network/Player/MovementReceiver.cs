using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementReceiver : MonoBehaviour
{
    [SerializeField] private float _smooth = 10f;

    private Vector3? _targetPos;
    
    public void SetMovements(Vector3 pos)
    {
        _targetPos = pos;

        Debug.Log("swag");
    }

    private void Update()
    {
        if (_targetPos == null) return;
        
        transform.position = Vector3.Lerp(transform.position, _targetPos.Value, Time.deltaTime * _smooth);
    }
}
