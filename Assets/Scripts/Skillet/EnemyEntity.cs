using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(EnemyAI))]
public class EnemyEntity : MonoBehaviour
{
    public event EventHandler OnTakeHit;
    public event EventHandler OnDeath;

    //[SerializeField] private int maxHealth;
    [SerializeField] private EnemySO enemySO;
    private int currentHealth;

    private PolygonCollider2D polygonCollider2D;
    private BoxCollider2D _boxCollider2D;
    private EnemyAI _enemyAI;

    private void Awake()
    {
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _enemyAI = GetComponent<EnemyAI>();
    }

    private void Start()
    {
        currentHealth = enemySO.enemyHealth;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent(out Player player)) 
        {
            player.TakeDamage(transform, enemySO.enemyDamageAmount);
        }
    }

    public void PolygonColliderTurnOff()
    {
        polygonCollider2D.enabled = false;
    }

    public void PolygonColliderTurnOn()
    {
        polygonCollider2D.enabled = true;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        OnTakeHit?.Invoke(this, EventArgs.Empty);
        DetectDeath();
    }

    private void DetectDeath()
    {
        if (currentHealth <= 0)
        {
            _boxCollider2D.enabled = false;
            polygonCollider2D.enabled = false;

            _enemyAI.SetDeathState();

            OnDeath?.Invoke(this, EventArgs.Empty);
        }
    }
}
