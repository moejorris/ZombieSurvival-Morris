
using UnityEngine;
using UnityEngine.AI;

public class ZombieMovement : MonoBehaviour
{
    [SerializeField] Transform target;
    enum ZombieState {idle, walk, run, attack};

    public bool running;
    [SerializeField] float attackTriggerDistance = 2.0f;

    [SerializeField] float walkSpeed = 1.8f;
    [SerializeField] float runSpeed = 4.0f;

    ZombieState currentState;
    ZombieState previousState = ZombieState.attack; //zombies are never attacking on spawn, this is done so when the zombies spawn they will always be playing a sound effect

    NavMeshAgent navMeshAgent;
    [SerializeField] Animator animator;

    EnemySoundBank soundBank;

    void Awake()
    {
        soundBank = GetComponent<EnemySoundBank>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        currentState = ZombieState.walk;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent.Warp(transform.position);
    }

    void FixedUpdate()
    {
        DetermineState();
    }

    void DetermineState()
    {
        if(target == null)
        {
            currentState = ZombieState.idle;
            Debug.LogWarning("Target is null. Zombie will idle until there is a target.");
            return;
        }

        float distanceFromTarget = Vector3.Distance(transform.position, target.position);

        if(distanceFromTarget <= attackTriggerDistance)
        {            
            currentState = ZombieState.attack;
        }
        else if(distanceFromTarget > attackTriggerDistance)
        {
            currentState = running ? ZombieState.run : ZombieState.walk;
        }
        else
        {
            currentState = ZombieState.idle;
            Debug.LogError("Zombie Determine State defaulted to idle.");
        }

        HandleStates();
        previousState = currentState;
    }

    void HandleStates()
    {
        switch(currentState)
        {
            case ZombieState.idle:
            navMeshAgent.speed = 0;
            break;

            case ZombieState.attack:
            navMeshAgent.speed = 0;
            navMeshAgent.SetDestination(target.position);
            break;

            case ZombieState.walk:
            navMeshAgent.speed = walkSpeed;
            navMeshAgent.SetDestination(target.position);
            break;

            case ZombieState.run:
            navMeshAgent.speed = runSpeed;
            navMeshAgent.SetDestination(target.position);
            break;
        }
        AnimatorUpdate();
        HandleSounds();
    }

    void HandleSounds()
    {
        if(previousState != currentState)
        {
            if(currentState == ZombieState.idle || currentState == ZombieState.walk)
            {
                InvokeRepeating("TellSoundBankToPlayGroanSound", 0.5f, 5);
            }
            else if(currentState == ZombieState.run)
            {
                InvokeRepeating("TellSoundBankToPlayRunSound", 0.5f, 4);
            }
        }
    }

    void AnimatorUpdate()
    {
        animator.SetFloat("moveSpeed", navMeshAgent.velocity.magnitude);
    }

    void TellSoundBankToPlayGroanSound() //dummy functions for invoking
    {
        soundBank.PlayPatrolSound();
    }

    void TellSoundBankToPlayRunSound()
    {
        soundBank.PlayChaseSound();
    }
}
