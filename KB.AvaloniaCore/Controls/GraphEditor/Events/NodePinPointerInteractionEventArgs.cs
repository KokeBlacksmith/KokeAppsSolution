using Avalonia;

namespace KB.AvaloniaCore.Controls.GraphEditor.Events;

/// <summary>
/// Event args on pointer interaction with a <see cref="NodePin"/>.
/// <para/>
/// Has information about the <see cref="NodePin"/> and the pointer position.
/// </summary>
public class NodePinPointerInteractionEventArgs
{
    public NodePin Pin { get; }
    public Point Point { get; }

    public NodePinPointerInteractionEventArgs(NodePin pin, Point point)
    {
        Pin = pin;
        Point = point;
    }
}
