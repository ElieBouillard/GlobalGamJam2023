using UnityEngine;

public class WeaponAnimEvents : MonoBehaviour
{
    [SerializeField] private Weapon _weapon = null;

    public void OnAttackFrame()
    {
        if (_weapon == null)
            _weapon = GetComponentInParent<Weapon>();

        _weapon.Attack();
    }
}
