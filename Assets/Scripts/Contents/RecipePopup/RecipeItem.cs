using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeItem : MonoBehaviour
{
    #region Inspector

    public Image img;
    public TMP_Text title;

    #endregion

    public void Set(Ingredient ingredient)
    {
        title.text = ingredient.name;
    }
}