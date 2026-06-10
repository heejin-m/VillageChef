using System.Threading.Tasks;

public interface IWindow
{
    public void Awake();

    public Task<bool> OpenReady();

    public Task Open();

    public void StartProcess();

    public void CloseProcess();
}