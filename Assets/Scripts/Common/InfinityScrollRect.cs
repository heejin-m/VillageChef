using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InfinityScrollRect : ScrollRect
{
    [System.Serializable]
    public class ItemUpdateEvent : UnityEvent<RectTransform, int>
    {
    }

    #region Inspector

    public RectTransform itemPrefab;
    public Vector2 itemSize = new Vector2(100f, 100f);
    public Vector2 spacing = Vector2.zero;
    public RectOffset padding = new RectOffset();
    public int constraintCount = 1;
    public int extraVisibleCount = 2;
    public bool hideTemplateOnAwake = true;
    public ItemUpdateEvent onUpdateItem;

    #endregion

    private readonly System.Collections.Generic.List<RectTransform> _items = new System.Collections.Generic.List<RectTransform>();

    private int _totalCount;
    private int _firstIndex = -1;
    private bool _initialized;

    public int TotalCount => _totalCount;

    protected override void Start()
    {
        base.Start();
        if (!Initialize())
        {
            return;
        }

        UpdateItems(true);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        onValueChanged.AddListener(OnScrollValueChanged);
    }

    protected override void OnDisable()
    {
        onValueChanged.RemoveListener(OnScrollValueChanged);
        base.OnDisable();
    }

    protected override void SetContentAnchoredPosition(Vector2 position)
    {
        base.SetContentAnchoredPosition(position);
        UpdateItems(false);
    }

    public void SetTotalCount(int count, bool resetPosition = true)
    {
        if (!Initialize())
        {
            return;
        }

        _totalCount = Mathf.Max(0, count);
        _firstIndex = -1;
        SetContentSize();
        EnsurePool();

        if (resetPosition)
        {
            content.anchoredPosition = Vector2.zero;
        }

        UpdateItems(true);
    }

    public void Refresh()
    {
        UpdateItems(true);
    }

    public void ScrollToIndex(int index)
    {
        if (!Initialize())
        {
            return;
        }

        if (_totalCount <= 0)
        {
            content.anchoredPosition = Vector2.zero;
            return;
        }

        index = Mathf.Clamp(index, 0, _totalCount - 1);
        Vector2 position = content.anchoredPosition;

        if (vertical)
        {
            position.y = padding.top + GetLineIndex(index) * GetStep().y;
        }
        else
        {
            position.x = -(padding.left + GetLineIndex(index) * GetStep().x);
        }

        content.anchoredPosition = position;
        UpdateItems(true);
    }

    private bool Initialize()
    {
        if (_initialized)
        {
            return true;
        }

        if (content == null)
        {
            Debug.LogError($"{nameof(InfinityScrollRect)} needs a content RectTransform.", this);
            return false;
        }

        if (itemPrefab == null && content.childCount > 0)
        {
            itemPrefab = content.GetChild(0) as RectTransform;
        }

        if (itemPrefab != null)
        {
            if (itemSize.x <= 0f)
            {
                itemSize.x = itemPrefab.rect.width;
            }

            if (itemSize.y <= 0f)
            {
                itemSize.y = itemPrefab.rect.height;
            }

            itemPrefab.gameObject.SetActive(!hideTemplateOnAwake);
        }

        vertical = vertical || !horizontal;
        horizontal = horizontal && !vertical;
        _initialized = true;
        return true;
    }

    private void OnScrollValueChanged(Vector2 value)
    {
        UpdateItems(false);
    }

    private void SetContentSize()
    {
        Vector2 size = content.sizeDelta;
        int lineCount = GetLineCount(_totalCount);

        if (vertical)
        {
            size.y = padding.top + padding.bottom + lineCount * itemSize.y + Mathf.Max(0, lineCount - 1) * spacing.y;
        }
        else
        {
            size.x = padding.left + padding.right + lineCount * itemSize.x + Mathf.Max(0, lineCount - 1) * spacing.x;
        }

        content.sizeDelta = size;
    }

    private void EnsurePool()
    {
        if (itemPrefab == null || viewport == null)
        {
            return;
        }

        constraintCount = Mathf.Max(1, constraintCount);
        int needCount = GetVisibleCount();

        while (_items.Count < needCount)
        {
            RectTransform item = Instantiate(itemPrefab, content);
            item.name = $"{itemPrefab.name}_{_items.Count}";
            item.anchorMin = new Vector2(0f, 1f);
            item.anchorMax = new Vector2(0f, 1f);
            item.pivot = new Vector2(0f, 1f);
            item.sizeDelta = itemSize;
            item.gameObject.SetActive(false);
            _items.Add(item);
        }
    }

    private void UpdateItems(bool force)
    {
        if (!_initialized || content == null || itemPrefab == null || viewport == null)
        {
            return;
        }

        EnsurePool();

        if (_totalCount <= 0)
        {
            HideAllItems();
            return;
        }

        int firstIndex = GetFirstVisibleIndex();

        if (!force && firstIndex == _firstIndex)
        {
            return;
        }

        _firstIndex = firstIndex;

        for (int i = 0; i < _items.Count; ++i)
        {
            int dataIndex = firstIndex + i;
            RectTransform item = _items[i];

            if (dataIndex < 0 || dataIndex >= _totalCount)
            {
                item.gameObject.SetActive(false);
                continue;
            }

            item.gameObject.SetActive(true);
            item.anchoredPosition = GetItemPosition(dataIndex);
            onUpdateItem?.Invoke(item, dataIndex);
        }
    }

    private int GetFirstVisibleIndex()
    {
        Vector2 step = GetStep();
        float offset = vertical ? content.anchoredPosition.y - padding.top : -content.anchoredPosition.x - padding.left;
        float itemStep = vertical ? step.y : step.x;

        if (itemStep <= 0f)
        {
            return 0;
        }

        int firstLine = Mathf.Max(0, Mathf.FloorToInt(offset / itemStep) - extraVisibleCount);
        return Mathf.Clamp(firstLine * constraintCount, 0, Mathf.Max(0, _totalCount - 1));
    }

    private int GetVisibleCount()
    {
        Vector2 step = GetStep();
        float viewportSize = vertical ? viewport.rect.height : viewport.rect.width;
        float itemStep = vertical ? step.y : step.x;

        if (itemStep <= 0f)
        {
            return 0;
        }

        int visibleLineCount = Mathf.CeilToInt(viewportSize / itemStep) + extraVisibleCount * 2 + 1;
        return Mathf.Min(_totalCount, visibleLineCount * Mathf.Max(1, constraintCount));
    }

    private Vector2 GetStep()
    {
        return itemSize + spacing;
    }

    private Vector2 GetItemPosition(int index)
    {
        int line = GetLineIndex(index);
        int cross = index % Mathf.Max(1, constraintCount);

        if (vertical)
        {
            return new Vector2(padding.left + cross * GetStep().x, -(padding.top + line * GetStep().y));
        }

        return new Vector2(padding.left + line * GetStep().x, -(padding.top + cross * GetStep().y));
    }

    private int GetLineIndex(int index)
    {
        return index / Mathf.Max(1, constraintCount);
    }

    private int GetLineCount(int count)
    {
        int safeConstraintCount = Mathf.Max(1, constraintCount);
        return Mathf.CeilToInt(Mathf.Max(0, count) / (float)safeConstraintCount);
    }

    private void HideAllItems()
    {
        for (int i = 0; i < _items.Count; ++i)
        {
            _items[i].gameObject.SetActive(false);
        }
    }
}
