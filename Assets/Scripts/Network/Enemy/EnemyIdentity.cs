using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyIdentity : MonoBehaviour, IHittable
{
    //UnityInspector
    [SerializeField] protected float _attackRange = 1f;
    [SerializeField] protected float _attackRadius = 0.5f;
    [SerializeField] protected float _attackCooldown = 3f;
    [SerializeField] protected int _initialLife = 1;
    
    public int Id { private set; get; }

    [Header("References")]
    protected NavMeshAgent _agent;
    protected Animator _animator;
    private AudioSource _audioSource;
    
    protected bool _isInAttack;
    protected float _attackClock;
    protected int _currLife;
    
    protected static readonly int AttackAnimKey = Animator.StringToHash("Attack");
    protected static readonly int WalkAnimKey = Animator.StringToHash("Walk");

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }
    
    protected void Initialize(int id)
    {
        Id = id;
        _animator.SetBool(WalkAnimKey,true);
        _currLife = _initialLife;
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

    public virtual void Attack()
    {
        if (_audioSource.clip != null) _audioSource.Play();
        
        Collider[] colliders = Physics.OverlapSphere(transform.position + Vector3.up + transform.forward * _attackRange, _attackRadius);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out IHittable hittable))
            {
                hittable.OnHit(new HitData(){Team = Team.Enemy});
            }
        }
    }

    public void EndAttack()
    {
        _isInAttack = false;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up + transform.forward * _attackRange, _attackRadius);
    }

    public void OnHit(HitData hitData)
    {
        if (hitData.Team != Team.Player) return;

        //Todo: Play Feedbacks.
        
        if(hitData.Damage == 0) return;

        _currLife -= hitData.Damage;

        if (_currLife <= 0)
        {
            NetworkManager.Instance.ClientMessages.SendEnemyDeath(Id);
            Death();
        }
    }

    public void Death()
    {
        GameManager.Instance.EnemySpawners.RemoveEnemy(this);
        Destroy(gameObject);
    }
}