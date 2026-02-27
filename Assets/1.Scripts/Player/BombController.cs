using UnityEngine;


public class BombController : MonoBehaviour
{
    public float gravity = 20f;
    public float damage = 50f;
    public float radius = 4f;
    public GameObject explosionFX;

    private bool isBoom = true;


    private Vector3 velocity;

    private void Start()
    {
        isBoom = true;
    }

    void Update()
    {
        velocity += Vector3.down * gravity * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
    }


    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Anhbooom: " + collision);
        //if (!collision.gameObject.CompareTag("Player"))
        if (isBoom) 
        {
            Explode();
        }
    }


    void Explode()
    {
        isBoom = false;
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);


        foreach (Collider hit in hits)
        {
            //Debug.Log("Hit" + hit.tag);
            EnemyBase enemy = hit.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                Debug.Log("Hit" + hit.tag);
                Debug.Log("HitDame" + damage);
                enemy.TakeDamage(damage);
            }    
            DamageShow dameNum = hit.GetComponent<DamageShow>();
            if (dameNum != null)
            {
                Debug.Log("HitDame");
                dameNum.ShowDamage((int)damage, hit.gameObject.transform.position);
            }
        }

        GamePlaySoudVFX.Instance.BoomPlay();
        GameObject eff =  Instantiate(explosionFX, transform.position + new Vector3(0,1,0), Quaternion.identity);
        Destroy(eff, 0.3f);
        Destroy(gameObject);
    }
}