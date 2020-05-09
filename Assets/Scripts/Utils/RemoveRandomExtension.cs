using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class RemoveRandomExtension
{
    public static int RemoveRandom<T>(this List<T> list, out T element)
    {
        if (list.Count == 0)
        {
            Debug.LogError("Trying to retrieve an element from an empty list");
            element = default;
            return -1;
        }
        int random = UnityEngine.Random.Range(0, list.Count);
        element = list[random];
        list.RemoveAt(random);
        return random;
    }

}
