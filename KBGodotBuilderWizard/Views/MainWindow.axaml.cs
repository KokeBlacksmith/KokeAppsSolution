using System;
using Avalonia.Controls;
using KBAvaloniaCore.Miscellaneous;
using KBGodotBuilderWizard.Models;

namespace KBGodotBuilderWizard.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        _CheckForSaveDataConfiguration();
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
        configurationWidow.ShowDialog(this);
    }
}