using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOSGameLib
{
    public class SimpleGame : SOSGameMain
    {
        public override int RedScore { get; set; }
        public override int BlueScore { get; set; }

        public SimpleGame(int boardSize) : base(boardSize) { }

        public override bool MakeMove(int row, int col, char letter)
        {
            if (GameOver || Board[row, col] != Player.None) return false;

            Board[row, col] = CurrentPlayer;

            if (CheckForSOS(row, col, letter))
            {
                Winner = CurrentPlayer;
                GameOver = true;
            }
            else
            {
                // Switch turns only if no SOS was formed
                CurrentPlayer = CurrentPlayer == Player.Red ? Player.Blue : Player.Red;
            }
            if (IsBoardFull())
            {
                GameOver = true;
                Winner = Player.None;  // Draw
            }

            return true;
        }
    }
}
