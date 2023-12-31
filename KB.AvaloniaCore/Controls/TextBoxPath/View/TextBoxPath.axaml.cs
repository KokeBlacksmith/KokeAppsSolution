using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using KB.SharpCore.IO;
using KB.SharpCore.Utils;

namespace KB.AvaloniaCore.Controls;

public partial class TextBoxPath : UserControl
{
    public readonly static StyledProperty<string> PathTextProperty = AvaloniaProperty.Register<TextBoxPath, string>(nameof(TextBoxPath.PathText), String.Empty);

    public readonly static StyledProperty<EPathType> PathTypeProperty = AvaloniaProperty.Register<TextBoxPath, EPathType>(nameof(TextBoxPath.PathType), EPathType.Directory);

    static TextBoxPath()
    {
        PathTextProperty.Changed.AddClassHandler<TextBoxPath>((textBoxPath, change) => textBoxPath._OnPathTextPropertyChanged(change));
        HorizontalAlignmentProperty.Changed.AddClassHandler<TextBoxPath>((textBoxPath, change) => textBoxPath._OnHorizontalAlignmentPropertyChanged(change));
        VerticalAlignmentProperty.Changed.AddClassHandler<TextBoxPath>((textBoxPath, change) => textBoxPath._OnVerticalAlignmentPropertyChanged(change));
    }


    public TextBoxPath()
    {
        InitializeComponent();
        PathText = String.Empty;
    }

    public string PathText
    {
        get { return GetValue(TextBoxPath.PathTextProperty); }
        set
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                Path = new KB.SharpCore.IO.Path();
            }
            else
            {
                Path = new KB.SharpCore.IO.Path(value);
            }

            SetValue(TextBoxPath.PathTextProperty, value);
        }
    }
    
    public KB.SharpCore.IO.Path Path { get; private set; }

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
        _btSearchPath!.Click += _OnSearchPathClick;
    }

    private async void _OnSearchPathClick(object? sender, RoutedEventArgs e)
    {
        Window parentWindow = this.FindAncestorOfType<Window>()!;
        TopLevel topLevel = TopLevel.GetTopLevel(this)!;
        string? selectedPath = null;

        string? currentPath = PathText;
        IStorageFolder? startPath = null;
        if (!String.IsNullOrWhiteSpace(currentPath))
        {
            startPath = await topLevel.StorageProvider.TryGetFolderFromPathAsync(currentPath!);
        }
        
        if (startPath == null)
        {
            startPath = await topLevel.StorageProvider.TryGetFolderFromPathAsync(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop));
        }

        if (PathType == EPathType.Directory)
        {
            IReadOnlyList<IStorageFolder> directories = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
            {
                AllowMultiple = false,
                SuggestedStartLocation = startPath
            });

            if(CollectionHelper.HasAny(directories))
            {
                IStorageFolder directory = directories.First();
                selectedPath = directory.Path.LocalPath;
                CollectionHelper.Dispose(directories);
            }
        }
        else
        {
            IReadOnlyList<IStorageFile> files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                AllowMultiple = false,
                SuggestedStartLocation = startPath
            });
            
            if (CollectionHelper.HasAny(files))
            {
                IStorageFile file = files.First();
                selectedPath = file.Path.LocalPath;
                CollectionHelper.Dispose(files);
            }
        }

        if (!String.IsNullOrWhiteSpace(selectedPath))
        {
            PathText = selectedPath;
        }
    }

    private void _OnPathTextPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        DataValidationErrors.ClearErrors(_tbPath);
        
        _tbPath.Text = Path.GetPath();
        if(!Path.IsValidPath())
        {
            string errorMessage = Path.IsEmpty ? "Path has no value" : $"Value: '{Path.GetPath()}'";
            DataValidationErrors.SetError(_tbPath, new DataValidationException($"Invalid path! {errorMessage}"));
        }
    }

    private void _OnHorizontalAlignmentPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        _container.HorizontalAlignment = change.GetNewValue<HorizontalAlignment>();
    }

    private void _OnVerticalAlignmentPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        _container.VerticalAlignment = change.GetNewValue<VerticalAlignment>();
    }
}