using System.Collections.Generic;

public class FieldSupplyStrategy : IIngredientSupplyStrategy
{
    public void Supply(List<IngredientSupplyInfo> infos)
    {
        foreach (var info in infos)
        {
            // 밭 재료 수급 로직
            if (info == null)
                continue;

            // 밭 데이터가 아니면 처리하지 않음
            if (info.SupplyType != eIngredientSupplyType.Field)
                continue;

            // 쿨타임 지났는지 판단
            if (!info.IsOverCoolTime())
                continue;

            // 인벤토리에 재료 추가
            var amount = info.Amount;
            var isSuccessAdd = ModelCenter.Inventory.AddItem(info.IngredientId, info.Amount);

            // 마지막 수급 시간 변경
            if (isSuccessAdd)
            {
                ModelCenter.Supply.SetLastSupplyTime(info.Id);
                var desc = $"{info.ItemName}를 {amount}개 획득했습니다.";
                UISystemManager.Instance.ShowToast(desc);
            }
        }
    }
}