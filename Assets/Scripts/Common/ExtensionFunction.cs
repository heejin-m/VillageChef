using UnityEngine.UI;

public static class ExtensionFunction
{
    public static void SetOnClickEvent(this Button button, System.Action eventDelegate, bool isClearTriggers = true)
    {
        if (isClearTriggers) button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            eventDelegate?.Invoke();
        });
    }
}