using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelScript : MonoBehaviour {

    public string NextLevelName;
    public FadeScript Fader;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (NextLevelName != null)
            {
                Fader.Fade();
                
                Invoke("InvokeNextLevel", 1);
            }
            else
            {
                Debug.Log("Next level name not set");
                
            }
        }
    }
    private void InvokeNextLevel()
    {
        SceneManager.LoadScene(NextLevelName);
    }
}
