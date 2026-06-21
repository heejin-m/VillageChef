using System;
using UnityEngine;

public class IngredientSupplyInfo
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 저장정보
    /// </summary>
    public IngredientSupplySaveInfo SaveInfo { get; private set; }
    /// <summary>
    /// 데이터
    /// </summary>
    public IngredientsSupply IngredientsSupply { get; private set; }
    /// <summary>
    /// 수급되는 재료 아이템 데이터
    /// </summary>
    public InventoryItem InventoryItem { get; private set; }
    /// <summary>
    /// 수급처
    /// </summary>
    public eIngredientSupplyType SupplyType => this.IngredientsSupply.SupplyType;
    /// <summary>
    /// 수급되는 재료 Id
    /// </summary>
    public int IngredientId => this.IngredientsSupply.inventoryItemId;
    /// <summary>
    /// 수급되는 기본 재료량
    /// </summary>
    public int BaseAmount => this.IngredientsSupply.amount;
    /// <summary>
    /// 시간 계산 후 수급되는 재료량
    /// </summary>
    public int Amount => this.IngredientsSupply.amount * GetAmountStack();
    /// <summary>
    /// 수급되는 재료 이름
    /// </summary>
    public string ItemName => InventoryItem.name;

    /// <summary>
    /// 쿨타임 지났는지 판단
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public bool IsOverCoolTime()
    {
        double elapsedSeconds = (DateTime.Now - SaveInfo.LastSupplyTime).TotalSeconds;
        return elapsedSeconds >= IngredientsSupply.coolTime;
    }
    /// <summary>
    /// 쿨타임 몇 회 지났는지 판단.
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public int GetAmountStack()
    {
        if (!IsOverCoolTime())
            return 0;

        double elapsedSeconds = (DateTime.Now - SaveInfo.LastSupplyTime).TotalSeconds;
        float time = (float)elapsedSeconds / (float)IngredientsSupply.coolTime;
        return Mathf.Clamp(Mathf.FloorToInt(time), 0, this.IngredientsSupply.maxStack);
    }
    /// <summary>
    /// 마지막 수급 시간 변경
    /// </summary>
    public void SetLastSupplyTime(DateTime supplyTime)
    {
        SaveInfo.LastSupplyTime = supplyTime;
    }


    #region ## Contructor ##

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="data"></param>
    /// <param name="saveInfo"></param>
    public IngredientSupplyInfo(int id, IngredientSupplySaveInfo saveInfo) : base()
    {
        var data = DataManager.Instance.GetData<IngredientsSupplyData>();

        this.Id = id;
        this.SaveInfo = saveInfo ?? new IngredientSupplySaveInfo
        {
            id = id,
            LastSupplyTime = DateTime.Now
        };
        this.IngredientsSupply = data.GetData(id);

        var inventoryItemData = DataManager.Instance.GetData<InventoryItemData>();
        this.InventoryItem = inventoryItemData.GetData(IngredientsSupply.inventoryItemId);
    }

    #endregion
}
