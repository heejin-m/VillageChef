using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    private void Awake()
    {
        if (GameManager.Instance != null && GameManager.Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this);
        ModelCenter.ReleaseInstance();
    }

    public void Release()
    {
        ModelCenter.ReleaseInstance();
    }
}