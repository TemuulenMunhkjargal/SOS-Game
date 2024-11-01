using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using SOSGameLib;

namespace SOSGameUI
{
    public partial class GameWindow : Window
    {
        private SOSGameMain _game;

        public GameWindow(int boardSize, string gameMode)
        {
            InitializeComponent();
            GameModeTextBlock.Text = gameMode;

            if (gameMode == "Simple")
            {
                _game = new SimpleGame(boardSize);
            }
            else
            {
                _game = new GeneralGame(boardSize);
            }

            InitializeBoard(boardSize);
            UpdateTurnDisplay();
            UpdateScoreDisplay();
        }

        private void InitializeBoard(int size)
        {
            GameGrid.Children.Clear();
            GameGrid.RowDefinitions.Clear();
            GameGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < size; i++)
            {
                GameGrid.RowDefinitions.Add(new RowDefinition());
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    Button button = new Button();
                    button.Click += Cell_Click;
                    GameGrid.Children.Add(button);
                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);
                }
            }
        }

        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int row = Grid.GetRow(button);
            int col = Grid.GetColumn(button);

            char selectedLetter = SButton.IsChecked == true ? 'S' : 'O';

            if (_game.MakeMove(row, col, selectedLetter))
            {
                button.Content = selectedLetter;
                UpdateScoreDisplay();
                UpdateTurnDisplay();

                if (_game.GameOver)
                {
                    string winnerMessage = _game.Winner == Player.None ? "It's a draw!" : $"{_game.Winner} wins!";
                    MessageBox.Show(winnerMessage, "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Invalid Move! Cell is occupied.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LetterButton_Click(object sender, RoutedEventArgs e)
        {
            var clickedButton = (ToggleButton)sender;
            
            // Ensure one button is always selected
            if (clickedButton == SButton)
            {
                OButton.IsChecked = false;
                SButton.IsChecked = true;
            }
            else
            {
                SButton.IsChecked = false;
                OButton.IsChecked = true;
            }
        }

        private void UpdateScoreDisplay()
        {
            if (_game is GeneralGame generalGame)
            {
                RedScoreTextBlock.Text = generalGame.RedScore.ToString();
                BlueScoreTextBlock.Text = generalGame.BlueScore.ToString();
            }
            else
            {
                RedScoreTextBlock.Text = "N/A";
                BlueScoreTextBlock.Text = "N/A";
            }
        }

        private void UpdateTurnDisplay()
        {
            CurrentTurnTextBlock.Text = _game.CurrentPlayer == Player.Red ? "Red's Turn" : "Blue's Turn";
        }

        private void EndGame_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Game Ended");
            this.Close();
        }
    }
}