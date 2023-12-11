using Avalonia.Collections;

namespace KB.AvaloniaCore.Controls.GraphEditor;

public class NodeConnectionCollection : AvaloniaList<NodeConnection>
{
    public NodeConnection? GetConnection(NodePin pin)
    {
        return this.FirstOrDefault(x => x.SourcePin == pin || x.TargetPin == pin);
    }

    public bool HasConnection(NodePin? source, NodePin? target)
    {
        if(source == null && target == null)
        {
            return false;
        }

        return this.Any(x => x.SourcePin == source && x.TargetPin == target);
    }

    public new void Add(NodeConnection connection)
    {
        if (HasConnection(connection.SourcePin, connection.TargetPin))
        {
            throw new InvalidOperationException("Connection already exists");
        }

        base.Add(connection);
    }

    public bool TryAdd(NodeConnection connection)
    {
        try
        {             
            Add(connection);
        }
        catch (InvalidOperationException)
        {
            return false;
        }

        return true;
    }

    public bool Remove(NodePin source, NodePin target)
    {
        NodeConnection? connection = this.FirstOrDefault(x => x.SourcePin == source && x.TargetPin == target);
        if (connection is not null)
        {
            return base.Remove(connection);
        }

        return false;
    }

    public new void Insert(int index, NodeConnection connection)
    {
        if (HasConnection(connection.SourcePin, connection.TargetPin))
        {
            throw new InvalidOperationException("Connection already exists");
        }

        base.Insert(index, connection);
    }

    public bool TryInsert(int index, NodeConnection connection)
    {
        try
        {
            Insert(index, connection);
        }
        catch (InvalidOperationException)
        {
            return false;
        }

        return true;
    }

    public new void InsertRange(int index, IEnumerable<NodeConnection> connections)
    {
        foreach (var connection in connections)
        {
            if (HasConnection(connection.SourcePin, connection.TargetPin))
            {
                throw new InvalidOperationException("Connection already exists");
            }
        }

        base.InsertRange(index, connections);
    }

    public bool TryInsertRange(int index, IEnumerable<NodeConnection> connections)
    {
        try
        {
            InsertRange(index, connections);
        }
        catch (InvalidOperationException)
        {
            return false;
        }

        return true;
    }

    public new void AddRange(IEnumerable<NodeConnection> connections)
    {
        foreach (var connection in connections)
        {
            if (HasConnection(connection.SourcePin, connection.TargetPin))
            {
                throw new InvalidOperationException("Connection already exists");
            }
        }

        base.AddRange(connections);
    }

    public bool TryAddRange(IEnumerable<NodeConnection> connections)
    {
        try
        {
            AddRange(connections);
        }
        catch (InvalidOperationException)
        {
            return false;
        }

        return true;
    }
}
