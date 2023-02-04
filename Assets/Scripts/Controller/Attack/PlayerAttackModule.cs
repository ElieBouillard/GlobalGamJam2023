public class PlayerAttackModule
{
    public Weapon CurrentWeapon { get; private set; }

    public bool SetWeapon(Weapon weapon)
    {
        if (CurrentWeapon != null)
            CurrentWeapon.OnUnequiped();

        CurrentWeapon = weapon;

        if (CurrentWeapon != null)
            CurrentWeapon.OnEquiped();

        return true;
    }

    public bool CanAttack()
    {
        // TODO: Check attack cooldown.
        return CurrentWeapon != null;
    }

    public void Update()
    {
    }
}
