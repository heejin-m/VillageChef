using System;
using System.Collections.Generic;
using UnityEngine;

public enum eNPCTalk
{
    Sell_Talk = 0,
    Buy_Talk,
    Sad_Talk,
    Happy_Talk,
    Hello_Talk,
}

[CreateAssetMenu(fileName = "ShopNPCTalk", menuName = "Scriptable Objects/ShopNPCTalk")]
public class ShopNPCTalk : ScriptableObject
{
    #region Insepctor

    [SerializeField] private List<TalkStruct> talkList = new();

    #endregion

    private readonly Dictionary<eNPCTalk, List<string>> _talkCache = new();
    private bool _isCacheDirty = true;

    public IReadOnlyList<TalkStruct> TalkList => talkList;

    private void OnEnable()
    {
        RebuildCache();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _isCacheDirty = true;
    }
#endif

    public bool TryGetRandomTalk(eNPCTalk talkType, out string talk)
    {
        EnsureCache();

        talk = string.Empty;
        if (!_talkCache.TryGetValue(talkType, out var list) || list.Count <= 0)
        {
            return false;
        }

        talk = list[UnityEngine.Random.Range(0, list.Count)];
        return true;
    }

    public string GetRandomTalk(eNPCTalk talkType)
    {
        return TryGetRandomTalk(talkType, out var talk) ? talk : string.Empty;
    }

    private void EnsureCache()
    {
        if (_isCacheDirty)
        {
            RebuildCache();
        }
    }

    private void RebuildCache()
    {
        _talkCache.Clear();

        if (talkList == null)
        {
            _isCacheDirty = false;
            return;
        }

        foreach (var item in talkList)
        {
            if (string.IsNullOrWhiteSpace(item.talk))
            {
                continue;
            }

            if (!_talkCache.TryGetValue(item.eNPCTalk, out var list))
            {
                list = new List<string>();
                _talkCache.Add(item.eNPCTalk, list);
            }

            list.Add(item.talk);
        }

        _isCacheDirty = false;
    }

    [Serializable]
    public struct TalkStruct
    {
        public eNPCTalk eNPCTalk;
        public string talk;
    }
}
