using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour
{
    private NavMeshSurface[] navMeshSurfaces;
    [SerializeField]
    private NPCFindPeak npcFindPeak;

    // Should be before setting the destination. 
    public void Init ( )
    {
        navMeshSurfaces = FindObjectsOfType<NavMeshSurface> ( );
        BakeMesh ( );
    }

    // Called from procedural mesh
    public void BakeMesh ( )
    {
        for ( int i = 0; i < navMeshSurfaces.Length; i++ )
        {
            navMeshSurfaces [ i ].BuildNavMesh ( );
        }
    }

    private void Update ( )
    {
        // Only for testing
        if ( Input.GetKeyUp ( KeyCode.Space ) )
        {
            BakeMesh ( );

            // Finds new peak and sets destination
            npcFindPeak.SetPeak ( );
        }
    }
}
