using Avalonia;
using Avalonia.Input;

namespace KB.AvaloniaCore.IO;

public static class HotkeyInputHelper
{
    public static bool IsKeyMatch(KeyEventArgs e, Key key, KeyModifiers modifiers = KeyModifiers.None)
    {
        return e.Key == key && e.KeyModifiers == modifiers;
    }

    public static bool IsCopy(KeyEventArgs e)
    {
        var keymap = Application.Current!.PlatformSettings!.HotkeyConfiguration;
        return keymap.Copy.Any(gesture => gesture.Matches(e));
    }
    
    public static bool IsCut(KeyEventArgs e)
    {
        var keymap = Application.Current!.PlatformSettings!.HotkeyConfiguration;
        return keymap.Cut.Any(gesture => gesture.Matches(e));
    }
    
    public static bool IsPaste(KeyEventArgs e)
    {
        var keymap = Application.Current!.PlatformSettings!.HotkeyConfiguration;
        return keymap.Paste.Any(gesture => gesture.Matches(e));
    }
    
    public static bool IsUndo(KeyEventArgs e)
    {
        var keymap = Application.Current!.PlatformSettings!.HotkeyConfiguration;
        return keymap.Undo.Any(gesture => gesture.Matches(e));
    }
    
    public static bool IsRedo(KeyEventArgs e)
    {
        var keymap = Application.Current!.PlatformSettings!.HotkeyConfiguration;
        return keymap.Redo.Any(gesture => gesture.Matches(e));
    }
    
    public static bool IsSelectAll(KeyEventArgs e)
    {
        var keymap = Application.Current!.PlatformSettings!.HotkeyConfiguration;
        return keymap.SelectAll.Any(gesture => gesture.Matches(e));
    }
    
    public static bool IsMoveCursorToTheStartOfLine(KeyEventArgs e)
    {
        var keymap = Application.Current!.PlatformSettings!.HotkeyConfiguration;
        return keymap.MoveCursorToTheStartOfLine.Any(gesture => gesture.Matches(e));
    }
    
    public static bool IsMoveCursorToTheEndOfLine(KeyEventArgs e)
    {
        var keymap = Application.Current!.PlatformSettings!.HotkeyConfiguration;
        return keymap.MoveCursorToTheEndOfLine.Any(gesture => gesture.Matches(e));
    }
    
    
    public static bool IsMoveCursorToTheStartOfDocument(KeyEventArgs e)
    {
        var keymap = Application.Current!.PlatformSettings!.HotkeyConfiguration;
        return keymap.MoveCursorToTheStartOfDocument.Any(gesture => gesture.Matches(e));
    }
    
    public static bool IsMoveCursorToTheEndOfDocument(KeyEventArgs e)
    {
        var keymap = Application.Current!.PlatformSettings!.HotkeyConfiguration;
        return keymap.MoveCursorToTheEndOfDocument.Any(gesture => gesture.Matches(e));
    }
    
    public static bool IsMoveCursorToTheStartOfLineWithSelection(KeyEventArgs e)
    {
        var keymap = Application.Current!.PlatformSettings!.HotkeyConfiguration;
        return keymap.MoveCursorToTheStartOfLineWithSelection.Any(gesture => gesture.Matches(e));
    }
    
    public static bool IsMoveCursorToTheEndOfLineWithSelection(KeyEventArgs e)
    {
        var keymap = Application.Current!.PlatformSettings!.HotkeyConfiguration;
        return keymap.MoveCursorToTheEndOfLineWithSelection.Any(gesture => gesture.Matches(e));
    }
    
    public static bool IsMoveCursorToTheStartOfDocumentWithSelection(KeyEventArgs e)
    {
        var keymap = Application.Current!.PlatformSettings!.HotkeyConfiguration;
        return keymap.MoveCursorToTheStartOfDocumentWithSelection.Any(gesture => gesture.Matches(e));
    }
    
    public static bool IsMoveCursorToTheEndOfDocumentWithSelection(KeyEventArgs e)
    {
        var keymap = Application.Current!.PlatformSettings!.HotkeyConfiguration;
        return keymap.MoveCursorToTheEndOfDocumentWithSelection.Any(gesture => gesture.Matches(e));
    }
    
    public static bool IsOpenContextMenu(KeyEventArgs e)
    {
        var keymap = Application.Current!.PlatformSettings!.HotkeyConfiguration;
        return keymap.OpenContextMenu.Any(gesture => gesture.Matches(e));
    }
    
    public static bool IsBack(KeyEventArgs e)
    {
        var keymap = Application.Current!.PlatformSettings!.HotkeyConfiguration;
        return keymap.Back.Any(gesture => gesture.Matches(e));
    }
    
    public static bool IsPageLeft(KeyEventArgs e)
    {
        var keymap = Application.Current!.PlatformSettings!.HotkeyConfiguration;
        return keymap.PageLeft.Any(gesture => gesture.Matches(e));
    }
    
    public static bool IsPageRight(KeyEventArgs e)
    {
        var keymap = Application.Current!.PlatformSettings!.HotkeyConfiguration;
        return keymap.PageRight.Any(gesture => gesture.Matches(e));
    }
    
    public static bool IsPageUp(KeyEventArgs e)
    {
        var keymap = Application.Current!.PlatformSettings!.HotkeyConfiguration;
        return keymap.PageUp.Any(gesture => gesture.Matches(e));
    }
    
    public static bool IsPageDown(KeyEventArgs e)
    {
        var keymap = Application.Current!.PlatformSettings!.HotkeyConfiguration;
        return keymap.PageDown.Any(gesture => gesture.Matches(e));
    }
    
}