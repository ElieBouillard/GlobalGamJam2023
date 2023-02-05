using System.Linq;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct WeaponPickup
    {
        public static WeaponPickup Empty => new WeaponPickup
        {
            WeaponType = WeaponType.None,
            Preview = null
        };

        public WeaponType WeaponType;
        public GameObject Preview;

        public bool IsEmpty => WeaponType == WeaponType.None;
    }

    [Header("Generation Data")]
    [SerializeField] private WeaponSpawnData _spawnData = null;
    [SerializeField] private WeaponPickup[] _previews = null;
    [SerializeField, Range(0f, 1f)] private float _generateOnStartChance = 1f;

    [Header("View")]
    [SerializeField] private Transform _previewsContainer = null;
    [SerializeField] private float _previewRotationSpeed = 180f;

    public WeaponPickup Weapon { get; private set; }

    public void OnWeaponPickedUp()
    {
        UnityEngine.Assertions.Assert.IsTrue(CanPickup(), $"Picked up an empty weapon!");

        Weapon.Preview.SetActive(false);
        Weapon = WeaponPickup.Empty;

        StartCoroutine(RespawnWeapon());

        // TODO: Audio (spatialized).
    }

    public bool CanPickup()
    {
        return !Weapon.IsEmpty;
    }

    private void GenerateWeapon()
    {
        WeaponType weaponType = _spawnData.GetRandomWeaponType();
        WeaponPickup pickup = _previews.FirstOrDefault(o => o.WeaponType == weaponType);
        Weapon = pickup;
        UnityEngine.Assertions.Assert.IsFalse(Weapon.IsEmpty, $"Generated an empty weapon!");
        Weapon.Preview.SetActive(true);

        // TODO: Audio (spatialized).
    }

    private System.Collections.IEnumerator RespawnWeapon()
    {
        float cooldown = Random.Range(_spawnData.CooldownMinMax.x, _spawnData.CooldownMinMax.y);
        yield return new WaitForSeconds(cooldown);
        GenerateWeapon();
    }

    private void Start()
    {
        foreach (WeaponPickup preview in _previews)
            preview.Preview.SetActive(false);

        if (Random.value < _generateOnStartChance)
            GenerateWeapon();
        else
            StartCoroutine(RespawnWeapon());

        if (_previewsContainer != null)
            _previewsContainer.Rotate(0f, Random.Range(0, 360f), 0f, Space.World);
    }

    private void Update()
    {
        if (_previewsContainer != null)
            _previewsContainer.Rotate(0f, _previewRotationSpeed * Time.deltaTime, 0f, Space.World);
    }
}
