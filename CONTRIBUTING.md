# Contributing to Tic Tac Fusion

Thank you for your interest in contributing to **Tic Tac Fusion**! We welcome contributions from developers of all skill levels to help improve gameplay, visuals, AI algorithms, audio, and platform support.

---

## 📋 Table of Contents

- [Code of Conduct](#code-of-conduct)
- [How Can I Contribute?](#how-can-i-contribute)
  - [Reporting Bugs](#reporting-bugs)
  - [Suggesting Enhancements](#suggesting-enhancements)
  - [Pull Requests](#pull-requests)
- [Development Setup](#development-setup)
- [Project Architecture](#project-architecture)
- [Coding Guidelines](#coding-guidelines)
- [Git Commit Guidelines](#git-commit-guidelines)

---

## 📜 Code of Conduct

This project and everyone participating in it is governed by the [Tic Tac Fusion Code of Conduct](./CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code.

---

## 💡 How Can I Contribute?

### Reporting Bugs
If you encounter a bug, please create a new issue using our [Bug Report Template](https://github.com/JordanCJ7/Tic-Tac-Fusion/issues/new?template=bug_report.md) with:
- A clear and descriptive title.
- Exact steps to reproduce the issue.
- Expected behavior vs. actual behavior.
- Screenshots or screen recordings (if visual).
- Your environment details (Windows version, .NET runtime version).

### Suggesting Enhancements
Feature requests and gameplay enhancement suggestions are welcome! Submit an issue via the [Feature Request Template](https://github.com/JordanCJ7/Tic-Tac-Fusion/issues/new?template=feature_request.md) with:
- Clear description of the proposed feature or visual enhancement.
- Rationale on why this feature would benefit players.
- Any mockups, diagrams, or algorithmic concepts.

### Pull Requests
1. **Fork** the repository and create your branch from `main`:
   ```bash
   git checkout -b feature/amazing-feature
   ```
2. **Implement** your changes following our coding style and architecture.
3. **Verify & Build** locally:
   ```bash
   dotnet build Tic-Tac-Fusion.sln
   ```
4. **Commit** your changes with clear, concise messages using [Conventional Commits](https://www.conventionalcommits.org/).
5. **Push** to your fork and open a Pull Request against `main`.

---

## 💻 Development Setup

### Prerequisites
- **Operating System:** Windows 10 (1809+) or Windows 11 (64-bit)
- **SDK:** [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- **IDE:** Visual Studio 2022 (v17.12+ with *.NET Desktop Development* workload), VS Code (with C# Dev Kit), or JetBrains Rider

### Build & Run
```bash
# Clone the repository
git clone https://github.com/JordanCJ7/Tic-Tac-Fusion.git
cd Tic-Tac-Fusion

# Restore and build the solution
dotnet build Tic-Tac-Fusion.sln

# Launch the game
dotnet run --project TicTacFusion
```

### Packaging & Standalone Builds
To test the standalone single-file distribution and installer package:
```powershell
powershell -ExecutionPolicy Bypass -File .\build-installer.ps1
```

---

## 🏗️ Project Architecture

When adding new features, please maintain our modular folder separation:

```
TicTacFusion/
├── AI/             # Heuristic search, Minimax, and bot decision algorithms
├── Audio/          # Procedural 44.1kHz WAV synthesizer and audio players
├── Effects/        # 60 FPS Particle Canvas, screen animations, and visual shaders
├── Models/         # Data contracts, theme definitions, and stats persistence
├── App.xaml        # Reusable styles, button control templates, and brushes
├── MainWindow.xaml # Glassmorphism UI layouts, HUDs, and modal dialogs
└── MainWindow.xaml.cs # Game lifecycle coordinator and event wiring
```

---

## 📐 Coding Guidelines

- **Language Version:** C# 13 / .NET 9.
- **Null Safety:** Enable `#nullable enable` across all files.
- **WPF Cleanliness:** Avoid mixing UI rendering loops with AI computation — run CPU-intensive AI searches asynchronously (`Task.Run` or async methods) to keep the 60 FPS UI smooth.
- **Audio Integrity:** Keep sound effects procedural or embed clean lossless WAV buffers via `SoundManager`.
- **Theme Consistency:** When creating UI components, bind colors to `CurrentTheme` (e.g., `PlayerXColor`, `PlayerOColor`, `CardBackground`) rather than hardcoding static colors.

---

## 🔀 Git Commit Guidelines

Please use semantic commit messages:
- `feat:` A new feature or gameplay enhancement
- `fix:` A bug fix
- `docs:` Documentation updates
- `style:` Formatting or UI visual style tweaks without logic changes
- `refactor:` Code restructuring without feature or bug changes
- `perf:` Performance optimizations
- `chore:` Maintenance, build scripts, or dependency updates
