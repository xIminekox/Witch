using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[SelectionBase]
public class Player : MonoBehaviour {

    public static Player Instance { get; private set; }
    public event EventHandler OnPlayerDeath;

    [SerializeField] private float movingSpeed = 5f;
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private float damageRecoveryTime = 0.5f;

    Vector2 inputVector;

    private Rigidbody2D rb;
    private KnockBack knockBack;

    private float minMovingSpeed = 0.1f;
    private bool isRunning = false;

    private int currentHealth;
    private bool canTakeDamage;
    private bool isAlive;

    public GameObject bullet;

    private void Awake() {
        Instance = this;
        rb = GetComponent<Rigidbody2D>(); 
        knockBack = GetComponent<KnockBack>();
    }

    private void Start() 
    {
        currentHealth = maxHealth;
        canTakeDamage = true;
        isAlive = true;
        GameInput.Instance.OnPlayerAttack += GameInput_OnPlayerAttack;
    }

    private void Update()
    {
        inputVector = GameInput.Instance.GetMovementVector();
    }

    private void FixedUpdate()
    {
        if (knockBack.IsGettingKnockBack)
        {
            return;
        }
        
        HandleMovement();
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    public Vector3 GetPlayerPosition()
    {
        Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        return playerScreenPosition;
    }

    public void TakeDamage(Transform damageSource, int damage)
    {
        if (canTakeDamage && isAlive)
        {
            canTakeDamage = false;
            currentHealth = Mathf.Max(0, currentHealth -= damage);
            knockBack.GetKnockedBack(damageSource);

            StartCoroutine(DamageRecoveryRoutine());
        }

        DetectDeath();
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    private void DetectDeath()
    {
        if(currentHealth == 0 && isAlive)
        {
            isAlive = false;
            knockBack.StopKnockBackMovement();
            GameInput.Instance.DisableMovement();

            OnPlayerDeath?.Invoke(this, EventArgs.Empty);
        }
    }

    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }

    private void GameInput_OnPlayerAttack(object sender, System.EventArgs e)
    {
        Instantiate(bullet, transform.position, Quaternion.identity);
    }

    private void HandleMovement()
    {
        rb.MovePosition(rb.position + inputVector * (movingSpeed * Time.fixedDeltaTime));

        if (Mathf.Abs(inputVector.x) > minMovingSpeed || Mathf.Abs(inputVector.y) > minMovingSpeed)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }
    }
}
