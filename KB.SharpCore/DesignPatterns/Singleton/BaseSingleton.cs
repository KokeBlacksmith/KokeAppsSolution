namespace KB.SharpCore.DesignPatterns.Singleton;

public abstract class BaseSingleton<TSingleton>
    where TSingleton : BaseSingleton<TSingleton>, new()
{
    private static BaseSingleton<TSingleton>? s_instance;
    
    private BaseSingleton()
    {
    }

    public static TSingleton Instance
    {
        get
        {
            BaseSingleton<TSingleton>.s_instance ??= new TSingleton();
            return (BaseSingleton<TSingleton>.s_instance as TSingleton)!;
        }
    }
}