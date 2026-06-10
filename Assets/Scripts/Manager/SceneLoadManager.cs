using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using static SceneEnum;

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
    }
}