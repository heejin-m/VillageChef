using UnityEngine;
using UnityEngine.UI;

public static class ExtensionMethod
{
    public static void SetOnClickEvent(this Button button, System.Action eventDelegate, bool isClearTriggers = true)
    {
        if (isClearTriggers) button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            eventDelegate?.Invoke();
        });
    }

    public static void Initialize(this Transform transform)
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
}