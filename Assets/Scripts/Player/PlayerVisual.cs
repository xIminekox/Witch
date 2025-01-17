using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private const string IS_RUNNING = "IsRunning";
    private const string IS_DIE = "IsDie";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Player.Instance.OnPlayerDeath += Instance_OnPlayerDeath;
    }

    private void Instance_OnPlayerDeath(object sender, System.EventArgs e)
    {
        animator.SetBool(IS_DIE, true);
    }

    private void Update()
    {
        if (Player.Instance.IsAlive()) 
        {
            AdjustPlayerFacingDirection();
        }

        animator.SetBool(IS_RUNNING, Player.Instance.IsRunning());
    }

    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePose = GameInput.Instance.GetMousePosition();
        Vector3 playerPosition = Player.Instance.GetPlayerPosition();

        if (mousePose.x < playerPosition.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }
}
