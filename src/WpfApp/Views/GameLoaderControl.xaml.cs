using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp.Views
{
    /// <summary>
    /// Interaction logic for GameLoaderControl.xaml
    /// </summary>
    public partial class GameLoaderControl : UserControl
    {
        public GameLoaderControl()
        {
            InitializeComponent();
        }

        private void Browse_save_click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog openFileDialog = new SaveFileDialog();
            openFileDialog.Filter = "txt files (*.txt)|*.txt|JSON files (*.json)|*.json";

            if (openFileDialog.ShowDialog() == true)
            {
                pathToFile.Text = openFileDialog.FileName;
                pathToFile.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }

        private void Browse_load_click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "txt files (*.txt)|*.txt|JSON files (*.json)|*.json";

            if (openFileDialog.ShowDialog() == true)
            {
                pathToFile.Text = openFileDialog.FileName;
                pathToFile.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }
    }
}
