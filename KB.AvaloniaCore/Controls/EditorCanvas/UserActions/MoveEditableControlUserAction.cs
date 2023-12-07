using Avalonia;
using KB.SharpCore.DesignPatterns;

namespace KB.AvaloniaCore.Controls.UserActions;

public class MoveEditableControlUserAction : IUserAction
{
    private readonly IEditableControl[] _controls;
    private readonly Point[] _newPositions;
    private readonly Point[] _oldPositions;

    public MoveEditableControlUserAction(IEnumerable<IEditableControl> controls, IEnumerable<Point> oldPositions, IEnumerable<Point> newPositions)
    {
        _controls = controls.ToArray();
        _newPositions = newPositions.ToArray();
        _oldPositions = oldPositions.ToArray();
    }

    public void Do()
    {
        for (int i = 0; i < _controls.Length; ++i)
        {
            _controls[i].PositionX = _newPositions[i].X;
            _controls[i].PositionY = _newPositions[i].Y;
        }
    }

    public void Undo()
    {
        for (int i = 0; i < _controls.Length; ++i)
        {
            _controls[i].PositionX = _oldPositions[i].X;
            _controls[i].PositionY = _oldPositions[i].Y;
        }
    }
    
}
