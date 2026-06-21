using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class PopupManager : SingletonBehaviour<PopupManager>
{
    [SerializeField] private Transform popupRoot;
    [SerializeField] private int popupSortingOrderStep = 10;

    /// <summary>
    /// 팝업 정책 관리용 SO
    /// </summary>
    private PopupConfig _popupConfig = null;
    private AsyncOperationHandle<PopupConfig> _handle;
    /// <summary>
    /// 팝업 스택
    /// stack: 열림 순서 관리
    /// </summary>
    private readonly Stack<OpenedPopup> _popupStack = new();
    private readonly Dictionary<ePopup, AsyncOperationHandle<GameObject>> _cachedHandles = new();
    /// <summary>
    /// 팝업 컬렉션
    /// dictionary: 인스턴스 캐시 (Instantiate 비용 절감)
    /// </summary>
    private readonly Dictionary<ePopup, OpenedPopup> _openedPopups = new();

    /// <summary>
    /// 열린 팝업 정보
    /// </summary>
    private class OpenedPopup
    {
        public ePopup PopupType;
        public PopupWindow Popup;
        public PopupConfigData Config;
        public Canvas Canvas;
    }

    /// <summary>
    /// 초기화
    /// </summary>
    public async Task Initialize()
    {
        await LoadConfigSO();
        this.CloseAll();
    }

    /// <summary>
    /// 릴리즈
    /// </summary>
    public void Release()
    {
        ReleaseConfigSO();
        this.CloseAll();
    }

    /// <summary>
    /// 팝업 정책 S.O 로드
    /// </summary>
    private async Task LoadConfigSO()
    {
        ReleaseConfigSO();

        _handle = Addressables.LoadAssetAsync<PopupConfig>("PopupConfig.asset");
        _popupConfig = await _handle.Task;

        if (_popupConfig == null)
        {
            Debug.LogError("PopupConfig not found");
            return;
        }
    }

    /// <summary>
    /// 팝업 정책 S.O 릴리즈
    /// </summary>
    private void ReleaseConfigSO()
    {
        if (_handle.IsValid())
        {
            Addressables.Release(_handle);
        }
    }

    /// <summary>
    /// 팝업 열기
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ePopup"></param>
    /// <returns></returns>
    public async Task<T> OpenPopup<T>(ePopup ePopup) where T : PopupWindow
    {
        PopupConfigData config = GetConfigOrDefault(ePopup);

        if (!_openedPopups.TryGetValue(ePopup, out var openedPopup))
        {
            var prefab = await LoadPopupPrefab(ePopup);
            if (prefab == null) return null;

            if (!_openedPopups.TryGetValue(ePopup, out openedPopup))
            {
                GameObject obj = Instantiate(prefab, popupRoot);
                T newPopup = obj.GetComponent<T>();

                if (newPopup == null)
                {
                    Debug.LogError($"Popup component missing: {typeof(T).Name}");
                    Destroy(obj);
                    return null;
                }

                openedPopup = new OpenedPopup
                {
                    PopupType = ePopup,
                    Popup = newPopup,
                    Config = config,
                };

                _openedPopups.Add(ePopup, openedPopup);
            }
            else
            {
                config = openedPopup.Config;
            }
        }
        else
        {
            openedPopup.Config = config;
        }

        T popup = openedPopup.Popup as T;
        if (popup == null)
        {
            Debug.LogError($"Popup component missing: {typeof(T).Name}");
            _openedPopups.Remove(ePopup);
            Destroy(openedPopup.Popup.gameObject);
            return null;
        }

        if (popup.IsOpen)
        {
            return popup;
        }

        ApplyNextSortingOrder(openedPopup);
        popup.ApplyConfig(openedPopup.Config, () => ClosePopup(openedPopup));
        await popup.Open();

        if (popup.IsOpen)
        {
            _popupStack.Push(openedPopup);
        }

        return popup;
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

        // 중복 로드된 경우 누수 방지
        if (_cachedHandles.TryGetValue(ePopup, out _))
        {
            Addressables.Release(handle); // 중복 핸들 즉시 해제
            return _cachedHandles[ePopup].Result;
        }

        _cachedHandles.Add(ePopup, handle);
        return prefab;
    }

    /// <summary>
    /// 최상단 팝업 닫기
    /// </summary>
    public void ClosePopup()
    {
        if (_popupStack.Count <= 0)
            return;

        OpenedPopup openedPopup = _popupStack.Pop();
        ClosePopupInternal(openedPopup);
    }

    /// <summary>
    /// 지정한 팝업이 최상단인 경우 닫기
    /// </summary>
    /// <param name="openedPopup"></param>
    private void ClosePopup(OpenedPopup openedPopup)
    {
        if (_popupStack.Count <= 0 || _popupStack.Peek() != openedPopup)
        {
            return;
        }

        _popupStack.Pop();
        ClosePopupInternal(openedPopup);
    }

    /// <summary>
    /// 팝업 닫기 처리
    /// </summary>
    /// <param name="openedPopup"></param>
    private void ClosePopupInternal(OpenedPopup openedPopup)
    {
        if (openedPopup == null || openedPopup.Popup == null)
        {
            return;
        }

        openedPopup.Popup.Close();
        ResetSortingOrder(openedPopup);

        if (openedPopup.Config.destroyOnClose)
        {
            _openedPopups.Remove(openedPopup.PopupType);
            Destroy(openedPopup.Popup.gameObject);
        }
    }

    /// <summary>
    /// 모든 팝업 닫기 및 파괴/참조해제
    /// </summary>
    public void CloseAll()
    {
        // 팝업 오브젝트 Destroy
        foreach (var openedPopup in _openedPopups.Values)
        {
            if (openedPopup?.Popup != null)
            {
                ResetSortingOrder(openedPopup);
                Destroy(openedPopup.Popup.gameObject);
            }
        }

        _openedPopups.Clear();
        _popupStack.Clear();

        ReleaseCache();
    }

    private void ApplyNextSortingOrder(OpenedPopup openedPopup)
    {
        if (openedPopup == null || openedPopup.Popup == null)
            return;

        Canvas canvas = GetOrCreateCanvas(openedPopup);
        canvas.overrideSorting = true;
        canvas.sortingOrder = GetMaxOpenedSortingOrder() + popupSortingOrderStep;
    }

    private void ResetSortingOrder(OpenedPopup openedPopup)
    {
        if (openedPopup == null || openedPopup.Popup == null)
            return;

        Canvas canvas = GetOrCreateCanvas(openedPopup);
        canvas.overrideSorting = true;
        canvas.sortingOrder = 0;
    }

    private Canvas GetOrCreateCanvas(OpenedPopup openedPopup)
    {
        if (openedPopup.Canvas != null)
            return openedPopup.Canvas;

        Canvas canvas = openedPopup.Popup.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = openedPopup.Popup.gameObject.AddComponent<Canvas>();
        }

        openedPopup.Canvas = canvas;
        return canvas;
    }

    private int GetMaxOpenedSortingOrder()
    {
        int maxSortingOrder = 0;

        foreach (var openedPopup in _openedPopups.Values)
        {
            if (openedPopup == null || openedPopup.Popup == null || !openedPopup.Popup.IsOpen)
                continue;

            Canvas canvas = openedPopup.Canvas != null ? openedPopup.Canvas : openedPopup.Popup.GetComponent<Canvas>();
            if (canvas == null)
                continue;

            maxSortingOrder = Mathf.Max(maxSortingOrder, canvas.sortingOrder);
        }

        return maxSortingOrder;
    }

    /// <summary>
    /// 팝업 정책 가져오기
    /// </summary>
    /// <param name="ePopup"></param>
    /// <returns></returns>
    private PopupConfigData GetConfigOrDefault(ePopup ePopup)
    {
        if (_popupConfig != null && _popupConfig.TryGetConfig(ePopup, out var config))
        {
            return config;
        }

        Debug.LogWarning($"PopupConfig missing: {ePopup}. Use default popup config.");
        return PopupConfigData.CreateDefault(ePopup);
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
