using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PopupWindow : MonoBehaviour, IWindow
{
    [SerializeField] private GameObject dimObject;
    [SerializeField] private Button dimButton;

    private bool _isOpenStarted;
    private Action _onDimClick;
    public bool IsOpen => _isOpenStarted;

    public virtual void Awake()
    {
    }

    public virtual Task<bool> OpenReady()
    {
        return Task.FromResult(true);
    }

    public virtual async Task Open()
    {
        if (_isOpenStarted)
        {
            return;
        }

        if (!await OpenReady())
        {
            return;
        }

        this.gameObject.SetActive(true);
        _isOpenStarted = true;
        StartProcess();
    }

    public virtual void Close()
    {
        if (!_isOpenStarted)
        {
            return;
        }

        CloseProcess();
        _isOpenStarted = false;
        ClearDimClick();
        this.gameObject.SetActive(false);
    }

    public void ApplyConfig(PopupConfigData config, Action onDimClick)
    {
        bool useDim = config != null && config.useDim;
        bool closeOnDimClick = useDim && config.closeOnDimClick;

        if (dimObject != null)
        {
            dimObject.SetActive(useDim);
        }

        ClearDimClick();

        if (dimButton == null || !closeOnDimClick)
        {
            return;
        }

        _onDimClick = onDimClick;
        dimButton.onClick.AddListener(OnClickDim);
    }

    private void ClearDimClick()
    {
        if (dimButton != null)
        {
            dimButton.onClick.RemoveListener(OnClickDim);
        }

        _onDimClick = null;
    }

    private void OnClickDim()
    {
        _onDimClick?.Invoke();
    }

    public virtual void StartProcess()
    {
    }

    public virtual void CloseProcess()
    {
    }
}
