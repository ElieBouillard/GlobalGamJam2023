public class PlayerAttackModule
{
    public Weapon CurrentWeapon { get; private set; }

    private int _currentWeaponHits;
    private float _cooldownTimer;

    public void OnWeaponHit()
    {
        _currentWeaponHits++;

        if (_currentWeaponHits >= CurrentWeapon.MaximumHits)
            SetWeapon(null);
        else
            CurrentWeapon.UpdateHitsFeedback(_currentWeaponHits);
    }

    public bool SetWeapon(Weapon weapon)
    {
        if (CurrentWeapon != null)
            CurrentWeapon.OnUnequiped();

        CurrentWeapon = weapon;
        _currentWeaponHits = 0;
        ResetCooldown();

        if (CurrentWeapon != null)
            CurrentWeapon.OnEquiped();

        return true;
    }

    public void Attack(IShakable shookOnAttack = null)
    {
        UnityEngine.Assertions.Assert.IsTrue(CanAttack(), $"Tried attacking though {nameof(CanAttack)} method returned false!");

        _cooldownTimer = CurrentWeapon.Cooldown;
        CurrentWeapon.PlayAttackAnimation(this, shookOnAttack);
    }

    public bool CanAttack()
    {
        return CurrentWeapon != null && _cooldownTimer <= 0f;
    }

    private void ResetCooldown()
    {
        _cooldownTimer = 0f;
    }

    private void UpdateCooldown()
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= UnityEngine.Time.deltaTime;
    }

    public void Update()
    {
        UpdateCooldown();
    }
}
