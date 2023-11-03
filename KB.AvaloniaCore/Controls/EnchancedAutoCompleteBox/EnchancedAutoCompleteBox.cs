using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using KB.AvaloniaCore.ReactiveUI;

namespace KB.AvaloniaCore.Controls;

public class EnchancedAutoCompleteBox : AutoCompleteBox
{
    private readonly KeyGesture _confirmOnReturnGesture = new KeyGesture(Key.Enter);
    private readonly KeyGesture _tabSelectSuggestionGesture = new KeyGesture(Key.Tab);
    private TextBox? _textBox;

    #region Events
    public event EventHandler<RoutedEventArgs>? ConfirmReturn;
    #endregion

    #region StyledProperties

    public readonly static StyledProperty<bool> ConfirmOnReturnProperty = AvaloniaProperty.Register<EnchancedAutoCompleteBox, bool>(nameof(EnchancedAutoCompleteBox.ConfirmOnReturn));
    public readonly static StyledProperty<GenericCommand<string?>> ConfirmOnReturnCommandProperty = AvaloniaProperty.Register<EnchancedAutoCompleteBox, GenericCommand<string?>>(nameof(EnchancedAutoCompleteBox.ConfirmOnReturnCommand));
    public readonly static StyledProperty<bool> SilenceTabKeyProperty = AvaloniaProperty.Register<EnchancedAutoCompleteBox, bool>(nameof(EnchancedAutoCompleteBox.SilenceTabKey));

    public bool ConfirmOnReturn
    {
        get { return GetValue(EnchancedAutoCompleteBox.ConfirmOnReturnProperty); }
        set { SetValue(EnchancedAutoCompleteBox.ConfirmOnReturnProperty, value); }
    }

    public GenericCommand<string?> ConfirmOnReturnCommand
    {
        get { return GetValue(EnchancedAutoCompleteBox.ConfirmOnReturnCommandProperty); }
        set { SetValue(EnchancedAutoCompleteBox.ConfirmOnReturnCommandProperty, value); }
    }

    public bool SilenceTabKey
    {
        get { return GetValue(EnchancedAutoCompleteBox.SilenceTabKeyProperty); }
        set { SetValue(EnchancedAutoCompleteBox.SilenceTabKeyProperty, value); }
    }

    #endregion

    public void SelectAllText()
    {
        _textBox!.SelectAll();
    }

    public void UnselectAllText()
    {
        if(_textBox!.Text != null)
        {
            _textBox.SelectionStart = _textBox.Text.Length;
            _textBox.SelectionEnd = _textBox.Text.Length;
        }
    }

    #region InheritedMembers

    // So this class is a TextBox with some extra functionality.
    // Without this, the SmartTextBox would not be rendered.
    protected override Type StyleKeyOverride
    {
        get { return typeof(AutoCompleteBox); }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _textBox = e.NameScope.Find<TextBox>("PART_TextBox")!;
    }


    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (ConfirmOnReturn && _confirmOnReturnGesture.Matches(e))
        {

            if (ConfirmReturn != null)
            {
                RoutedEventArgs args = new RoutedEventArgs()
                {
                    Source = this,
                };

                ConfirmReturn.Invoke(this, args);
            }

            ConfirmOnReturnCommand?.Execute(Text);
            _textBox!.Focus();
            e.Handled = true;
        }
        else if(SilenceTabKey && _tabSelectSuggestionGesture.Matches(e))
        {
            e.Handled = true;
        }
    }

    #endregion
}
