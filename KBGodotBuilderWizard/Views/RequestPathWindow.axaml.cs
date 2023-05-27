using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using KBAvaloniaCore.Controls;
using KBAvaloniaCore.IO;

namespace KBGodotBuilderWizard.Views;

public partial class RequestPathWindow : Window
{
    private Path? _path;
    private TextBoxPath _pathTextBox;
    public RequestPathWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        _pathTextBox = this.FindControl<TextBoxPath>("tb_path");
    }

    public Path? Path
    {
        get { return _pathTextBox.Path;}
    }
    
    private void _OnConfirmButtonClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
    
    private void _OnCancelButtonClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
}