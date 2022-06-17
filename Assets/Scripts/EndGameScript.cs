using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameScript : MonoBehaviour {
    public FadeScript Fader;
    public string NextLevelName;
    private bool mCollided = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (NextLevelName != null && mCollided == false)
        {
            mCollided = true;
            Fader.Fade();

            Invoke("InvokeNextLevel", 1);
        }
        else
        {
            Debug.Log("Next level name not set");

        }
    }
    private void InvokeNextLevel()
    {
        SceneManager.LoadScene(NextLevelName);
    }
}
