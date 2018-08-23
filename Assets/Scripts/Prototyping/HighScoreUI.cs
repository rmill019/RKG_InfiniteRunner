using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HighScoreUI : MonoBehaviour
{
    public int m_maxNumOfScores = 10;
    public GameObject HighScoreList;


    private List<int> HighScores = new List<int>();
    private PlayerHighScores playerScores;
    private HighScoreTracker m_highScoreTracker;
   
    private void Awake()
    {
        m_highScoreTracker = new HighScoreTracker();
        m_highScoreTracker.CanSave = true;

        // Initialize our HighScores List to 0
        for (int i = 0; i < m_maxNumOfScores; i++)
        {
            HighScores.Add(0);
        }
    }

    // Use this for initialization
    void Start ()
    {
        playerScores = m_highScoreTracker.LoadScores();

        if (playerScores != null)
        {
            for (int i = 0; i < playerScores.HighScores.Count; i++)
            {
                HighScores[i] = playerScores.HighScores[i];
                print(HighScores[i]);
            }
        }

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

}


    
