namespace KB.SharpCore.DesignPatterns.Singleton;

public abstract class BaseSingleton<TSingleton>
    where TSingleton : BaseSingleton<TSingleton>
{
    private static BaseSingleton<TSingleton>? s_instance;

    public static TSingleton Instance
    {
        get
        {
            if(BaseSingleton<TSingleton>.s_instance == null)
            {
                BaseSingleton<TSingleton>.s_instance = (TSingleton)Activator.CreateInstance(typeof(TSingleton), nonPublic: true)!;
            }

            return (BaseSingleton<TSingleton>.s_instance as TSingleton)!;
        }
    }
}