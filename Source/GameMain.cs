using System;

namespace SOSGameLib
{

    public enum GameMode { Simple, General }
    public enum Player { None, Red, Blue }



    public class SOSGameMain
    {
        public int BoardSize { get; private set; }
        public GameMode Mode { get; private set; }
        public Cell[,] Board { get; private set; }

        public SOSGameMain(int boardSize, GameMode mode)
        {
            if (boardSize < 3)
                throw new ArgumentException("Board size must be greater than 2.");

            BoardSize = boardSize;
            Mode = mode;
            Board = new Cell[BoardSize, BoardSize];  // Initialize the board with cells
            InitializeBoard();
        }

        // Initialize the board with empty cells
        private void InitializeBoard()
        {
            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    Board[row, col] = new Cell();  // Set each board position to an empty cell
                }
            }
        }

        public bool MakeMove(int row, int col, Player player, char letter)
        {
            if (row < 0 || col < 0 || row >= BoardSize || col >= BoardSize)
                throw new ArgumentOutOfRangeException("Move is out of board range.");

            if (Board[row, col].Owner != Player.None)
                throw new InvalidOperationException("Cell is already occupied.");

            // Simulate S/O placement without checking SOS or determining winner yet.
            if (letter == 'S' || letter == 'O')
            {
                Board[row, col].Owner = player;   // Set the player who made the move
                Board[row, col].Letter = letter;  // Place the letter in the cell
                return true;
            }
            return false;
        }

        // Get the owner of the cell (which player occupies it)
        public Player GetCellOwner(int row, int col)
        {
            if (row < 0 || col < 0 || row >= BoardSize || col >= BoardSize)
                throw new ArgumentOutOfRangeException("Cell is out of range.");

            return Board[row, col].Owner;
        }

        // Get the letter ('S' or 'O') placed in the cell
        public char? GetCellLetter(int row, int col)
        {
            if (row < 0 || col < 0 || row >= BoardSize || col >= BoardSize)
                throw new ArgumentOutOfRangeException("Cell is out of range.");

            return Board[row, col].Letter;
        }
    }

    public class Cell
    {
        public Player Owner { get; set; }
        public char? Letter { get; set; }  // Nullable char to allow empty cells

        public Cell()
        {
            Owner = Player.None;
            Letter = null;  // No letter is placed initially
        }
    }
}