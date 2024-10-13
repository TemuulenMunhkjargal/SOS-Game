using SOSGameLib;
using System.Windows;
using System.Windows.Controls;

namespace SOSGameUI
{
    public partial class MainWindow : Window
    {
        private SOSGameMain _game;
        private Player _currentPlayer;

        public MainWindow()
        {
            InitializeComponent();
            _currentPlayer = Player.Red;
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            int boardSize = int.Parse(BoardSizeTextBox.Text);
            GameMode mode = SimpleRadioButton.IsChecked == true ? GameMode.Simple : GameMode.General;

            _game = new SOSGameMain(boardSize, mode);

            // Initialize the game board (e.g., display grid)
            InitializeBoard(boardSize);
        }

        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            var button = (System.Windows.Controls.Button)sender;
            int row = System.Windows.Controls.Grid.GetRow(button);
            int col = System.Windows.Controls.Grid.GetColumn(button);
            char letter = LetterComboBox.SelectedValue.ToString()[0];

            if (_game.MakeMove(row, col, _currentPlayer, letter))
            {
                button.Content = letter;
                _currentPlayer = _currentPlayer == Player.Red ? Player.Blue : Player.Red; // Switch turns
            }
            else
            {
                MessageBox.Show("Invalid Move!");
            }
        }

        private void InitializeBoard(int size)
        {
            GameGrid.RowDefinitions.Clear();
            GameGrid.ColumnDefinitions.Clear();
            GameGrid.Children.Clear();

            // Create row and column definitions for the Grid
            for (int i = 0; i < size; i++)
            {
                GameGrid.RowDefinitions.Add(new RowDefinition());
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Add buttons to the grid and assign row/column positions
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    var button = new System.Windows.Controls.Button();
                    button.Click += Cell_Click;
                    GameGrid.Children.Add(button);
                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);
                }
            }
        }
    }
}