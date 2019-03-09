using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentLord : MonoBehaviour
{

    private Opponent[] opponents;

    private int index;
    private int hits = 0;
    public int Hits { get { return hits; } }


    public void Init ( int count, GameObject prefab )
    {
        opponents = new Opponent [ count ];
        index = 0;

        for ( int i = 0; i < count; i++ )
        {
            GameObject temp = Instantiate (prefab, Vector3.zero, Quaternion.identity);

            if ( temp.GetComponent<Opponent> ( ) != null )
            {

                opponents [ i ] = temp.GetComponent<Opponent> ( );
                opponents [ i ].transform.SetParent ( this.transform );
                opponents [ i ].Index = i;
                opponents [ i ].Dead = false;
                opponents [ i ].GetComponent<Renderer> ( ).material.color = new Color ( Random.Range ( .5f, 2.0f ), Random.Range ( .5f, 2.0f ), Random.Range ( .5f, 2.0f ) );
                opponents [ i ].Init ( );
            }
            else
            {
                Debug.LogError ( "Opponent component missing." );
            }
        }
    }

    public void OnBeat ( )
    {
        if ( GameLord.Instance.Ddebug )
        {
            Debug.Log ( "OpponentLord.OnBeat called." );
        }

        for ( int i = 0; i < opponents.Length; i++ )
        {
            opponents [ i ].OnBeat ( );
        }
    }

    public void ReleaseAll ( )
    {
        foreach ( Opponent o in opponents )
        {
            if ( o != null )
            {
                if ( !o.Dead )
                {
                    float hitMag = Random.Range(0.01f, 5.0f);
                    Vector3 hitDir = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), 0.0f);
                    o.gameObject.SetActive ( true );
                    o.transform.localPosition = Vector3.zero;
                    o.transform.rotation *= Quaternion.LookRotation ( hitDir );
                    o.EnableOpponent ( hitDir, hitMag );
                }
            }
            
        }

        
    }

    public void ReleaseOpponent ( Vector3 hitDir, float hitMag )
    {
        if ( GameLord.Instance.Ddebug )
        {
            Debug.Log ( "ReleaseOpponent called." );
        }

        if ( GameLord.Instance.GameState != GameLord.GameStates.Playing )
        {
            if ( GameLord.Instance.Ddebug )
            {
                Debug.LogWarning ( "Trying to release in not playing." );
            }

            return;
        }

        index++;
        if ( index > opponents.Length - 1 )
        {
            index = 0;
        }

        if ( opponents [ index ] != null )
        {
            if ( !opponents [ index ].Dead )
            {
                if ( GameLord.Instance.Ddebug )
                {
                    Debug.Log ( "Releasing opponent." );
                }

                opponents [ index ].gameObject.SetActive ( true );
                opponents [ index ].transform.localPosition = Vector3.zero;
                opponents [ index ].transform.rotation *= Quaternion.LookRotation ( hitDir );
                opponents [ index ].EnableOpponent ( hitDir, hitMag );
            }
            else
            {
                if ( GameLord.Instance.Ddebug )
                {
                    Debug.Log ( "Trying to release but opponent is dead." );
                }
            }

        }
        else
        {
            if ( GameLord.Instance.Ddebug )
            {
                Debug.LogWarning ( "ReleaseOpponent called on null opponent." );
            }

        }


    }

    public void UpdateOpponentLord ( )
    {

        for ( int i = 0; i < opponents.Length; i++ )
        {
            if ( opponents [ i ].gameObject.activeInHierarchy )
            {
                opponents [ i ].UpdateOpponent ( );
            }

        }
    }

    public void ResetOpponent ( int index )
    {
        opponents [ index ].gameObject.SetActive ( false );
        opponents [ index ].transform.position = Vector3.zero;
        opponents [ index ].transform.rotation = Quaternion.identity;
    }

    public void DisableOpponents ( )
    {
        for ( int i = 0; i < opponents.Length; i++ )
        {
            opponents [ i ].gameObject.SetActive ( false );
            opponents [ index ].transform.position = Vector3.zero;
            opponents [ index ].transform.rotation = Quaternion.identity;
        }
    }

    public void EnableOpponents ( )
    {

    }

    public void Reset ( )
    {
        hits = 0;
    }

    private void OnCollisionEnter ( Collision collision )
    {

        if ( collision.gameObject.layer == 12 )
        {
            if ( GameLord.Instance.Ddebug )
            {
                Debug.Log ( "Hit by hitbox." );
            }

            Vector3 hitDir = transform.position - collision.gameObject.transform.position;
            float hitMag = hitDir.magnitude;
            hitDir.Normalize ( );

            hits++;
            if ( GameLord.Instance.Ddebug )
            {
                Debug.Log ( "Hits: " + hits );
            }

            if ( hits > 3 )
            {
                int emphasis = 3;
                for ( int i = 0; i < emphasis; i++ )
                {
                    ReleaseAll ( );
                }

                CoHelp.Instance.DoWhen ( 3.0f, delegate { GameLord.Instance.GameState = GameLord.GameStates.Won; } );
                return;
            }

            ReleaseOpponent ( hitDir, hitMag );
        }

    }
}
