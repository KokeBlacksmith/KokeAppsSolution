using KB.SharpCore.DesignPatterns;

namespace KB.AvaloniaCore.Controls.UserActions;

public class ScaleEditableControlUserAction : IUserAction
{
    private readonly IEditableControl[] _controls;
    private readonly double[] _newWidths;
    private readonly double[] _newHeights;
    private readonly double[] _oldWidths;
    private readonly double[] _oldHeights;

    public ScaleEditableControlUserAction(IEnumerable<IEditableControl> controls, IEnumerable<double> oldWidths, IEnumerable<double> oldHeights, IEnumerable<double> newWidths, IEnumerable<double> newHeights)
    {
        _controls = controls.ToArray();
        _newWidths = newWidths.ToArray();
        _newHeights = newHeights.ToArray();
        _oldWidths = oldWidths.ToArray();
        _oldHeights = oldHeights.ToArray();
    }

    public void Do()
    {
        for (int i = 0; i < _controls.Length; ++i)
        {
            _controls[i].Width = _newWidths[i];
            _controls[i].Height = _newHeights[i];
        }
    }

    public void Undo()
    {
        for (int i = 0; i < _controls.Length; ++i)
        {
            _controls[i].Width = _oldWidths[i];
            _controls[i].Height = _oldHeights[i];
        }
    }   
}
