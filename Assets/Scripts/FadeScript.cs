using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour {
    public Image FadeColour;
    public Text LevelStartName;
    public float LevelTextDuration = 4;
    public int level;
    public void Start()
    {
        UpdateSave();
        Color fixedColor = FadeColour.color;
        fixedColor.a = 1;
        FadeColour.color = fixedColor;
        FadeColour.CrossFadeAlpha(0, 1, true);
        Invoke("FadeInText", LevelTextDuration / 4);
        Invoke("FadeOutText", LevelTextDuration / 2);
    }
    public void Fade()
    {
        Color fixedColor = FadeColour.color;
        fixedColor.a = 1;
        FadeColour.color = fixedColor;
        FadeColour.CrossFadeAlpha(0f, 0f, true);
        FadeColour.CrossFadeAlpha(1, 1, true);
    }
    public void FadeInText()
    {
        Color fixedColor = LevelStartName.color;
        fixedColor.a = 1;
        LevelStartName.color = fixedColor;
        LevelStartName.CrossFadeAlpha(1, LevelTextDuration / 2, true);
    }
    public void FadeOutText()
    {
        Color fixedColor = LevelStartName.color;
        fixedColor.a = 1;
        LevelStartName.color = fixedColor;
        LevelStartName.CrossFadeAlpha(0, LevelTextDuration/2, true);
    }
    private void UpdateSave()
    {
        StreamWriter sw = new StreamWriter(GameController.PATH + "SaveGame.txt");
        Debug.Log(GameController.PATH);
        sw.WriteLine(level);
        sw.Close();
    }
}
