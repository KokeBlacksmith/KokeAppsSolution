using Avalonia;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace KB.AvaloniaCore.Injection;

public static class VisualExtension
{
    public static T? FindChildOfType<T>(this Visual visual) 
        where T : Visual
    {
        return visual.FindLogicalDescendantOfType<T>();
    }
    
    public static T GetChildOfType<T>(this Visual visual) 
        where T : Visual
    {
        return visual.FindChildOfType<T>() ?? throw new Exception($"Could not find child of type {typeof(T).Name} in the parent visual {visual}.");
    }

    public static T? FindChildOfTypeWithName<T>(this Visual visual, string name) 
        where T : Visual
    {
        foreach (Visual child in visual.GetVisualDescendants())
        {
            if(child is T typedChild && typedChild.Name == name)
            {
                return typedChild;
            }
        }

        return null;
    }

    public static T GetChildOfTypeWithName<T>(this Visual visual, string name) 
        where T : Visual
    {
        return visual.FindChildOfTypeWithName<T>(name) ?? throw new Exception($"Could not find child of type {typeof(T).Name} with name {name} in the parent visual {visual}.");
    }

    public static T? FindParentOfType<T>(this Visual visual) 
        where T : Visual
    {
        return visual.FindAncestorOfType<T>();
    }

    public static T GetParentOfType<T>(this Visual visual) 
        where T : Visual
    {
        return visual.FindParentOfType<T>() ?? throw new Exception($"Could not find parent of type {typeof(T).Name} in the child visual {visual}.");
    }

    public static T? FindParentOfTypeWithName<T>(this Visual visual, string name) 
        where T : Visual
    {
        foreach (Visual parent in visual.GetVisualAncestors())
        {
            if(parent is T typedParent && typedParent.Name == name)
            {
                return typedParent;
            }
        }

        return null;
    }

    public static T GetParentOfTypeWithName<T>(this Visual visual, string name) 
        where T : Visual
    {
        return visual.FindParentOfTypeWithName<T>(name) ?? throw new Exception($"Could not find parent of type {typeof(T).Name} with name {name} in the child visual {visual}.");
    }

    public static T? FindParentOfTypeIncludeSelf<T>(this Visual visual)
        where T : Visual
    {
        return visual.FindAncestorOfType<T>(includeSelf: true);
    }

    public static T GetParentOfTypeIncludeSelf<T>(this Visual visual)
        where T : Visual
    {
        return visual.FindParentOfTypeIncludeSelf<T>() ?? throw new Exception($"Could not find parent of type {typeof(T).Name} in the child visual {visual}.");
    }

    public static Point? GetRelativePosition(this Visual visual, Visual relativeToAncestor)
    {
        if(visual == relativeToAncestor)
        {
            return new Point(0, 0);
        }

        return visual.TranslatePoint(new Point(0, 0), relativeToAncestor);
    }

    public static Point GetRelativePositionOrThrow(this Visual visual, Visual relativeToAncestor)
    {
        return visual.GetRelativePosition(relativeToAncestor) ?? throw new Exception($"Could not get relative position of visual {visual} to visual {relativeToAncestor}.");
    }

    public static double GetHalfWidth(this Layoutable self)
    {
        return self.Width * 0.5d;
    }

    public static double GetHalfHeight(this Layoutable self)
    {
        return self.Height * 0.5d;
    }

    public static double GetHalfBoundsWidth(this Layoutable self)
    {
        return self.Bounds.Width * 0.5d;
    }

    public static double GetHalfBoundsHeight(this Layoutable self)
    {
        return self.Bounds.Height * 0.5d;
    }

    public static Point GetCenter(this Layoutable self)
    {
        return new Point(self.GetHalfWidth(), self.GetHalfHeight());
    }

    //public static Point? GetRelativePosition(this Point point, Visual relativeToAncestor)
    //{
    //    return point.GetRelativePosition(relativeToAncestor as Visual);
    //}
}