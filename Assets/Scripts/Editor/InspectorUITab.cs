using UnityEditor;

[CustomEditor(typeof(UITab))]
public class InspectorUITab : Editor
{
    private UITab _uiTab = null;

    private void OnEnable()
    {
        _uiTab = target as UITab;
        if (_uiTab != null)
        {
            _uiTab.State = UITab.eState.Enable;
            UITabController tabController = _uiTab.GetComponentInParent<UITabController>();
            if (tabController != null)
            {
                tabController.Clear();
                tabController.SelectTab((short)_uiTab.transform.GetSiblingIndex());
            }
        }
    }
}