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

#if UNITY_EDITOR
    private void OnValidate()
    {
        ValidatePopupList();
    }

    private void ValidatePopupList()
    {
        if (popupList == null || popupList.Count <= 0)
        {
            Debug.LogWarning($"{name}: PopupConfig list is empty.", this);
            return;
        }

        HashSet<ePopup> registeredPopups = new();
        HashSet<ePopup> duplicatedPopups = new();

        foreach (var popupConfigData in popupList)
        {
            if (popupConfigData == null)
            {
                Debug.LogWarning($"{name}: PopupConfig contains null data.", this);
                continue;
            }

            if (!registeredPopups.Add(popupConfigData.popupType))
            {
                duplicatedPopups.Add(popupConfigData.popupType);
            }
        }

        foreach (var duplicatedPopup in duplicatedPopups)
        {
            Debug.LogWarning($"{name}: Duplicated popup config: {duplicatedPopup}", this);
        }

        foreach (ePopup popupType in Enum.GetValues(typeof(ePopup)))
        {
            if (!registeredPopups.Contains(popupType))
            {
                Debug.LogWarning($"{name}: Missing popup config: {popupType}", this);
            }
        }
    }
#endif
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
