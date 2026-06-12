using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIFx : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    #region Inspector

    [Header("ClickScale")]
    public bool isActiveScale = true;
    public Vector3 pressedScale = new Vector3(1.05f, 1.05f, 1);
    public float openScaleDuration = 0.25f;

    #endregion

    private RectTransform _tweenTarget;
    private Sequence _mSequence;
    private Vector3 _mScale;


    private void Awake()
    {
        _tweenTarget = this.transform.GetComponent<RectTransform>();

        _mScale = _tweenTarget.localScale;
    }

    private void OnEnable()
    {
        _tweenTarget.localScale = _mScale;
    }

    private void OnDisable()
    {
        _mSequence.Kill();
        _tweenTarget.localScale = _mScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.enabled && isActiveScale)
        {
            _mSequence.Kill();

            _mSequence = DOTween.Sequence().SetAutoKill();
            _mSequence.Append(_tweenTarget.DOScale(pressedScale, openScaleDuration).SetEase(Ease.OutBack)).SetUpdate(true);
            _mSequence.timeScale = 1;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (this.enabled && isActiveScale)
        {
            _mSequence.Kill();

            _mSequence = DOTween.Sequence().SetAutoKill();
            _mSequence.Append(_tweenTarget.DOScale(_mScale, 0.1f).SetEase(Ease.InSine)).SetUpdate(true);
            _mSequence.timeScale = 1;
        }
    }
}
