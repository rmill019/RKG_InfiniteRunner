using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{

    bool b_canStart = false;
    AsyncOperation asyncLoad;

    private void Start()
    {
        StartCoroutine(LoadLevelAsync("Prototype01"));
    }

    private void Update()
    {
        if (asyncLoad != null)
            print("Progress: " + asyncLoad.progress);
    }

    public void LoadLevel (string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public IEnumerator LoadLevelAsync (string sceneName)
    {
       asyncLoad = SceneManager.LoadSceneAsync(sceneName);
       asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void QuitGame ()
    {
        Application.Quit();
        Debug.Log("Player Quit Game");
    }

    public void AllowSceneActivation ()
    {
        if (asyncLoad != null)
            asyncLoad.allowSceneActivation = true;
    }
}
