using Avalonia;
using Avalonia.Controls;
using ConsoleCompanionAPI.Data;
using Avalonia.Media;

namespace KB.ConsoleCompanion.CommandView;

public partial class CommandItemView : UserControl
{
    public readonly static StyledProperty<ConsoleCommand> ConsoleCommandProperty = AvaloniaProperty.Register<CommandItemView, ConsoleCommand>(nameof(CommandItemView.ConsoleCommand));

    private readonly IBrush _infoBackground;
    private readonly IBrush _warningBackground;
    private readonly IBrush _errorBackground;
    private readonly IBrush _userInputBackground;

    public CommandItemView()
    {
        InitializeComponent();
        _infoBackground = Brush.Parse("#2ADFF4");
        _warningBackground = Brush.Parse("#E4DE14");
        _errorBackground = Brush.Parse("#C22929");
        _userInputBackground = Brush.Parse("#25B411");
    }

    public ConsoleCommand ConsoleCommand
    {
        get { return GetValue(CommandItemView.ConsoleCommandProperty); }
        set { SetValue(CommandItemView.ConsoleCommandProperty, value); }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if(change.Property == CommandItemView.ConsoleCommandProperty)
        {
            if(ConsoleCommand != null)
            {
                _typeTextBlock.Text = ConsoleCommand.Type.ToString();
                _messageTextBlock.Text = ConsoleCommand.Command;
                _timeTextBlock.Text = ConsoleCommand.Time.ToString("HH:mm tt");

                switch (ConsoleCommand.Type)
                {
                    case ConsoleCommand.ECommandType.Info:
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                        _border.Background = _infoBackground;
                        _messageTextBlock.Foreground = Brushes.White;
                        break;
                    case ConsoleCommand.ECommandType.Warning:
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                        _border.Background = _warningBackground;
                        _messageTextBlock.Foreground = Brushes.Orange;
                        break;
                    case ConsoleCommand.ECommandType.Error:
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                        _border.Background = _errorBackground;
                        _messageTextBlock.Foreground = Brushes.White;
                        break;
                    default:
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right;
                        _border.Background = _userInputBackground;
                        _messageTextBlock.Foreground = Brushes.White;
                        break;
                }
            }
        }
    }
}
