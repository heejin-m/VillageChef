using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;
using static AtlasEnum;

#region ## Reference Link ##
//https://powderinthebox.tistory.com/entry/Sprite-Atlas-%EC%8A%A4%ED%94%84%EB%9D%BC%EC%9D%B4%ED%8A%B8-%EC%95%84%ED%8B%80%EB%9D%BC%EC%8A%A4
//https://docs.unity3d.com/ScriptReference/U2D.SpriteAtlasManager.html
//https://kondeneenen.com/spriteatlas_for_aas/
//https://shibuya24.info/entry/unity-spriteatlas#%E6%B3%A8%E6%84%8F%E7%82%B9%E2%91%A0%E5%85%83%E7%94%BB%E5%83%8F%E3%82%92Resources%E3%83%95%E3%82%A9%E3%83%AB%E3%83%80%E3%81%AB%E5%85%A5%E3%82%8C%E3%81%A6%E3%81%AF%E3%83%80%E3%83%A1
//https://soowankim.github.io/2019-02-21/Using-spriteAtlas-lateBinding-in-unity/
//https://drehzr.tistory.com/1266
#endregion

/// <summary>
/// 아틀라스가 호출될 때 SpriteAtlasManager의 atlasRequested 이벤트 실행
/// 아틀라스를 한번에 로드하는 것이 아닌 필요할 때 각각 로드됨.
/// </summary>
public static class AtlasLoadManager
{
    private static Dictionary<string, AsyncOperationHandle<SpriteAtlas>> _cachedAtlas = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void RegisterSpriteAtlasManagerEvent()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode != LoadSceneMode.Single) return;
        Clear();
    }

    /// <summary>
    /// 클리어
    /// </summary>
    private static void Clear()
    {
        string[] removeIgnoreKeys = { eAtlas.CommonUI.ToString() };
        List<string> removeKeys = new List<string>();

        foreach (var item in _cachedAtlas)
        {
            if (!Array.Exists(removeIgnoreKeys, ignoreKey => ignoreKey.Equals(item.Key)))
                removeKeys.Add(item.Key);
        }

        foreach (string key in removeKeys)
        {
            if (_cachedAtlas[key].IsValid())
                Addressables.Release(_cachedAtlas[key]);
            _cachedAtlas.Remove(key);
        }
    }

    /// <summary>
    /// 아틀라스 불러오기
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static SpriteAtlas GetSpriteAtlas(eAtlas type)
    {
        string key = type.ToString();
        string path = Utils.GetDescription(type);
        if (!_cachedAtlas.TryGetValue(key, out var handle))
        {
            handle = Addressables.LoadAssetAsync<SpriteAtlas>(path);
            handle.WaitForCompletion();
            if (!_cachedAtlas.ContainsKey(key))
                _cachedAtlas.Add(key, handle);
        }
        return handle.Result;
    }

    /// <summary>
    /// 이미지 스프라이트 변경
    /// </summary>
    /// <param name="img"></param>
    /// <param name="atlasType"></param>
    /// <param name="resourceName"></param>
    public static void SetImageSprite(Image img, eAtlas atlasType, string resourceName)
    {
        if (img)
        {
            // 아틀라스 로드
            SpriteAtlas atlas = GetSpriteAtlas(atlasType);
            if (atlas)
            {
                Sprite sprite = atlas.GetSprite(resourceName);
                if (sprite)
                {
                    img.gameObject.SetActive(true);
                    img.sprite = sprite;
                }
            }
        }
    }

    public static Sprite GetSprite(eAtlas atlasType, string resourceName)
    {
        SpriteAtlas atlas = GetSpriteAtlas(atlasType);

        if (atlas == null) return null;

        return atlas.GetSprite(resourceName);
    }
}
