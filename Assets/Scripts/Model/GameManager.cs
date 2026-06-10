using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameManager : SingletonBehaviour<GameManager>
{
    #region Inspector

    [SerializeField] private Transform playerRoot;

    #endregion

    private const string PLAYER_PREFAB_PATH = "Assets/AddressableAssets/Character/Player.prefab";
    private AsyncOperationHandle<GameObject> _playerHandle = new();
    private GameObject _playerObject;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this);

        PopupManager.Instance.Initialize();
        ModelCenter.ReleaseInstance();
    }

    public void Release()
    {
        PopupManager.Instance.Release();
        ModelCenter.ReleaseInstance();
    }

    public async Task<bool> LoadPlayer(Transform spawnPoint)
    {
        if (!_playerHandle.IsValid())
        {
            _playerHandle = Addressables.LoadAssetAsync<GameObject>(PLAYER_PREFAB_PATH);
            GameObject prefab = await _playerHandle.Task;

            if (_playerHandle.Status != AsyncOperationStatus.Succeeded || prefab == null)
            {
                return false;
            }
        }

        if (_playerObject == null)
        {
            _playerObject = Instantiate(_playerHandle.Result, playerRoot);
        }

        SetPlayerPos(spawnPoint);
        return true;
    }

    private void SetPlayerPos(Transform spawnPoint)
    {
        _playerObject.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
    }
}