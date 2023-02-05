using UnityEngine;

public class WeaponAnimEvents : MonoBehaviour
{
    [SerializeField] private Weapon _weapon = null;

    public void OnAttackFrame()
    {
        _weapon = GetComponentInChildren<Weapon>(false);
        _weapon.Attack();
    }
}
