using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    #region Insepctor

    public Image img;
    public TMP_Text cnt;

    #endregion

    public void Set(InventoryItemInfo info)
    {
        switch (info.InventoryItem.ItemType)
        {
            case eInventoryItemType.Ingredient:
            case eInventoryItemType.Dish:
            case eInventoryItemType.Recipe:
                AtlasLoadManager.SetImageSprite(img, eAtlas.FoodUI, info.InventoryItem.resourceName);
                break;
            default:
                break;
        }
    }

    public void SetCnt(int amount)
    {
        cnt.text = amount.ToString();
    }
}