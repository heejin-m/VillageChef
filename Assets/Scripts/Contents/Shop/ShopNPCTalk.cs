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

    public List<TalkStruct> talkList;

    #endregion

    [Serializable]
    public struct TalkStruct
    {
        public eNPCTalk eNPCTalk;
        public string talk;
    }
}