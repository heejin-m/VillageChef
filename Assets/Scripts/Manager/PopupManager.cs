using System.Collections.Generic;
using UnityEngine;

public class PopupManager : SingletonBehaviour<PopupManager>
{
    [SerializeField] private Transform popupRoot;

    /// <summary>
    /// 팝업 스택
    /// </summary>
    private readonly Stack<GameObject> _popupStack = new();

    /// <summary>
    /// 초기화
    /// </summary>
    public void Initialize()
    {
        this.CloseAll();
    }

    /// <summary>
    /// 팝업 열기
    /// </summary>
    /// <param name="popup"></param>
    public void OpenPopup(PopupEnum.ePopup popup)
    {
        string path = popup.GetDescription();

        GameObject prefab = Resources.Load<GameObject>(path);

        if (prefab == null)
        {
            Debug.LogError($"Popup prefab load failed: {path}");
            return;
        }

        GameObject popupObject = Instantiate(prefab, popupRoot);

        _popupStack.Push(popupObject);
    }

    /// <summary>
    /// 팝업 닫기
    /// </summary>
    public void ClosePopup()
    {
        if (_popupStack.Count <= 0)
            return;

        GameObject popupObject = _popupStack.Pop();

        if (popupObject != null)
            Destroy(popupObject);
    }

    /// <summary>
    /// 모든 팝업 닫기
    /// </summary>
    public void CloseAll()
    {
        if (_popupStack.Count == 0)
            return;

        while (_popupStack.Count > 0)
        {
            GameObject popupObject = _popupStack.Pop();

            if (popupObject != null)
            {
                Destroy(popupObject);
            }
        }
    }
}