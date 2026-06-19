using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoadManager : SingletonBehaviour<SceneLoadManager>
{
    private AsyncOperationHandle<SceneInstance> _currentSceneHandle = new();
    public SceneConfig CurrentSceneConfig { get; private set; }
    public string CurrentBgmKey => CurrentSceneConfig?.bgmKey ?? string.Empty;
    public bool ShowLoadingScreen => CurrentSceneConfig?.showLoadingScreen ?? true;

    /// <summary>
    /// 단일 씬 로드
    /// </summary>
    /// <param name="eScene"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public async Task SingleSceneLoad(eScene eScene, SceneConfig config = null)
    {
        config ??= SceneConfig.CreateDefault(eScene);

        if (config.sceneType != eScene)
        {
            Debug.LogWarning($"SceneConfig sceneType mismatch. request: {eScene}, config: {config.sceneType}");
        }

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
        CurrentSceneConfig = config;
        await OpenSceneWindow(scene.Scene);
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
