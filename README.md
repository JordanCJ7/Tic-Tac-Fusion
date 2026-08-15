# ⚔️ Tic Tac Fusion: Next-Gen PC Edition

[![.NET 9.0](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Platform Windows](https://img.shields.io/badge/Platform-Windows%20x64-0078D6?style=for-the-badge&logo=windows&logoColor=white)](https://microsoft.com)
[![License MIT](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)](./LICENSE)
[![Build & Release](https://img.shields.io/badge/CI%2FCD-GitHub%20Actions-2088FF?style=for-the-badge&logo=githubactions&logoColor=white)](https://github.com/JordanCJ7/Tic-Tac-Fusion/actions)

A high-end, visually stunning desktop implementation of **Tic Tac Fusion** built with **C# / .NET 9** and **Windows Presentation Foundation (WPF)**. Features high-tech glassmorphism, dynamic theme engine, vector glow animations, particle physics, synthesized sound effects, advanced AI engine, single-click Windows setup installer, and zero-dependency standalone portable distribution.

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

### Option 1: Windows Setup Installer (Recommended for Players)
1. Download **`TicTacFusion-Setup-v1.0.0.exe`** from the [GitHub Releases](https://github.com/JordanCJ7/Tic-Tac-Fusion/releases) page.
2. Run the installer setup wizard.
3. Installs to `Program Files`, adds Desktop and Start Menu shortcuts, and registers in Windows Settings with a full uninstaller.

### Option 2: Standalone Portable ZIP (Zero Install)
1. Download **`TicTacFusion-v1.0.0-Portable-win-x64.zip`** from the [GitHub Releases](https://github.com/JordanCJ7/Tic-Tac-Fusion/releases) page.
2. Extract the archive anywhere on your PC.
3. Double-click `TicTacFusion.exe` to play immediately on any 64-bit Windows PC without installing .NET.

### Option 3: Build & Run from Source (Developers)
1. Clone repository:
   ```bash
   git clone https://github.com/JordanCJ7/Tic-Tac-Fusion.git
   cd Tic-Tac-Fusion
   ```
2. Build solution:
   ```bash
   dotnet build Tic-Tac-Fusion.sln
   ```
3. Run game:
   ```bash
   dotnet run --project TicTacFusion
   ```

---

## 📦 Local Packaging & Installer Creation

To compile the self-contained single-file binary, build the portable ZIP package, and generate the Windows Setup Installer (`.exe`) on your computer, run:

```powershell
powershell -ExecutionPolicy Bypass -File .\build-installer.ps1
```

This generates:
- 📁 **Portable Archive**: `dist/TicTacFusion-v1.0.0-Portable-win-x64.zip`
- 💿 **Windows Setup Installer**: `installer-output/TicTacFusion-Setup-v1.0.0.exe`

---

## 🛠️ Project Structure

```
Tic-Tac-Fusion/
├── .github/
│   ├── ISSUE_TEMPLATE/
│   │   ├── bug_report.md         # Windows PC-specific bug report form
│   │   └── feature_request.md    # Feature suggestion template
│   ├── workflows/
│   │   └── build.yml             # Automated CI/CD, testing & release packaging
│   └── pull_request_template.md  # Contributor PR checklist
├── TicTacFusion/
│   ├── AI/
│   │   └── GameAI.cs             # Minimax & Directional Heuristic AI Engine
│   ├── Audio/
│   │   └── SoundManager.cs       # Procedural 44.1kHz audio synthesizer & player
│   ├── Effects/
│   │   └── ParticleSystem.cs     # 60 FPS Particle Canvas & victory fireworks
│   ├── Models/
│   │   ├── GameStats.cs          # Local persistent stats & records tracker
│   │   └── Theme.cs              # Multi-palette color & glow themes
│   ├── App.xaml                  # Modern vector styles & button templates
│   ├── MainWindow.xaml           # Glassmorphism UI layout & modals
│   ├── MainWindow.xaml.cs        # Main game controller & animation coordinator
│   └── TicTacFusion.csproj       # .NET 9.0 Windows WPF project
├── build-installer.ps1           # Automated 1-click build & packaging script
├── installer.iss                 # Inno Setup Windows installer recipe
├── Directory.Build.props         # Global build configuration
├── .gitignore                    # Clean .NET repository ignore rules
├── CODE_OF_CONDUCT.md           # Contributor Covenant Code of Conduct v2.1
├── CONTRIBUTING.md               # Contribution guidelines & architecture guide
├── SECURITY.md                  # Vulnerability reporting policy & SLA
├── LICENSE                       # MIT License
└── README.md                     # Documentation
```

---

## 🤝 Contributing & Community
We welcome contributions! Please check out our:
- [Contributing Guidelines](./CONTRIBUTING.md)
- [Code of Conduct](./CODE_OF_CONDUCT.md)
- [Security Policy](./SECURITY.md)

---

## 📜 License
This project is licensed under the MIT License - see the [LICENSE](./LICENSE) file for details.

© 2026 | Created with ❤️ by [Janitha Gamage](https://github.com/JordanCJ7)
