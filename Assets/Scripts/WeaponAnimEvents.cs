using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class WeaponAnimEvents : MonoBehaviour
{
    [SerializeField] private Weapon _weapon = null;
    [SerializeField] private bool _isLocal;
    
    public void OnAttackFrame()
    {
        if (!_isLocal) return;
        
        _weapon = GetComponentInChildren<Weapon>(false);
        _weapon.Attack();
    }
}
