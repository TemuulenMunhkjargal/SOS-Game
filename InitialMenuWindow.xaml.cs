using System.Windows;
using System.Windows.Controls;

namespace SOSGameUI
{
    public partial class InitialMenuWindow : Window
    {
        public InitialMenuWindow()
        {
            InitializeComponent();
        }

        private void PlayGame_Click(object sender, RoutedEventArgs e)
        {
            // Validate board size
            if (!int.TryParse(BoardSizeTextBox.Text, out int boardSize) || boardSize <= 2)
            {
                MessageBox.Show("Please enter a valid board size (greater than 2).", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Get game mode
            string gameMode = (GameModeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(gameMode))
            {
                MessageBox.Show("Please select a game mode.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Get player types
            bool redIsComputer = RedComputerRadio.IsChecked ?? false;
            bool blueIsComputer = BlueComputerRadio.IsChecked ?? false;

            // Open Game Window and pass values
            GameWindow gameWindow = new GameWindow(boardSize, gameMode, redIsComputer, blueIsComputer);
            gameWindow.Show();
            this.Close();  // Close the initial menu window
        }
    }
}