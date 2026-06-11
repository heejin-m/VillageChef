using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class PopupManager : SingletonBehaviour<PopupManager>
{
    [SerializeField] private Transform popupRoot;

    /// <summary>
    /// 팝업 스택
    /// </summary>
    private readonly Stack<GameObject> _popupStack = new();
    private readonly Dictionary<ePopup, AsyncOperationHandle<GameObject>> _cachedHandles = new();

    /// <summary>
    /// 초기화
    /// </summary>
    public void Initialize()
    {
        this.CloseAll();
    }

    public void Release()
    {
        this.CloseAll();
    }

    /// <summary>
    /// 팝업 로드
    /// </summary>
    /// <param name="ePopup"></param>
    /// <returns></returns>
    private async Task<GameObject> LoadPopupPrefab(ePopup ePopup)
    {
        if (_cachedHandles.TryGetValue(ePopup, out var cachedHandle))
        {
            if (cachedHandle.IsValid())
            {
                return cachedHandle.Result;
            }

            _cachedHandles.Remove(ePopup);
        }

        string path = ePopup.GetDescription();
        var handle = Addressables.LoadAssetAsync<GameObject>(path);

        GameObject prefab = await handle.Task;

        if (handle.Status != AsyncOperationStatus.Succeeded || prefab == null)
        {
            Debug.LogError($"Popup prefab load failed: {path}");

            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }

            return null;
        }

        _cachedHandles.Add(ePopup, handle);
        return prefab;
    }

    /// <summary>
    /// 팝업 열기
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ePopup"></param>
    /// <returns></returns>
    public async Task<T> OpenPopup<T>(ePopup ePopup) where T : PopupWindow
    {
        GameObject prefab = await LoadPopupPrefab(ePopup);

        if (prefab == null)
        {
            return null;
        }

        GameObject obj = Instantiate(prefab, popupRoot);
        T popup = obj.GetComponent<T>();

        if (popup == null)
        {
            Debug.LogError($"Popup component missing: {typeof(T).Name}");
            Destroy(obj);
            return null;
        }

        _popupStack.Push(obj);

        await popup.Open();

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
        GameObject obj = _popupStack.Pop();
        if (obj != null)
        {
            Destroy(obj);
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
            GameObject obj = _popupStack.Pop();

            if (obj != null)
            {
                Destroy(obj);
            }
        }

        ReleaseCache();
    }

    /// <summary>
    /// 캐시 Release
    /// </summary>
    public void ReleaseCache()
    {
        foreach (var handle in _cachedHandles.Values)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }

        _cachedHandles.Clear();
    }
}