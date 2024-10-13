using SOSGameLib;
using System.Windows;
using System.Windows.Controls;

namespace SOSGameUI
{
    public partial class GameWindow : Window
    {
        private SOSGameMain _game;
        private Player _currentPlayer;

        public GameWindow(int boardSize, string gameMode)
        {
            InitializeComponent();

            // Initialize the game based on the board size and game mode
            GameMode mode = gameMode == "Simple" ? GameMode.Simple : GameMode.General;
            _game = new SOSGameMain(boardSize, mode);

            // Set initial player turn
            _currentPlayer = Player.Red;

            // Update game mode in UI
            GameModeTextBlock.Text = gameMode;

            // Initialize the game grid
            InitializeBoard(boardSize);
        }

        private void InitializeBoard(int size)
        {
            GameGrid.RowDefinitions.Clear();
            GameGrid.ColumnDefinitions.Clear();
            GameGrid.Children.Clear();

            // Create row and column definitions for the grid
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
                    var button = new Button();
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

            // Ensure that the player has selected a letter (S or O)
            if (LetterComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a letter (S or O) before making a move.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Exit the method early if no selection is made
            }

            // Check if the cell already contains an S or O
            if (button.Content != null)
            {
                MessageBox.Show("This cell is already occupied. Please choose another cell.", "Invalid Move", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Exit the method early if the cell is already occupied
            }

            // Retrieve the selected letter (S or O)
            char letter = (LetterComboBox.SelectedItem as ComboBoxItem).Content.ToString()[0];

            // Now proceed with making the move
            if (_game.MakeMove(row, col, _currentPlayer, letter))
            {
                button.Content = letter;  // Set the content of the button to the chosen letter
                _currentPlayer = _currentPlayer == Player.Red ? Player.Blue : Player.Red;  // Switch turns

                // Update current turn in UI
                CurrentTurnTextBlock.Text = _currentPlayer == Player.Red ? "Red" : "Blue";
            }
            else
            {
                MessageBox.Show("Invalid Move!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EndGame_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Game Ended");
            this.Close(); // Close the game window
        }
    }
}
