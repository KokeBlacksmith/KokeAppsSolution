using Avalonia;
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
}