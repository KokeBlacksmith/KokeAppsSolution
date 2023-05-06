using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace KBGodotBuilderWizard.Views;

public partial class ConfigurationWindow : Window
{
    public ConfigurationWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}