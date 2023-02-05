using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MovementReceiver : MonoBehaviour
{
    [SerializeField] private float _smoothMovements = 10f;
    [SerializeField] private float _smoothAnimations = 10f;

    private Animator _animator;
    
    private Vector3? _targetPos;
    private float _targetY;

    private Vector3? _targetInput;
    
    private static readonly int VelocityY = Animator.StringToHash("VelocityY");
    private static readonly int VelocityX = Animator.StringToHash("VelocityX");
    private static readonly int Die = Animator.StringToHash("Die");

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void SetMovements(Vector3 pos, float y)
    {
        _targetPos = pos;
        _targetY = y;
    }

    public void SetAnimations(Vector3 input)
    {
        _targetInput = input;
    }

    public void SetDeathAnim(bool value)
    {
        _animator.SetBool(Die, value);
    }
    
    private void Update()
    {
        if (_targetInput != null)
        {
            _animator.SetFloat(VelocityY, Mathf.Lerp(_animator.GetFloat(VelocityY), _targetInput.Value.z, Time.deltaTime * _smoothAnimations));
            _animator.SetFloat(VelocityX, Mathf.Lerp(_animator.GetFloat(VelocityX), _targetInput.Value.x, Time.deltaTime * _smoothAnimations));
        }

        if (_targetPos != null)
        {        
            transform.position = Vector3.Lerp(transform.position, _targetPos.Value, Time.unscaledDeltaTime * _smoothMovements);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,_targetY,0), Time.deltaTime * _smoothMovements);
        }
    }
}
