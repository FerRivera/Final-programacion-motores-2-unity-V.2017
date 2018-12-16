using UnityEngine;
using UnityEditor;

public class ScriptableObjectsCreator
{
    public static void CreatePathConfig()
    {
        ScriptableObjectUtility.CreateAsset<PathConfig>("Resources/PathConfig");
    }

    public static void CreateVesselsConfig()
    {
        ScriptableObjectUtility.CreateAsset<VesselsSaved>("Resources/VesselsConfig");
    }

    public static void CreateSceneButtonsConfig()
    {
        ScriptableObjectUtility.CreateAsset<SceneButtonsConfig>("Resources/SceneButtonsConfig");
    }
}