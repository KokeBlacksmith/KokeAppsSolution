namespace KB.SharpCore.Collections;

public class DisposableList<T> : List<T>, IDisposable
 where T : IDisposable
{
      public void Dispose()
      {
          foreach (T item in this)
          {
              item.Dispose();
          }
          
          GC.SuppressFinalize(this);
      }
}

public class DisposableList : DisposableList<IDisposable>
{
    
}