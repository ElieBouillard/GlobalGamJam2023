using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationContoller : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private PlayerController _controller;
    
    private static readonly int VelocityX = Animator.StringToHash("VelocityX");
    private static readonly int VelocityY = Animator.StringToHash("VelocityY");

    private void Awake()
    {
        _controller = GetComponent<PlayerController>();
    }

    private void Update()
    {
        _animator.SetFloat(VelocityY, _controller.GetMovementDirection().z);
        _animator.SetFloat(VelocityX, _controller.GetMovementDirection().x);
    }
}
