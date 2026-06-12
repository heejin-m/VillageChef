using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UITab : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    #region Inspector

    public GameObject enableGroup;
    public GameObject disableGroup;

    public List<GameObject> enableList;
    public List<GameObject> disableList;

    #endregion

    public enum eState
    {
        Enable,
        Disable,
    }

    public eState State
    {
        get
        {
            return _state;
        }
        set
        {
            SetState(value);
        }
    }

    public Action<UITab> onClick;
    public Action<UITab> onUnuseClick;
    private Action<PointerEventData> _beginDragAction = null;
    private Action<PointerEventData> _dragAction = null;
    private Action<PointerEventData> _endDragAction = null;

    private Button _button;
    private eState _state = eState.Enable;

    public ushort Index { get; private set; } = 0;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(TabClick);

        RegisterEventScrollRect();
    }

    private void RegisterEventScrollRect()
    {
        ScrollRect scrollRect = GetComponentInParent<ScrollRect>();
        if (scrollRect)
        {
            _beginDragAction += scrollRect.OnBeginDrag;
            _dragAction += scrollRect.OnDrag;
            _endDragAction += scrollRect.OnEndDrag;
        }
    }

    /// <summary>
    /// 버튼 인덱스 세팅
    /// </summary>
    /// <param name="index"></param>
    public void SetIndex(ushort index)
    {
        this.Index = index;
    }

    /// <summary>
    /// 버튼 상태 수정
    /// </summary>
    /// <param name="state">상태값</param>
    protected virtual void SetState(eState state)
    {
        this._state = state;

        if (enableGroup != null)
            enableGroup.SetActive(state == eState.Enable);
        if (disableGroup != null)
            disableGroup.SetActive(state == eState.Disable);

        if (enableList != null)
        {
            for (int i = 0; i < enableList.Count; ++i)
            {
                enableList[i].SetActive(state == eState.Enable);
            }
        }

        if (disableList != null)
        {
            for (int i = 0; i < disableList.Count; ++i)
            {
                disableList[i].SetActive(state == eState.Disable);
            }
        }
    }

    protected virtual void TabClick()
    {
        onClick?.Invoke(this);
    }

    #region ## Drag Events ##

    public void SetDragEvent(Action<PointerEventData> action1, Action<PointerEventData> action2, Action<PointerEventData> action3)
    {
        _beginDragAction = action1;
        _dragAction = action2;
        _endDragAction = action3;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _beginDragAction?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _dragAction?.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _endDragAction?.Invoke(eventData);
    }

    #endregion
}