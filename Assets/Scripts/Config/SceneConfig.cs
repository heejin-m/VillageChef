using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneConfig", menuName = "Scriptable Objects/SceneConfig")]
public class SceneConfig : ScriptableObject
{
    [SerializeField] private List<SceneConfigData> sceneList = new();

    public bool TryGetConfig(eScene sceneType, out SceneConfigData config)
    {
        config = sceneList.Find(x => x.sceneType == sceneType);
        return config != null;
    }
}

[Serializable]
public class SceneConfigData
{
    public eScene sceneType;
    public string bgmKey;
    public bool showLoadingScreen = true;

    public static SceneConfigData CreateDefault(eScene sceneType)
    {
        return new SceneConfigData
        {
            sceneType = sceneType,
            bgmKey = string.Empty,
            showLoadingScreen = true,
        };
    }
}