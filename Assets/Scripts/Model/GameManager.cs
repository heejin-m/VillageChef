public class GameManager : SingletonBehaviour<GameManager>
{
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
}