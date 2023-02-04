using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Spawn Data", menuName = "GGJ2023/Weapon Spawn Data")]
public class WeaponSpawnData : ScriptableObject
{
    [System.Serializable]
    public struct WeaponItem
    {
        public WeaponType WeaponType;
        public int Weight;
    }

    [SerializeField] private WeaponItem[] _weapons = null;
    [SerializeField, Min(0f)] private Vector2 _cooldownMinMax = new Vector2(10f, 10f);

    public Vector2 CooldownMinMax => _cooldownMinMax;

    public WeaponType GetRandomWeaponType()
    {
        int totalWeight = _weapons.Sum(o => o.Weight);
        int currentWeight = 0;
        int random = Random.Range(0, totalWeight);

        for (int i = 0; i < _weapons.Length; ++i)
        {
            WeaponItem weaponItem = _weapons[i];
            currentWeight += weaponItem.Weight;
            if (currentWeight > random)
                return weaponItem.WeaponType;
        }

        return _weapons[^1].WeaponType;
    }
}
