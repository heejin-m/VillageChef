using System.Threading.Tasks;
using UnityEngine;

public class VillageWindow : FrameWindow
{
    #region Inspector

    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private CollisionEventTrigger2D doorCollider;

    #endregion


    public override async Task<bool> OpenReady()
    {
        bool isPlayerLoaded = await LoadPlayer();
        return isPlayerLoaded && await base.OpenReady();
    }

    private async Task<bool> LoadPlayer()
    {
        // 캐릭터 로드, 이미 로드된 상태라면 위치만 보정
        var result = await GameManager.Instance.LoadPlayer(playerSpawnPoint);
        return result;
    }

    public override void StartProcess()
    {
        base.StartProcess();

        doorCollider.triggerEntered += OnDoorEnter;
    }

    public override void CloseProcess()
    {
        doorCollider.triggerExited -= OnDoorEnter;

        base.CloseProcess();
    }

    #region ## Collider Action ##

    private async void OnDoorEnter(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        _ = await PopupManager.Instance.OpenPopup<ShopPopup>(ePopup.ShopPopup);
    }

    #endregion
}