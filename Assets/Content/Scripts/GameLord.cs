using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class GameLord : MonoBehaviour
{
    private static GameLord instance;
    public static GameLord Instance { get { return instance; } }

    [SerializeField]
    private Text text;

    [SerializeField]
    private GameObject playerPrefab;
    private PlayerLord player;
    public PlayerLord Player { get { return player; } }

    [SerializeField]
    private GameObject opponentPrefab;

    [SerializeField]
    private GameObject opponentLordPrefab;
    private OpponentLord opponentLord;
    public OpponentLord OpponentLord { get { return opponentLord; } }

    [SerializeField]
    private GameObject camPrefab;
    private Camera cam;
    public Camera Cam { get { return cam; } }

    [SerializeField]
    private GameObject environmentPrefab;

    private int opponentCount;
    private float textWait;

    public enum GameStates { Default, Playing, Lost, Won };
    public GameStates GameState;

    public bool Ddebug { get; set; }

    private float timer;
    private float timerDur = 60.0f / 20.0f;

    [SerializeField]
    private bool test = false;

    private void Start ( )
    {
        instance = this;
        textWait = 6.0f;
        opponentCount = 30;
        Ddebug = false;

        timer = timerDur;
        GameState = GameStates.Default;

        text.text = "PINATA BIRTHDAY GIRL!";

        InitPlayer ( );
        InitOpponentLord ( );
        InitCam ( );
        Instantiate ( environmentPrefab, Vector3.zero, Quaternion.identity );

        // Start game in a few seconds.
        CoHelp.Instance.DoWhen ( textWait / 2.0f, delegate { StartCoroutine ( GameLoop ( ) ); } );


    }

    private void InitPlayer ( )
    {
        GameObject temp = Instantiate ( playerPrefab, new Vector3(-2.5f, 0.0f, 0.0f), Quaternion.LookRotation ( Vector3.right ) );
        if ( temp.GetComponent<PlayerLord> ( ) != null )
        {
            player = temp.GetComponent<PlayerLord> ( );
            player.Init ( );
        }
        else
        {
            Debug.LogError ( "PlayerLord component not found." );
        }
    }

    private void InitCam ( )
    {
        Vector3 pos = -15.0f * Vector3.forward + 5.0f * Vector3.up;
        GameObject temp = Instantiate ( camPrefab, pos, Quaternion.LookRotation(Vector3.forward) );
        if ( temp.GetComponent<Camera> ( ) != null )
        {
            cam = temp.GetComponent<Camera> ( );

        }
        else
        {
            Debug.LogError ( "Camera component not found." );
        }

    }

    private void InitOpponentLord ( )
    {
        GameObject temp = Instantiate ( opponentLordPrefab );
        if ( temp.GetComponentInChildren<OpponentLord> ( ) != null )
        {
            opponentLord = temp.GetComponentInChildren<OpponentLord> ( );
            opponentLord.Init ( opponentCount, opponentPrefab );
            opponentLord.DisableOpponents ( );
        }
        else
        {
            Debug.LogError ( "OpponentLord component not found." );
        }


    }


    private void Update ( )
    {
        if ( GameState != GameStates.Playing )
        {
            return;
        }

        /*
        timer -= Time.deltaTime;
        if ( timer < 0.0f )
        {
            timer = timerDur;

            OnBeat ( );
        }
        */

        player.UpdatePlayerLord ( );
        opponentLord.UpdateOpponentLord ( );
    }

    private IEnumerator GameLoop ( )
    {
        timer = timerDur;
        GameState = GameStates.Default;

        yield return StartCoroutine ( GameStarting ( ) );

        yield return StartCoroutine ( GamePlaying ( ) );

        yield return StartCoroutine ( GameEnding ( ) );

        if ( GameState == GameStates.Won )
        {
            // Restart game in a few seconds. 
            CoHelp.Instance.DoWhen ( textWait / 2.0f, delegate { SceneManager.LoadScene ( 0 ); } );
        }
        else
        {
            // Restart game loop in a few seconds. 
            CoHelp.Instance.DoWhen ( textWait / 2.0f, delegate { StartCoroutine ( GameLoop ( ) ); } );

        }

    }

    private IEnumerator GameStarting ( )
    {

        player.Reset ( );
        player.DisablePlayer ( );

        opponentLord.Reset ( );
        opponentLord.DisableOpponents ( );

        if ( test )
        {
            opponentLord.gameObject.SetActive ( false );
        }
        text.text = "It's your birthday! Hit the pinata.";

        yield return textWait;
    }

    private IEnumerator GamePlaying ( )
    {
        GameState = GameStates.Playing;
        player.EnablePlayer ( );
        opponentLord.EnableOpponents ( );

        text.text = string.Empty;

        while ( GameState == GameStates.Playing )
        {
            yield return null;
        }
    }

    private IEnumerator GameEnding ( )
    {
        player.DisablePlayer ( );
        opponentLord.DisableOpponents ( );

        if ( GameState == GameStates.Won )
        {
            text.text = "Yaaaaay you're all grown up!";
        }
        else
        {
            text.text = "Hey don't leave your own party!  Stick around!!!";
        }

        // Make the spring still. 
        yield return textWait;
    }
}
