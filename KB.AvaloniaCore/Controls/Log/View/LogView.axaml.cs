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
    public readonly static StyledProperty<ICollection<LogMessage>> MessagesProperty = 
        AvaloniaProperty.Register<LogView, ICollection<LogMessage>>(nameof(LogView.Messages));
    
    public readonly static StyledProperty<bool> ColorizeMessagesProperty = 
        AvaloniaProperty.Register<LogView, bool>(nameof(LogView.ColorizeMessages), true);
    
    public LogView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        _messagesListView = this.FindControl<ItemsControl>(nameof(LogView._messagesListView));
    }
    
    
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


    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        LogView self = (LogView)change.Sender;
        if (change.Property == LogView.MessagesProperty)
        {
            self._messagesListView.ItemsSource = change.GetNewValue<ICollection<LogMessage>>();
        }
        else if (change.Property == LogView.ColorizeMessagesProperty)
        {
            //self._messagesListView.ItemTemplate = self._messagesListView.ItemTemplate;
        }
        
        // else if (change.Property == TextBoxPath.HorizontalAlignmentProperty)
        // {
        //     textBoxPath._container.HorizontalAlignment = change.GetNewValue<HorizontalAlignment>();
        // }
        // else if (change.Property == TextBoxPath.VerticalAlignmentProperty)
        // {
        //     textBoxPath._container.VerticalAlignment = change.GetNewValue<VerticalAlignment>();
        // }
    }
    
    private void _OnClearButtonClick(object sender, RoutedEventArgs e)
    {
        if(Messages == null || !Messages.Any())
        {
            return;
        }

        if(this.StyledPropertyHasBindingMode(LogView.MessagesProperty, BindingMode.TwoWay))
        {
            Messages = (Activator.CreateInstance(Messages.GetType()) as ICollection<LogMessage>)!;
        }
        else
        {
            Messages.Clear();
        }
    }
    
    private void _OnSaveButtonClick(object sender, RoutedEventArgs e)
    {
        if(Messages == null || !Messages.Any())
        {
            return;
        }
    }
}