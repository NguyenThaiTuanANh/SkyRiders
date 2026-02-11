using UnityEngine;


public enum WeaponType { Bomb, MachineGun, Rocket }


public class WeaponController : MonoBehaviour
{
    public WeaponType currentWeapon = WeaponType.Bomb;


    [Header("Spawn Points")]
    public Transform bombPoint;
    public Transform gunPoint;


    [Header("Prefabs")]
    public GameObject bombPrefab;
    public GameObject bulletPrefab;
    public GameObject rocketPrefab;


    public float fireRate = 0.15f;
    private float fireTimer;


    void Update()
    {
        fireTimer += Time.deltaTime;


        if (Input.GetButton("Fire1"))
        {
            Fire();
        }
    }


    void Fire()
    {
        if (fireTimer < fireRate) return;
        fireTimer = 0f;


        switch (currentWeapon)
        {
            case WeaponType.Bomb:
                Instantiate(bombPrefab, bombPoint.position, Quaternion.identity);
                break;


            case WeaponType.MachineGun:
                Instantiate(bulletPrefab, gunPoint.position, gunPoint.rotation);
                break;


            case WeaponType.Rocket:
                Instantiate(rocketPrefab, gunPoint.position, gunPoint.rotation);
                break;
        }
    }
}