using System;
using System.Collections.Generic;

namespace TicTacFusion.AI
{
    public class GameAI
    {
        private readonly Random _random = new();

        public (int row, int col) GetMove(char[,] board, int boardSize, int winLength, int difficulty, char aiPlayer, char humanPlayer)
        {
            return difficulty switch
            {
                0 => GetRandomMove(board, boardSize),
                1 => _random.Next(2) == 0 ? GetSmartMove(board, boardSize, winLength, aiPlayer, humanPlayer) : GetRandomMove(board, boardSize),
                2 => GetSmartMove(board, boardSize, winLength, aiPlayer, humanPlayer),
                _ => GetRandomMove(board, boardSize)
            };
        }

        private (int row, int col) GetRandomMove(char[,] board, int boardSize)
        {
            var openSpots = new List<(int, int)>();
            for (int r = 0; r < boardSize; r++)
                for (int c = 0; c < boardSize; c++)
                    if (board[r, c] == ' ')
                        openSpots.Add((r, c));

            if (openSpots.Count == 0) return (-1, -1);
            return openSpots[_random.Next(openSpots.Count)];
        }

        private (int row, int col) GetSmartMove(char[,] board, int boardSize, int winLength, char aiPlayer, char humanPlayer)
        {
            // 1. Immediate Win Check
            for (int r = 0; r < boardSize; r++)
            {
                for (int c = 0; c < boardSize; c++)
                {
                    if (board[r, c] == ' ')
                    {
                        board[r, c] = aiPlayer;
                        if (CheckWin(board, boardSize, winLength, r, c, aiPlayer))
                        {
                            board[r, c] = ' ';
                            return (r, c);
                        }
                        board[r, c] = ' ';
                    }
                }
            }

            // 2. Immediate Block Check
            for (int r = 0; r < boardSize; r++)
            {
                for (int c = 0; c < boardSize; c++)
                {
                    if (board[r, c] == ' ')
                    {
                        board[r, c] = humanPlayer;
                        if (CheckWin(board, boardSize, winLength, r, c, humanPlayer))
                        {
                            board[r, c] = ' ';
                            return (r, c);
                        }
                        board[r, c] = ' ';
                    }
                }
            }

            // 3. If 3x3, use full Minimax
            if (boardSize == 3)
            {
                return GetMinimaxMove(board, boardSize, winLength, aiPlayer, humanPlayer);
            }

            // 4. For larger boards (4x4, 5x5, 6x6), use Depth-Limited Heuristic Evaluation
            return GetHeuristicMove(board, boardSize, winLength, aiPlayer, humanPlayer);
        }

        #region 3x3 Minimax

        private (int row, int col) GetMinimaxMove(char[,] board, int boardSize, int winLength, char aiPlayer, char humanPlayer)
        {
            int bestScore = int.MinValue;
            (int row, int col) bestMove = (-1, -1);

            for (int r = 0; r < boardSize; r++)
            {
                for (int c = 0; c < boardSize; c++)
                {
                    if (board[r, c] == ' ')
                    {
                        board[r, c] = aiPlayer;
                        int score = Minimax(board, 0, false, int.MinValue, int.MaxValue, aiPlayer, humanPlayer);
                        board[r, c] = ' ';

                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestMove = (r, c);
                        }
                    }
                }
            }

            return bestMove.row != -1 ? bestMove : GetRandomMove(board, boardSize);
        }

