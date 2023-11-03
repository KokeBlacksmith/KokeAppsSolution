using System.Net.Mime;
using System.Text;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using KB.AvaloniaCore.Injection;
using KB.AvaloniaCore.ReactiveUI;
using KB.SharpCore.Collections;

namespace KB.AvaloniaCore.Controls;

public class EnchancedTextBox : TextBox
{

    #region Fields
    
    // Parent TextBox has [TemplatePart("PART_ScrollViewer", typeof(ScrollViewer))]
    private ScrollViewer? _scrollViewer;
    private readonly KeyGesture _goToEndGesture = new KeyGesture(Key.End, KeyModifiers.Control);
    private readonly KeyGesture _confirmOnReturnGesture = new KeyGesture(Key.Enter);

    #endregion

    public EnchancedTextBox()
    {
    }

    #region Events
    public event EventHandler<RoutedEventArgs>? ConfirmReturn;
    #endregion

    #region StyledProperties

    public readonly static StyledProperty<bool> UpdateCaretPositionAtEndProperty = AvaloniaProperty.Register<EnchancedTextBox, bool>(nameof(EnchancedTextBox.UpdateCaretPositionAtEnd));
    public readonly static StyledProperty<bool> IsCaretAtEndProperty = AvaloniaProperty.Register<EnchancedTextBox, bool>(nameof(EnchancedTextBox.IsCaretAtEnd));


    public readonly static StyledProperty<bool> ConfirmOnReturnProperty = AvaloniaProperty.Register<EnchancedTextBox, bool>(nameof(EnchancedTextBox.ConfirmOnReturn));
    public readonly static StyledProperty<GenericCommand<string?>> ConfirmOnReturnCommandProperty = AvaloniaProperty.Register<EnchancedTextBox, GenericCommand<string?>>(nameof(EnchancedTextBox.ConfirmOnReturnCommand));

    public bool UpdateCaretPositionAtEnd
    {
        get { return GetValue(EnchancedTextBox.UpdateCaretPositionAtEndProperty); }
        set { SetValue(EnchancedTextBox.UpdateCaretPositionAtEndProperty, value); }
    }

    public bool IsCaretAtEnd
    {
        get { return GetValue(EnchancedTextBox.IsCaretAtEndProperty); }
        private set { SetValue(EnchancedTextBox.IsCaretAtEndProperty, value); }
    }

    public bool ConfirmOnReturn
    {
        get { return GetValue(EnchancedTextBox.ConfirmOnReturnProperty); }
        set { SetValue(EnchancedTextBox.ConfirmOnReturnProperty, value); }
    }

    public GenericCommand<string?> ConfirmOnReturnCommand
    {
        get { return GetValue(EnchancedTextBox.ConfirmOnReturnCommandProperty); }
        set { SetValue(EnchancedTextBox.ConfirmOnReturnCommandProperty, value); }
    }

    #endregion

    #region PublicMembers

    public void AppendLine(string line)
    {
        Text += line + Environment.NewLine;
    }

    public void AppendLines(IEnumerable<string> lines)
    {
        StringBuilder sb = new StringBuilder();
        foreach (string line in lines)
        {
            sb.AppendLine(line);
        }

        AppendLine(sb.ToString());
    }

    #endregion

    #region PrivateMembers

    private void _OnTextPropertyChanged(AvaloniaPropertyChangedEventArgs args)
    {
        if (!IsCaretAtEnd && UpdateCaretPositionAtEnd)
        {
            _ScrollToEnd();
        }
    }

    private void _ScrollToEnd()
    {
        _scrollViewer?.ScrollToEnd();
        CaretIndex = Text?.Length ?? 0;
        _UpdateIsCaretAtEnd();
    }

    private void _UpdateIsCaretAtEnd()
    {
        IsCaretAtEnd = CaretIndex >= (Text?.Length ?? 0);
    }

    private void _OnConfirmOnReturnPropertyChanged()
    {
        if(ConfirmOnReturn && AcceptsReturn)
        {
            AcceptsReturn = false;
        }
    }

    private void _OnAcceptsReturnPropertyChanged()
    {
        if (ConfirmOnReturn && AcceptsReturn)
        {
            ConfirmOnReturn = false;
        }
    }

    #endregion

    #region InheritedMembers

    // So this class is a TextBox with some extra functionality.
    // Without this, the SmartTextBox would not be rendered.
    protected override Type StyleKeyOverride
    {
        get { return typeof(TextBox); }
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == TextBox.TextProperty)
        {
            _OnTextPropertyChanged(change);
        }
        else if (change.Property == TextBox.CaretIndexProperty)
        {
            _UpdateIsCaretAtEnd();
        }
        else if (change.Property == EnchancedTextBox.ConfirmOnReturnProperty)
        {
            _OnConfirmOnReturnPropertyChanged();
        }
        else if (change.Property == TextBox.AcceptsReturnProperty)
        {
            _OnAcceptsReturnPropertyChanged();
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
        else if(ConfirmOnReturn && _confirmOnReturnGesture.Matches(e))
        {

            if(ConfirmReturn != null)
            {
                RoutedEventArgs args = new RoutedEventArgs()
                {
                    Source = this,
                };

                ConfirmReturn.Invoke(this, args);
            }

            ConfirmOnReturnCommand?.Execute(Text);
        }
    }
    
    #endregion
}