using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
public abstract class EnemyBase : MonoBehaviour
{
    [Header("Enemy Info")]
    public EnemyType enemyType = EnemyType.Soldier;
    public float maxHP = 100f;

    public Transform player;

    [Header("UI")]
    public EnemyHealthBar healthBar;

    protected HealthSystem health;
    //protected Transform player;
    protected EnemyAnimationController anim;


    protected virtual void Awake()
    {
        if (enemyType == EnemyType.Soldier) anim = GetComponent<EnemyAnimationController>();
        if (enemyType == EnemyType.Soldier && anim == null)
        { 
            Debug.Log("TTTTTTT");
        }
        health = GetComponent<HealthSystem>();
        if (health != null)
        {
            health.SetMaxHealth(maxHP, true);
        }
    }

    protected virtual void OnEnable()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        if (health != null)
        {
            health.OnHealthChanged += OnHealthChanged;
            health.OnDeath += Die;
            health.SetMaxHealth(maxHP, true);
            health.ResetHealth();
            if (healthBar != null)
                healthBar.SetHealth(maxHP);
        }

        if (EnemyManager.Instance != null)
            EnemyManager.Instance.Register(this);
    }

    protected virtual void OnDisable()
    {
        if (EnemyManager.Instance != null)
            EnemyManager.Instance.Unregister(this);

        if (health != null)
        {
            health.OnHealthChanged -= OnHealthChanged;
            health.OnDeath -= Die;
        }
    }

    protected virtual void OnHealthChanged(float current)
    {
        if (healthBar != null)
            healthBar.SetFill(current);
    }

    protected virtual void Die()
    {
        if (enemyType == EnemyType.Soldier)
        {
            anim.PlayDie();
        }
        Destroy(gameObject, 0.5f);
    }

    public virtual void TakeDamage(float amount)
    {
        if (enemyType == EnemyType.Soldier)
        {
            anim.PlayHit();
        }
        if (health != null)
            health.TakeDamage(amount);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("CharShot"))
        {
            TakeDamage(50);
        }
    }
}