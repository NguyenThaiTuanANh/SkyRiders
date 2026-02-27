using System.Collections;
using UnityEngine;

public class TankEnemy : EnemyBase
{
    [Header("Detection")]
    public float detectionRange = 20f;
    public float bodyRotateSpeed = 3f;
    public float turretRotateSpeed = 6f;

    [Header("Attack")]
    public Transform turret;
    public Transform firePoint;
    public GameObject shellPrefab; // should use TankShell
    public float fireCooldown = 2.5f;

    private bool isReloading;

    void Update()
    {
        if (player == null || health == null || health.IsDead)
        {
            return;
        }

        Vector3 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;
        if (distance > detectionRange) return;

        // rotate tank body on horizontal plane
        Vector3 flatDir = new Vector3(toPlayer.x, 0f, toPlayer.z);
        if (flatDir.sqrMagnitude > 0.001f)
        {
            Quaternion bodyRot = Quaternion.LookRotation(flatDir.normalized, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, bodyRot, bodyRotateSpeed * Time.deltaTime);
        }

        //// rotate turret to player
        //if (turret != null)
        //{
        //    Quaternion turretRot = Quaternion.LookRotation(toPlayer.normalized, Vector3.up);
        //    turret.rotation = Quaternion.Lerp(turret.rotation, turretRot, turretRotateSpeed * Time.deltaTime);
        //}

        if (turret != null)
        {
            Vector3 dir = toPlayer.normalized;

            // -------- YAW (trục Y) --------
            Vector3 flatDir1 = new Vector3(dir.x, 0f, dir.z);

            if (flatDir1.sqrMagnitude > 0.001f)
            {
                Quaternion yawRot = Quaternion.LookRotation(flatDir1, Vector3.up);

                // bù lệch model (-90 độ ban đầu của bạn)
                yawRot *= Quaternion.Euler(0f, -90f, 0f);

                float targetYaw = yawRot.eulerAngles.y;

                // -------- PITCH (trục Z) --------
                // tính góc lên xuống theo độ cao player
                float pitchAngle = Mathf.Atan2(dir.y, flatDir.magnitude) * Mathf.Rad2Deg;

                // tùy model, thường phải đảo dấu
                //pitchAngle = -pitchAngle;

                turret.rotation = Quaternion.Lerp(
                    turret.rotation,
                    Quaternion.Euler(
                        turret.eulerAngles.x,   // giữ nguyên X
                        targetYaw,              // Y luôn hướng player
                        pitchAngle              // Z ngẩng / cúi theo player
                    ),
                    turretRotateSpeed * Time.deltaTime
                );
            }
        }

        if (!isReloading)
            StartCoroutine(FireShell());
    }

    private IEnumerator FireShell()
    {
        if (shellPrefab == null || firePoint == null) yield break;
        isReloading = true;

        SpawnShell();

        yield return new WaitForSeconds(fireCooldown);
        isReloading = false;
    }

    private void SpawnShell()
    {
        Vector3 dir = (player.position - firePoint.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
        GamePlaySoudVFX.Instance.PlayerFire();
        Instantiate(shellPrefab, firePoint.position, rot);
    }
}
