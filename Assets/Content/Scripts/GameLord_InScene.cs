using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class GameLord_InScene: MonoBehaviour
{
    private static GameLord_InScene instance;
    public static GameLord_InScene Instance { get { return instance; } }

    [SerializeField]
    private Text text;

    [SerializeField]
    private PlayerLord player;
    public PlayerLord Player { get { return player; } }

    [SerializeField]
    private OpponentLord opponentLord;
    public OpponentLord OpponentLord { get { return opponentLord; } }

    [SerializeField]
    private Camera cam;
    public Camera Cam { get { return cam; } }

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

        // Start game in a few seconds.
        CoHelp.Instance.DoWhen ( textWait / 2.0f, delegate { StartCoroutine ( GameLoop ( ) ); } );


    }

    private void InitPlayer ( )
    {

    }

    private void InitCam ( )
    {

    }

    private void InitOpponentLord ( )
    {
        //opponentLord.Init ( opponentCount, opponentPrefab );
    }

    private void OnBeat ( )
    {
        if ( Ddebug )
        {
            Debug.Log ( "OnBeat called." );
        }
        //player.OnBeat ( );
        //opponentLord.OnBeat ( );
    }


    private void Update ( )
    {
        if ( GameState != GameStates.Playing )
        {
            return;
        }

        timer -= Time.deltaTime;
        if ( timer < 0.0f )
        {
            timer = timerDur;

            OnBeat ( );
        }

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
