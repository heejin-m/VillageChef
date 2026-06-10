using System.Threading.Tasks;
using UnityEngine;

public class PopupWindow : MonoBehaviour, IWindow
{
    public virtual async Task Open()
    {
    }

    public virtual Task<bool> OpenReady()
    {
        return Task.FromResult(true);
    }

    public virtual void Awake()
    {
    }

    public virtual void StartProcess()
    {
    }

    public virtual void CloseProcess()
    {
    }
}