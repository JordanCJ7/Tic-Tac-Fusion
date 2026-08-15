using System.Windows.Media;

namespace TicTacFusion.Models
{
    public class GameTheme
    {
        public string Name { get; set; } = "Cyber Neon";
        
        // Colors
        public Color BackgroundStart { get; set; }
        public Color BackgroundEnd { get; set; }
        public Color CardBackground { get; set; }
        public Color CardBorder { get; set; }
        
        public Color PlayerXColor { get; set; }
        public Color PlayerXGlow { get; set; }
        public Color PlayerOColor { get; set; }
        public Color PlayerOGlow { get; set; }

        public Color TileBackground { get; set; }
        public Color TileHover { get; set; }
        public Color TileBorder { get; set; }

        public Color AccentColor { get; set; }

        public static GameTheme CyberNeon => new()
        {
            Name = "Cyber Neon",
            BackgroundStart = Color.FromRgb(10, 15, 30),
            BackgroundEnd = Color.FromRgb(20, 10, 40),
            CardBackground = Color.FromArgb(180, 15, 23, 42),
            CardBorder = Color.FromArgb(120, 56, 189, 248),
            PlayerXColor = Color.FromRgb(0, 240, 255), // Electric Cyan
            PlayerXGlow = Color.FromArgb(200, 0, 240, 255),
            PlayerOColor = Color.FromRgb(255, 0, 127), // Neon Magenta
            PlayerOGlow = Color.FromArgb(200, 255, 0, 127),
            TileBackground = Color.FromArgb(140, 30, 41, 59),
            TileHover = Color.FromArgb(200, 51, 65, 85),
            TileBorder = Color.FromArgb(70, 148, 163, 184),
            AccentColor = Color.FromRgb(0, 240, 255)
        };

        public static GameTheme ObsidianEmerald => new()
        {
            Name = "Obsidian Emerald",
            BackgroundStart = Color.FromRgb(13, 17, 23),
            BackgroundEnd = Color.FromRgb(16, 30, 26),
            CardBackground = Color.FromArgb(190, 18, 26, 23),
            CardBorder = Color.FromArgb(120, 46, 204, 113),
            PlayerXColor = Color.FromRgb(0, 230, 118), // Emerald Green
            PlayerXGlow = Color.FromArgb(200, 0, 230, 118),
            PlayerOColor = Color.FromRgb(255, 214, 0), // Golden Amber
            PlayerOGlow = Color.FromArgb(200, 255, 214, 0),
            TileBackground = Color.FromArgb(140, 24, 38, 30),
            TileHover = Color.FromArgb(200, 36, 56, 45),
            TileBorder = Color.FromArgb(70, 74, 222, 128),
            AccentColor = Color.FromRgb(0, 230, 118)
        };

        public static GameTheme SynthwaveSunset => new()
        {
            Name = "Synthwave Sunset",
            BackgroundStart = Color.FromRgb(22, 11, 40),
            BackgroundEnd = Color.FromRgb(50, 12, 56),
            CardBackground = Color.FromArgb(190, 35, 18, 55),
            CardBorder = Color.FromArgb(120, 255, 107, 0),
            PlayerXColor = Color.FromRgb(255, 107, 0), // Radiant Orange
            PlayerXGlow = Color.FromArgb(200, 255, 107, 0),
            PlayerOColor = Color.FromRgb(189, 0, 255), // Neon Violet
            PlayerOGlow = Color.FromArgb(200, 189, 0, 255),
            TileBackground = Color.FromArgb(140, 48, 20, 70),
            TileHover = Color.FromArgb(200, 70, 28, 100),
            TileBorder = Color.FromArgb(70, 232, 121, 249),
            AccentColor = Color.FromRgb(255, 107, 0)
        };
    }
}

