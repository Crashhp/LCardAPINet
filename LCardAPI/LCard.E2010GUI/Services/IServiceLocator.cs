namespace LCard.E2010GUI.Services
{
    public interface IServiceLocator
    {
        T GetInstance<T>() where T : class;
    }
}