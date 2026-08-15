using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Threading;
using TicTacFusion.AI;
using TicTacFusion.Audio;
using TicTacFusion.Effects;
using TicTacFusion.Models;

namespace TicTacFusion
{
    public partial class MainWindow : Window
    {
        // Game State
        private char[,] _board = null!;
        private char _currentPlayer = 'X';
        private bool _gameEnded = false;
        private int _boardSize = 3;
        private int _winLength = 3;
        private bool _isAIGame = false;
        private int _aiDifficulty = 2; // Default: Master
        private bool _isTimedMode = false;

        // Move History for Undo
        private readonly Stack<(int row, int col, char player)> _moveHistory = new();
        private int _moveCount = 0;

        // Timed Mode Mechanics
        private const double TimeLimitPerMove = 5.0;
        private double _timeLeft = TimeLimitPerMove;
        private readonly DispatcherTimer _moveTimer = new();
        private DateTime _matchStartTime;
        private DateTime _moveStartTime;
        private double _totalTimeX = 0;
        private double _totalTimeO = 0;

        // Systems
        private readonly GameAI _ai = new();
        private ParticleSystem? _particleSystem;
        private GameStats _stats = new();

        // Theme Engine
        private int _themeIndex = 0;
        private readonly GameTheme[] _themes = {
            GameTheme.CyberNeon,
            GameTheme.ObsidianEmerald,
            GameTheme.SynthwaveSunset
        };
        private GameTheme CurrentTheme => _themes[_themeIndex];

        public MainWindow()
        {
            InitializeComponent();
            _stats = GameStats.Load();

            _moveTimer.Interval = TimeSpan.FromMilliseconds(50);
            _moveTimer.Tick += MoveTimer_Tick;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _particleSystem = new ParticleSystem(AmbientParticleCanvas, EffectsCanvas);
            ApplyTheme(CurrentTheme);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _particleSystem?.InitAmbientParticles(35);
        }

        #region Theme Engine

        private void BtnTheme_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.Instance.PlayClick();
            _themeIndex = (_themeIndex + 1) % _themes.Length;
            ApplyTheme(CurrentTheme);
        }

        private void ApplyTheme(GameTheme theme)
        {
            BgStop1.Color = theme.BackgroundStart;
            BgStop2.Color = theme.BackgroundEnd;

            TitleGrad1.Color = theme.PlayerXColor;
            TitleGrad2.Color = theme.PlayerOColor;

            LobbyCard.Background = new SolidColorBrush(theme.CardBackground);
            LobbyCard.BorderBrush = new SolidColorBrush(theme.CardBorder);

            BoardCard.Background = new SolidColorBrush(theme.CardBackground);
            BoardCard.BorderBrush = new SolidColorBrush(theme.CardBorder);

            _particleSystem?.UpdateThemeColors(theme.AccentColor);

            // Update UI if in active game
            if (GamePlayScreen.Visibility == Visibility.Visible)
            {
                UpdateTurnIndicators();
            }
        }

        #endregion

        #region Audio & Stats Header Actions

