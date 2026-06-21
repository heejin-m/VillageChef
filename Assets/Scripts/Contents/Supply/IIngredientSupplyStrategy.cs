using System.Collections.Generic;

public interface IIngredientSupplyStrategy
{
    void Supply(List<IngredientSupplyInfo> infos);
}