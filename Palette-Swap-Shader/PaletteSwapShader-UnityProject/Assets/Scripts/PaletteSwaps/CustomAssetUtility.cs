using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CustomAssetUtility  {

#if UNITY_EDITOR
    
    public static T CreateAsset<T>(string path) where T : ScriptableObject
    {
        T asset = null;

        asset = ScriptableObject.CreateInstance<T>();

        string newPath = AssetDatabase.GenerateUniqueAssetPath(path);

        AssetDatabase.CreateAsset(asset, newPath);

        AssetDatabase.SaveAssets();

        return asset;
    }
#endif
}
