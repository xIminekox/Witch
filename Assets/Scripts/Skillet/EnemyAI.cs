using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using WitchAdventure.Utils;

[SelectionBase]
public class EnemyAI : MonoBehaviour {
    [SerializeField] private State startingState;
    [SerializeField] private float roamingDistanceMax = 7f;
    [SerializeField] private float roamingDistanceMin = 3f;
    [SerializeField] private float roamingTimerMax = 2f;

    [SerializeField] private bool isChasingEnemy = false;
    [SerializeField] private float chasingDistance = 4f;
    [SerializeField] private float chasingSpeedMultiplier = 2f;

    [SerializeField] private bool isAttakingEnemy = false;
    [SerializeField] private float attakingDistance = 2f;
    [SerializeField] private float attakingRate = 2f;
    private float nextAttackTime = 0f;

    private NavMeshAgent navMeshAgent;
    private State currentState;
    private float roamingTimer;
    private Vector3 roamingPosition;
    private Vector3 startingPosition;

    private float roamingSpeed;
    private float chasingSpeed;

    private float nextCheckDirectionTime = 0f;
    private float checkDirectionDuration = 0.1f;
    private Vector3 lastPosition;

    public event EventHandler OnEnemyAttack;

    public bool IsRunning
    {
        get
        {
            if (navMeshAgent.velocity == Vector3.zero)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    private enum State
    {
        Roaming,
        Idle,
        Death,
        Attacking,
        Chasing
    }

    private void Start()
    {
        startingPosition = transform.position;
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        currentState = startingState;

        roamingSpeed = navMeshAgent.speed;
        chasingSpeed = navMeshAgent.speed * chasingSpeedMultiplier;
    }

    private void Update()
    {
        StateHandler();
        MovementDirectionHandler();

    }

    public void SetDeathState()
    {
        navMeshAgent.ResetPath();
        currentState = State.Death;
    }

    private void StateHandler()
    {
        switch (currentState)
        {
            case State.Roaming:
                roamingTimer -= Time.deltaTime;
                if (roamingTimer < 0)
                {
                    Roaming();
                    roamingTimer = roamingTimerMax;
                }
                CheckCurrentState();
                break;

            case State.Death:
                break;

            case State.Chasing:
                ChasingTarget();
                CheckCurrentState();
                break;

            case State.Attacking:
                AttakingTarget();
                CheckCurrentState();
                break;

            default:
            case State.Idle:
                break;
        }
    }

    private void ChasingTarget()
    {
        navMeshAgent.SetDestination(Player.Instance.transform.position);
    }

    public float GetRoamingAnimationSpeed()
    {
        return navMeshAgent.speed / roamingSpeed;
    }

    private void CheckCurrentState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);
        State newState = State.Roaming;

        if (isChasingEnemy)
        {
            if (distanceToPlayer <= chasingDistance)
            {
                newState = State.Chasing;
            }
        }

        if (isAttakingEnemy) {
            if (distanceToPlayer <= attakingDistance) { 
                newState = State.Attacking;
            }
        }

        if (newState != currentState) 
        {
            if (newState == State.Chasing)
            { 
                navMeshAgent.ResetPath();
                navMeshAgent.speed = chasingSpeed;
            }
            else if (newState == State.Roaming)
            {
                roamingTimer = 0f;
                navMeshAgent.speed = roamingSpeed;
            }
            else if (newState == State.Attacking)
            {
                navMeshAgent.ResetPath();
            }

            currentState = newState;
        }
    }

    private void AttakingTarget()
    {
        if (Time.time > nextAttackTime)
        {
            OnEnemyAttack?.Invoke(this, EventArgs.Empty);
            nextAttackTime = Time.time + attakingRate;
        }
    }

    private void MovementDirectionHandler()
    {
        if (Time.time > nextCheckDirectionTime)
        {
            if (IsRunning)
            {
                ChangeFacingDirection(lastPosition, transform.position);
            }
            else if (currentState == State.Attacking)
            {
                ChangeFacingDirection(transform.position, Player.Instance.transform.position);
            }

            lastPosition = transform.position;
            nextCheckDirectionTime = Time.time + checkDirectionDuration;
        }
    }

    private void Roaming()
    {
        roamingPosition = GetRoamongPosition();
        navMeshAgent.SetDestination(roamingPosition);
    }

    private Vector3 GetRoamongPosition()
    {
        return startingPosition + Utils.GetRandomDir() * UnityEngine.Random.Range(roamingDistanceMin, roamingDistanceMax);
    }

    private void ChangeFacingDirection(Vector3 sourcePosition, Vector3 targetPosition)
    {
        if (sourcePosition.x > targetPosition.x)
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        } 
        else 
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
