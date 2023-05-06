using System.Data;
using System.Windows.Input;
using Avalonia.Controls;
using KBAvaloniaCore.Miscellaneous;
using ReactiveUI;

namespace AvaloniaTestApp.ViewModels;
public class MainWindowViewModel : BaseViewModel
{
    private string _textBoxString = "Hello World!";
    private string _buttonString = "First button!";
    private int _buttonCountClick = 0;
    private int _buttonSpinner = 10;
    
    public MainWindowViewModel()
    {
    }
    
    public string Greeting => "Welcome to Avalonia!";

    public string TextBoxString
    {
        get
        {
            return _textBoxString;
        }
        set
        {
            this.RaiseAndSetIfChanged(ref _textBoxString, value);
        }
    }
    
    public string ButtonString
    {
        get
        {
            return _buttonString + _buttonCountClick;
        }
    }
    
    public int ButtonSpinnerNum
    {
        get
        {
            return _buttonSpinner;
        }
        set
        {
            this.RaiseAndSetIfChanged(ref _buttonSpinner, value);
        }
    }
    
    public ICommand ButtonClickCommand => ReactiveCommand.Create(ButtonClick); 
    
    public void ButtonClick()
    {
        _buttonCountClick++;
        Window w = new Window();
        w.Show();
        this.RaisePropertyChanged(nameof(MainWindowViewModel.ButtonString));
    }
}
