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
