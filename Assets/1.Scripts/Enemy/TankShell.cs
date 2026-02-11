using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tank shell with AoE + VFX system
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class TankShell : MonoBehaviour
{
    [Header("Gameplay")]
    public float speed = 18f;
    public float damage = 30f;
    public float radius = 2.5f;
    public float lifeTime = 6f;

    [Header("VFX")]
    public GameObject muzzlePrefab;
    public GameObject explosionFX;
    public GameObject hitPrefab;
    public List<GameObject> trails;

    private bool exploded;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        SpawnMuzzleVFX();

        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.forward * speed * Time.fixedDeltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (exploded) 
        if (collision.gameObject.CompareTag("Tank")) { return; } ;
        DetachTrails();

        ContactPoint contact = collision.contacts[0];
        SpawnHitVFX(contact.point, contact.normal);

        Explode();
    }

    void Explode()
    {
        if (exploded) return;
        exploded = true;

        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in hits)
        {
            PlayerHealth player = hit.GetComponent<PlayerHealth>();
            // if (player != null)
            //     player.TakeDamage(damage);
        }

        //if (explosionFX != null)
        //{
        //    GameObject fx = Instantiate(explosionFX, transform.position, Quaternion.identity);
        //    Destroy(fx, 0.5f);
        //}

        Destroy(gameObject);
    }

    #region VFX

    void SpawnMuzzleVFX()
    {
        if (muzzlePrefab == null) return;

        GameObject muzzle = Instantiate(muzzlePrefab, transform.position, transform.rotation);
        var ps = muzzle.GetComponent<ParticleSystem>();

        if (ps != null)
            Destroy(muzzle, ps.main.duration);
        else
            Destroy(muzzle, 1f);
    }

    void SpawnHitVFX(Vector3 pos, Vector3 normal)
    {
        if (hitPrefab == null) return;

        Quaternion rot = Quaternion.FromToRotation(Vector3.up, normal);
        GameObject hit = Instantiate(hitPrefab, pos, rot);

        var ps = hit.GetComponent<ParticleSystem>();
        if (ps != null)
            Destroy(hit, ps.main.duration);
        else
            Destroy(hit, 1f);
    }

    void DetachTrails()
    {
        if (trails == null || trails.Count == 0) return;

        foreach (var trail in trails)
        {
            if (trail == null) continue;

            trail.transform.parent = null;
            var ps = trail.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Stop();
                Destroy(trail, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(trail, 1f);
            }
        }
    }

    #endregion

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
