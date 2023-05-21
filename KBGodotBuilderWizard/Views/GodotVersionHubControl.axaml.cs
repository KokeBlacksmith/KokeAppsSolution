using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using KBAvaloniaCore.Miscellaneous;
using KBGodotBuilderWizard.Models;

namespace KBGodotBuilderWizard.Views;

public partial class GodotVersionHubControl : UserControl
{
    public GodotVersionHubControl()
    {
        InitializeComponent();
        _CheckForSaveDataConfiguration();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    private void _CheckForSaveDataConfiguration()
    {
        ConfigurationData configurationData = new ConfigurationData();
        Result loadResult = configurationData.Load();
        if (loadResult.IsFailure)
        {
            KBAvaloniaCore.Miscellaneous.MessageBoxHelper.ShowErrorDialog(loadResult);
            return;
        }
        
        if (configurationData.IsValid(out _))
        {
            return;
        }

        ConfigurationWindow configurationWidow = new ConfigurationWindow();
        configurationWidow.ShowDialog(this.GetVisualParent<Window>());
    }
}