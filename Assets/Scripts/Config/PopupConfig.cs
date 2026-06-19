using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PopupConfig", menuName = "Scriptable Objects/PopupConfig")]
public class PopupConfig : ScriptableObject
{
    [SerializeField] private List<PopupConfigData> popupList = new();

    public bool TryGetConfig(ePopup popupType, out PopupConfigData config)
    {
        config = popupList.Find(x => x.popupType == popupType);
        return config != null;
    }
}

[Serializable]
public class PopupConfigData
{
    public enum ePopupLayer
    {
        Normal,
        System,
        Toast,
        Loading
    }

    public ePopup popupType;
    public ePopupLayer layer;
    public bool useDim;
    public bool closeOnDimClick;
    public bool destroyOnClose;
    public int sortingOrder;

    public static PopupConfigData CreateDefault(ePopup popupType)
    {
        return new PopupConfigData
        {
            popupType = popupType,
            closeOnDimClick = false,
            destroyOnClose = false,
            useDim = true,
        };
    }
}
