using System.Collections.Generic;
using UnityEngine;

public class InventoryModel : AbstractModel
{
    /// <summary>
    /// id 딕셔너리
    /// </summary>
    public Dictionary<ushort, InventoryItemInfo> _inventoryItemInfoDict = new Dictionary<ushort, InventoryItemInfo>();

    public void Set(List<InventoryItemInfo> inventoryItemInfos)
    {
    }
}