using UnityEngine;


public class EnemyWeapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float fireRate = 1f;
    public float range = 15f;


    private float timer;
    private Transform player;


    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }


    public void TryShoot()
    {
        if (Vector3.Distance(transform.position, player.position) > range) return;


        timer += Time.deltaTime;
        if (timer < fireRate) return;


        timer = 0f;
        Instantiate(bulletPrefab, transform.position, transform.rotation);
    }
}