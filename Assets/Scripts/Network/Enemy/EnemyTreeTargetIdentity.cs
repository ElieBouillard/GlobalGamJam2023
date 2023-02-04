using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyTreeTargetIdentity : EnemyIdentity
{
    private Vector3? _targetPos;

    public void Initialize(int id, Vector3 targetPos)
    {
        Initialize(id);
        _targetPos = targetPos;
        _agent.SetDestination(targetPos);
    }

    protected override void Update()
    {
        if (!_agent.enabled) return;

        if ((_targetPos.Value - transform.position).magnitude <= _attackRange && !_isInAttack)
        {
            _agent.ResetPath();
            _animator.SetBool(WalkAnimKey, false);
            _isInAttack = true;
        }

        if ((_targetPos.Value - transform.position).magnitude >= _attackRange && !_isInAttack)
        {
            _agent.SetDestination(_targetPos.Value);
            _animator.SetBool(WalkAnimKey, true);
            if(_attackClock != 0) _attackClock = 0;
        }
        
        if (_isInAttack)
        {
            if (_attackClock > 0)
            {
                _attackClock -= Time.deltaTime;
            }
            else
            {
                _attackClock = _attackCooldown; 
                AttackFeedback();
            }
        }
    }
}