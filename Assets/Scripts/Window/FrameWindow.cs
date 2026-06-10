using System.Threading.Tasks;
using UnityEngine;

public class FrameWindow : MonoBehaviour, IWindow
{
    #region Inspector

    #endregion

    private bool _isOpenStarted;

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

        _isOpenStarted = true;
        StartProcess();
    }

    public virtual void StartProcess()
    {

    }

    public virtual void CloseProcess()
    {
        _isOpenStarted = false;
    }
}
