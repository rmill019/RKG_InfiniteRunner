using System.Collections.Generic;
using System;

[Serializable]  // Make this a struct instead of a class?
 public class PlayerHighScores
  {
     // TODO should this be private and we make a getter / setter
     private List<int> highScores;

    // Default Constructor
    public PlayerHighScores() { highScores = new List<int>(); }

    // Constructor
    public PlayerHighScores(List<int> scores) { highScores = scores; }

     public List<int> HighScores
      {
         get { return highScores; }
         set { highScores = value; }
      }
  }
