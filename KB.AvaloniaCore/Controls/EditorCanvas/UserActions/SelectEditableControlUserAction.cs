using KB.SharpCore.DesignPatterns;

namespace KB.AvaloniaCore.Controls.UserActions;

public class SelectEditableControlUserAction : IUserAction
{
    private readonly IEnumerable<IEditableControl> _oldSelection;
    private readonly IEnumerable<IEditableControl> _newSelection;
    private readonly EditorCanvas _canvas;

    /// <summary>
    /// Constructor.
    /// It will create a new instance of <see cref="SelectEditableControlUserAction"/> with the given parameters.
    /// The IEditableControl arrays will be converted to IEditableControl arrays.
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="oldSelection"></param>
    /// <param name="newSelection"></param>
    public SelectEditableControlUserAction(EditorCanvas canvas, IEnumerable<IEditableControl> oldSelection, IEnumerable<IEditableControl> newSelection)
    {
        _oldSelection = oldSelection.ToArray();
        _newSelection = newSelection.ToArray();
        _canvas = canvas;
    }


    public void Do()
    {
        foreach (var item in _oldSelection)
        {
            item.IsSelected = false;
        }

        _canvas.SelectedItems.Clear();
        _canvas.SelectedItems.AddRange(_newSelection);

        foreach (var item in _newSelection)
        {
            item.IsSelected = true;
        }
    }

    public void Undo()
    {
        foreach (var item in _newSelection)
        {
            item.IsSelected = false;
        }

        _canvas.SelectedItems.Clear();
        _canvas.SelectedItems.AddRange(_oldSelection);

        foreach (var item in _oldSelection)
        {
            item.IsSelected = true;
        }
    }
}
