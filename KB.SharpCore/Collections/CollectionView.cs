using System.Collections;
using System.ComponentModel;

namespace KB.SharpCore.Collections;

public class CollectionView<T> : IEnumerable<T>
{
    private List<T> _collection;
    private List<T> _filteredCollection;
    private Func<T, bool> _filterCallback;
    
    #region IEnumerable
    
    // private struct Enumerator<T> : IEnumerator<T>
    // {
    //     
    //     T Current
    //     {
    //         get;
    //     }
    //
    //     public void Dispose()
    //     {
    //         // TODO release managed resources here
    //     }
    // }
    
    public IEnumerator<T> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    #endregion
}