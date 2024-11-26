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
        private GameRecorder _gameRecorder;  
        private GameReplayer _gameReplayer;  
        private int _currentReplayMove;      
        private bool _isReplayMode;          
        private System.Windows.Threading.DispatcherTimer _replayTimer;
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

            _gameRecorder = new GameRecorder(gameMode, boardSize, redIsComputer, blueIsComputer);
            _computerPlayer = new ComputerPlayer();

            InitializeBoard(boardSize);
            InitializeMenus(); 
            UpdateTurnDisplay();
            UpdateScoreDisplay();

            _computerMoveTimer = new System.Windows.Threading.DispatcherTimer();
            _computerMoveTimer.Tick += ComputerMove_Tick;
            _computerMoveTimer.Interval = TimeSpan.FromSeconds(1);

            _replayTimer = new System.Windows.Threading.DispatcherTimer();
            _replayTimer.Tick += ReplayMove_Tick;
            _replayTimer.Interval = TimeSpan.FromSeconds(1);

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
                var button = GetButtonAt(row, col);
                if (button != null)
                {
                    SButton.IsChecked = (letter == 'S');
                    OButton.IsChecked = (letter == 'O');

                    if (_game.MakeMove(row, col, letter))
                    {
                        // Record the computer's move
                        _gameRecorder.RecordMove(_game.CurrentPlayer, row, col, letter);

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
        private void InitializeMenus()
        {
            var menuBar = new Menu();
            var fileMenu = new MenuItem { Header = "_File" };

            var saveGame = new MenuItem { Header = "_Save Game" };
            saveGame.Click += SaveGame_Click;

            var loadGame = new MenuItem { Header = "_Load Game" };
            loadGame.Click += LoadGame_Click;

            fileMenu.Items.Add(saveGame);
            fileMenu.Items.Add(loadGame);
            menuBar.Items.Add(fileMenu);

            // Add the menu bar to the top of the window
            var mainGrid = (Grid)Content;
            mainGrid.RowDefinitions.Insert(0, new RowDefinition { Height = GridLength.Auto });
            Grid.SetRow(menuBar, 0);
            mainGrid.Children.Add(menuBar);

            // Adjust the rows of other elements
            foreach (UIElement element in mainGrid.Children)
            {
                if (element != menuBar)
                {
                    Grid.SetRow(element, Grid.GetRow(element) + 1);
                }
            }
        }
        private async void SaveGame_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                DefaultExt = ".sos",
                Filter = "SOS Game Files (*.sos)|*.sos"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    await _gameRecorder.SaveToFile(dialog.FileName);
                    MessageBox.Show("Game saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private async void LoadGame_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".sos",
                Filter = "SOS Game Files (*.sos)|*.sos"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _gameReplayer = await GameReplayer.LoadFromFile(dialog.FileName);
                    StartReplay();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void StartReplay()
        {
            // Reset the game state
            _isReplayMode = true;
            _currentReplayMove = 0;
            _computerMoveTimer.Stop();

            // Create new game with loaded settings
            if (_gameReplayer.GameMode == "Simple")
                _game = new SimpleGame(_gameReplayer.BoardSize);
            else
                _game = new GeneralGame(_gameReplayer.BoardSize);

            // Reset the UI
            InitializeBoard(_gameReplayer.BoardSize);
            UpdateTurnDisplay();
            UpdateScoreDisplay();

            // Start the replay
            _replayTimer.Start();
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
        private void ReplayMove_Tick(object sender, EventArgs e)
        {
            if (_currentReplayMove >= _gameReplayer.Moves.Count)
            {
                _replayTimer.Stop();
                _isReplayMode = false;
                MessageBox.Show("Replay finished!", "Replay Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var move = _gameReplayer.Moves[_currentReplayMove];
            if (_game.MakeMove(move.Row, move.Col, move.Letter))
            {
                var button = GetButtonAt(move.Row, move.Col);
                if (button != null)
                {
                    button.Content = move.Letter;
                    UpdateScoreDisplay();
                    UpdateTurnDisplay();
                }
            }

            _currentReplayMove++;
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
            if (_isReplayMode)
                return;

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
                // Record the move
                _gameRecorder.RecordMove(_game.CurrentPlayer, row, col, selectedLetter);

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