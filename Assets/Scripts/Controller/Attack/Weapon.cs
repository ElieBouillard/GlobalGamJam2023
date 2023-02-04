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

    [Header("Data")]
    [SerializeField, Min(0f)] private float _radius = 1f;
    [SerializeField, Min(0f)] private Vector2 _distance = new Vector2(1f, 0f);
    [SerializeField, Min(0f)] private float _damage = 10f;
    [SerializeField, Min(0f)] private float _cooldown = 0.33f;

    [Header("Animation")]
    [SerializeField, Min(0f)] private float _backToIdleDelay = 0.5f;

    [Header("VFX")]
    [SerializeField, Range(0f, 1f)] private float _noHitTrauma = 0.1f;
    [SerializeField, Range(0f, 1f)] private float _hitTrauma = 0.2f;
    [SerializeField, Min(0)] private int _freezeFrameDelay = 0;
    [SerializeField, Min(0f)] private float _freezeFrameDuration = 0.05f;

    private Coroutine _backToIdleCoroutine;
    private IShakable _shookOnAttack;

    public float Cooldown => _cooldown;

    public virtual void PlayAttackAnimation(IShakable shookOnAttack = null)
    {
        _animator.SetTrigger(ANIM_PARAM_ATTACK);
        _shookOnAttack = shookOnAttack;

        Attack();

        StopBackToIdleCoroutine();
        _backToIdleCoroutine = StartCoroutine(BackToIdleCoroutine());
    }

    public void Attack()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position + transform.forward * _distance.x + transform.up * _distance.y, _radius);
        IEnumerable<IHittable> hittables = targets.Where(o => o.TryGetComponent<IHittable>(out _)).Select(o => o.GetComponent<IHittable>());

        foreach (IHittable hittable in hittables)
            hittable.OnHit(HitData.Empty);

        if (hittables.Any())
        {
            FreezeFrameManager.FreezeFrame(_freezeFrameDelay, _freezeFrameDuration, 0f, true);
            _shookOnAttack?.SetTrauma(_hitTrauma);
        }
        else
        {
            _shookOnAttack?.SetTrauma(_noHitTrauma);
        }
    }

    public virtual void OnEquiped()
    {
        // Equip animation?
        gameObject.SetActive(true);
    }

    public virtual void OnUnequiped()
    {
        StopBackToIdleCoroutine();
        gameObject.SetActive(false);
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
        _animator.SetTrigger(ANIM_PARAM_IDLE);
    }

    private void Reset()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + transform.forward * _distance.x + transform.up * _distance.y, _radius);
    }
}
