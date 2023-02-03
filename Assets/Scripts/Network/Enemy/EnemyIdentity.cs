using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyIdentity : MonoBehaviour
{
    //UnityInspector
    [SerializeField] protected float _attackRange = 1f;
    [SerializeField] protected float _attackRadius = 0.5f;
    [SerializeField] protected float _attackCooldown = 3f;
    
    public int Id { private set; get; }

    //References
    protected NavMeshAgent _agent;
    protected Animator _animator;
    
    protected bool _isInAttack;
    protected float _attackClock;
    
    protected static readonly int AttackAnimKey = Animator.StringToHash("Attack");
    protected static readonly int WalkAnimKey = Animator.StringToHash("Walk");

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
    }
    
    protected void Initialize(int id)
    {
        Id = id;
        _animator.SetBool(WalkAnimKey,true);
    }

    protected virtual void Update()
    {
        if(!_isInAttack) return;
        
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

    private void AttackFeedback()
    {
        _animator.SetTrigger(AttackAnimKey);
    }

    public virtual void Attack(){}


    public virtual void EndAttack()
    {
        _isInAttack = false;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up + transform.forward * _attackRange, _attackRadius);
    }
}