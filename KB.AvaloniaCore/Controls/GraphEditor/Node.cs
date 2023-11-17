using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace KB.AvaloniaCore.Controls.GraphEditor;

[TemplatePart("PART_ContentPresenter", typeof(ContentPresenter))]
[TemplatePart("PART_Border", typeof(Border))]
public class Node : TemplatedControl
{
    private Border? _border;
    private ContentPresenter? _contentPresenter;

    #region StyledProperties

    public static readonly StyledProperty<object?> ContentProperty = AvaloniaProperty.Register<Node, object?>(nameof(Node.Content));
    public static readonly StyledProperty<Vector2> PositionProperty = AvaloniaProperty.Register<Node, Vector2>(nameof(Node.Position));
    
    [Content]
    public object? Content
    {
        get { return GetValue(Node.ContentProperty); }
        set { SetValue(Node.ContentProperty, value); }
    }

    public Vector2 Position
    {
        get { return GetValue(Node.PositionProperty); }
        set { SetValue(Node.PositionProperty, value); }
    }

    #endregion

}
