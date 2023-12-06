namespace KB.SharpCore.DesignPatterns.UserAction;

public class UserActionInvoker
{
    private List<IUserAction> _userActions = new List<IUserAction>();
    private int _currentIndex = -1;

    public void AddUserAction(IUserAction userAction)
    {
        if (_currentIndex < _userActions.Count - 1)
        {
            _userActions.RemoveRange(_currentIndex + 1, _userActions.Count - _currentIndex - 1);
        }

        _userActions.Add(userAction);
        _currentIndex++;
    }

    public void Undo()
    {
        if (CanUndo)
        {
            _userActions[_currentIndex].Undo();
            _currentIndex--;
        }
    }

    public void Redo()
    {
        if (CanRedo)
        {
            _currentIndex++;
            _userActions[_currentIndex].Do();
        }
    }

    public void Clear()
    {
        _userActions.Clear();
        _currentIndex = -1;
    }

    public bool CanUndo => _currentIndex >= 0;

    public bool CanRedo => _currentIndex < _userActions.Count - 1;

    public int Count =>_userActions.Count;

    public int CurrentIndex =>_currentIndex;

    public IUserAction? CurrentUserAction
    {
        get
        {
            if(Count > 0)
            {
                return _userActions[_currentIndex];
            }

            return null;
        }
    }

    public IUserAction GetUserActionAtIndex(int index)
    {
        return _userActions[index];
    }

    public IEnumerable<IUserAction> GetUserActions()
    {
        return _userActions;
    }

    public void SetUserActions(IEnumerable<IUserAction> userActions)
    {
        _userActions = new List<IUserAction>(userActions);
    }

    public void SetCurrentIndex(int index)
    {
        _currentIndex = index;
    }

    public void SetCurrentUserAction(IUserAction userAction)
    {
        _userActions[_currentIndex] = userAction;
    }

    public void SetUserActionAtIndex(int index, IUserAction userAction)
    {
        _userActions[index] = userAction;
    }

    public void RemoveUserActionAtIndex(int index)
    {
        _userActions.RemoveAt(index);
        if(index <= _currentIndex)
        {
            --_currentIndex;
        }
    }

    public void RemoveUserAction(IUserAction userAction)
    {
        int index = _userActions.IndexOf(userAction);
        RemoveUserActionAtIndex(index);
    }

    public void RemoveUserActionsRange(int index, int count)
    {
        _userActions.RemoveRange(index, count);
        if(index <= _currentIndex)
        {
            _currentIndex -= count;
        }
    }

    public void RemoveUserActionsRange(IUserAction userAction, int count)
    {
        int index = _userActions.IndexOf(userAction);
        RemoveUserActionsRange(index, count);
    }
}
