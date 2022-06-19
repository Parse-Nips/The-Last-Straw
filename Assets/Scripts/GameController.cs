using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public static string PATH;
    public static bool DEBUGMODE = true;
    public static bool IS_MOBILE;

    public bool DebugMode;

    private void Awake()
    {
        DEBUGMODE = DebugMode;
        PATH = Application.persistentDataPath;

        IS_MOBILE = Application.isMobilePlatform;
    }
}
