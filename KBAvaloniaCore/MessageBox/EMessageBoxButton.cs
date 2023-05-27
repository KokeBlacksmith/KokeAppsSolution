using MessageBox.Avalonia.Enums;

namespace KBAvaloniaCore.MessageBox;

public enum EMessageBoxButton
{
    Ok,
    YesNo,
    OkCancel,
    OkAbort,
    YesNoCancel,
    YesNoAbort,
}

[Flags]
public enum EMessageBoxButtonResult
{
    Ok = 0,
    Yes = 1,
    No = 2,
    Abort = EMessageBoxButtonResult.No | EMessageBoxButtonResult.Yes, // 0x00000003
    Cancel = 4,
    None = EMessageBoxButtonResult.Cancel | EMessageBoxButtonResult.Yes, // 0x00000005
}

/// <summary>
/// Convert the wrapper enum to the Avalonia enum
/// </summary>
internal static class EMessageBoxButtonConversion
{
    public static ButtonEnum ToButtonEnum(this EMessageBoxButton button)
    {
        return button switch
        {
            EMessageBoxButton.Ok => ButtonEnum.Ok,
            EMessageBoxButton.YesNo => ButtonEnum.YesNo,
            EMessageBoxButton.OkCancel => ButtonEnum.OkCancel,
            EMessageBoxButton.OkAbort => ButtonEnum.OkAbort,
            EMessageBoxButton.YesNoCancel => ButtonEnum.YesNoCancel,
            EMessageBoxButton.YesNoAbort => ButtonEnum.YesNoAbort,
            _ => throw new NotImplementedException(),
        };
    }
    
    public static EMessageBoxButtonResult ToEMessageBoxButtonResult(this ButtonResult button)
    {
        return button switch
        {
            ButtonResult.Ok => EMessageBoxButtonResult.Ok,
            ButtonResult.Yes => EMessageBoxButtonResult.Yes,
            ButtonResult.No => EMessageBoxButtonResult.No,
            ButtonResult.Abort => EMessageBoxButtonResult.Abort,
            ButtonResult.Cancel => EMessageBoxButtonResult.Cancel,
            ButtonResult.None => EMessageBoxButtonResult.None,
            _ => throw new NotImplementedException(),
        };
    }
}