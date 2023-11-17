﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Metadata;

namespace KB.AvaloniaCore.Controls.GraphEditor;

[TemplatePart("PART_ContentPresenter", typeof(ContentPresenter))]
[TemplatePart("PART_Border", typeof(Border))]
public abstract class Node : TemplatedControl
{
    private Border? _border;
    private ContentPresenter? _contentPresenter;

    #region StyledProperties

    public static readonly StyledProperty<Layoutable?> ContentProperty = AvaloniaProperty.Register<Node, Layoutable?>(nameof(Node.Content));
    public static readonly StyledProperty<double> PositionXProperty = AvaloniaProperty.Register<Node, double>(nameof(Node.PositionX));
    public static readonly StyledProperty<double> PositionYProperty = AvaloniaProperty.Register<Node, double>(nameof(Node.PositionY));
    
    public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<Node, bool>(nameof(Node.IsSelected));
    public static readonly StyledProperty<bool> IsDraggedProperty = AvaloniaProperty.Register<Node, bool>(nameof(Node.IsDragged));


    [Content]
    public Layoutable? Content
    {
        get { return GetValue(Node.ContentProperty); }
        set { SetValue(Node.ContentProperty, value); }
    }

    public double PositionX
    {
        get { return GetValue(Node.PositionXProperty); }
        set { SetValue(Node.PositionXProperty, value); }
    }

    public double PositionY
    {
        get { return GetValue(Node.PositionYProperty); }
        set { SetValue(Node.PositionYProperty, value); }
    }

    public bool IsSelected
    {
        get { return GetValue(Node.IsSelectedProperty); }
        set { SetValue(Node.IsSelectedProperty, value); }
    }

    public bool IsDragged
    {
        get { return GetValue(Node.IsDraggedProperty); }
        set { SetValue(Node.IsDraggedProperty, value); }
    }

    #endregion

    #region Inhertied Members

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _border = e.NameScope.Get<Border>("PART_Border");
        _contentPresenter = e.NameScope.Get<ContentPresenter>("PART_ContentPresenter");
    }

    #endregion
}
