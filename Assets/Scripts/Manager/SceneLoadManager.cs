using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoadManager : SingletonBehaviour<SceneLoadManager>
{
    private AsyncOperationHandle<SceneInstance> _currentSceneHandle = new();

    public async Task SingleSceneLoad(eScene eScene)
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
        await OpenSceneWindow(scene.Scene);
    }

    private async Task OpenSceneWindow(Scene scene)
    {
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
