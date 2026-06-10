using System.Threading.Tasks;
using UnityEngine;

public class HomeWindow : FrameWindow
{
    #region Inspector

    [SerializeField] private Transform playerSpawnPoint;

    #endregion

    public override async Task<bool> OpenReady()
    {
        bool isPlayerLoaded = await LoadPlayer();


        return isPlayerLoaded && await base.OpenReady();
    }

    public override void StartProcess()
    {
        base.StartProcess();
    }

    public override void CloseProcess()
    {
        base.CloseProcess();
    }

    private async Task<bool> LoadPlayer()
    {
        // 캐릭터 로드, 이미 로드된 상태라면 위치만 보정
        var result = await GameManager.Instance.LoadPlayer(playerSpawnPoint);
        return result;
    }
}
