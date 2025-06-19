using System.Collections.Generic;
using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public static class CloningUtils
{
    public static List<T> GetCloneOf<T>(List<ICloneable> list)
    {
        List<T> stormTrooper = new List<T>();
        foreach (ICloneable bobaFet in list)
        {
            if (bobaFet is T)
            {
                stormTrooper.Add((T)bobaFet.Clone());
            }
            else
            {
                string message = "tried casting cloned list to different type, this is a coding error and should only occur if this method was called improperly\ntried casting type: " + bobaFet.ToString() + " to: " + nameof(T);
                throw new Exception(message);
            }
        }
        return stormTrooper;
    }

    public static T[] GetCloneOf<T>(ICloneable[] array)
    {
        List<T> stormTrooper = new List<T>();
        foreach (ICloneable bobaFet in array)
        {
            if (bobaFet is T)
            {
                stormTrooper.Add((T)bobaFet.Clone());
            }
            else
            {
                string message = "tried casting cloned list to different type, this is a coding error and should only occur if this method was called improperly\ntried casting type: " + bobaFet.ToString() + " to: " + nameof(T);
                throw new Exception(message);
            }
        }
        return stormTrooper.ToArray();
    }
}
