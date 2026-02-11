using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 25f;
    public float damage = 10f;


    void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;
    }


    void OnTriggerEnter(Collider other)
    {
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
            enemy.TakeDamage(damage);


        Destroy(gameObject);
    }
}