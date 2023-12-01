using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KB.AvaloniaCore.Controls;

/// <summary>
/// Shape to select multiple elements.
/// It is a rectangle with border. And selection is done by checking if the element is inside the rectangle.
/// </summary>
internal class EditorMultiSelectBox : Rectangle
{
    private Point _startPosition;
    public EditorMultiSelectBox()
    {
        Stroke = Brushes.DarkGray;
        StrokeThickness = 2.0d;
        Fill = Brushes.White;
        Opacity = 0.5d;
        IsVisible = false;
    }

    public bool IsActive => IsVisible;

    public void Start(Point position)
    {
        IsVisible = true;
        Width = 0;
        Height = 0;

        Canvas.SetLeft(this, position.X);
        Canvas.SetTop(this, position.Y);
        _startPosition = position;
    }

    public void Update(Point position)
    {
        double width = System.Math.Abs(position.X - _startPosition.X);
        double height = System.Math.Abs(position.Y - _startPosition.Y);

        if (position.X < _startPosition.X)
        {
            Canvas.SetLeft(this, position.X);
        }
        else
        {
            Canvas.SetLeft(this, _startPosition.X);
        }

        if (position.Y < _startPosition.Y)
        {
            Canvas.SetTop(this, position.Y);
        }
        else
        {
            Canvas.SetTop(this, _startPosition.Y);
        }

        Width = width;
        Height = height;
    }

    public void End()
    {
        this.IsVisible = false;
    }

    public IEnumerable<Visual> GetSelectedItems(IEnumerable<Visual> visuals)
    {
        List<Visual> visualsToSelect = new List<Visual>();
        foreach (Visual visual in visuals)
        {
            if (this.Bounds.Intersects(visual.Bounds))
            {
                visualsToSelect.Add(visual);
            }
        }

        return visualsToSelect;
    }
}
