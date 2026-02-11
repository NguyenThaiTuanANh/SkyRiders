public class TurretEnemy : EnemyBase
{
    public EnemyWeapon weapon;


    void Update()
    {
        weapon.TryShoot();
    }
}