using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float jumpForce = 36.0f;

    public float accel = 20.0f;
    public float maxSpeed = 12.0f;
    public float deccel = 20.0f;

    [SerializeField]
    private Animator anim;
    private int jumpHash;
    private int jumpLayer;
    private int punchHash;
    private int punchLayer;
    private Rigidbody rb;

    private Quaternion ogQuat;


    private bool isOnGround;

    private Vector3 velocity = Vector3.zero;
    private float oAnimSpeed=0.0f;
    [SerializeField]
    private float animChangeSpeedMult = 1.3f;

    // [SerializeField]
    // private GameObject hitBox;

    public void Start ( )
    {
        // anim = GetComponent<Animator> ( );
        rb = GetComponent<Rigidbody> ( );
        ogQuat = transform.rotation;


        jumpLayer = anim.GetLayerIndex ( "Jump" );
        jumpHash = Animator.StringToHash ( "Jump" );

        //if ( GameLord.Instance.Ddebug )
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(jumpLayer);
            Debug.Log ( "Jump Layer: " + jumpLayer );
            Debug.Log ( "Jump Hash: " + jumpHash );
        }

        punchLayer = anim.GetLayerIndex ( "Punch" );
        punchHash = Animator.StringToHash ( "Punch" );

        //if ( GameLord.Instance.Ddebug )
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo ( punchLayer );
            Debug.Log ( "Punch Layer: " + punchLayer );
            Debug.Log ( "Punch Hash: " + punchHash );
        }
    }

    public bool AmPunching ( )
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(punchLayer);
        if ( stateInfo.IsName ( "Punch.Punch" ) )
        // if ( stateInfo.shortNameHash == punchHash )
        {
            //if ( GameLord.Instance.Ddebug )
            {
                Debug.Log ( "In punch state" );
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Update ( )
    {
        if ( Input.GetKeyDown ( KeyCode.E ) )
        {
            anim.SetTrigger ( punchHash );
        }
        /*
        if ( AmPunching ( ) )
        {
            hitBox.SetActive ( true );
        }
        else
        {
            hitBox.SetActive ( false );
        }
        */

        if ( Input.GetKeyDown ( KeyCode.Space ) )
        {
            velocity.y = jumpForce;
            isOnGround = false;
            anim.SetTrigger ( jumpHash );
        }

        // Simulate momentum.
        float hInput = Input.GetAxisRaw("Horizontal");
        float vInput = Input.GetAxisRaw("Vertical");

        Vector3 accel = new Vector3(0.0f, 0.0f, 0.0f);
        accel.x = hInput * this.accel;
        accel.y = Physics.gravity.y;
        accel.z = vInput * this.accel;

        velocity.x = Mathf.Lerp ( velocity.x, 0.0f, deccel * Time.deltaTime );
        velocity.z = Mathf.Lerp ( velocity.z, 0.0f, deccel * Time.deltaTime );

        velocity += accel * Time.deltaTime;
        velocity.x = Mathf.Clamp ( velocity.x, -maxSpeed, maxSpeed );
        velocity.z = Mathf.Clamp ( velocity.z, -maxSpeed, maxSpeed );

        transform.position += velocity * Time.deltaTime;

        // Update animation.
        float animSpeed = Mathf.Abs(velocity.x + velocity.z);
        animSpeed = Mathf.Lerp ( oAnimSpeed, animSpeed, animChangeSpeedMult * Time.deltaTime );
        animSpeed = Mathf.Clamp01 ( animSpeed );
        anim.SetFloat ( "Speed", animSpeed );
        oAnimSpeed = animSpeed;
        // Debug.Log ( animSpeed );


        // HACKL3Y: Add raycast ground and transition from jump to root anim.
        if ( transform.position.y <= 0.0f )
        {
            velocity.y = 0.0f;
            isOnGround = true;
            transform.position = new Vector3 ( transform.position.x, 0.0f, transform.position.z );
        }
        else
        {
            isOnGround = false;
        }

        // HACKL3Y: do this for real, on a curve that eases in and out. 
        float dead = 0.1f;
        if ( hInput > dead || hInput < -dead || vInput > dead || vInput < -dead )
        {
            transform.rotation = Quaternion.Slerp ( transform.rotation, Quaternion.LookRotation ( new Vector3 ( velocity.x, 0.0f, velocity.z ) ), animChangeSpeedMult * Time.deltaTime );

        }

    }
}
