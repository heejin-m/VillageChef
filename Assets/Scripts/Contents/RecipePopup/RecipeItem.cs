using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeItem : MonoBehaviour
{
    #region Inspector

    public Image img;
    public TMP_Text title;

    #endregion

    public void Set(byte id)
    {
        var ingredientData = DataManager.Instance.GetData<IngredientData>();
        var data = ingredientData.GetData(id);

        AtlasLoadManager.SetImageSprite(img, eAtlas.FoodUI, data.ResourceName);
        title.text = data.Name;
    }
}