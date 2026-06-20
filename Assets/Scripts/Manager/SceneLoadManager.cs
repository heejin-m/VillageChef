using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoadManager : SingletonBehaviour<SceneLoadManager>
{
    #region Inspector

    public GameObject loadingCanvas;

    #endregion

    private const string DEFAULT_BGM_KEY = "VillageChef_BGM";
    private const float MIN_LOADING_TIME = 1f;

    /// <summary>
    /// 씬 정책 관리용 SO
    /// </summary>
    private SceneConfig _sceneConfig = null;
    private AsyncOperationHandle<SceneConfig> _handle;
    /// <summary>
    /// 씬 관리용 handle
    /// </summary>
    private AsyncOperationHandle<SceneInstance> _currentSceneHandle = new();
    /// <summary>
    /// 씬 오디오 관리용 Handle
    /// </summary>
    private AsyncOperationHandle<AudioClip> _currentBgmHandle = new();
    private AudioSource _bgmSource;
    private string _currentBgmKey = string.Empty;

    /// <summary>
    /// 초기화
    /// </summary>
    public async Task Initialize()
    {
        await LoadConfigSO();
    }

    /// <summary>
    /// 릴리즈
    /// </summary>
    public void Release()
    {
        ReleaseConfigSO();
    }

    /// <summary>
    /// 씬 정책 S.O 로드
    /// </summary>
    private async Task LoadConfigSO()
    {
        ReleaseConfigSO();

        _handle = Addressables.LoadAssetAsync<SceneConfig>("SceneConfig.asset");
        _sceneConfig = await _handle.Task;

        if (_sceneConfig == null)
        {
            Debug.LogError("SceneConfig not found");
            return;
        }
    }

    /// <summary>
    /// 씬 정책 S.O 릴리즈
    /// </summary>
    private void ReleaseConfigSO()
    {
        if (_handle.IsValid())
        {
            Addressables.Release(_handle);
        }
    }

    /// <summary>
    /// 씬 정책 가져오기
    /// </summary>
    /// <param name="ePopup"></param>
    /// <returns></returns>
    private SceneConfigData GetConfigOrDefault(eScene eScene)
    {
        if (_sceneConfig != null && _sceneConfig.TryGetConfig(eScene, out var config))
        {
            return config;
        }

        Debug.LogWarning($"SceneConfig missing: {eScene}. Use default scene config.");
        return SceneConfigData.CreateDefault(eScene);
    }

    /// <summary>
    /// 단일 씬 로드
    /// </summary>
    /// <param name="eScene"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public async Task SingleSceneLoad(eScene eScene, SceneConfigData config = null)
    {
        config ??= GetConfigOrDefault(eScene);
        bool showLoadingScreen = config.showLoadingScreen;
        float loadingStartTime = Time.time;

        SetLoadingCanvas(showLoadingScreen);

        if (config.sceneType != eScene)
        {
            Debug.LogWarning($"SceneConfig sceneType mismatch. request: {eScene}, config: {config.sceneType}");
        }

        try
        {

            // 이전 씬 언로드
            if (_currentSceneHandle.IsValid())
            {
                await Addressables.UnloadSceneAsync(_currentSceneHandle).Task;
            }

            var handle = Addressables.LoadSceneAsync(eScene.GetDescription(), LoadSceneMode.Single);

            SceneInstance scene = await handle.Task;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }

                return;
            }

            _currentSceneHandle = handle;
            await PlayBgm(config);
            await OpenSceneWindow(scene.Scene);
        }
        finally
        {
            await WaitMinimumLoadingTime(showLoadingScreen, loadingStartTime);
            SetLoadingCanvas(false);
        }
    }

    private void SetLoadingCanvas(bool isActive)
    {
        if (loadingCanvas == null)
        {
            return;
        }

        loadingCanvas.SetActive(isActive);
    }

    private async Task WaitMinimumLoadingTime(bool showLoadingScreen, float loadingStartTime)
    {
        if (!showLoadingScreen)
        {
            return;
        }

        float elapsedTime = Time.time - loadingStartTime;
        float remainTime = MIN_LOADING_TIME - elapsedTime;
        if (remainTime > 0f)
        {
            await Task.Delay((int)(remainTime * 1000f));
        }
    }

    private async Task PlayBgm(SceneConfigData config)
    {
        string bgmKey = string.IsNullOrWhiteSpace(config?.bgmKey) ? DEFAULT_BGM_KEY : config.bgmKey;
        if (_bgmSource != null && _bgmSource.isPlaying && _currentBgmKey == bgmKey)
        {
            return;
        }

        EnsureBgmSource();

        if (_currentBgmHandle.IsValid())
        {
            _bgmSource.Stop();
            _bgmSource.clip = null;
            Addressables.Release(_currentBgmHandle);
            _currentBgmHandle = new AsyncOperationHandle<AudioClip>();
        }

        _currentBgmHandle = Addressables.LoadAssetAsync<AudioClip>(bgmKey);
        AudioClip clip = await _currentBgmHandle.Task;

        if (_currentBgmHandle.Status != AsyncOperationStatus.Succeeded || clip == null)
        {
            Debug.LogError($"BGM load failed: {bgmKey}");

            if (_currentBgmHandle.IsValid())
            {
                Addressables.Release(_currentBgmHandle);
                _currentBgmHandle = new AsyncOperationHandle<AudioClip>();
            }

            _currentBgmKey = string.Empty;
            return;
        }

        _currentBgmKey = bgmKey;
        _bgmSource.clip = clip;
        _bgmSource.loop = true;
        _bgmSource.Play();
    }

    private void EnsureBgmSource()
    {
        if (_bgmSource != null)
        {
            return;
        }

        GameObject bgmObject = new GameObject("BGM AudioSource");
        bgmObject.transform.SetParent(transform);
        _bgmSource = bgmObject.AddComponent<AudioSource>();
        _bgmSource.playOnAwake = false;
        _bgmSource.loop = true;
        _bgmSource.volume = 0.5f;
    }

    /// <summary>
    /// 씬 FrameWindow 열기
    /// </summary>
    /// <param name="scene"></param>
    /// <returns></returns>
    private async Task OpenSceneWindow(Scene scene)
    {
        PopupManager.Instance.CloseAll();

        foreach (GameObject rootObject in scene.GetRootGameObjects())
        {
            FrameWindow frameWindow = rootObject.GetComponentInChildren<FrameWindow>(true);
            if (frameWindow == null)
            {
                continue;
            }

            await frameWindow.Open();
            return;
        }

        Debug.LogWarning($"{scene.name} scene does not have a FrameWindow.");
    }
}
