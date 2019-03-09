using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CoHelp : MonoBehaviour
{
    private static CoHelp instance;
    public static CoHelp Instance
    {
        get
        {
            if ( !instance )
            {
                instance = new GameObject ( ).AddComponent<CoHelp> ( );
                DontDestroyOnLoad ( instance.gameObject );
            }
            return instance;
        }
    }
}

public static class CoHelpExtensions
{
    public static void SafeCall ( this Action action )
    {
        if ( action != null )
        {
            action ( );
        }
    }

    public static void DoWhen ( this MonoBehaviour mono, Func<bool> condition, Action callback )
    {
        mono.StartCoroutine ( Co_DoWhen ( condition, callback ) );
    }

    public static IEnumerator Co_DoWhen ( Func<bool> condition, Action callback )
    {
        while ( !condition ( ) )
        {
            yield return null;
        }
        callback.SafeCall ( );
    }

    public static void DoWhen ( this MonoBehaviour mono, float delay, Action callback )
    {
        mono.StartCoroutine ( Co_DoWhen ( delay, callback ) );
    }

    public static IEnumerator Co_DoWhen ( float delay, Action callback )
    {
        yield return new WaitForSeconds ( delay );
        callback.SafeCall ( );
    }
}
