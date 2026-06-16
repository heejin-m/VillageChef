using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Loop Grid Scroll Rect Custom", 54)]
    [DisallowMultipleComponent]
    public class LoopGridScrollRectCustom : LoopScrollRectCustom
    {
        private bool _isMainAxisDrag = false;
        private ScrollRect _parent = null;

        protected LoopGridScrollRectCustom()
        {
            direction = LoopScrollRectDirection.Vertical;
        }

        protected override float GetSize(RectTransform item, bool includeSpacing)
        {
            float size = includeSpacing ? contentSpacing : 0;
            if (m_GridLayout != null)
            {
                size += IsHorizontal ? m_GridLayout.cellSize.x : m_GridLayout.cellSize.y;
            }
            else
            {
                size += IsHorizontal ? LoopScrollSizeUtils.GetPreferredWidth(item) : LoopScrollSizeUtils.GetPreferredHeight(item);
            }
            size *= IsHorizontal ? m_Content.localScale.x : m_Content.localScale.y;
            return size;
        }

        protected override float GetDimension(Vector2 vector)
        {
            return IsHorizontal ? -vector.x : vector.y;
        }

        protected override float GetAbsDimension(Vector2 vector)
        {
            return IsHorizontal ? vector.x : vector.y;
        }

        protected override Vector2 GetVector(float value)
        {
            return IsHorizontal ? new Vector2(-value, 0) : new Vector2(0, value);
        }

        protected override void Awake()
        {
            ConfigureDirection();
            base.Awake();
            _parent = transform.parent.GetComponentInParent<ScrollRect>();

            if (!m_Content)
            {
                return;
            }

            GridLayoutGroup layout = m_Content.GetComponent<GridLayoutGroup>();
            if (layout == null)
            {
                Debug.LogError("[LoopScrollRect] LoopGridScrollRectCustom needs a GridLayoutGroup on content.", this);
                return;
            }

            if (layout.constraint == GridLayoutGroup.Constraint.Flexible)
            {
                Debug.LogError("[LoopScrollRect] LoopGridScrollRectCustom supports only FixedColumnCount or FixedRowCount.", this);
            }
        }

        protected override bool UpdateItems(ref Bounds viewBounds, ref Bounds contentBounds)
        {
            return IsHorizontal
                ? UpdateHorizontalItems(ref viewBounds, ref contentBounds)
                : UpdateVerticalItems(ref viewBounds, ref contentBounds);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            _isMainAxisDrag = IsHorizontal
                ? Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y)
                : Mathf.Abs(eventData.delta.y) > Mathf.Abs(eventData.delta.x);

            if (_isMainAxisDrag)
            {
                base.OnBeginDrag(eventData);
            }
            else if (_parent)
            {
                _parent.OnBeginDrag(eventData);
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (_isMainAxisDrag)
            {
                base.OnDrag(eventData);
            }
            else if (_parent)
            {
                _parent.OnDrag(eventData);
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (_isMainAxisDrag)
            {
                base.OnEndDrag(eventData);
            }
            else if (_parent)
            {
                _parent.OnEndDrag(eventData);
            }

            _isMainAxisDrag = false;
        }

        private bool IsHorizontal => direction == LoopScrollRectDirection.Horizontal;

        private void ConfigureDirection()
        {
            if (!m_Content)
            {
                return;
            }

            GridLayoutGroup layout = m_Content.GetComponent<GridLayoutGroup>();
            if (layout == null)
            {
                return;
            }

            if (layout.constraint == GridLayoutGroup.Constraint.FixedRowCount)
            {
                direction = LoopScrollRectDirection.Horizontal;
            }
            else
            {
                direction = LoopScrollRectDirection.Vertical;
            }
        }

        private bool UpdateVerticalItems(ref Bounds viewBounds, ref Bounds contentBounds)
        {
            bool changed = false;

            if ((viewBounds.size.y < contentBounds.min.y - viewBounds.max.y) && itemTypeEnd > itemTypeStart)
            {
                int maxItemTypeStart = -1;
                if (totalCount >= 0)
                {
                    maxItemTypeStart = Mathf.Max(0, totalCount - (itemTypeEnd - itemTypeStart));
                }
                float currentSize = contentBounds.size.y;
                float elementSize = (currentSize - contentSpacing * (CurrentLines - 1)) / CurrentLines;
                ReturnToTempPool(true, itemTypeEnd - itemTypeStart);
                itemTypeStart = itemTypeEnd;

                int offsetCount = Mathf.FloorToInt((contentBounds.min.y - viewBounds.max.y) / (elementSize + contentSpacing));
                if (maxItemTypeStart >= 0 && itemTypeStart + offsetCount * contentConstraintCount > maxItemTypeStart)
                {
                    offsetCount = Mathf.FloorToInt((float)(maxItemTypeStart - itemTypeStart) / contentConstraintCount);
                }
                itemTypeStart += offsetCount * contentConstraintCount;
                if (totalCount >= 0)
                {
                    itemTypeStart = Mathf.Max(itemTypeStart, 0);
                }
                itemTypeEnd = itemTypeStart;

                float offset = offsetCount * (elementSize + contentSpacing);
                m_Content.anchoredPosition -= new Vector2(0, offset + (reverseDirection ? 0 : currentSize));
                contentBounds.center -= new Vector3(0, offset + currentSize / 2, 0);
                contentBounds.size = Vector3.zero;

                changed = true;
            }

            if ((viewBounds.min.y - contentBounds.max.y > viewBounds.size.y) && itemTypeEnd > itemTypeStart)
            {
                float currentSize = contentBounds.size.y;
                float elementSize = (currentSize - contentSpacing * (CurrentLines - 1)) / CurrentLines;
                ReturnToTempPool(false, itemTypeEnd - itemTypeStart);
                itemTypeEnd = itemTypeStart;

                int offsetCount = Mathf.FloorToInt((viewBounds.min.y - contentBounds.max.y) / (elementSize + contentSpacing));
                if (totalCount >= 0 && itemTypeStart - offsetCount * contentConstraintCount < 0)
                {
                    offsetCount = Mathf.FloorToInt((float)(itemTypeStart) / contentConstraintCount);
                }
                itemTypeStart -= offsetCount * contentConstraintCount;
                if (totalCount >= 0)
                {
                    itemTypeStart = Mathf.Max(itemTypeStart, 0);
                }
                itemTypeEnd = itemTypeStart;

                float offset = offsetCount * (elementSize + contentSpacing);
                m_Content.anchoredPosition += new Vector2(0, offset + (reverseDirection ? currentSize : 0));
                contentBounds.center += new Vector3(0, offset + currentSize / 2, 0);
                contentBounds.size = Vector3.zero;

                changed = true;
            }

            if (viewBounds.min.y < contentBounds.min.y + m_ContentBottomPadding)
            {
                float size = NewItemAtEnd(), totalSize = size;
                while (size > 0 && viewBounds.min.y < contentBounds.min.y + m_ContentBottomPadding - totalSize)
                {
                    size = NewItemAtEnd();
                    totalSize += size;
                }
                if (totalSize > 0)
                    changed = true;
            }
            else if ((itemTypeEnd % contentConstraintCount != 0) && (itemTypeEnd < totalCount || totalCount < 0))
            {
                NewItemAtEnd();
            }

            if (viewBounds.max.y > contentBounds.max.y - m_ContentTopPadding)
            {
                float size = NewItemAtStart(), totalSize = size;
                while (size > 0 && viewBounds.max.y > contentBounds.max.y - m_ContentTopPadding + totalSize)
                {
                    size = NewItemAtStart();
                    totalSize += size;
                }
                if (totalSize > 0)
                    changed = true;
            }

            if (viewBounds.min.y > contentBounds.min.y + threshold + m_ContentBottomPadding)
            {
                float size = DeleteItemAtEnd(), totalSize = size;
                while (size > 0 && viewBounds.min.y > contentBounds.min.y + threshold + m_ContentBottomPadding + totalSize)
                {
                    size = DeleteItemAtEnd();
                    totalSize += size;
                }
                if (totalSize > 0)
                    changed = true;
            }

            if (viewBounds.max.y < contentBounds.max.y - threshold - m_ContentTopPadding)
            {
                float size = DeleteItemAtStart(), totalSize = size;
                while (size > 0 && viewBounds.max.y < contentBounds.max.y - threshold - m_ContentTopPadding - totalSize)
                {
                    size = DeleteItemAtStart();
                    totalSize += size;
                }
                if (totalSize > 0)
                    changed = true;
            }

            if (changed)
            {
                ClearTempPool();
            }

            return changed;
        }

        private bool UpdateHorizontalItems(ref Bounds viewBounds, ref Bounds contentBounds)
        {
            bool changed = false;

            if ((viewBounds.size.x < contentBounds.min.x - viewBounds.max.x) && itemTypeEnd > itemTypeStart)
            {
                float currentSize = contentBounds.size.x;
                float elementSize = (currentSize - contentSpacing * (CurrentLines - 1)) / CurrentLines;
                ReturnToTempPool(false, itemTypeEnd - itemTypeStart);
                itemTypeEnd = itemTypeStart;

                int offsetCount = Mathf.FloorToInt((contentBounds.min.x - viewBounds.max.x) / (elementSize + contentSpacing));
                if (totalCount >= 0 && itemTypeStart - offsetCount * contentConstraintCount < 0)
                {
                    offsetCount = Mathf.FloorToInt((float)(itemTypeStart) / contentConstraintCount);
                }
                itemTypeStart -= offsetCount * contentConstraintCount;
                if (totalCount >= 0)
                {
                    itemTypeStart = Mathf.Max(itemTypeStart, 0);
                }
                itemTypeEnd = itemTypeStart;

                float offset = offsetCount * (elementSize + contentSpacing);
                m_Content.anchoredPosition -= new Vector2(offset + (reverseDirection ? currentSize : 0), 0);
                contentBounds.center -= new Vector3(offset + currentSize / 2, 0, 0);
                contentBounds.size = Vector3.zero;

                changed = true;
            }

            if ((viewBounds.min.x - contentBounds.max.x > viewBounds.size.x) && itemTypeEnd > itemTypeStart)
            {
                int maxItemTypeStart = -1;
                if (totalCount >= 0)
                {
                    maxItemTypeStart = Mathf.Max(0, totalCount - (itemTypeEnd - itemTypeStart));
                    maxItemTypeStart = (maxItemTypeStart / contentConstraintCount) * contentConstraintCount;
                }
                float currentSize = contentBounds.size.x;
                float elementSize = (currentSize - contentSpacing * (CurrentLines - 1)) / CurrentLines;
                ReturnToTempPool(true, itemTypeEnd - itemTypeStart);
                itemTypeStart = itemTypeEnd;

                int offsetCount = Mathf.FloorToInt((viewBounds.min.x - contentBounds.max.x) / (elementSize + contentSpacing));
                if (maxItemTypeStart >= 0 && itemTypeStart + offsetCount * contentConstraintCount > maxItemTypeStart)
                {
                    offsetCount = Mathf.FloorToInt((float)(maxItemTypeStart - itemTypeStart) / contentConstraintCount);
                }
                itemTypeStart += offsetCount * contentConstraintCount;
                if (totalCount >= 0)
                {
                    itemTypeStart = Mathf.Max(itemTypeStart, 0);
                }
                itemTypeEnd = itemTypeStart;

                float offset = offsetCount * (elementSize + contentSpacing);
                m_Content.anchoredPosition += new Vector2(offset + (reverseDirection ? 0 : currentSize), 0);
                contentBounds.center += new Vector3(offset + currentSize / 2, 0, 0);
                contentBounds.size = Vector3.zero;

                changed = true;
            }

            if (viewBounds.max.x > contentBounds.max.x - m_ContentRightPadding)
            {
                float size = NewItemAtEnd(), totalSize = size;
                while (size > 0 && viewBounds.max.x > contentBounds.max.x - m_ContentRightPadding + totalSize)
                {
                    size = NewItemAtEnd();
                    totalSize += size;
                }
                if (totalSize > 0)
                    changed = true;
            }
            else if ((itemTypeEnd % contentConstraintCount != 0) && (itemTypeEnd < totalCount || totalCount < 0))
            {
                NewItemAtEnd();
            }

            if (viewBounds.min.x < contentBounds.min.x + m_ContentLeftPadding)
            {
                float size = NewItemAtStart(), totalSize = size;
                while (size > 0 && viewBounds.min.x < contentBounds.min.x + m_ContentLeftPadding - totalSize)
                {
                    size = NewItemAtStart();
                    totalSize += size;
                }
                if (totalSize > 0)
                    changed = true;
            }

            if (viewBounds.max.x < contentBounds.max.x - threshold - m_ContentRightPadding)
            {
                float size = DeleteItemAtEnd(), totalSize = size;
                while (size > 0 && viewBounds.max.x < contentBounds.max.x - threshold - m_ContentRightPadding - totalSize)
                {
                    size = DeleteItemAtEnd();
                    totalSize += size;
                }
                if (totalSize > 0)
                    changed = true;
            }

            if (viewBounds.min.x > contentBounds.min.x + threshold + m_ContentLeftPadding)
            {
                float size = DeleteItemAtStart(), totalSize = size;
                while (size > 0 && viewBounds.min.x > contentBounds.min.x + threshold + m_ContentLeftPadding + totalSize)
                {
                    size = DeleteItemAtStart();
                    totalSize += size;
                }
                if (totalSize > 0)
                    changed = true;
            }

            if (changed)
            {
                ClearTempPool();
            }

            return changed;
        }
    }
}
