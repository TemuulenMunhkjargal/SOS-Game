using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOSGameLib
{
    public class GeneralGame : SOSGameMain
    {
        public override int RedScore { get; set; }
        public override int BlueScore { get; set; }

        public GeneralGame(int boardSize) : base(boardSize)
        {
            RedScore = 0;
            BlueScore = 0;
        }

        public override bool MakeMove(int row, int col, char letter)
        {
            if (GameOver || Board[row, col] != Player.None) return false;

            Board[row, col] = CurrentPlayer;

            if (CheckForSOS(row, col, letter))
            {
                // Increment the score of the current player
                if (CurrentPlayer == Player.Red)
                    RedScore++;
                else
                    BlueScore++;
            }
            else
            {
                // Switch turns only if no SOS was formed
                CurrentPlayer = CurrentPlayer == Player.Red ? Player.Blue : Player.Red;
            }

            // Check if the board is full
            if (IsBoardFull())
            {
                GameOver = true;
                if (RedScore > BlueScore)
                    Winner = Player.Red;
                else if (BlueScore > RedScore)
                    Winner = Player.Blue;
                else
                    Winner = Player.None;  // Draw
            }

            return true;
        }

    }
}
