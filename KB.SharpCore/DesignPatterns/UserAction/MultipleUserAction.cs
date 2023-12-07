namespace KB.SharpCore.DesignPatterns.UserAction;

public class MultipleUserAction : IUserAction
{
    #region Fields

    /// <summary>
    /// List of user actions to execute.
    /// </summary>
    private readonly List<IUserAction> _userActions;
    #endregion

    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="userActions">List of user actions to execute.</param>
    public MultipleUserAction(IEnumerable<IUserAction> userActions)
    {
        _userActions = new List<IUserAction>(userActions);
    }

    public MultipleUserAction(params IUserAction[] userActions)
    {
        _userActions = new List<IUserAction>(userActions);
    }
    
    #endregion

    #region Methods

    public void AddUserAction(IUserAction userAction)
    {
        _userActions.Add(userAction);
    }


    /// <summary>
    /// Do the action.
    /// </summary>
    public void Do()
    {
        foreach (var userAction in _userActions)
        {
            userAction.Do();
        }
    }

    /// <summary>
    /// Undo the action.
    /// </summary>
    public void Undo()
    {
        foreach (var userAction in _userActions)
        {
            userAction.Undo();
        }
    }
    
    #endregion
}
