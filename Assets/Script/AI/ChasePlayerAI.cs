using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChasePlayerAI : MonoBehaviour
{
    public Transform player;
    public float chaseRange = 50.0f;
    public float attackRange = 2.0f;

    private NavMeshAgent agent;
    private float distanceToPlayer;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();       
    }

    void ChasePlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);  
    }

    void Attack()
    {
        agent.isStopped = true;
        transform.LookAt(player);
        Debug.Log("Attacking player");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,attackRange);
    }

    void StopChasing()
    {
        agent.isStopped = true;
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            StopChasing();
        }

        if (distanceToPlayer <= attackRange)
        {
            Attack();
        }
    }
}
