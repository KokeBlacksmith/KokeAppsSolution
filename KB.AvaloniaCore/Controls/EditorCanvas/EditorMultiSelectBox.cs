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
    public EditorMultiSelectBox()
    {
        Stroke = Brushes.Black;
        StrokeThickness = 2.0d;
        StrokeThickness = 1;
        Fill = Brushes.Blue;
        Opacity = 0.5d;
        IsVisible = false;

        //BorderBrush = Brushes.Black;
        //BorderThickness = new Thickness(2.0d);
        //Background = Brushes.Blue;
    }

    public void Start(Point position)
    {
        IsVisible = true;
        Width = 0;
        Height = 0;

        Canvas.SetLeft(this, position.X);
        Canvas.SetTop(this, position.Y);
    }

    public void Update(Point position)
    {
        double left = Canvas.GetLeft(this);
        double top = Canvas.GetTop(this);

        double width = position.X - left;
        double height = position.Y - top;

        //if (width < 0)
        //{
        //    Canvas.SetLeft(this, position.X);
        //    this.Width = System.Math.Abs(width);
        //}
        //else
        //{
        //    this.Width = width;
        //}

        //if (height < 0)
        //{
        //    Canvas.SetTop(this, position.Y);
        //    this.Height = System.Math.Abs(height);
        //}
        //else
        //{
        //    this.Height = height;
        //}

        this.Width = width;
        this.Height = height;
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
