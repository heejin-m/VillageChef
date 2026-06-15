using UnityEngine;

public class InventorySlotUI : MonoBehaviour
{
    #region Insepctor

    public ItemUI itemUI;

    #endregion

    public void Set(InventoryItemInfo info)
    {
        itemUI.Set(info);
        itemUI.SetCnt(info.Cnt);
    }
}