        private int Minimax(char[,] board, int depth, bool isMaximizing, int alpha, int beta, char aiPlayer, char humanPlayer)
        {
            if (HasPlayerWon(board, 3, 3, aiPlayer)) return 10 - depth;
            if (HasPlayerWon(board, 3, 3, humanPlayer)) return depth - 10;
            if (IsFull(board, 3)) return 0;
            if (depth >= 6) return 0; // Guard

            if (isMaximizing)
            {
                int maxEval = int.MinValue;
                for (int r = 0; r < 3; r++)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        if (board[r, c] == ' ')
                        {
                            board[r, c] = aiPlayer;
                            int eval = Minimax(board, depth + 1, false, alpha, beta, aiPlayer, humanPlayer);
                            board[r, c] = ' ';
                            maxEval = Math.Max(maxEval, eval);
                            alpha = Math.Max(alpha, eval);
                            if (beta <= alpha) break;
                        }
                    }
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                for (int r = 0; r < 3; r++)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        if (board[r, c] == ' ')
                        {
                            board[r, c] = humanPlayer;
                            int eval = Minimax(board, depth + 1, true, alpha, beta, aiPlayer, humanPlayer);
                            board[r, c] = ' ';
                            minEval = Math.Min(minEval, eval);
                            beta = Math.Min(beta, eval);
                            if (beta <= alpha) break;
                        }
                    }
                }
                return minEval;
            }
        }

        #endregion

        #region Heuristic Search for Larger Boards

        private (int row, int col) GetHeuristicMove(char[,] board, int boardSize, int winLength, char aiPlayer, char humanPlayer)
        {
            int bestScore = int.MinValue;
            var bestMoves = new List<(int, int)>();

            for (int r = 0; r < boardSize; r++)
            {
                for (int c = 0; c < boardSize; c++)
                {
                    if (board[r, c] == ' ')
                    {
                        int score = EvaluatePosition(board, boardSize, winLength, r, c, aiPlayer, humanPlayer);
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestMoves.Clear();
                            bestMoves.Add((r, c));
                        }
                        else if (score == bestScore)
                        {
                            bestMoves.Add((r, c));
                        }
                    }
                }
            }

            if (bestMoves.Count > 0)
                return bestMoves[_random.Next(bestMoves.Count)];

            return GetRandomMove(board, boardSize);
        }

        private int EvaluatePosition(char[,] board, int boardSize, int winLength, int r, int c, char aiPlayer, char humanPlayer)
        {
            int score = 0;

            // Center proximity bonus
            double center = (boardSize - 1) / 2.0;
            double distToCenter = Math.Abs(r - center) + Math.Abs(c - center);
            score += (int)((boardSize * 2 - distToCenter) * 3);

            // Simulate move
            board[r, c] = aiPlayer;
            score += ScoreDirectionalChains(board, boardSize, winLength, r, c, aiPlayer) * 5;
            board[r, c] = ' ';

            // Simulate opponent block utility
            board[r, c] = humanPlayer;
            score += ScoreDirectionalChains(board, boardSize, winLength, r, c, humanPlayer) * 4;
            board[r, c] = ' ';

            return score;
        }

        private int ScoreDirectionalChains(char[,] board, int boardSize, int winLength, int r, int c, char player)
        {
            int score = 0;
            int[] dr = { 0, 1, 1, 1 };
            int[] dc = { 1, 0, 1, -1 };

            for (int d = 0; d < 4; d++)
            {
                int count = 1;
                int openEnds = 0;

                // Positive direction
                int step = 1;
                while (true)
                {
                    int nr = r + dr[d] * step;
                    int nc = c + dc[d] * step;
                    if (nr < 0 || nr >= boardSize || nc < 0 || nc >= boardSize) break;
                    if (board[nr, nc] == player) count++;
                    else if (board[nr, nc] == ' ') { openEnds++; break; }
                    else break;
                    step++;
                }

                // Negative direction
                step = 1;
                while (true)
                {
                    int nr = r - dr[d] * step;
                    int nc = c - dc[d] * step;
                    if (nr < 0 || nr >= boardSize || nc < 0 || nc >= boardSize) break;
                    if (board[nr, nc] == player) count++;
                    else if (board[nr, nc] == ' ') { openEnds++; break; }
                    else break;
                    step++;
                }

                if (count >= winLength) score += 1000;
                else if (count == winLength - 1 && openEnds > 0) score += 150 * openEnds;
                else if (count == winLength - 2 && openEnds > 1) score += 40;
                else score += count * 5;
            }

            return score;
        }

        #endregion

        #region Helpers

        public static bool CheckWin(char[,] board, int boardSize, int winLength, int r, int c, char player)
        {
            int[] dr = { 0, 1, 1, 1 };
            int[] dc = { 1, 0, 1, -1 };

            for (int d = 0; d < 4; d++)
            {
                int count = 1;
                int step = 1;
                while (true)
                {
                    int nr = r + dr[d] * step;
                    int nc = c + dc[d] * step;
                    if (nr < 0 || nr >= boardSize || nc < 0 || nc >= boardSize || board[nr, nc] != player) break;
                    count++;
                    step++;
                }

                step = 1;
                while (true)
                {
                    int nr = r - dr[d] * step;
                    int nc = c - dc[d] * step;
                    if (nr < 0 || nr >= boardSize || nc < 0 || nc >= boardSize || board[nr, nc] != player) break;
                    count++;
                    step++;
                }

                if (count >= winLength) return true;
            }
            return false;
        }

        private static bool HasPlayerWon(char[,] board, int boardSize, int winLength, char player)
        {
            for (int r = 0; r < boardSize; r++)
                for (int c = 0; c < boardSize; c++)
                    if (board[r, c] == player && CheckWin(board, boardSize, winLength, r, c, player))
                        return true;
            return false;
        }

        private static bool IsFull(char[,] board, int size)
        {
            for (int r = 0; r < size; r++)
                for (int c = 0; c < size; c++)
                    if (board[r, c] == ' ') return false;
            return true;
        }

        #endregion
    }
}

