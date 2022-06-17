using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramerateControllerScript : MonoBehaviour
{
    public int Framerate;
    public bool UseFramerate;
    [Range(0, 2)] public int Vsync;


    private void Awake()
    {
    #if UNITY_EDITOR
        if (UseFramerate) QualitySettings.vSyncCount = 0;
        else QualitySettings.vSyncCount = Vsync;
        Application.targetFrameRate = Framerate;
    #endif
    }
}
