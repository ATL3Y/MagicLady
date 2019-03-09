using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCFindPeak : MonoBehaviour
{
    public NavMeshSurface[] navMeshSurfaces;
    public Vector3 origPeakPos;
    [SerializeField]
    private Transform destination;
    public NavMeshAgent navMeshAgent;

    public void Init ( )
    {
        navMeshSurfaces = FindObjectsOfType<NavMeshSurface> ( );
        navMeshAgent = GetComponent<NavMeshAgent> ( );
        destination.transform.position = origPeakPos;
        navMeshAgent.SetDestination ( destination.transform.position );
    }

    public void SetPeak ( )
    {
        for ( int i = 0; i < navMeshSurfaces.Length; i++ )
        {
            Mesh mesh = navMeshSurfaces [ i ].GetComponent<MeshFilter> ( ).mesh; // Could be sharedmesh in future...
            Vector3[] vertices = mesh.vertices;
            // Vector3[] normals = mesh.normals;

            for ( int j = 0; j < vertices.Length; j++ )
            {
                if ( vertices [ j ].y > destination.transform.position.y )
                {
                    destination.transform.position = vertices [ j ];
                }
            }
        }

        navMeshAgent.SetDestination ( destination.transform.position );
    }
}
