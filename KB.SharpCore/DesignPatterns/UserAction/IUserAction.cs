namespace KB.SharpCore.DesignPatterns;

/// <summary>
/// Interface for a user action. Hnadle Do and Undo.
/// Uses Command pattern.
/// </summary>
public interface IUserAction
{
    /// <summary>
    /// Do the action.
    /// </summary>
    void Do();
    
    /// <summary>
    /// Undo the action.
    ///</summary>
    void Undo();
}
