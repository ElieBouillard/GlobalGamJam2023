using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private const string ANIM_PARAM_IDLE = "Idle";
    private const string ANIM_PARAM_ATTACK = "Attack";

    [Header("References")]
    [SerializeField] private Animator _animator = null;
    [SerializeField] private Transform _mainRoot = null;
    [SerializeField] private Material _material = null;

    [Header("Data")]
    [SerializeField, Min(0f)] private WeaponType _weaponType = WeaponType.None;
    [SerializeField, Min(0f)] private float _radius = 1f;
    [SerializeField, Min(0f)] private Vector2 _distance = new Vector2(1f, 0f);
    [SerializeField, Min(0)] private int _damage = 1;
    [SerializeField, Min(1)] private int _maximumHits = 5;
    [SerializeField, Min(0f)] private float _cooldown = 0.33f;

    [Header("Animation")]
    [SerializeField, Min(0f)] private float _backToIdleDelay = 0.5f;

    [Header("VFX")]
    [SerializeField] private GameObject _hitParticles = null;
    [SerializeField, Min(0f)] private float _hitForwardAdd = 1f;
    [SerializeField, Range(0f, 1f)] private float _noHitTrauma = 0.1f;
    [SerializeField, Range(0f, 1f)] private float _hitTrauma = 0.2f;
    [SerializeField, Min(0)] private int _freezeFrameDelay = 0;
    [SerializeField, Min(0f)] private float _freezeFrameDuration = 0.05f;

    private Coroutine _backToIdleCoroutine;
    private IShakable _shookOnAttack;
    private PlayerAttackModule _playerAttackModule;

    private ushort _id;
    
    public WeaponType WeaponType => _weaponType;

    public int MaximumHits => _maximumHits;

    public float Cooldown => _cooldown;

    private void Start()
    {
        _id = _mainRoot.GetComponent<PlayerIdentity>().GetId;
    }

    public virtual void PlayAttackAnimation(PlayerAttackModule attackModule, IShakable shookOnAttack = null)
    {
        _animator.SetTrigger(ANIM_PARAM_ATTACK);
        _playerAttackModule = attackModule;
        _shookOnAttack = shookOnAttack;

        if (gameObject.activeSelf)
        {
            StopBackToIdleCoroutine();
            _backToIdleCoroutine = StartCoroutine(BackToIdleCoroutine());
        }

        // TODO: attack audio.
    }

    public void Attack()
    {
        Vector3 attackPosition = _mainRoot.transform.position + Camera.main.transform.forward.WithY(0).normalized * _distance.x + _mainRoot.transform.up * _distance.y;
        Collider[] targets = Physics.OverlapSphere(attackPosition, _radius);
        IEnumerable<IHittable> hittables = targets.Where(o => o.TryGetComponent<IHittable>(out _) && !o.TryGetComponent<PlayerController>(out _))
                                                  .Select(o => o.GetComponent<IHittable>());

        HitData hitData = new HitData
        {
            Team = Team.Player,
            Damage = _damage,
            PlayerSourceId = _id,
        };

        foreach (IHittable hittable in hittables)
            hittable.OnHit(hitData);

        if (hittables.Any())
        {
            _playerAttackModule.OnWeaponHit();
    
            if (_hitParticles != null)
                Instantiate(_hitParticles, attackPosition + Camera.main.transform.forward.WithY(0).normalized * _hitForwardAdd, _hitParticles.transform.rotation);
            
            FreezeFrameManager.FreezeFrame(_freezeFrameDelay, _freezeFrameDuration, 0f, true);
            _shookOnAttack?.SetTrauma(_hitTrauma);
            // TODO: any target hit audio.
        }
        else
        {
            _shookOnAttack?.SetTrauma(_noHitTrauma);
        }
    }

    public virtual void OnEquiped()
    {
        // _animator.SetTrigger(ANIM_PARAM_IDLE); // Equip animation.
        gameObject.SetActive(true);
        // TODO: weapon audio.
    }

    public virtual void OnUnequiped()
    {
        StopBackToIdleCoroutine();
        gameObject.SetActive(false);
    }

    public void UpdateHitsFeedback(int currentHits)
    {
        float percentage = (MaximumHits - currentHits) / (float)MaximumHits;
        
        if (_material != null)
            _material.SetFloat("_Threshold", 1f - percentage);
    }
    
    private void StopBackToIdleCoroutine()
    {
        if (_backToIdleCoroutine != null)
        {
            StopCoroutine(_backToIdleCoroutine);
            _backToIdleCoroutine = null;
        }
    }

    private IEnumerator BackToIdleCoroutine()
    {
        yield return new WaitForSeconds(_backToIdleDelay);
        // _animator.SetTrigger(ANIM_PARAM_IDLE);
    }

    private void Reset()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_mainRoot.transform.position + Camera.main.transform.forward.WithY(0).normalized * _distance.x + _mainRoot.transform.up * _distance.y, _radius);
    }
}