using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreTracker : MonoBehaviour
{
    public List<IntVariable> HighScoreScriptObjs = new List<IntVariable>();
    public List<int> HighScores = new List<int>();
    public GameObject HighScoreList;
    public int m_maxNumOfScores = 8;

    private int num = -1;

	// Use this for initialization
	void Start () {
        PopulateHighScoreList();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
            AddRandomNumber();
	}

    public void AddRandomNumber()
    {
        int num = Random.Range(1, 2000);
        print("Num: " + num);
        HighScores.Add(num);
        HighScores.Sort();
        HighScores.Reverse();
        UpdateHighScoreList();
        StoreInScriptObj();
    }

    // This method does not persist once the game has been turned off. We need to serialize the data
    // and deserialize on Start and populate our list
    public void StoreInScriptObj()
    {
        for (int i = 0; i < HighScores.Count; i++)
        {
            HighScoreScriptObjs[i].Value = HighScores[i];
        }
    }

    public void PopulateHighScoreList ()
    {
        for (int i = 0; i < HighScoreScriptObjs.Count; i++)
        {
            Text scoreText = HighScoreList.transform.GetChild(i).GetChild(1).GetComponent<Text>();
            scoreText.text = HighScoreScriptObjs[i].Value.ToString();
        }
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
}
