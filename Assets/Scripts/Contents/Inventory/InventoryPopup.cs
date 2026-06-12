using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryPopup : PopupWindow
{
    #region Insepctor

    public UITabController tabController;
    public ScrollRect scrollRect;
    public ObjectPool pool;

    #endregion

    private eInventoryItemType _currentTab = eInventoryItemType.Ingredient;
    private List<InventoryItemInfo> inventoryItemInfos = new List<InventoryItemInfo>();

    public override void Awake()
    {
        base.Awake();

        tabController.onChangeTabIndex += OnChangeTabIndex;
    }

    public override void StartProcess()
    {
        pool.Create();

        base.StartProcess();

        _currentTab = eInventoryItemType.Ingredient;
        SetData();
        UpdateUI();
    }

    public override void CloseProcess()
    {
        base.CloseProcess();
        pool.HideAll();
    }

    private void OnChangeTabIndex(ushort index)
    {
        _currentTab = (eInventoryItemType)index;
        SetData();
        UpdateUI();
    }

    private void SetData()
    {
        inventoryItemInfos = ModelCenter.Inventory.GetItemListByType(_currentTab);
    }

    private void UpdateUI()
    {
        pool.HideAll();
        if (inventoryItemInfos == null || inventoryItemInfos.Count <= 0) return;

        foreach (var info in inventoryItemInfos)
        {
            if (info.IsHave)
            {
                var item = pool.Get<InventorySlotUI>();
                item.transform.SetParent(pool.transform);
                item.transform.Initialize();
                item.gameObject.SetActive(true);
                item.Set(info);
            }
        }
    }
}