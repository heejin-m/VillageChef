using System.Collections.Generic;

public class OrchardSupplyStrategy : IIngredientSupplyStrategy
{
    private const float BONUS_RATE = 0.3f;
    private const int BONUS_AMOUNT = 1;

    public void Supply(List<IngredientSupplyInfo> infos)
    {
        foreach (var info in infos)
        {
            // 과수원 재료 수급 로직
            if (info == null)
                continue;

            // 과수원 데이터가 아니면 처리하지 않음
            if (info.SupplyType != eIngredientSupplyType.Orchard)
                continue;

            // 쿨타임 지났는지 판단
            if (!info.IsOverCoolTime())
                continue;

            // 인벤토리에 재료 추가
            // 과수원은 일정 확률로 추가 지급
            int amount = info.Amount;

            if (UnityEngine.Random.value < BONUS_RATE)
            {
                amount += BONUS_AMOUNT;
            }

            bool isSuccessAdd = ModelCenter.Inventory.AddItem(info.IngredientId, amount);

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