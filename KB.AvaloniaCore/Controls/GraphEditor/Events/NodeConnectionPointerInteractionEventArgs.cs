using Avalonia;

namespace KB.AvaloniaCore.Controls.GraphEditor.Events;

/// <summary>
/// Event args on pointer interaction with a <see cref="NodePin"/>.
/// <para/>
/// Has information about the <see cref="NodePin"/> and the pointer position.
/// </summary>
public class NodeConnectionPointerInteractionEventArgs
{
    public NodeConnection Connection { get; }
    public Point Point { get; }

    public NodeConnectionPointerInteractionEventArgs(NodeConnection connection, Point point)
    {
        Connection = connection;
        Point = point;
    }
}
