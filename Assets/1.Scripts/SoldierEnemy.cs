using System.Collections;
using UnityEngine;

public class SoldierEnemy : EnemyBase
{
    [Header("Detection")]
    public float detectionRange = 15f;
    public float rotateSpeed = 5f;

    [Header("Attack")]
    public Transform firePoint;
    public GameObject bulletPrefab; // should use EnemyProjectile
    public float fireCooldown = 2f;
    public int burstCount = 3;
    public float burstInterval = 0.25f;


    private bool isAttacking;

    void Update()
    {
        if (player == null || health == null || health.IsDead)
        {
            return;
        }

        Vector3 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;
        if (distance > detectionRange) return;

        //// face player smoothly
        //Quaternion targetRot = Quaternion.LookRotation(toPlayer.normalized, Vector3.up);
        //transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);

        RotateToPlayer(player.position);

        if (!isAttacking)
            StartCoroutine(FireBurst());
    }

    private IEnumerator FireBurst()
    {
        if (bulletPrefab == null || firePoint == null) yield break;
        isAttacking = true;

        //anim.PlayShoot();
        for (int i = 0; i < burstCount; i++)
        {
            anim.PlayShoot();
            SpawnBullet();
            Invoke("StopShoot", 0.25f);
            yield return new WaitForSeconds(burstInterval);
        }
        yield return new WaitForSeconds(fireCooldown);
        isAttacking = false;
    }

    void StopShoot()
    {
        anim.StopShoot();
    }

    private void SpawnBullet()
    {
        Vector3 dir = (player.position - firePoint.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
        Instantiate(bulletPrefab, firePoint.position, rot);
    }

    void RotateToPlayer(Vector3 playerPos)
    {
        Vector3 dir = playerPos - transform.position;

        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion lookRot = Quaternion.LookRotation(dir.normalized, Vector3.up);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRot,
            rotateSpeed * Time.deltaTime
        );
    }
}
