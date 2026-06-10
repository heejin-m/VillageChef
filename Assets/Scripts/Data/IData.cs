using System.Threading.Tasks;

public interface IData
{
    Task Initialize();
    void Release();
}
