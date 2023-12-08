using KB.SharpCore.Utils;

namespace KB.SharpCore.DesignPatterns.UserAction;

public class UserActionInvoker
{
    // TODO: Search for a better way to do this.
    public static bool IsUserActionsPaused = false;

    public UserActionInvoker(uint capacity = 100)
    {
        _userActions = new IUserAction[capacity];
    }

    private readonly IUserAction?[] _userActions;
    private int _currentIndex = -1;
    private int _count = 0;

    public bool AddUserAction(IUserAction userAction)
    {
        if(IsUserActionsPaused)
        {
            return false;
        }

        if(_userActions.Length <= _count + 1)
        {
            // We will max the capacity of the array.
            RemoveUserActionAtIndex(0);
        }

        _userActions[_count] = userAction;
        _currentIndex++;
        _count++;
        return true;
    }

    public void Undo()
    {
        if (CanUndo)
        {
            _userActions[_currentIndex]!.Undo();
            _currentIndex--;
        }
    }

    public void Redo()
    {
        if (CanRedo)
        {
            _currentIndex++;
            _userActions[_currentIndex]!.Do();
        }
    }

    public void Clear()
    {
        for (int i = 0; i < _userActions.Length - 1; ++i)
        {
            _userActions[i] = default(IUserAction);
        }

        _currentIndex = -1;
        _count = 0;
    }

    public bool CanUndo => _currentIndex >= 0;

    public bool CanRedo => _currentIndex < _count - 1;

    public int Count => _count;

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

    public IUserAction? GetUserActionAtIndex(uint index)
    {
        _ValidateIndex((int)index);
        return _userActions[index];
    }

    public IEnumerable<IUserAction> GetUserActions()
    {
        return _userActions.Where(action => action != null)!;
    }

    public void SetUserActions(IEnumerable<IUserAction> userActions)
    {
        Clear();
        foreach(IUserAction action in userActions)
        {
            AddUserAction(action);
        }
    }

    public void SetCurrentIndex(uint index)
    {
        _ValidateIndex((int)index);
        _currentIndex = (int)index;
    }

    public void SetCurrentUserAction(IUserAction userAction)
    {
        _userActions[_currentIndex] = userAction;
    }

    public void SetUserActionAtIndex(uint index, IUserAction userAction)
    {
        _ValidateIndex((int)index);
        _userActions[index] = userAction;
    }

    public void RemoveUserActionAtIndex(uint index)
    {
        _ValidateIndex((int)index);

        if (_userActions[index] == null)
        {
            return;
        }

        _userActions[index] = null;

        for (int i = (int)index; i < _userActions.Length - 2; ++i)
        {
            _userActions[i] = _userActions[i + 1];
        }

        _userActions[_userActions.Length - 1] = default(IUserAction);

        if (index <= _currentIndex)
        {
            _currentIndex--;
        }

        _count--;
    }

    public void RemoveUserAction(IUserAction userAction)
    {
        for(uint i = 0; i < _userActions.Length - 1; ++i)
        {
            if (_userActions[i] == userAction)
            {
                RemoveUserActionAtIndex(i);
                return;
            }
        }

        throw new ArgumentException("User action not found.", nameof(userAction));
    }

    public void RemoveUserActionsRange(int index, int count)
    {
        _ValidateIndex(index);

        if (index + count > _count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Index + count must be less than the count of user actions.");
        }

        for (int i = index; i < index + count; ++i)
        {
            _userActions[i] = default(IUserAction);
        }

        for (int i = index; i < index + count; ++i)
        {
            _userActions[i] = _userActions[i + count];
        }

        if(index <= _currentIndex)
        {
            _currentIndex -= count;
        }

        _count -= count;
    }

    public void RemoveUserActionsRange(IUserAction userAction, int count)
    {
        int index  = CollectionHelper.IndexOf(_userActions, userAction);
        if(index >= 0)
        {
            RemoveUserActionsRange(index, count);
        }
    }

    private void _ValidateIndex(int index)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index must be greater than zero.");
        }

        if (index >= _count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index must be less than the count of user actions.");
        }
    }
}
