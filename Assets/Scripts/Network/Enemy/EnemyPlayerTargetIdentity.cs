using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlayerTargetIdentity : EnemyIdentity
{
    [SerializeField] private Transform _playerTarget;

    private void Start()
    {
        Initialize(0);
    }

    public void Initialize(int id, Transform playerTarget)
    {
        Initialize(id);
        _playerTarget = playerTarget;
    }

    protected override void Update()
    {
        if (!_isInAttack)
        {
            if ((_playerTarget.position - transform.position).magnitude <= _attackRange)
            {
                _agent.ResetPath();
                _animator.SetBool(WalkAnimKey, false);
                _isInAttack = true;
            }
            else
            {
                _agent.SetDestination(_playerTarget.position);
                if(!_animator.GetBool(WalkAnimKey)) _animator.SetBool(WalkAnimKey, true);
                if(_attackClock != 0) _attackClock = 0;
            }
        }
        
        base.Update();
    }
}
