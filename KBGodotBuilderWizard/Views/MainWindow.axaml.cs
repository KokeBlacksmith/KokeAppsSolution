using System;
using System.IO;
using System.Reflection;
using Avalonia.Controls;
using KBGodotBuilderWizard.Models;

namespace KBGodotBuilderWizard.Views
{
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
            ConfigurationFileData configurationFileData = new ConfigurationFileData();
            configurationFileData.Load();
            if (configurationFileData.IsValid(out _))
            {
                return;
            }
            
            ConfigurationWindow configurationWidow = new ConfigurationWindow();
            configurationWidow.ShowDialog(this);

        }

    }
}