using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ProceduralMesh : MonoBehaviour
{
    /*
     * All you have to do is
     * 1. Create a clone of the mesh in init
     * 2. (optional) Save the original mesh -- this is for a reset at runtime
     * 3. If the handles are moved, 
     *      Local: Calc a cMesh
     *      Global: Save the cMesh to the oMesh
     *      Save the nMesh to the cMesh
     */
    [SerializeField]
    private NavMeshSurface navMeshSurface;

    [SerializeField]
    private NavMeshAgent navMeshAgent;

    [SerializeField]
    private GameObject vertColPrefab;

    public Vert_Col[] cols;

    public Mesh oMesh;
    public Mesh cMesh;
    public MeshFilter oMeshFilter;
    public int[] tris;
    public Vector3[] verts;

    [SerializeField]
    private float radius = 0.2f;
    [SerializeField]
    private float pull = 0.3f;

    [SerializeField]
    public List<Vector3[]> allTrisList;


    private void Start ( )
    {
        InitMesh ( );
        InitCols ( );
        navMeshSurface.BuildNavMesh ( );
        navMeshAgent.SetDestination ( destination.transform.position );
    }

    // Clone mesh filter so you don't mess up unity's.
    private void InitMesh ( )
    {
        // Store the old mesh
        oMeshFilter = GetComponent<MeshFilter> ( );
        oMesh = oMeshFilter.mesh;

        // Create a new mesh.  
        // Copy the old mesh to the new mesh.
        cMesh = new Mesh ( );
        cMesh.name = "clone";
        cMesh.vertices = oMesh.vertices;
        cMesh.triangles = oMesh.triangles;
        cMesh.normals = oMesh.normals;
        cMesh.uv = oMesh.uv;
        cMesh.RecalculateNormals ( );

        oMeshFilter.mesh = cMesh; // WHY... storage?

        verts = cMesh.vertices;
        tris = cMesh.triangles;

        print ( "cloned original MF." );
    }

    // Add sphere colliders to each point
    private void InitCols ( )
    {
        cols = new Vert_Col [ verts.Length ];
        for ( int i = 0; i < verts.Length; i++ )
        {
            GameObject temp = GameObject.Instantiate(vertColPrefab);
            cols [ i ] = temp.GetComponent<Vert_Col> ( );
            cols [ i ].index = i;
            cols [ i ].proceduralMesh = this;
            cols [ i ].transform.SetParent ( this.transform );
            cols [ i ].transform.localPosition = verts [ i ];
            cols [ i ].speed = 1.0f;
        }
    }

    // Reset the new mesh to whatever it was originally.
    private void Reset ( )
    {
        cMesh.vertices = oMesh.vertices;
        cMesh.triangles = oMesh.triangles;
        cMesh.normals = oMesh.normals;
        cMesh.uv = oMesh.uv;
        oMeshFilter.mesh = cMesh; // WHY... storage?

        verts = cMesh.vertices;
        tris = cMesh.triangles;
    }
    [SerializeField]
    private Transform destination;

    // Pulling only one vertex pt, results in broken mesh.
    private void PullOneVertex ( int index, Vector3 newPos )
    {
        verts [ index ] = newPos;
        cMesh.vertices = verts;
        cMesh.RecalculateNormals ( );
    }

    // Called from Vert_Col
    public void DoAction ( int index, Vector3 localPos )
    {
        // For testing
        // PullOneVertex ( index, localPos );

        PullSimilarVertices ( index, localPos );

        // Recalc nav mesh now... 
        navMeshSurface.BuildNavMesh ( );

        // Find the highest point
        for ( int i = 0; i < verts.Length; i++ )
        {
            if ( verts [ i ].y > destination.transform.position.y )
            {
                destination.transform.position = verts [ i ];
            }
        }

        // Set new peak destination, and send the player there.
        navMeshAgent.SetDestination ( destination.transform.position );

        // Set cols to new mesh pos...
        for ( int i = 0; i < cols.Length; i++ )
        {
            cols [ i ].transform.localPosition = verts [ i ];
        }
    }

    // Need to pull verts from any triangles that share a corner. 
    private void PullSimilarVertices ( int index, Vector3 newPos )
    {
        Vector3 targetVertexPos = verts[index];
        List<int> relatedVertices = FindRelatedVertices(targetVertexPos, false); // True lets nearby vcerts distort? 
        foreach ( int i in relatedVertices )
        {
            verts [ i ] = newPos;
        }
        cMesh.vertices = verts;
        cMesh.RecalculateNormals ( );
    }

    // Returns List of int that is related to the targetPt.
    // Optimize
    private List<int> FindRelatedVertices ( Vector3 targetPt, bool findConnected )
    {
        // list of int
        List<int> relatedVerts = new List<int>();

        int idx = 0;
        Vector3 pos;

        // loop through triangle array of indices
        for ( int t = 0; t < tris.Length; t++ )
        {
            // current idx return from tris
            idx = tris [ t ];
            // current pos of the vertex
            pos = verts [ idx ];
            // if current pos is same as targetPt
            if ( pos == targetPt )
            {
                // add to list
                relatedVerts.Add ( idx );
                // if find connected vertices
                if ( findConnected )
                {
                    // min 
                    // - prevent running out of count
                    if ( t == 0 )
                    {
                        relatedVerts.Add ( tris [ t + 1 ] );
                    }
                    // max 
                    // - prevent runnign out of count
                    if ( t == tris.Length - 1 )
                    {
                        relatedVerts.Add ( tris [ t - 1 ] );
                    }
                    // between 1 ~ max-1 
                    // - add idx from triangles before t and after t 
                    if ( t > 0 && t < tris.Length - 1 )
                    {
                        relatedVerts.Add ( tris [ t - 1 ] );
                        relatedVerts.Add ( tris [ t + 1 ] );
                    }
                }
            }
        }

        // return compiled list of int..
        return relatedVerts;
    }
}
