﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

	public void LoadLevel (string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame ()
    {
        Application.Quit();
        Debug.Log("Player Quit Game");
    }
}
