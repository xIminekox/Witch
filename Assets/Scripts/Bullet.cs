using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private int damageAmount = 5;

    public float timeDestroy = 3f;
    public float speed = 3f;
    public Rigidbody2D rb;

    private CircleCollider2D circleCollider2D;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Vector3 diference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotateZ = Mathf.Atan2(diference.y, diference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotateZ - 90f);

        rb.linearVelocity = transform.up * speed;
        Invoke("DestroyBullet", timeDestroy);
    }

    private void Awake()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent(out EnemyEntity enemyEntity)) 
        {
            enemyEntity.TakeDamage(damageAmount); 
        }
    }

    void DestroyBullet()
    {
        Destroy(this.gameObject);
    }
}
