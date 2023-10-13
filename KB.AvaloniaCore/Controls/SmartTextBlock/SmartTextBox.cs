using System.Net.Mime;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using KB.AvaloniaCore.Injection;
using KB.AvaloniaCore.ReactiveUI;
using KB.SharpCore.Collections;

namespace KB.AvaloniaCore.Controls;

public class SmartTextBox : TextBox
{
    // Parent TextBox has [TemplatePart("PART_ScrollViewer", typeof(ScrollViewer))]
    private ScrollViewer? _scrollViewer;

    #region Gestures

    private readonly KeyGesture _goToEndGesture = new KeyGesture(Key.End, KeyModifiers.Control);

    #endregion
    
    // So this class is a TextBox with some extra functionality.
    // Without this, the SmartTextBox would not be rendered.
    protected override Type StyleKeyOverride
    {
        get { return typeof(TextBox); }
    }

    #region PublicMembers
    
    public void Clear()
    {
        Text = String.Empty;
    }

    public void AppendLine(string line)
    {
        Text += Environment.NewLine + line;
    }

    public void AppendLines(IEnumerable<string> lines)
    {
        foreach (string line in lines)
        {
            AppendLine(line);
        }
    }
    
    #endregion
    
    #region PrivateMembers
    
    private void _ScrollToEnd()
    {
        _scrollViewer?.ScrollToEnd();
        CaretIndex = Text?.Length ?? 0;
    }
    
    #endregion

    #region StyledProperties

    public readonly static StyledProperty<bool> UpdateCaretPositionAtEndProperty = AvaloniaProperty.Register<SmartTextBox, bool>(nameof(SmartTextBox.UpdateCaretPositionAtEnd));

    public bool UpdateCaretPositionAtEnd
    {
        get { return GetValue(SmartTextBox.UpdateCaretPositionAtEndProperty); }
        set { SetValue(SmartTextBox.UpdateCaretPositionAtEndProperty, value); }
    }
    
    private void _OnTextPropertyChanged(AvaloniaPropertyChangedEventArgs args)
    {
        // True if the caret is at the end of the text or text was empty
        bool wasCaretAtEnd = CaretIndex >= (args.GetOldValue<string>()?.Length ?? CaretIndex);
        if (!wasCaretAtEnd || !UpdateCaretPositionAtEnd)
        {
            return;
        }

        _ScrollToEnd();
    }
    
    #endregion
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SmartTextBox.TextProperty)
        {
            _OnTextPropertyChanged(change);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _scrollViewer = e.NameScope.Find<ScrollViewer>("PART_ScrollViewer")!;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        
        if (_goToEndGesture.Matches(e))
        {
            _ScrollToEnd();
        }
    }
}