using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SOSGameLib
{
    public class GameMove
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public char Letter { get; set; }
        public Player Player { get; set; }

        public override string ToString()
        {
            return $"{Player},{Row},{Col},{Letter}";
        }

        public static GameMove FromString(string moveString)
        {
            var parts = moveString.Split(',');
            return new GameMove
            {
                Player = (Player)Enum.Parse(typeof(Player), parts[0]),
                Row = int.Parse(parts[1]),
                Col = int.Parse(parts[2]),
                Letter = parts[3][0]
            };
        }
    }

    public class GameRecorder
    {
        private List<GameMove> moves;
        private string gameMode;
        private int boardSize;
        private bool redIsComputer;
        private bool blueIsComputer;

        public GameRecorder(string gameMode, int boardSize, bool redIsComputer, bool blueIsComputer)
        {
            moves = new List<GameMove>();
            this.gameMode = gameMode;
            this.boardSize = boardSize;
            this.redIsComputer = redIsComputer;
            this.blueIsComputer = blueIsComputer;
        }

        public void RecordMove(Player player, int row, int col, char letter)
        {
            moves.Add(new GameMove
            {
                Player = player,
                Row = row,
                Col = col,
                Letter = letter
            });
        }

        public async Task SaveToFile(string filePath)
        {
            var lines = new List<string>
            {
                $"GameMode: {gameMode}",
                $"BoardSize: {boardSize}",
                $"RedIsComputer: {redIsComputer}",
                $"BlueIsComputer: {blueIsComputer}",
                "Moves:"
            };

            lines.AddRange(moves.ConvertAll(move => move.ToString()));
            System.IO.File.WriteAllLines(filePath, lines);
        }
    }

    public class GameReplayer
    {
        public string GameMode { get; private set; }
        public int BoardSize { get; private set; }
        public bool RedIsComputer { get; private set; }
        public bool BlueIsComputer { get; private set; }
        public List<GameMove> Moves { get; private set; }

        public static async Task<GameReplayer> LoadFromFile(string filePath)
        {
            var lines = System.IO.File.ReadAllLines(filePath);
            var replayer = new GameReplayer
            {
            GameMode = lines[0].Split(new[] { ": " }, StringSplitOptions.None)[1],
            BoardSize = int.Parse(lines[1].Split(new[] { ": " }, StringSplitOptions.None)[1]),
            RedIsComputer = bool.Parse(lines[2].Split(new[] { ": " }, StringSplitOptions.None)[1]),
            BlueIsComputer = bool.Parse(lines[3].Split(new[] { ": " }, StringSplitOptions.None)[1]),

            Moves = new List<GameMove>()
            };

            for (int i = 5; i < lines.Length; i++)
            {
                replayer.Moves.Add(GameMove.FromString(lines[i]));
            }

            return replayer;
        }
    }
}