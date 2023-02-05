using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    [SerializeField] private SkinnedMeshRenderer _meshRenderer = null;

    public int Id { private set; get; }

    [Header("References")]
    protected NavMeshAgent _agent;
    protected Animator _animator;
    private AudioSource _audioSource;

    private NetworkManager _networkManager;
    
    protected bool _isInAttack;
    protected float _attackClock;
    protected int _currLife;
    
    protected static readonly int AttackAnimKey = Animator.StringToHash("Attack");
    protected static readonly int WalkAnimKey = Animator.StringToHash("Walk");
    private static readonly int DieAnimKey = Animator.StringToHash("Die");

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        _audioSource = GetComponent<AudioSource>();

        _networkManager = NetworkManager.Instance;

        _meshRenderer.material.SetFloat("_CutoffHeight", 3f);
    }

    protected void Initialize(int id)
    {
        Id = id;
        _animator.SetBool(WalkAnimKey,true);
        _currLife = _initialLife;
    }

    protected virtual void Update() { }

    protected void AttackFeedback()
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
                hittable.OnHit(new HitData(){Team = Team.Enemy, Damage = 1});
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

        if(hitData.Damage == 0) return;

        _currLife -= hitData.Damage;

        if (_currLife <= 0)
        {
            NetworkManager.Instance.ClientMessages.SendEnemyDeath(Id);
            Death(hitData.PlayerSourceId);
        }
    }

    public void Death(ushort playerId)
    {
        transform.forward = (_networkManager.Players[playerId].transform.position - transform.position).WithY(0).normalized;

        _animator.SetTrigger(DieAnimKey);
        GetComponent<Collider>().enabled = false;
        _agent.enabled = false;
        GameManager.Instance.EnemySpawners.RemoveEnemy(this);
        
        StartCoroutine(Delete());
    }

    private IEnumerator Delete()
    {
        yield return new WaitForSeconds(1.5f);

        Material material = _meshRenderer.material;

        for (float t = 0f; t <= 1f; t += Time.deltaTime)
        {
            material.SetFloat("_CutoffHeight", (1f - t) * 3f);
            yield return null;
        }

        Destroy(gameObject);
    }
}