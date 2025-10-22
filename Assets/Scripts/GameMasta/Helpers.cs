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
    public static ConnectionPoint.SideEnum DetermineSide(Vector3 localPosition, int roomWidth, int roomHeight, float tolerance = 0.5f)
    {
        if (localPosition.x >= roomWidth / 2 - tolerance && localPosition.x <= roomWidth / 2 + tolerance)
            return ConnectionPoint.SideEnum.East;
        if (localPosition.x >= -roomWidth / 2 - tolerance && localPosition.x <= -roomWidth / 2 + tolerance)
            return ConnectionPoint.SideEnum.West;
        if (localPosition.y >= roomHeight / 2 - tolerance && localPosition.y <= roomHeight / 2 + tolerance)
            return ConnectionPoint.SideEnum.North;
        if (localPosition.y >= -roomHeight / 2 - tolerance && localPosition.y <= -roomHeight / 2 + tolerance)
            return ConnectionPoint.SideEnum.South;
        return ConnectionPoint.SideEnum.None;
    }
}