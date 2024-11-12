using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ComputerPlayer.cs
namespace SOSGameLib
{
    public class ComputerPlayer
    {
        private readonly Random _random = new Random();

        public (int row, int col, char letter) GetMove(SOSGameMain game)
        {
            // First try to make an SOS
            for (int row = 0; row < game.BoardSize; row++)
            {
                for (int col = 0; col < game.BoardSize; col++)
                {
                    if (game.Board[row, col] == Player.None)
                    {
                        // Try 'S' first
                        if (WouldFormSOS(game, row, col, 'S'))
                        {
                            return (row, col, 'S');
                        }
                        // Then try 'O'
                        if (WouldFormSOS(game, row, col, 'O'))
                        {
                            return (row, col, 'O');
                        }
                    }
                }
            }

            // If no winning move, make a strategic move
            var potentialMoves = new List<(int row, int col, char letter)>();

            for (int row = 0; row < game.BoardSize; row++)
            {
                for (int col = 0; col < game.BoardSize; col++)
                {
                    if (game.Board[row, col] == Player.None)
                    {
                        // Check if this position could be part of a future SOS
                        if (HasPotentialSOS(game, row, col, 'S'))
                        {
                            potentialMoves.Add((row, col, 'S'));
                        }
                        if (HasPotentialSOS(game, row, col, 'O'))
                        {
                            potentialMoves.Add((row, col, 'O'));
                        }
                    }
                }
            }

            // If there are strategic moves available, choose one randomly
            if (potentialMoves.Count > 0)
            {
                return potentialMoves[_random.Next(potentialMoves.Count)];
            }

            // If no strategic moves, make a random move
            while (true)
            {
                int row = _random.Next(game.BoardSize);
                int col = _random.Next(game.BoardSize);
                if (game.Board[row, col] == Player.None)
                {
                    return (row, col, _random.Next(2) == 0 ? 'S' : 'O');
                }
            }
        }

        private bool WouldFormSOS(SOSGameMain game, int row, int col, char letter)
        {
            // Create a temporary copy of the game state
            var tempLetters = (char[,])game.Letters.Clone();
            tempLetters[row, col] = letter;

            int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

            if (letter == 'S')
            {
                // Check if this 'S' would form the start or end of an SOS
                for (int dir = 0; dir < 8; dir++)
                {
                    // Check as start of SOS
                    int r1 = row + dx[dir];
                    int c1 = col + dy[dir];
                    int r2 = r1 + dx[dir];
                    int c2 = c1 + dy[dir];

                    if (IsInBounds(game.BoardSize, r1, c1) && IsInBounds(game.BoardSize, r2, c2))
                    {
                        if (game.Letters[r1, c1] == 'O' && game.Letters[r2, c2] == 'S')
                            return true;
                    }

                    // Check as end of SOS
                    r1 = row - dx[dir];
                    c1 = col - dy[dir];
                    r2 = r1 - dx[dir];
                    c2 = c1 - dy[dir];

                    if (IsInBounds(game.BoardSize, r1, c1) && IsInBounds(game.BoardSize, r2, c2))
                    {
                        if (game.Letters[r1, c1] == 'O' && game.Letters[r2, c2] == 'S')
                            return true;
                    }
                }
            }
            else if (letter == 'O')
            {
                // Check if this 'O' would form the middle of an SOS
                for (int dir = 0; dir < 8; dir++)
                {
                    int rPrev = row - dx[dir];
                    int cPrev = col - dy[dir];
                    int rNext = row + dx[dir];
                    int cNext = col + dy[dir];

                    if (IsInBounds(game.BoardSize, rPrev, cPrev) && IsInBounds(game.BoardSize, rNext, cNext))
                    {
                        if (game.Letters[rPrev, cPrev] == 'S' && game.Letters[rNext, cNext] == 'S')
                            return true;
                    }
                }
            }

            return false;
        }

        private bool HasPotentialSOS(SOSGameMain game, int row, int col, char letter)
        {
            int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

            if (letter == 'S')
            {
                // Check if this position could be part of a future SOS as 'S'
                for (int dir = 0; dir < 8; dir++)
                {
                    int r1 = row + dx[dir];
                    int c1 = col + dy[dir];
                    int r2 = r1 + dx[dir];
                    int c2 = c1 + dy[dir];

                    if (IsInBounds(game.BoardSize, r1, c1) && IsInBounds(game.BoardSize, r2, c2))
                    {
                        if ((game.Board[r1, c1] == Player.None || game.Letters[r1, c1] == 'O') &&
                            (game.Board[r2, c2] == Player.None || game.Letters[r2, c2] == 'S'))
                            return true;
                    }
                }
            }
            else if (letter == 'O')
            {
                // Check if this position could be part of a future SOS as 'O'
                for (int dir = 0; dir < 8; dir++)
                {
                    int rPrev = row - dx[dir];
                    int cPrev = col - dy[dir];
                    int rNext = row + dx[dir];
                    int cNext = col + dy[dir];

                    if (IsInBounds(game.BoardSize, rPrev, cPrev) && IsInBounds(game.BoardSize, rNext, cNext))
                    {
                        if ((game.Board[rPrev, cPrev] == Player.None || game.Letters[rPrev, cPrev] == 'S') &&
                            (game.Board[rNext, cNext] == Player.None || game.Letters[rNext, cNext] == 'S'))
                            return true;
                    }
                }
            }

            return false;
        }

        private bool IsInBounds(int boardSize, int row, int col)
        {
            return row >= 0 && row < boardSize && col >= 0 && col < boardSize;
        }
    }
}
