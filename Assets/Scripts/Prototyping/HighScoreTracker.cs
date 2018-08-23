using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class HighScoreTracker
{
    private bool b_canSave = false;
    // Properties
    public bool CanSave
    {
        get { return b_canSave; }
        set { b_canSave = value; }
    }

    public void SaveScores (List<int> listToSave)
    {
        string filePath = Application.persistentDataPath + "\\HighScores.dat";

        // Save the data
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(filePath);    // What if the file exists
        PlayerHighScores data = new PlayerHighScores(listToSave);
        bf.Serialize(file, data);
        file.Close();
    }

    public PlayerHighScores LoadScores ()
    {
        string filePath = Application.persistentDataPath + "\\HighScores.dat";
        // Make sure our file exists
        if (File.Exists(filePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filePath, FileMode.Open);
            PlayerHighScores data = (PlayerHighScores)bf.Deserialize(file);
            file.Close();

            return data;
        }
        else
            return null;
    }
}
