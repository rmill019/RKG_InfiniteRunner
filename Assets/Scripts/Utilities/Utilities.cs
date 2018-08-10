using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities : object {

    // Randomly choose one of the given values
    public static T Choose<T>(T x, T y)
    {
        float random = Random.Range(0f, 1f);

        if (random < 0.5f)
        {
            return x;
        }
        else
        {
            return y;
        }
    }
}
