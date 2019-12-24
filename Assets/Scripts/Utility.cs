using System.Collections;
using System.Collections.Generic;

public static class Utility
{
    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        // pseudo random number generator
        System.Random prng = new System.Random(seed);
        for (int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = prng.Next(i, array.Length);
            // need to make sure it's an int
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }
        return array;
    }
}
