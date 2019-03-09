using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCSimplePatrol : MonoBehaviour
{
    [SerializeField]
    private bool patrolWaiting;

    [SerializeField]
    private float totalWaitTime = 3f;

    [SerializeField]
    private float switchProbability = .2f;

    [SerializeField]
    List<Waypoint> patrolPoints;

    private NavMeshAgent navMeshAgent;
    private int currentPatrolIndex;
    private bool travelling;
    private bool waiting;
    private bool patrolForward;
    private float waitTimer;

    // Use this for initialization
    void Start ( )
    {
        navMeshAgent = GetComponent<NavMeshAgent> ( );

        if(patrolPoints != null && patrolPoints.Count >= 2 )
        {
            currentPatrolIndex = 0;
            SetDestination ( );
        }
    }

    // Update is called once per frame
    void Update ( )
    {
        if ( travelling && navMeshAgent.remainingDistance <= 1.0f )
        {
            travelling = false;

            if ( patrolWaiting )
            {
                waiting = true;
                waitTimer = 0.0f;
            }else
            {
                ChangePatrolPoint ( );
                SetDestination ( );
            }
        }

        if ( waiting )
        {
            waitTimer += Time.deltaTime;
            if(waitTimer >= totalWaitTime )
            {
                waiting = false;
                ChangePatrolPoint ( );
                SetDestination ( );
            }
        }
    }

    private void SetDestination ( )
    {
        if(patrolPoints != null )
        {
            Vector3 targetVector = patrolPoints[currentPatrolIndex].transform.position;
            navMeshAgent.SetDestination ( targetVector );
            travelling = true;

        }
    }

    private void ChangePatrolPoint ( )
    {
        if(UnityEngine.Random.Range(0.0f, 1.0f) <= switchProbability )
        {
            patrolForward = !patrolForward;
        }

        if ( patrolForward )
        {
            currentPatrolIndex = ( currentPatrolIndex + 1 ) % patrolPoints.Count;
        }else
        {
            if(--currentPatrolIndex < 0 )
            {
                currentPatrolIndex = patrolPoints.Count - 1;
            }
        }
    }
}
