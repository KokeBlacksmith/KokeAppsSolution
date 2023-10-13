using System.Collections.Specialized;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using KB.AvaloniaCore.Controls.Log;
using KB.AvaloniaCore.ReactiveUI;
using KB.SharpCore.Collections;
using KB.SharpCore.Utils;

namespace KB.AvaloniaCore.Controls;

public partial class LogView : UserControl
{
    private ILogView _currentLogView;

    public LogView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        // styled properties
        this.SubscribeToStyledPropertyChanged(LogView.MessagesProperty, _OnMessagesPropertyChanged);
        this.SubscribeToStyledPropertyChanged(LogView.ColorizeMessagesProperty, _OnColorizeMessagesPropertyChanged);

        // xaml controls
        // _messagessListView = this.FindControl<ItemsControl>(nameof(LogView._messagesListView));
        _currentLogView = this.FindControl<TextLogView>(nameof(LogView._logTextBlock))!;
    }

    #region StyledProperties

    public readonly static StyledProperty<ICollection<LogMessage>> MessagesProperty = AvaloniaProperty.Register<LogView, ICollection<LogMessage>>(nameof(LogView.Messages));

    public readonly static StyledProperty<bool> ColorizeMessagesProperty = AvaloniaProperty.Register<LogView, bool>(nameof(LogView.ColorizeMessages), true);

    public ICollection<LogMessage> Messages
    {
        get { return GetValue(LogView.MessagesProperty); }
        set { SetValue(LogView.MessagesProperty, value); }
    }

    public bool ColorizeMessages
    {
        get { return GetValue(LogView.ColorizeMessagesProperty); }
        set { SetValue(LogView.ColorizeMessagesProperty, value); }
    }

    #endregion

    #region StyledPropertyChanged

    private void _OnMessagesPropertyChanged(LogView sender, AvaloniaPropertyChangedEventArgs args)
    {
        // _messagesListView.ItemsSource = args.GetNewValue<ICollection<LogMessage>>();
        if(args.OldValue is INotifyCollectionChanged oldCollectionNotify)
        {
            oldCollectionNotify.CollectionChanged -= _OnMessagesCollectionNotifyChange;
        }

        _currentLogView!.Clear();
        _currentLogView!.AddMessages(args.GetNewValue<ICollection<LogMessage>>());
        
        if(args.NewValue is INotifyCollectionChanged newCollectionNotify)
        {
            newCollectionNotify.CollectionChanged += _OnMessagesCollectionNotifyChange;
        }
    }

    private void _OnMessagesCollectionNotifyChange(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (CollectionHelper.HasAny(args.NewItems))
        {
            _currentLogView!.AddMessages(args.NewItems!.Cast<LogMessage>());
        }
    }

    private void _OnColorizeMessagesPropertyChanged(LogView sender, AvaloniaPropertyChangedEventArgs args) { }

    #endregion

    #region ViewMethods

    private void _OnClearButtonClick(object sender, RoutedEventArgs e)
    {
        if (Messages == null || !Messages.Any())
        {
            return;
        }

        Messages.Clear();
        _currentLogView.Clear();
    }

    private void _OnSaveButtonClick(object sender, RoutedEventArgs e)
    {
        if (Messages == null || !Messages.Any())
        {
            return;
        }
    }

    #endregion
}