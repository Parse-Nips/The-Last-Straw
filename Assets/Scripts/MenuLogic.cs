using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLogic : MonoBehaviour {
    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            MainMenuButton();
        }
    }
    public void StartButton()
    {
        Debug.Log("Starting the Game (Intro)");
        SceneManager.LoadScene("Intro-Cinematic", LoadSceneMode.Single);
    }

	public void CreditsButton()
    {
        Debug.Log("Going to Credits");
        SceneManager.LoadScene("Credits", LoadSceneMode.Single);
    }

	public void QuitButton()
    {
        Debug.Log("Exiting");
        Application.Quit();
    }
    
    public void MainMenuButton()
    {
        Debug.Log("Going to Main Menu");
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    public void ResumeButton()
    {
        try
        {
            StreamReader sr = new StreamReader(GameController.PATH + "SaveGame.txt");
            int level;
            level = int.Parse(sr.ReadLine());
            sr.Close();
            Debug.Log("Resuming level " + level);
            SceneManager.LoadScene("Level " + level, LoadSceneMode.Single);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            StartButton();
        }
    }
}
