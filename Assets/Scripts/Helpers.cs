using System.Collections.Generic;
using UnityEngine;

public class HelperFunctions
{
    public static List<T> GetScriptableObjectsOfType<T>(string path) where T : ScriptableObject
    {
#if UNITY_EDITOR
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(T).Name, new[] { path });
        List<T> scriptableObjects = new List<T>();
        foreach (string guid in guids)
        {
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset != null)
            {
                scriptableObjects.Add(asset);
            }
        }
        return scriptableObjects;
#else
        return null;
#endif
    }
    public static ConnectionPoint.SideEnum DetermineSide(Vector3 localPosition)
    {
        float absX = Mathf.Abs(localPosition.x);
        float absY = Mathf.Abs(localPosition.y);
        if (absX < 1.0f)
        {
            return ConnectionPoint.SideEnum.West;
        }
        else if (absY < 1.0f)
        {
            return ConnectionPoint.SideEnum.South;
        }
        else if (absX >= absY)
        {
            return ConnectionPoint.SideEnum.East;
        }
        else
        {
            return ConnectionPoint.SideEnum.North;
        }
    }
}