using UnityEngine;
using UnityEngine.UI;

public class CloseButton : MonoBehaviour
{
    private Button _closeButton;

    private void Awake()
    {
        _closeButton = this.GetComponent<Button>();
        _closeButton.SetOnClickEvent(OnClickCloseButton);
    }

    private void OnClickCloseButton()
    {
        UISystemManager.Instance.BackPress();
    }
}
