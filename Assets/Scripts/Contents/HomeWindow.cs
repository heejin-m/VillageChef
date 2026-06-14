using System.Threading.Tasks;
using UnityEngine;

public class HomeWindow : FrameWindow
{
    #region Inspector

    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private CollisionEventTrigger2D recipeBookCollider;
    [SerializeField] private CollisionEventTrigger2D bedCollider;
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

        recipeBookCollider.collisionEntered += OnRecipeEnter;
        recipeBookCollider.collisionExited += OnRecipeExit;
        bedCollider.triggerEntered += OnBedEnter;
        bedCollider.triggerExited += OnBedExit;
        doorCollider.triggerEntered += OnDoorEnter;
    }

    public override void CloseProcess()
    {
        recipeBookCollider.collisionEntered -= OnRecipeEnter;
        recipeBookCollider.collisionExited -= OnRecipeExit;
        bedCollider.triggerEntered -= OnBedEnter;
        bedCollider.triggerExited -= OnBedExit;
        doorCollider.triggerExited -= OnDoorEnter;

        base.CloseProcess();
    }

    #region ## Collider Action ##

    private async void OnRecipeEnter(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        var popup = await PopupManager.Instance.OpenPopup<RecipePopup>(ePopup.RecipePopup);
    }

    private async void OnRecipeExit(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        PopupManager.Instance.ClosePopup();
    }

    private async void OnBedEnter(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        // 저장
        //SaveManager.Save();
    }

    private async void OnBedExit(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        // 저장
        //SaveManager.Save();
    }

    private async void OnDoorEnter(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        await SceneLoadManager.Instance.SingleSceneLoad(eScene.Village);
    }

    #endregion
}