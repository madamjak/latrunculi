using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfApp.Helpers;

namespace WpfApp.Views
{
    /// <summary>
    /// Interaction logic for UserControl2.xaml
    /// </summary>
    public partial class GameplayControl : UserControl
    {
        private const int GridRowNumbers = 7;
        private const int GridColumnNumbers = 8;

        public GameplayControl()
        {
            InitializeComponent();
            SetupBoard();
        }

        private void SetupBoard()
        {
            for (int i = -1; i < GridRowNumbers; i++)
            {
                if(i == -1)
                {
                    var row = new RowDefinition();
                    row.Height = new GridLength(0.1, GridUnitType.Star);
                    Board.RowDefinitions.Add(row);
                }
                else
                {
                    var row = new RowDefinition();
                    row.Height = new GridLength(1, GridUnitType.Star);
                    row.MinHeight = 50;
                    Board.RowDefinitions.Add(row);
                }
                

                for (int j = -1; j < GridColumnNumbers; j++)
                {
                    if(i == -1)
                    {
                        if(j == -1)
                        {
                            var column = new ColumnDefinition();
                            column.Width = new GridLength(0.2, GridUnitType.Star);
                            Board.ColumnDefinitions.Add(column);
                        }
                        else
                        {
                            var column = new ColumnDefinition();
                            column.Width = new GridLength(1, GridUnitType.Star);
                            column.MinWidth = 50;
                            Board.ColumnDefinitions.Add(column);
                        }
                    }
                    
                    if(i == -1 && j > -1)
                    {
                        var header = new Image();
                        header.Stretch = System.Windows.Media.Stretch.Fill;
                        header.Source = BoardImagesHelper.GetBoardColumnLabelImageSource(j);
                        header.SetValue(Grid.RowProperty, i + 1);
                        header.SetValue(Grid.ColumnProperty, j + 1);

                        Board.Children.Add(header);
                    }

                    if (i > -1 && j == -1)
                    {
                        var header = new Image();
                        header.Stretch = System.Windows.Media.Stretch.Fill;
                        header.Source = BoardImagesHelper.GetBoardRowLabelImageSource(i);
                        header.SetValue(Grid.RowProperty, i + 1);
                        header.SetValue(Grid.ColumnProperty, j + 1);

                        Board.Children.Add(header);
                    }

                    //actual board
                    if (i > -1 && j > -1)
                    {
                        var image = new Image();
                        image.SetBinding(Image.SourceProperty, new Binding(string.Format("Model.Board[{0}][{1}].ImageSource", i, j)));
                        image.Stretch = System.Windows.Media.Stretch.Fill;

                        var b = new Button();
                        b.SetBinding(System.Windows.Controls.Primitives.ButtonBase.CommandProperty, new Binding("PositionSelected"));
                        b.SetBinding(System.Windows.Controls.Primitives.ButtonBase.CommandParameterProperty, new Binding(string.Format("Model.Board[{0}][{1}]", i, j)));
                        b.Content = image;

                        b.SetValue(Grid.RowProperty, i + 1);
                        b.SetValue(Grid.ColumnProperty, j + 1);

                        Board.Children.Add(b);
                    }
                }
            }
        }

        private void Lightbox_close(object sender, RoutedEventArgs e)
        {
            Lightbox.Visibility = Visibility.Hidden;
        }

        private void Lightbox_open(object sender, RoutedEventArgs e)
        {
            Lightbox.Visibility = Visibility.Visible;
        }
    }
}
