using Avalonia.Controls;

namespace KB.ConsoleCompanionStandalone.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public string[]? Args
        {
            get { return m_ConsoleCompanionView.Args; }
            set 
            { 
                m_ConsoleCompanionView.Args = value;
            }
        }
    }
}