# ⚔️ Tic-Tac-Toe: Next-Gen PC Edition

[![.NET 9.0](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Platform Windows](https://img.shields.io/badge/Platform-Windows%20x64-0078D6?style=for-the-badge&logo=windows&logoColor=white)](https://microsoft.com)
[![License MIT](https://img.shields.io/badge/License-MIT-green.style=for-the-badge)](./LICENSE)

A high-end, visually stunning desktop implementation of Tic-Tac-Toe built with **C# / .NET 9** and **Windows Presentation Foundation (WPF)**. Features high-tech glassmorphism, dynamic theme engine, vector glow animations, particle physics, synthesized sound effects, advanced AI engine, and zero-dependency standalone installation.

---

## ✨ Features & Highlights

### 🎨 Visual & Graphics Engine
- **Glassmorphism Aesthetic**: Modern translucent dark card panels with subtle glowing neon borders.
- **Dynamic Theme Switcher**:
  - 🌌 **Cyber Neon**: Electric Cyan vs Neon Magenta with dark slate panels.
  - 🌿 **Obsidian Emerald**: Vivid Emerald Green vs Golden Amber.
  - 🌆 **Synthwave Sunset**: Radiant Neon Orange vs Electric Violet.
- **Dynamic Vector Glyphs**: Smooth pop-in animations with high-intensity bloom and neon glow drop-shadows.
- **Interactive Particle System**: 60 FPS ambient background particles and celebratory victory confetti/burst explosions.

### 🎮 Gameplay Modes & Mechanics
- **Player vs Player**: Local 2-player duel with active turn indicators.
- **Player vs AI (3 Difficulty Tiers)**:
  - **Novice (Easy)**: Casual, randomized moves.
  - **Adept (Medium)**: Balanced 50/50 heuristic choice.
  - **Master (Hard)**: Unbeatable **Minimax with Alpha-Beta Pruning** (3×3) and deep directional chain heuristic analysis (4×4, 5×5, 6×6).
- **Multiple Board Dimensions**:
  - **3×3 Classic** (3 in a row)
  - **4×4 Extended** (4 in a row)
  - **5×5 Advanced** (4 in a row)
  - **6×6 Master** (5 in a row)
- **Speed Blitz Mode**: 5-second countdown timer with audio ticks, turn skipping on timeout, and tie-breaker statistics.
- **Undo Move Support**: Step back moves at any time during local or AI matches.
- **Player Records & Statistics**: Persistent match stats tracking win rates, streaks, and fastest win records.

### 🔊 Procedural Audio Synthesizer
- In-memory synthesized WAV sound effects (no missing audio assets or external codecs required):
  - UI clicks, laser swoosh placements, countdown warning ticks, and victory/defeat fanfares.
  - Instant one-click mute/unmute audio toggle.

---

## 🚀 Installation & Running

### Option 1: Standalone Portable Package (No .NET Install Needed)
1. Download the latest `TicTacToe-v2.0-Standalone-win-x64.zip` from the [Releases](https://github.com/JordanCJ7/Tic-Tac-Toe-Game-PC/releases) page.
2. Extract the archive.
3. Run `TicTacToe.exe` directly on any 64-bit Windows PC.

### Option 2: Setup Installer (Inno Setup)
- Compile `installer.iss` with [Inno Setup](https://jrsoftware.org/isinfo.php) or run `.\build-installer.ps1` to produce `TicTacToe-Setup-v2.0.0.exe`.

### Option 3: Build & Run from Source
1. Clone repository:
   ```bash
   git clone https://github.com/JordanCJ7/Tic-Tac-Toe-Game-PC.git
   cd Tic-Tac-Toe-Game-PC
   ```
2. Build solution:
   ```bash
   dotnet build
   ```
3. Run game:
   ```bash
   dotnet run --project TicTacToe
   ```

---

## 🛠️ Project Structure

```
Tic-Tac-Toe-Game-PC/
├── TicTacToe/
│   ├── AI/
│   │   └── GameAI.cs             # Minimax & Heuristic AI Engine
│   ├── Audio/
│   │   └── SoundManager.cs       # Procedural 44.1kHz audio generator & player
│   ├── Effects/
│   │   └── ParticleSystem.cs     # 60 FPS Particle Canvas & visual explosions
│   ├── Models/
│   │   ├── GameStats.cs          # Local persistent stats & records tracker
│   │   └── Theme.cs              # Multi-palette color & glow themes
│   ├── App.xaml                  # Modern vector styles & button templates
│   ├── MainWindow.xaml           # Glassmorphism UI layout & modals
│   ├── MainWindow.xaml.cs        # Main game controller & animation coordinator
│   └── TicTacToe.csproj          # .NET 9.0 Windows WPF project
├── build-installer.ps1           # Automated 1-click build & packaging script
├── installer.iss                 # Inno Setup Windows installer recipe
├── Directory.Build.props         # Global build configuration
├── .gitignore                    # Clean .NET repository ignore rules
├── LICENSE                       # MIT License
└── README.md                     # Documentation
```

---

## 📜 License
This project is licensed under the MIT License - see the [LICENSE](./LICENSE) file for details.

© 2026 | Created with ❤️ by [Janitha Gamage](https://github.com/JordanCJ7)
