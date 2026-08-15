using System;
using System.IO;
using System.Text.Json;

namespace TicTacFusion.Models
{
    public class GameStats
    {
        public int TotalGames { get; set; } = 0;
        public int WinsX { get; set; } = 0;
        public int WinsO { get; set; } = 0;
        public int Draws { get; set; } = 0;
        public int CurrentStreak { get; set; } = 0;
        public int BestStreak { get; set; } = 0;
        public double FastestWinSeconds { get; set; } = 0;

        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TicTacFusion",
            "stats.json"
        );

        public static GameStats Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    string json = File.ReadAllText(FilePath);
                    return JsonSerializer.Deserialize<GameStats>(json) ?? new GameStats();
                }
            }
            catch
            {
                // Fallback to fresh stats
            }
            return new GameStats();
        }

        public void Save()
        {
            try
            {
                string dir = Path.GetDirectoryName(FilePath)!;
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, json);
            }
            catch
            {
                // Ignore save errors
            }
        }

        public void RecordGame(char winner, double elapsedSeconds)
        {
            TotalGames++;
            if (winner == 'X')
            {
                WinsX++;
                CurrentStreak = (CurrentStreak >= 0) ? CurrentStreak + 1 : 1;
                if (CurrentStreak > BestStreak)
                    BestStreak = CurrentStreak;

                if (FastestWinSeconds == 0 || elapsedSeconds < FastestWinSeconds)
                    FastestWinSeconds = elapsedSeconds;
            }
            else if (winner == 'O')
            {
                WinsO++;
                CurrentStreak = 0;
            }
            else
            {
                Draws++;
                CurrentStreak = 0;
            }
            Save();
        }

        public void Reset()
        {
            TotalGames = 0;
            WinsX = 0;
            WinsO = 0;
            Draws = 0;
            CurrentStreak = 0;
            BestStreak = 0;
            FastestWinSeconds = 0;
            Save();
        }
    }
}

