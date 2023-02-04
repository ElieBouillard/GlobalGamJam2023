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
        if (!_isInAttack)
        {
            if ((_targetPos.Value - transform.position).magnitude <= _attackRange)
            {
                _agent.ResetPath();
                _animator.SetBool(WalkAnimKey, false);
                _isInAttack = true;
            }
        }
        
        base.Update();
    }

    public override void Attack()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + Vector3.up + transform.forward * _attackRange, _attackRadius);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out TreeBehaviour tree))
            {
                // tree.TakeDamage();
            }
        }
    }
}