        private void BtnSound_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.Instance.IsMuted = !SoundManager.Instance.IsMuted;
            BtnSound.Content = SoundManager.Instance.IsMuted ? "🔇" : "🔊";
            if (!SoundManager.Instance.IsMuted) SoundManager.Instance.PlayClick();
        }

        private void BtnStats_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.Instance.PlayClick();
            StatTotalGames.Text = _stats.TotalGames.ToString();
            StatWinsX.Text = _stats.WinsX.ToString();
            StatWinsO.Text = _stats.WinsO.ToString();
            StatDraws.Text = _stats.Draws.ToString();
            StatBestStreak.Text = _stats.BestStreak + " 🔥";
            StatFastestWin.Text = _stats.FastestWinSeconds > 0 ? $"{_stats.FastestWinSeconds:F1}s" : "N/A";
            StatsModal.Visibility = Visibility.Visible;
        }

        private void CloseStats_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.Instance.PlayClick();
            StatsModal.Visibility = Visibility.Collapsed;
        }

        private void ResetStats_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.Instance.PlayClick();
            if (MessageBox.Show("Are you sure you want to reset all records and statistics?", "Confirm Reset", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _stats.Reset();
                BtnStats_Click(sender, e);
            }
        }

        #endregion

        #region Lobby Setup Handlers

        private void SelectPVP_Click(object sender, MouseButtonEventArgs e)
        {
            SoundManager.Instance.PlayClick();
            _isAIGame = false;
            CardPVP.BorderBrush = new SolidColorBrush(CurrentTheme.AccentColor);
            CardPVP.BorderThickness = new Thickness(2);
            CardAI.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 51, 65, 85));
            CardAI.BorderThickness = new Thickness(1.5);
            AIDifficultySection.Visibility = Visibility.Collapsed;
        }

        private void SelectAI_Click(object sender, MouseButtonEventArgs e)
        {
            SoundManager.Instance.PlayClick();
            _isAIGame = true;
            CardAI.BorderBrush = new SolidColorBrush(CurrentTheme.AccentColor);
            CardAI.BorderThickness = new Thickness(2);
            CardPVP.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 51, 65, 85));
            CardPVP.BorderThickness = new Thickness(1.5);
            AIDifficultySection.Visibility = Visibility.Visible;
        }

        private void DiffEasy_Click(object sender, RoutedEventArgs e) => SetAIDifficulty(0, BtnDiffEasy);
        private void DiffMedium_Click(object sender, RoutedEventArgs e) => SetAIDifficulty(1, BtnDiffMed);
        private void DiffHard_Click(object sender, RoutedEventArgs e) => SetAIDifficulty(2, BtnDiffHard);

        private void SetAIDifficulty(int diff, Button selectedBtn)
        {
            SoundManager.Instance.PlayClick();
            _aiDifficulty = diff;
            BtnDiffEasy.Style = (Style)FindResource("ModernButton");
            BtnDiffMed.Style = (Style)FindResource("ModernButton");
            BtnDiffHard.Style = (Style)FindResource("ModernButton");
            selectedBtn.Style = (Style)FindResource("AccentButton");
        }

        private void GridSize_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.Instance.PlayClick();
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int size))
            {
                _boardSize = size;
                _winLength = size switch
                {
                    3 => 3,
                    4 => 4,
                    5 => 4,
                    6 => 5,
                    _ => 3
                };

                BtnGrid3.Style = (Style)FindResource("ModernButton");
                BtnGrid4.Style = (Style)FindResource("ModernButton");
                BtnGrid5.Style = (Style)FindResource("ModernButton");
                BtnGrid6.Style = (Style)FindResource("ModernButton");
                btn.Style = (Style)FindResource("AccentButton");
            }
        }

        private void ModeClassic_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.Instance.PlayClick();
            _isTimedMode = false;
            BtnModeClassic.Style = (Style)FindResource("AccentButton");
            BtnModeTimed.Style = (Style)FindResource("ModernButton");
        }

        private void ModeTimed_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.Instance.PlayClick();
            _isTimedMode = true;
            BtnModeTimed.Style = (Style)FindResource("AccentButton");
            BtnModeClassic.Style = (Style)FindResource("ModernButton");
        }

        private void LaunchGame_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.Instance.PlayClick();
            StartGame();
        }

        #endregion

        #region Game Lifecycle

        private void StartGame()
        {
            _board = new char[_boardSize, _boardSize];
            for (int r = 0; r < _boardSize; r++)
                for (int c = 0; c < _boardSize; c++)
                    _board[r, c] = ' ';

            _currentPlayer = 'X';
            _gameEnded = false;
            _moveCount = 0;
            _moveHistory.Clear();
            _totalTimeX = 0;
            _totalTimeO = 0;
            _matchStartTime = DateTime.Now;

            // Update Player Names
            PlayerXLabel.Text = "PLAYER 1";
            PlayerOLabel.Text = _isAIGame ? $"AI ({GetAIDifficultyName()})" : "PLAYER 2";

            // Timed HUD
            TimerBarContainer.Visibility = _isTimedMode ? Visibility.Visible : Visibility.Collapsed;
            TimerSecondsText.Visibility = _isTimedMode ? Visibility.Visible : Visibility.Collapsed;
            PlayerXTimerLabel.Visibility = _isTimedMode ? Visibility.Visible : Visibility.Collapsed;
            PlayerOTimerLabel.Visibility = _isTimedMode ? Visibility.Visible : Visibility.Collapsed;

            // Generate Grid Tiles
            BuildGameBoardUI();

            _particleSystem?.ClearEffects();
            GameOverModal.Visibility = Visibility.Collapsed;
            LobbyScreen.Visibility = Visibility.Collapsed;
            GamePlayScreen.Visibility = Visibility.Visible;

            UpdateTurnIndicators();

            if (_isTimedMode)
            {
                StartMoveTimer();
            }
        }

        private string GetAIDifficultyName() => _aiDifficulty switch
        {
            0 => "Novice",
            1 => "Adept",
            2 => "Master",
            _ => "AI"
        };

        private void BuildGameBoardUI()
        {
            GameBoardGrid.Rows = _boardSize;
            GameBoardGrid.Columns = _boardSize;
            GameBoardGrid.Children.Clear();

            double maxBoardPixelSize = 380;
            double cellSize = Math.Floor((maxBoardPixelSize - (_boardSize * 8)) / _boardSize);

            for (int r = 0; r < _boardSize; r++)
            {
                for (int c = 0; c < _boardSize; c++)
                {
                    var cellBorder = new Border
                    {
                        Width = cellSize,
                        Height = cellSize,
                        Margin = new Thickness(4),
                        CornerRadius = new CornerRadius(12),
                        Background = new SolidColorBrush(CurrentTheme.TileBackground),
                        BorderBrush = new SolidColorBrush(CurrentTheme.TileBorder),
                        BorderThickness = new Thickness(1.5),
                        Tag = (r, c),
                        Cursor = Cursors.Hand,
                        SnapsToDevicePixels = true
                    };

                    cellBorder.MouseEnter += Cell_MouseEnter;
                    cellBorder.MouseLeave += Cell_MouseLeave;
                    cellBorder.MouseLeftButtonDown += Cell_Click;

                    GameBoardGrid.Children.Add(cellBorder);
                }
            }
        }

        private void Cell_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_gameEnded) return;
            if (sender is Border border && border.Tag is (int r, int c))
            {
                if (_board[r, c] == ' ')
                {
                    border.Background = new SolidColorBrush(CurrentTheme.TileHover);
                    border.BorderBrush = new SolidColorBrush(CurrentTheme.AccentColor);
                }
            }
        }

        private void Cell_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border border && border.Tag is (int r, int c))
            {
                if (_board[r, c] == ' ')
                {
                    border.Background = new SolidColorBrush(CurrentTheme.TileBackground);
                    border.BorderBrush = new SolidColorBrush(CurrentTheme.TileBorder);
                }
            }
        }

        #endregion

        #region Move Handling

        private async void Cell_Click(object sender, MouseButtonEventArgs e)
        {
            if (_gameEnded) return;
            if (_isAIGame && _currentPlayer == 'O') return; // Ignore user click during AI turn

            if (sender is Border border && border.Tag is (int r, int c))
            {
                if (_board[r, c] != ' ') return;

                await ProcessMove(r, c);
            }
        }

        private async Task ProcessMove(int row, int col)
        {
            if (_isTimedMode)
            {
                _moveTimer.Stop();
                double elapsed = (DateTime.Now - _moveStartTime).TotalSeconds;
                if (_currentPlayer == 'X') _totalTimeX += elapsed;
                else _totalTimeO += elapsed;
                UpdateTimerLabels();
            }

            _board[row, col] = _currentPlayer;
            _moveHistory.Push((row, col, _currentPlayer));
            _moveCount++;

            // Render Animated Mark
            int index = row * _boardSize + col;
            if (GameBoardGrid.Children[index] is Border cellBorder)
            {
                cellBorder.Child = CreateMarkVisual(_currentPlayer, cellBorder.Width);
                cellBorder.Cursor = Cursors.Arrow;
            }

            // Play SFX
            if (_currentPlayer == 'X') SoundManager.Instance.PlayPlaceX();
            else SoundManager.Instance.PlayPlaceO();

            // Check Win
            if (GameAI.CheckWin(_board, _boardSize, _winLength, row, col, _currentPlayer))
            {
                HandleGameWon(row, col, _currentPlayer);
                return;
            }

            // Check Draw
            if (_moveCount >= _boardSize * _boardSize)
            {
                HandleGameDraw();
                return;
            }

            // Switch Turn
            _currentPlayer = _currentPlayer == 'X' ? 'O' : 'X';
            UpdateTurnIndicators();

            if (_isTimedMode)
            {
                StartMoveTimer();
            }

            // Trigger AI Turn if applicable
            if (_isAIGame && _currentPlayer == 'O' && !_gameEnded)
            {
                await ExecuteAITurn();
            }
        }

        private async Task ExecuteAITurn()
        {
            TurnStatusText.Text = "AI THINKING...";
            await Task.Delay(new Random().Next(400, 800)); // Natural AI thinking delay

            if (_gameEnded) return;

            var (r, c) = _ai.GetMove(_board, _boardSize, _winLength, _aiDifficulty, 'O', 'X');
            if (r >= 0 && c >= 0)
            {
                await ProcessMove(r, c);
            }
        }

        #endregion

        #region Visual Mark Generator

        private UIElement CreateMarkVisual(char player, double cellSize)
        {
            var grid = new Grid { Width = cellSize, Height = cellSize };
            double strokeThickness = Math.Max(3.5, cellSize * 0.12);
            double pad = cellSize * 0.22;

            if (player == 'X')
            {
                var line1 = new Line
                {
                    X1 = pad, Y1 = pad,
                    X2 = cellSize - pad, Y2 = cellSize - pad,
                    Stroke = new SolidColorBrush(CurrentTheme.PlayerXColor),
                    StrokeThickness = strokeThickness,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                    Effect = new DropShadowEffect { Color = CurrentTheme.PlayerXGlow, BlurRadius = 14, ShadowDepth = 0, Opacity = 0.9 }
                };

                var line2 = new Line
                {
                    X1 = cellSize - pad, Y1 = pad,
                    X2 = pad, Y2 = cellSize - pad,
                    Stroke = new SolidColorBrush(CurrentTheme.PlayerXColor),
                    StrokeThickness = strokeThickness,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                    Effect = new DropShadowEffect { Color = CurrentTheme.PlayerXGlow, BlurRadius = 14, ShadowDepth = 0, Opacity = 0.9 }
                };

                grid.Children.Add(line1);
                grid.Children.Add(line2);
            }
            else
            {
                var ellipse = new Ellipse
                {
                    Width = cellSize - pad * 2,
                    Height = cellSize - pad * 2,
                    Stroke = new SolidColorBrush(CurrentTheme.PlayerOColor),
                    StrokeThickness = strokeThickness,
                    Effect = new DropShadowEffect { Color = CurrentTheme.PlayerOGlow, BlurRadius = 14, ShadowDepth = 0, Opacity = 0.9 }
                };
                grid.Children.Add(ellipse);
            }

            // Pop-in Scale Animation
            var scale = new ScaleTransform(0.2, 0.2, cellSize / 2, cellSize / 2);
            grid.RenderTransform = scale;

            var anim = new DoubleAnimation(0.2, 1.0, TimeSpan.FromMilliseconds(200))
            {
                EasingFunction = new BackEase { Amplitude = 0.5, EasingMode = EasingMode.EaseOut }
            };
            scale.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, anim);

            return grid;
        }

        #endregion

        #region Timed Mode Logic

        private void StartMoveTimer()
        {
            _timeLeft = TimeLimitPerMove;
            _moveStartTime = DateTime.Now;
            UpdateTimerDisplay();
            _moveTimer.Start();
        }

        private void MoveTimer_Tick(object? sender, EventArgs e)
        {
            _timeLeft -= 0.05;
            UpdateTimerDisplay();

            if (_timeLeft <= 2.0 && _timeLeft > 0 && Math.Abs(_timeLeft % 1.0) < 0.06)
            {
                SoundManager.Instance.PlayTimerTick();
            }

            if (_timeLeft <= 0)
            {
                _moveTimer.Stop();
                HandleTimeOut();
            }
        }

        private void UpdateTimerDisplay()
        {
            double pct = Math.Clamp(_timeLeft / TimeLimitPerMove, 0, 1.0);
            TimerProgressBar.Width = Math.Max(0, 100 * pct);
            TimerProgressBar.Fill = _timeLeft <= 2.0 
                ? new SolidColorBrush(Color.FromRgb(239, 68, 68)) // Warning Red
                : new SolidColorBrush(CurrentTheme.AccentColor);

            TimerSecondsText.Text = $"{Math.Max(0, _timeLeft):F1}s";
            TimerSecondsText.Foreground = TimerProgressBar.Fill;
        }

        private void UpdateTimerLabels()
        {
            PlayerXTimerLabel.Text = $"{_totalTimeX:F1}s";
            PlayerOTimerLabel.Text = $"{_totalTimeO:F1}s";
        }

        private async void HandleTimeOut()
        {
            TurnStatusText.Text = $"TIME EXPIRED: PLAYER {_currentPlayer}!";
            SoundManager.Instance.PlayLose();

            // Skip player turn
            _currentPlayer = _currentPlayer == 'X' ? 'O' : 'X';
            UpdateTurnIndicators();

            if (_isTimedMode)
            {
                StartMoveTimer();
            }

            if (_isAIGame && _currentPlayer == 'O' && !_gameEnded)
            {
                await ExecuteAITurn();
            }
        }

        #endregion

        #region Turn & Status HUD

        private void UpdateTurnIndicators()
        {
            TurnStatusText.Text = $"PLAYER {_currentPlayer}'S TURN";
            TurnStatusText.Foreground = new SolidColorBrush(_currentPlayer == 'X' ? CurrentTheme.PlayerXColor : CurrentTheme.PlayerOColor);

            if (_currentPlayer == 'X')
            {
                PlayerXCard.BorderBrush = new SolidColorBrush(CurrentTheme.PlayerXColor);
                PlayerXCard.BorderThickness = new Thickness(2.5);
                PlayerOCard.BorderBrush = new SolidColorBrush(Color.FromArgb(120, 51, 65, 85));
                PlayerOCard.BorderThickness = new Thickness(1.5);
            }
            else
            {
                PlayerOCard.BorderBrush = new SolidColorBrush(CurrentTheme.PlayerOColor);
                PlayerOCard.BorderThickness = new Thickness(2.5);
                PlayerXCard.BorderBrush = new SolidColorBrush(Color.FromArgb(120, 51, 65, 85));
                PlayerXCard.BorderThickness = new Thickness(1.5);
            }
        }

        #endregion

        #region End Game Handlers

        private void HandleGameWon(int row, int col, char winner)
        {
            _gameEnded = true;
            _moveTimer.Stop();

            double totalElapsed = (DateTime.Now - _matchStartTime).TotalSeconds;
            _stats.RecordGame(winner, totalElapsed);

            bool isPlayerWinner = (!_isAIGame || winner == 'X');

            if (isPlayerWinner)
            {
                SoundManager.Instance.PlayWin();
                VictoryIcon.Text = "👑";
                VictoryTitle.Text = _isAIGame ? "VICTORY ACHIEVED!" : $"PLAYER {winner} WINS!";
                VictoryTitle.Foreground = new SolidColorBrush(winner == 'X' ? CurrentTheme.PlayerXColor : CurrentTheme.PlayerOColor);
                VictorySubtitle.Text = $"Spectacular line completion in {_moveCount} moves!";
            }
            else
            {
                SoundManager.Instance.PlayLose();
                VictoryIcon.Text = "🤖";
                VictoryTitle.Text = "AI DOMINATES!";
                VictoryTitle.Foreground = new SolidColorBrush(CurrentTheme.PlayerOColor);
                VictorySubtitle.Text = "The machine found a tactical breakthrough.";
            }

            RecapMoves.Text = _moveCount.ToString();
            RecapTime.Text = $"{totalElapsed:F1}s";
            RecapStreak.Text = _stats.CurrentStreak + " 🔥";

            // Spawn celebratory particle explosion
            Point boardCenter = new Point(BoardCard.ActualWidth / 2, BoardCard.ActualHeight / 2);
            _particleSystem?.SpawnVictoryBurst(boardCenter, CurrentTheme.PlayerXColor, CurrentTheme.PlayerOColor, 80);

            GameOverModal.Visibility = Visibility.Visible;
        }

        private void HandleGameDraw()
        {
            _gameEnded = true;
            _moveTimer.Stop();

            double totalElapsed = (DateTime.Now - _matchStartTime).TotalSeconds;

            // In Timed Mode, tie-breaker: player with least time wins!
            if (_isTimedMode && Math.Abs(_totalTimeX - _totalTimeO) > 0.1)
            {
                char winner = _totalTimeX < _totalTimeO ? 'X' : 'O';
                _stats.RecordGame(winner, totalElapsed);
                SoundManager.Instance.PlayWin();

                VictoryIcon.Text = "⚡";
                VictoryTitle.Text = $"PLAYER {winner} WINS BY TIME!";
                VictoryTitle.Foreground = new SolidColorBrush(winner == 'X' ? CurrentTheme.PlayerXColor : CurrentTheme.PlayerOColor);
                VictorySubtitle.Text = $"Tie broken! X: {_totalTimeX:F1}s vs O: {_totalTimeO:F1}s";
            }
            else
            {
                _stats.RecordGame(' ', totalElapsed);
                SoundManager.Instance.PlayDraw();

                VictoryIcon.Text = "🤝";
                VictoryTitle.Text = "TACTICAL STALEMATE";
                VictoryTitle.Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184));
                VictorySubtitle.Text = "Full grid covered with no victor.";
            }

            RecapMoves.Text = _moveCount.ToString();
            RecapTime.Text = $"{totalElapsed:F1}s";
            RecapStreak.Text = _stats.CurrentStreak + " 🔥";

            GameOverModal.Visibility = Visibility.Visible;
        }

        #endregion

        #region In-Game Control Actions

        private void BtnUndo_Click(object sender, RoutedEventArgs e)
        {
            if (_gameEnded || _moveHistory.Count == 0) return;
            SoundManager.Instance.PlayClick();

            int undoSteps = (_isAIGame && _moveHistory.Count >= 2) ? 2 : 1;

            for (int i = 0; i < undoSteps && _moveHistory.Count > 0; i++)
            {
                var (r, c, _) = _moveHistory.Pop();
                _board[r, c] = ' ';
                _moveCount--;

                int index = r * _boardSize + c;
                if (GameBoardGrid.Children[index] is Border cellBorder)
                {
                    cellBorder.Child = null;
                    cellBorder.Cursor = Cursors.Hand;
                    cellBorder.Background = new SolidColorBrush(CurrentTheme.TileBackground);
                    cellBorder.BorderBrush = new SolidColorBrush(CurrentTheme.TileBorder);
                }
            }

            if (_isAIGame) _currentPlayer = 'X';
            else _currentPlayer = _currentPlayer == 'X' ? 'O' : 'X';

            UpdateTurnIndicators();

            if (_isTimedMode)
            {
                StartMoveTimer();
            }
        }

        private void BtnRestart_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.Instance.PlayClick();
            StartGame();
        }

        private void BtnBackToMenu_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.Instance.PlayClick();
            _moveTimer.Stop();
            _particleSystem?.ClearEffects();
            GamePlayScreen.Visibility = Visibility.Collapsed;
            GameOverModal.Visibility = Visibility.Collapsed;
            LobbyScreen.Visibility = Visibility.Visible;
        }

        private void Rematch_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.Instance.PlayClick();
            StartGame();
        }

        private void ModalBackToMenu_Click(object sender, RoutedEventArgs e)
        {
            BtnBackToMenu_Click(sender, e);
        }

        #endregion
    }
}

