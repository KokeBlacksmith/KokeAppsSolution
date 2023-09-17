using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using KB.SharpCore.IO;

namespace KB.AvaloniaCore.Controls;

public partial class TextBoxPath : UserControl
{
    public readonly static StyledProperty<string> PathTextProperty = AvaloniaProperty.Register<TextBoxPath, string>(nameof(TextBoxPath.PathText), String.Empty);

    public readonly static StyledProperty<EPathType> PathTypeProperty = AvaloniaProperty.Register<TextBoxPath, EPathType>(nameof(TextBoxPath.PathType), EPathType.Directory);

    public TextBoxPath()
    {
        InitializeComponent();
    }

    public string PathText
    {
        get { return GetValue(TextBoxPath.PathTextProperty); }
        set
        {
            SetValue(TextBoxPath.PathTextProperty, value);
            if (String.IsNullOrWhiteSpace(value))
            {
                Path = null;
            }
            else
            {
                Path = new KB.SharpCore.IO.Path(value);
            }
        }
    }
    
    public KB.SharpCore.IO.Path? Path { get; private set; }

    public EPathType PathType
    {
        get { return GetValue(TextBoxPath.PathTypeProperty); }
        set { SetValue(TextBoxPath.PathTypeProperty, value); }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        _container = this.FindControl<Grid>(nameof(TextBoxPath._container));
        _tbPath = this.FindControl<TextBox>(nameof(TextBoxPath._tbPath));
        _btSearchPath = this.FindControl<Button>(nameof(TextBoxPath._btSearchPath));
        _btSearchPath.Click += _OnSearchPathClick;
    }

    private async void _OnSearchPathClick(object sender, RoutedEventArgs e)
    {
        Window parentWindow = this.FindAncestorOfType<Window>();
        string selectedDirectory = null;

        if (PathType == EPathType.Directory)
        {
            OpenFolderDialog dialog = new OpenFolderDialog();
            selectedDirectory = await dialog.ShowAsync(parentWindow);
        }
        else
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.AllowMultiple = false;
            selectedDirectory = (await dialog.ShowAsync(parentWindow))?.FirstOrDefault();
        }

        if (!String.IsNullOrWhiteSpace(selectedDirectory))
        {
            PathText = selectedDirectory;
        }
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        TextBoxPath textBoxPath = (TextBoxPath)change.Sender;
        if (change.Property == TextBoxPath.PathTextProperty)
        {
            textBoxPath._tbPath.Text = change.GetNewValue<string>();
        }
        else if (change.Property == TextBoxPath.HorizontalAlignmentProperty)
        {
            textBoxPath._container.HorizontalAlignment = change.GetNewValue<HorizontalAlignment>();
        }
        else if (change.Property == TextBoxPath.VerticalAlignmentProperty)
        {
            textBoxPath._container.VerticalAlignment = change.GetNewValue<VerticalAlignment>();
        }
    }
}