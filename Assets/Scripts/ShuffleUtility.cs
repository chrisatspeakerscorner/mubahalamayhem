using System.Collections.Generic;
using UnityEngine;

public static class ShuffleUtility
{
    // Fisher-Yates shuffle algorithm
    public static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1); // UnityEngine.Random
            // Swap elements
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}