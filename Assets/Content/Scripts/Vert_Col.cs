using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vert_Col : MonoBehaviour
{
    public ProceduralMesh proceduralMesh;
    public int index;
    public float speed;
    private Vector3 oPos;

    private void OnEnable ( )
    {
        oPos = transform.localPosition;
    }

    /*
    private void OnCollisionStay ( Collision collision )
    {
        // Optimize
        // Move to the hand while it's colliding with you.
        if ( collision.collider.tag == "hand" )
        {
            transform.position = Vector3.MoveTowards ( transform.position, collision.collider.transform.position, speed * Time.deltaTime );
        }
    }

    private void OnCollisionExit ( Collision collision )
    {
        // When it exits, recalculate the mesh
        if ( collision.collider.tag == "hand" )
        {
            proceduralMesh.DoAction ( index, transform.localPosition );
        }
    }
    */

    private void Update ( )
    {
        // Testing without hand 
        if ( transform.localPosition != oPos && Input.GetKeyUp ( KeyCode.Space ) )
        {
            proceduralMesh.DoAction ( index, transform.localPosition );
            oPos = transform.localPosition;
        }
    }
}
