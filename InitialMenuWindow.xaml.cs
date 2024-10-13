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
            // Get user input
            int boardSize = int.Parse(BoardSizeTextBox.Text);
            string gameMode = (GameModeComboBox.SelectedItem as ComboBoxItem).Content.ToString();

            // Open Game Window and pass values
            GameWindow gameWindow = new GameWindow(boardSize, gameMode);
            gameWindow.Show();
            this.Close();  // Close the initial menu window
        }
    }
}