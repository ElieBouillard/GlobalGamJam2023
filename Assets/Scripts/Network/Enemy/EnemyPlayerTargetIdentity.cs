using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnemyPlayerTargetIdentity : EnemyIdentity
{
    private Transform _playerTarget;

    public void Initialize(int id, Transform playerTarget)
    {
        Initialize(id);
        _playerTarget = playerTarget;
    }

    protected override void Update()
    {
        if (!_agent.enabled) return;
        
        if (_playerTarget == null) return;

        if ((_playerTarget.position - transform.position).magnitude <= _attackRange)
        {
            if(_agent.hasPath) _agent.ResetPath();
            
            if (_attackClock > 0)
            {
                _attackClock -= Time.deltaTime;
            }
            else
            {
                if(_animator.GetBool(WalkAnimKey)) _animator.SetBool(WalkAnimKey, false);
                transform.forward = (_playerTarget.position - transform.position).WithY(0).normalized;
                AttackFeedback();
                _isInAttack = true;
                _attackClock = _attackCooldown;
            }
        }
        else
        {
            if (!_isInAttack)
            {
                if(!_animator.GetBool(WalkAnimKey)) _animator.SetBool(WalkAnimKey, true);
                _agent.SetDestination(_playerTarget.position);
                _attackClock = 0;
            }
        }
    }
}