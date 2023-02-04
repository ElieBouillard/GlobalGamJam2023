using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private const string ANIM_PARAM_ATTACK = "Attack";

    [Header("References")]
    [SerializeField] private Animator _animator = null;

    [Header("Data")]
    [SerializeField, Min(0f)] private float _radius = 1f;
    [SerializeField, Min(0f)] private Vector2 _distance = new Vector2(1f, 0f);
    [SerializeField, Min(0f)] private float _damage = 10f;
    [SerializeField, Min(0f)] private float _cooldown = 0.33f;

    [Header("VFX")]
    [SerializeField, Range(0f, 1f)] private float _noHitTrauma = 0.1f;
    [SerializeField, Range(0f, 1f)] private float _hitTrauma = 0.2f;

    private IShakable _shookOnAttack;

    public float Cooldown => _cooldown;

    public virtual void PlayAttackAnimation(IShakable shookOnAttack = null)
    {
        _animator.SetTrigger(ANIM_PARAM_ATTACK);

        _shookOnAttack = shookOnAttack;
        Attack();
    }

    public void Attack()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position + transform.forward * _distance.x + transform.up * _distance.y, _radius);
        IEnumerable<IHittable> hittables = targets.Where(o => o.TryGetComponent<IHittable>(out _)).Select(o => o.GetComponent<IHittable>());

        foreach (IHittable hittable in hittables)
            hittable.OnHit(HitData.Empty);

        _shookOnAttack?.SetTrauma(hittables.Any() ? _hitTrauma : _noHitTrauma);
    }

    public virtual void OnEquiped()
    {
        // Equip animation?
        gameObject.SetActive(true);
    }

    public virtual void OnUnequiped()
    {
        gameObject.SetActive(false);
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
