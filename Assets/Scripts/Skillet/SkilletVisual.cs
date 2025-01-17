using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SkilletVisual : MonoBehaviour
{
    [SerializeField] private EnemyAI _enemyAI;
    [SerializeField] private EnemyEntity _enemyEntity;
    private Animator _animator;

    private const string IS_RUNNING = "IsRunning";
    private const string CHASING_SPEED_MULTIPLIER = "ChasingSpeedMultiplier";
    private const string ATTACK = "Attack";
    private const string TAKE_HIT = "TakeHit";
    private const string IS_DIE = "IsDie";


    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _enemyEntity.OnTakeHit += _enemyEntity_OnTakeHit;
        _enemyEntity.OnDeath += _enemyEntity_OnDeath;

        _enemyAI.OnEnemyAttack += _enemyAI_OnEnemyAttack;
    }

    private void OnDestroy()
    {
        _enemyAI.OnEnemyAttack -= _enemyAI_OnEnemyAttack;
    }

    private void Update()
    {
        _animator.SetBool(IS_RUNNING, _enemyAI.IsRunning);
        _animator.SetFloat(CHASING_SPEED_MULTIPLIER, _enemyAI.GetRoamingAnimationSpeed());
    }

    public void TriggerAttackAnimationTurnOff()
    {
        _enemyEntity.PolygonColliderTurnOff();
    }

    public void TriggerAttackAnimationTurnOn()
    {
        _enemyEntity.PolygonColliderTurnOn();
    }

    private void _enemyAI_OnEnemyAttack(object sender, System.EventArgs e)
    {
        _animator.SetTrigger(ATTACK);
    }

    private void _enemyEntity_OnDeath(object sender, System.EventArgs e)
    {
        _animator.SetBool(IS_DIE, true);
    }

    private void _enemyEntity_OnTakeHit(object sender, System.EventArgs e)
    {
        _animator.SetTrigger(TAKE_HIT);
    }
}
