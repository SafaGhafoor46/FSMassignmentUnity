using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    public enum ENEMY_STATE { PATROL, CHASE, ATTACK };
    [SerializeField] private ENEMY_STATE currentState;
    [SerializeField] public ENEMY_STATE CurrentState
    {
        get { return currentState; }
        set {
            currentState = value;
            StopAllCoroutines();
            switch (currentState) 
            {
                case ENEMY_STATE.PATROL:
                    StartCoroutine(EnemyPatrol());
                    break;
                case ENEMY_STATE.ATTACK:
                    StartCoroutine(EnemyAttack());
                    break;
                case ENEMY_STATE.CHASE:
                    StartCoroutine(EnemyChase());
                    break;
            }
        }
    }
    private CheckMyVision checkMyVision = null;
    private NavMeshAgent agent = null;
    private Transform playerTransform = null;
    private Transform patrolDestination = null;
    public Health playerHealth = null;
    private float maxDamage = 10f;
    private void Awake()
    {
        checkMyVision = GetComponent<CheckMyVision>();
        agent = GetComponent<NavMeshAgent>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        playerTransform = playerHealth.GetComponent<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] destinations = GameObject.FindGameObjectsWithTag("Dest");
        patrolDestination = destinations[Random.Range(0, destinations.Length)].GetComponent<Transform>();
        CurrentState = ENEMY_STATE.PATROL;
    }
    public IEnumerator EnemyPatrol()
    {
        while (currentState == ENEMY_STATE.PATROL)
        {
            checkMyVision.sensitivity = CheckMyVision.enumSensitivity.High;
            agent.isStopped = false;
            agent.SetDestination(patrolDestination.position);
            while (agent.pathPending)
            {
                yield return null;
            }
            if (checkMyVision.targetInSight)
            {
                agent.isStopped = true;
                CurrentState = ENEMY_STATE.CHASE;
                yield break;
            }
             yield break;
        }
    }
    public IEnumerator EnemyChase()
    {
        Debug.Log("Enemy Chase satrting..!!");
        while (currentState == ENEMY_STATE.CHASE)
        {
            checkMyVision.sensitivity = CheckMyVision.enumSensitivity.Low;
            agent.isStopped = false;
            agent.SetDestination(checkMyVision.lastKnownSighting);
            while (agent.pathPending)
            {
                yield return null;
            }
            if (agent.remainingDistance <= agent.stoppingDistance)
                agent.isStopped = true;
            if (!checkMyVision.targetInSight)
                CurrentState = ENEMY_STATE.PATROL;
            else
                CurrentState = ENEMY_STATE.ATTACK;
            yield break;
        }
        yield break;
    }
    public IEnumerator EnemyAttack()
    {
        Debug.Log("i am attacking..!!");
        while (currentState == ENEMY_STATE.ATTACK)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);
            while (agent.pathPending)
            {
                yield return null;
            }
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                currentState = ENEMY_STATE.CHASE;
            }
            else
                playerHealth.HealthPoints = maxDamage * Time.deltaTime;
            yield return null;
        }
        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
