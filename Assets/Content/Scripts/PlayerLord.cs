using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLord : MonoBehaviour
{

    private PlayerMovement playerMovement;
    public PlayerMovement PlayerMovement { get { return playerMovement; } }

    private bool playMode;

    public void Init ( )
    {
        playMode = false;

        if ( GetComponent<PlayerMovement> ( ) != null )
        {
            playerMovement = GetComponent<PlayerMovement> ( );
        }
        else
        {
            Debug.LogError ( "PlayerMovement component not found." );
        }


        //playerMovement.Init ( );
    }



    public void DisablePlayer ( )
    {
        playMode = false;
    }

    public void EnablePlayer ( )
    {
        playMode = true;
    }

    public void Reset ( )
    {
        transform.position = new Vector3 ( 0.0f, 0.0f, 0.0f );
        transform.rotation = Quaternion.LookRotation ( -Vector3.forward );
    }

    public void UpdatePlayerLord ( )
    {
        //playerMovement.UpdatePlayerMovement ( );

    }

    private void OnTriggerEnter ( Collider other )
    {
        // If I hit a limit, I lose.
        if ( other.gameObject.layer == 8 )
        {
            GameLord.Instance.GameState = GameLord.GameStates.Lost;
        }
    }

    private void OnCollisionEnter ( Collision collision )
    {

    }
}
