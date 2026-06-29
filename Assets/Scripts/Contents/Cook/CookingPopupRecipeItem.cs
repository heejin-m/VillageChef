using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CookingPopupRecipeItem : MonoBehaviour
{
    #region Inspector

    public TMP_Text title;

    #endregion

    private int _id;
    private Button _button;
    private System.Action<int> _onClick;

    private void Awake()
    {
        _button = this.GetComponent<Button>();
        if (_button != null) _button.SetOnClickEvent(OnClick);
    }

    public void Set(int id, string name, System.Action<int> onClick)
    {
        _id = id;
        title.text = name;
        _onClick = onClick;
    }

    private void OnClick()
    {
        _onClick?.Invoke(_id);
    }
}