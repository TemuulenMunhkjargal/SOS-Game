using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using SOSGameLib;

namespace SOSGameUI
{
    public partial class GameWindow : Window
    {
        private SOSGameMain _game;
        private ComputerPlayer _computerPlayer;
        private bool _isRedComputer;
        private bool _isBlueComputer;
        private System.Windows.Threading.DispatcherTimer _computerMoveTimer;

        public GameWindow(int boardSize, string gameMode, bool redIsComputer, bool blueIsComputer)
        {
            InitializeComponent();
            GameModeTextBlock.Text = gameMode;
            _isRedComputer = redIsComputer;
            _isBlueComputer = blueIsComputer;

            if (gameMode == "Simple")
            {
                _game = new SimpleGame(boardSize);
            }
            else
            {
                _game = new GeneralGame(boardSize);
            }

            _computerPlayer = new ComputerPlayer();

            InitializeBoard(boardSize);
            UpdateTurnDisplay();
            UpdateScoreDisplay();

            // Initialize timer for computer moves
            _computerMoveTimer = new System.Windows.Threading.DispatcherTimer();
            _computerMoveTimer.Tick += ComputerMove_Tick;
            _computerMoveTimer.Interval = TimeSpan.FromSeconds(1);

            // Start computer move if red is computer
            if (_isRedComputer && _game.CurrentPlayer == Player.Red)
            {
                _computerMoveTimer.Start();
            }
        }
        private void ComputerMove_Tick(object sender, EventArgs e)
        {
            _computerMoveTimer.Stop();

            if ((_game.CurrentPlayer == Player.Red && _isRedComputer) ||
                (_game.CurrentPlayer == Player.Blue && _isBlueComputer))
            {
                var (row, col, letter) = _computerPlayer.GetMove(_game);

                // Find the button at the selected position
                var button = GetButtonAt(row, col);
                if (button != null)
                {
                    // Update the UI to show the selected letter
                    SButton.IsChecked = (letter == 'S');
                    OButton.IsChecked = (letter == 'O');

                    // Make the move
                    if (_game.MakeMove(row, col, letter))
                    {
                        button.Content = letter;
                        UpdateScoreDisplay();
                        UpdateTurnDisplay();

                        if (_game.GameOver)
                        {
                            string winnerMessage = _game.Winner == Player.None ? "It's a draw!" : $"{_game.Winner} wins!";
                            MessageBox.Show(winnerMessage, "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            // Schedule next computer move if needed
                            if ((_game.CurrentPlayer == Player.Red && _isRedComputer) ||
                                (_game.CurrentPlayer == Player.Blue && _isBlueComputer))
                            {
                                _computerMoveTimer.Start();
                            }
                        }
                    }
                }
            }
        }
        private Button GetButtonAt(int row, int col)
        {
            foreach (Button button in GameGrid.Children)
            {
                if (Grid.GetRow(button) == row && Grid.GetColumn(button) == col)
                {
                    return button;
                }
            }
            return null;
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
            // Ignore clicks if it's a computer's turn
            if ((_game.CurrentPlayer == Player.Red && _isRedComputer) ||
                (_game.CurrentPlayer == Player.Blue && _isBlueComputer))
            {
                return;
            }

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
                else
                {
                    // Schedule computer move if it's computer's turn
                    if ((_game.CurrentPlayer == Player.Red && _isRedComputer) ||
                        (_game.CurrentPlayer == Player.Blue && _isBlueComputer))
                    {
                        _computerMoveTimer.Start();
                    }
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