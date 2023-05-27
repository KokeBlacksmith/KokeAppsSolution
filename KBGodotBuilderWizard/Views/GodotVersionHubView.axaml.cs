using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using KBAvaloniaCore.MessageBox;
using KBAvaloniaCore.ReactiveUI;
using KBGodotBuilderWizard.Models;

namespace KBGodotBuilderWizard.Views;

public partial class GodotVersionHubView : UserControl
{
    public GodotVersionHubView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}