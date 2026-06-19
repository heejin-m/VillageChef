using System;
using UnityEngine;

[Serializable]
public class SceneConfig : ScriptableObject
{
    public eScene sceneType;
    public string bgmKey;
    public bool showLoadingScreen = true;

    public static SceneConfig CreateDefault(eScene sceneType)
    {
        return new SceneConfig
        {
            sceneType = sceneType,
            bgmKey = string.Empty,
            showLoadingScreen = true,
        };
    }
}
