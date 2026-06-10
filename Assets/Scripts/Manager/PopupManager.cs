using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static PopupEnum;

public class PopupEntry
{
    private ePopup _ePopup;
    private GameObject _popupObj;
    private AsyncOperationHandle<GameObject> _handle;

    public ePopup GetPopupType() => _ePopup;
    public GameObject GetObj() => _popupObj;
    public AsyncOperationHandle<GameObject> GetHandle() => _handle;

    public PopupEntry(ePopup ePopup, GameObject popupObj, AsyncOperationHandle<GameObject> handle) : base()
    {
        _ePopup = ePopup;
        _popupObj = popupObj;
        handle = _handle;
    }
}

public class PopupManager : SingletonBehaviour<PopupManager>
{
    [SerializeField] private Transform popupRoot;

    /// <summary>
    /// 팝업 스택
    /// </summary>
    private readonly Stack<PopupEntry> _popupStack = new();

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
    /// <typeparam name="T"></typeparam>
    /// <param name="ePopup"></param>
    /// <returns></returns>
    public async Task<T> OpenPopup<T>(ePopup ePopup) where T : PopupWindow
    {
        string path = ePopup.GetDescription();
        var handle = Addressables.LoadAssetAsync<GameObject>(path);

        GameObject prefab = await handle.Task;
        if (handle.Status != AsyncOperationStatus.Succeeded || prefab == null)
        {
            Debug.LogError($"Popup prefab load failed: {path}");

            if (handle.IsValid())
            {
                handle.Release();
            }

            return null;
        }

        GameObject obj = Instantiate(prefab, popupRoot);
        _popupStack.Push(new PopupEntry(ePopup, obj, handle));

        T popup = obj.GetComponent<T>();
        popup.Open();

        return popup;
    }

    /// <summary>
    /// 최상단 팝업 닫기
    /// </summary>
    public void ClosePopup()
    {
        if (_popupStack.Count <= 0)
            return;

        // 팝업 오브젝트 Destroy
        PopupEntry entry = _popupStack.Pop();
        if (entry != null)
        {
            Destroy(entry.GetObj());
        }

        // 핸들 Release
        var handle = entry.GetHandle();
        if (handle.IsValid())
        {
            handle.Release();
        }
    }

    /// <summary>
    /// 모든 팝업 닫기
    /// </summary>
    public void CloseAll()
    {
        if (_popupStack.Count == 0)
            return;

        // 팝업 오브젝트 Destroy
        while (_popupStack.Count > 0)
        {
            PopupEntry entry = _popupStack.Pop();

            if (entry != null)
            {
                Destroy(entry.GetObj());

                // 핸들 Release
                var handle = entry.GetHandle();
                if (handle.IsValid())
                {
                    handle.Release();
                }
            }
        }
    }
}