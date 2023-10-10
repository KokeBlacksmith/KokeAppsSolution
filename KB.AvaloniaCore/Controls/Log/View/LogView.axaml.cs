using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using KB.AvaloniaCore.Controls.Log;
using KB.AvaloniaCore.ReactiveUI;

namespace KB.AvaloniaCore.Controls;

public partial class LogView : UserControl
{
    private readonly IDisposable[] _disposableObservers = new IDisposable[2];
    
    public LogView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        
        // styled properties
        _disposableObservers[0] = this.SubscribeToStyledPropertyChanged(LogView.MessagesProperty, _OnMessagesPropertyChanged);
        _disposableObservers[1] = this.SubscribeToStyledPropertyChanged(LogView.ColorizeMessagesProperty, _OnColorizeMessagesPropertyChanged);
        
        // xaml controls
        _messagesListView = this.FindControl<ItemsControl>(nameof(LogView._messagesListView));
    }

    #region StyledProperties

    public readonly static StyledProperty<ICollection<LogMessage>> MessagesProperty = 
        AvaloniaProperty.Register<LogView, ICollection<LogMessage>>(nameof(LogView.Messages));
    
    public readonly static StyledProperty<bool> ColorizeMessagesProperty = 
        AvaloniaProperty.Register<LogView, bool>(nameof(LogView.ColorizeMessages), true);
    
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
        _messagesListView.ItemsSource = args.GetNewValue<ICollection<LogMessage>>();
    }
    
    private void _OnColorizeMessagesPropertyChanged(LogView sender, AvaloniaPropertyChangedEventArgs args)
    {
        
    }
    
    #endregion
    
    #region ViewMethods
    
    private void _OnClearButtonClick(object sender, RoutedEventArgs e)
    {
        if(Messages == null || !Messages.Any())
        {
            return;
        }

        Messages.Clear();
    }
    
    private void _OnSaveButtonClick(object sender, RoutedEventArgs e)
    {
        if(Messages == null || !Messages.Any())
        {
            return;
        }
    }
    
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        foreach (IDisposable observer in _disposableObservers)
        {
            observer.Dispose();
        }
    }
    
    #endregion
}