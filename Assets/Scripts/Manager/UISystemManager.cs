using DG.Tweening;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystemManager : SingletonBehaviour<UISystemManager>
{
    [Header("Toast")]
    [SerializeField] private RectTransform toastRoot;
    [SerializeField] private Vector2 toastAnchoredPosition = new Vector2(0f, 180f);
    [SerializeField] private float toastShowDuration = 1.5f;
    [SerializeField] private float toastFadeDuration = 0.2f;
    [SerializeField] private float toastMoveDistance = 24f;
    [SerializeField] private float toastStackSpacing = 108f;
    [SerializeField] private int maxToastCount = 3;

    private RectTransform _toastRoot;
    private readonly List<ToastItem> _toastItems = new();

    private class ToastItem
    {
        public RectTransform Rect;
        public CanvasGroup CanvasGroup;
        public Sequence Sequence;
    }

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

    public void ShowToast(string message)
    {
        if (string.IsNullOrEmpty(message))
            return;

        EnsureToastRoot();
        TrimToastItems();

        ToastItem toastItem = CreateToastItem(message);
        toastItem.CanvasGroup.alpha = 0f;
        toastItem.Rect.anchoredPosition = GetToastPosition(0) - new Vector2(0f, toastMoveDistance);
        _toastItems.Add(toastItem);
        RefreshToastPositions(toastItem);

        toastItem.Sequence = DOTween.Sequence().SetUpdate(true);
        toastItem.Sequence.Append(toastItem.CanvasGroup.DOFade(1f, toastFadeDuration));
        toastItem.Sequence.Join(toastItem.Rect.DOAnchorPos(GetToastPosition(0), toastFadeDuration).SetEase(Ease.OutQuad));
        toastItem.Sequence.AppendInterval(toastShowDuration);
        toastItem.Sequence.AppendCallback(() =>
        {
            toastItem.Rect.DOKill();
            toastItem.Rect.DOAnchorPos(toastItem.Rect.anchoredPosition + new Vector2(0f, toastMoveDistance), toastFadeDuration).SetEase(Ease.InQuad).SetUpdate(true);
        });
        toastItem.Sequence.Append(toastItem.CanvasGroup.DOFade(0f, toastFadeDuration));
        toastItem.Sequence.OnComplete(() => RemoveToast(toastItem));
    }

    private void EnsureToastRoot()
    {
        if (_toastRoot != null)
            return;

        RectTransform root = toastRoot;
        if (root == null)
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            root = canvas != null ? canvas.GetComponent<RectTransform>() : null;
        }

        if (root == null)
        {
            GameObject canvasObject = new GameObject("ToastCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(transform, false);

            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            root = canvasObject.GetComponent<RectTransform>();
        }

        GameObject toastRootObject = new GameObject("ToastRoot", typeof(RectTransform));
        toastRootObject.transform.SetParent(root, false);
        toastRootObject.transform.SetAsLastSibling();

        _toastRoot = toastRootObject.GetComponent<RectTransform>();
        _toastRoot.anchorMin = new Vector2(0.5f, 0f);
        _toastRoot.anchorMax = new Vector2(0.5f, 0f);
        _toastRoot.pivot = new Vector2(0.5f, 0f);
        _toastRoot.sizeDelta = Vector2.zero;
        _toastRoot.anchoredPosition = Vector2.zero;
    }

    private ToastItem CreateToastItem(string message)
    {
        GameObject toastObject = new GameObject("ToastMessage", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
        toastObject.transform.SetParent(_toastRoot, false);
        toastObject.transform.SetAsLastSibling();

        RectTransform toastRect = toastObject.GetComponent<RectTransform>();
        toastRect.anchorMin = new Vector2(0.5f, 0f);
        toastRect.anchorMax = new Vector2(0.5f, 0f);
        toastRect.pivot = new Vector2(0.5f, 0.5f);
        toastRect.sizeDelta = new Vector2(720f, 96f);
        toastRect.anchoredPosition = toastAnchoredPosition;

        Image background = toastObject.GetComponent<Image>();
        background.color = new Color(0.18f, 0.11f, 0.05f, 0.88f);
        background.raycastTarget = false;

        CanvasGroup toastCanvasGroup = toastObject.GetComponent<CanvasGroup>();
        toastCanvasGroup.alpha = 0f;
        toastCanvasGroup.blocksRaycasts = false;

        GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(toastRect, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(32f, 12f);
        textRect.offsetMax = new Vector2(-32f, -12f);

        TMP_Text toastText = textObject.GetComponent<TMP_Text>();
        toastText.text = message;
        toastText.alignment = TextAlignmentOptions.Center;
        toastText.color = new Color(1f, 0.94f, 0.78f, 1f);
        toastText.fontSize = 32f;
        toastText.enableAutoSizing = true;
        toastText.fontSizeMin = 18f;
        toastText.fontSizeMax = 32f;
        toastText.raycastTarget = false;

        return new ToastItem
        {
            Rect = toastRect,
            CanvasGroup = toastCanvasGroup,
        };
    }

    private void RefreshToastPositions(ToastItem excludeItem = null)
    {
        for (int i = 0; i < _toastItems.Count; i++)
        {
            ToastItem toastItem = _toastItems[_toastItems.Count - 1 - i];
            if (toastItem == excludeItem)
                continue;

            toastItem.Rect.DOKill();
            toastItem.Rect.DOAnchorPos(GetToastPosition(i), toastFadeDuration).SetEase(Ease.OutQuad).SetUpdate(true);
        }
    }

    private Vector2 GetToastPosition(int stackIndex)
    {
        return toastAnchoredPosition + new Vector2(0f, toastStackSpacing * stackIndex);
    }

    private void TrimToastItems()
    {
        while (_toastItems.Count >= maxToastCount)
        {
            RemoveToast(_toastItems[0]);
        }
    }

    private void RemoveToast(ToastItem toastItem)
    {
        if (toastItem == null)
            return;

        toastItem.Sequence?.Kill();
        _toastItems.Remove(toastItem);

        if (toastItem.Rect != null)
        {
            Destroy(toastItem.Rect.gameObject);
        }

        RefreshToastPositions();
    }

    protected override void OnDestroy()
    {
        foreach (ToastItem toastItem in _toastItems)
        {
            toastItem.Sequence?.Kill();
        }
        _toastItems.Clear();
        base.OnDestroy();
    }
}
