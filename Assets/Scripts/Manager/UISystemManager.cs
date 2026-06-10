using UnityEngine;

public class UISystemManager : SingletonBehaviour<UISystemManager>
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackPress();
        }
    }

    public void BackPress()
    {
        if (PopupManager.IsLive)
        {
            PopupManager.Instance.ClosePopup();
        }
    }
}