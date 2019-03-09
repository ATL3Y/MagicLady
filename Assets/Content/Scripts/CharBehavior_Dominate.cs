using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharBehavior_Dominate : MonoBehaviour
{
    // Nav
    public Transform destination;
    NavMeshAgent navMeshAgent;

    // Anim
    static Animator anim;
    public float speed = 0.1f;

    void Start ( )
    {
        navMeshAgent = GetComponent<NavMeshAgent> ( );
        anim = GetComponent<Animator> ( );
        SetDestination ( );
    }

    private void SetDestination ( )
    {
        if ( destination != null )
        {
            Vector3 targetVector = destination.transform.position;
            navMeshAgent.SetDestination ( targetVector );
            anim.SetBool ( "isCatWalking", true );
            anim.SetBool ( "isStabbing", false );
        }
    }
    /*
    // Update is called once per frame
    void Update ( )
    {
        if ( Vector3.Distance ( destination.position, this.transform.position ) < 30.0f )
        {
            Vector3 direction = destination.position - this.transform.position;
            direction.y = 0.0f;

            this.transform.rotation = Quaternion.Slerp ( this.transform.rotation, Quaternion.LookRotation ( direction ), 0.1f );

            anim.SetBool ( "isIdle", false );
            if ( direction.magnitude > 5.0f )
            {
                this.transform.Translate ( 0.0f, 0.0f, speed );
                anim.SetBool ( "isCatWalking", true );
                anim.SetBool ( "isStabbing", false );
            }
            else
            {
                anim.SetBool ( "isStabbing", true );
                anim.SetBool ( "isCatWalking", false );
            }
        }
        else
        {
            anim.SetBool ( "isIdle", true );
            anim.SetBool ( "isCatWalking", false );
            anim.SetBool ( "isStabbing", false );
        }
    }
    */
}



