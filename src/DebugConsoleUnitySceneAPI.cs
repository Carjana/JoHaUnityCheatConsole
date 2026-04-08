using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class DebugConsoleUnitySceneAPI : MonoBehaviour
{
    public Component GetComponentFromUnity(Type p)
    {
        Object[] objects = FindObjectsByType(p, FindObjectsInactive.Include, FindObjectsSortMode.None);
        if(objects.Length > 1)
        {
            Debug.LogWarning($"Found multiple components of type {p}, cheat command might not work as intended!");
        }
        if(objects.Length > 0 && objects[0] is Component component)
            return objects[0] as Component;

        if (objects.Length > 0 && objects[0] is not Component)
        {
            Debug.LogError("Found object is not a component!");
            return null;
        }
        
        Debug.LogError($"Failed to find Object of type {p} in scene");
        return null;
    }
}
