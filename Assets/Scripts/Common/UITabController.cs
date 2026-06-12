using System;
using System.Collections.Generic;
using UnityEngine;

public class UITabController : MonoBehaviour
{
    public Action<ushort> onChangeTabIndex;
    public Action<ushort> onSameTabIndex;

    private bool _isAlreadyUpdateUI = false;
    private UITab _currentTab = null;
    private List<UITab> _tabs = new List<UITab>();

    public ushort CurrentTabIndex => _currentTab != null ? _currentTab.Index : (ushort)0;

    private void Awake()
    {
        // UITab 리스트 생성
        CreateUITabList();

        // 시작탭 가져오기
        UITab tab = GetStartTab();
        if (tab != null)
        {
            _currentTab = tab;
        }
    }

    private void Start()
    {
        if (!_isAlreadyUpdateUI)
        {
            // UI 업데이트
            UpdateUI();
        }
    }

    public void SelectTab(UITab tab, bool notify = true)
    {
        if (_tabs.Count == 0)
        {
            CreateUITabList();
        }

        UITab findTab = _tabs.Find(item => item.Equals(tab));
        if (findTab != null)
        {
            _currentTab = findTab;
        }

        UpdateUI();
        if (notify) onChangeTabIndex?.Invoke(CurrentTabIndex);
    }

    /// <summary>
    /// 탭 선택
    /// </summary>
    /// <param name="index"></param>
    public void SelectTab(short index, bool notify = true)
    {
        if (_tabs.Count == 0)
        {
            CreateUITabList();
        }

        UITab tab = _tabs.Find(item => item.Index == index);
        if (tab != null)
        {
            _currentTab = tab;
        }

        UpdateUI();
        if (notify) onChangeTabIndex?.Invoke(CurrentTabIndex);
    }

    /// <summary>
    /// Clear
    /// </summary>
    public void Clear()
    {
        _tabs.Clear();
        _currentTab = null;
    }

    /// <summary>
    /// UI 리셋
    /// </summary>
    public void ResetUI()
    {
        Clear();
        CreateUITabList();

        // 현재탭이 세팅이 되어있지 않은 경우에만 시작탭을 가져온다.
        if (_currentTab == null)
        {
            UITab tab = GetStartTab();
            if (tab != null)
            {
                _currentTab = tab;
            }
        }

        // UI 업데이트
        UpdateUI();
    }

    /// <summary>
    /// UITab 리스트 생성
    /// </summary>
    private void CreateUITabList()
    {
        _tabs.Clear();
        _tabs.AddRange(this.gameObject.GetComponentsInChildren<UITab>(true));
        for (int i = 0; i < _tabs.Count; i++)
        {
            _tabs[i].SetIndex((ushort)i);
            _tabs[i].onClick = OnCilckTab;
        }
    }

    /// <summary>
    /// 탭 변경 콜백
    /// </summary>
    /// <param name="tab"></param>
    private void OnCilckTab(UITab tab)
    {
        if (_currentTab == null || !_currentTab.Equals(tab))
        {
            // 현재탭 변경
            _currentTab = tab;

            // UI 업데이트
            UpdateUI();

            // 콜백
            onChangeTabIndex?.Invoke(CurrentTabIndex);
        }
        else
        {
            onSameTabIndex?.Invoke(CurrentTabIndex);
        }
    }

    /// <summary>
    /// 시작탭 가져오기
    /// </summary>
    /// <returns></returns>
    private UITab GetStartTab()
    {
        UITab startTab = null;
        if (startTab == null)
        {
            // 시작탭으로 세팅된 UITab이 없다면 첫번쨰 것으로 가져온다.
            if (_tabs.Count > 0)
            {
                startTab = _tabs[0];
            }
        }

        return startTab;
    }

    /// <summary>
    /// UI 업데이트
    /// </summary>
    public void UpdateUI()
    {
        _isAlreadyUpdateUI = true;
        for (int i = 0; i < _tabs.Count; i++)
        {
            UITab tab = _tabs[i];
            if (tab.Equals(_currentTab))
            {
                tab.State = UITab.eState.Enable;
            }
            else
            {
                tab.State = UITab.eState.Disable;
            }
        }
    }

    /// <summary>
    /// 탭 선택
    /// </summary>
    /// <param name="index"></param>
    public void SetTab(short index, bool notify = true)
    {
        if (_tabs.Count == 0)
        {
            CreateUITabList();
        }

        UITab tab = _tabs.Find(item => item.Index == index);
        if (tab != null)
        {
            _currentTab = tab;
        }

        UpdateUI();

        if (notify) onChangeTabIndex?.Invoke(CurrentTabIndex);
    }
}
