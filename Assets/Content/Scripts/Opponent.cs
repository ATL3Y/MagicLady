using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : MonoBehaviour
{
    private float speed = 15.0f;
    public int Index { get; set; }
    private float timerDur = 0.5f;
    private float timer;
    private Renderer rend;
    private Rigidbody rb;
    public bool Dead { get; set; }
    private Color ogColor;

    public void OnBeat ( )
    {
        timer = timerDur;
    }

    public void Init ( )
    {
        if ( GetComponent<Renderer> ( ) != null )
        {

            rend = GetComponent<Renderer> ( );
            ogColor = rend.material.color;
        }
        else
        {
            Debug.LogError ( "Renderer component missing." );
        }

        if ( GetComponent<Rigidbody> ( ) != null )
        {

            rb = GetComponent<Rigidbody> ( );
        }
        else
        {
            Debug.LogError ( "Rigidbody component missing." );
        }


    }

    public void UpdateOpponent ( )
    {
        timer -= Time.deltaTime;
        if ( timer > 0.0f )
        {
            rend.material.color = new Color ( Random.Range ( .5f, 2.0f ), Random.Range ( .5f, 2.0f ), Random.Range ( .5f, 2.0f ) );
        }
        else
        {
            rend.material.color = ogColor;
        }


        if ( transform.position.y < -10.0f )
        {
            if ( GameLord.Instance.Ddebug )
            {
                Debug.Log ( "Resetting opponent " + Index + " due to ground bound." );
            }
            GameLord.Instance.OpponentLord.ResetOpponent ( Index );
        }
    }

    public void EnableOpponent ( Vector3 hitDir, float hitMag )
    {
        AddForce ( hitDir, hitMag );
    }

    private void AddForce(Vector3 hitDir, float hitMag )
    {
        hitMag *= 10.0f;
        hitDir += Vector3.up;

        // Flatten movement so opponents always hit the walls. 
        hitDir = new Vector3 ( hitDir.x, hitDir.y, 0.0f );

        hitDir.Normalize ( );
        rb.AddForce ( hitDir * hitMag, ForceMode.Impulse );
    }

    private void OnCollisionEnter ( Collision collision )
    {
        // If I hit a wall, bounce and reset.
        if ( collision.gameObject.layer == 8 )
        {
            Vector3 hitDir = transform.position - collision.gameObject.transform.position;

            AddForce ( hitDir, 1.0f );
            CoHelp.Instance.DoWhen ( 3.0f, delegate { GameLord.Instance.OpponentLord.ResetOpponent ( Index ); } );
        }

    }
}
