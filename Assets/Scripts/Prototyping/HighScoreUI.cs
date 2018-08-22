using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class HighScoreUI : MonoBehaviour
{
    public List<int> HighScores = new List<int>();
    public GameObject HighScoreList;
    public int m_maxNumOfScores = 8;

	// Use this for initialization
	void Start () {
        //LoadScores();
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            AddRandomNumber();
    }


    // TODO This is just a function for testing
    public void AddRandomNumber()
    {
        int num = UnityEngine.Random.Range(1, 2000);
        HighScores.Add(num);


        // Sort the scores and list them in descending order
        HighScores.Sort();
        HighScores.Reverse();

        //SaveScores();
        UpdateHighScoreList();
    }


    public void UpdateHighScoreList()
    {
        // Loop through our HighScores and assign it to the proper child Text object
        // located in our HighScoreList GameObject
        for (int i = 0; i < HighScores.Count; i++)
        {
            // Protect from out of range exception
            if (i < m_maxNumOfScores)
            {
                Text score = HighScoreList.transform.GetChild(i).transform.GetChild(1).GetComponent<Text>();
                score.text = HighScores[i].ToString();
            }
        }

        // We only want 10 HighScores so remove any after the 10th entry
        if (HighScores.Count > m_maxNumOfScores)
        {
            for (int i = m_maxNumOfScores - 1; i < HighScores.Count; i++)
            {
                HighScores.RemoveAt(i);
            }
        }
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(1000, 500, 300, 80), "Save"))
            SaveScores();

        if (GUI.Button(new Rect(1000, 250, 300, 80), "Load"))
            LoadScores();

    }

    public void SaveScores()
    {
        string filePath = Application.persistentDataPath + "\\HighScores.dat";
        
        // Save the data
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(filePath);    // What if the file exists?
        PlayerHighScores data = new PlayerHighScores(HighScores);
        bf.Serialize(file, data);

        file.Close();
    }

    public void LoadScores ()
    {
        // If this changes then the SaveScores Path should change as well
        string filePath = Application.persistentDataPath + "\\HighScores.dat";
        // Make sure our file exists
        if (File.Exists(filePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filePath, FileMode.Open);
            PlayerHighScores data = (PlayerHighScores)bf.Deserialize(file);
            file.Close();

            // Loop through the list in our PlayerHighScores data object and save that to our current
            // HighScores List
            HighScores = data.HighScores;

            // Once loaded update our UI
            UpdateHighScoreList();
        }
        else
            Debug.LogWarning("Could not find the following file: " + filePath);
        
    }
}

/*
 * Following is a container class to hold the players High score information when
 * we Serialize and Deserialize it
 */
 [Serializable]
 class PlayerHighScores
{
    // TODO should this be private and we make a getter / setter
    private List<int> highScores;

    // Constructor
    public PlayerHighScores(List<int> scores) { highScores = scores; }

    public List<int> HighScores
    {
        get { return highScores; }
        set { highScores = value; }
    }
}
    
