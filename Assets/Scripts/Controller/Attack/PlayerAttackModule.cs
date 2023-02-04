public class PlayerAttackModule
{
    public Weapon CurrentWeapon { get; private set; }

    public bool SetWeapon(Weapon weapon)
    {
        CurrentWeapon = weapon;
        return true;
    }

    public bool CanAttack()
    {
        // TODO: Check attack cooldown.
        return CurrentWeapon != null;
    }
}
