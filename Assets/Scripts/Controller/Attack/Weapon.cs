using UnityEngine;

public class Weapon : MonoBehaviour
{
    private const string ANIM_PARAM_ATTACK = "Attack";

    [Header("References")]
    [SerializeField] private Animator _animator = null;

    [Header("Data")]
    [SerializeField, Min(0f)] private float _radius = 1f;
    [SerializeField, Min(0f)] private float _distance = 1f;
    [SerializeField, Min(0f)] private float _damage = 10f;
    [SerializeField, Min(0f)] private float _cooldown = 0.33f;

    public virtual void TriggerAttack()
    {
        _animator.SetTrigger(ANIM_PARAM_ATTACK);
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
}
