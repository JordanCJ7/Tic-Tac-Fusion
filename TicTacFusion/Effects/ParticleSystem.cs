using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TicTacFusion.Effects
{
    public class ParticleSystem
    {
        private class AmbientParticle
        {
            public double X, Y, Vx, Vy, Radius, Alpha, AlphaSpeed;
            public Ellipse Shape = null!;
        }

        private class BurstParticle
        {
            public double X, Y, Vx, Vy, Life, MaxLife, Size, Rotation, RotSpeed;
            public Shape Shape = null!;
            public SolidColorBrush Brush = null!;
        }

        private readonly Canvas _ambientCanvas;
        private readonly Canvas _effectsCanvas;
        private readonly Random _random = new();
        private readonly List<AmbientParticle> _ambientParticles = new();
        private readonly List<BurstParticle> _burstParticles = new();
        private readonly DispatcherTimer _timer = new();

        public ParticleSystem(Canvas ambientCanvas, Canvas effectsCanvas)
        {
            _ambientCanvas = ambientCanvas;
            _effectsCanvas = effectsCanvas;

            _timer.Interval = TimeSpan.FromMilliseconds(16); // ~60 FPS
            _timer.Tick += Update;
            _timer.Start();

            InitAmbientParticles(35);
        }

        public void InitAmbientParticles(int count)
        {
            _ambientCanvas.Children.Clear();
            _ambientParticles.Clear();

            double w = Math.Max(_ambientCanvas.ActualWidth, 800);
            double h = Math.Max(_ambientCanvas.ActualHeight, 600);

            for (int i = 0; i < count; i++)
            {
                var p = new AmbientParticle
                {
                    X = _random.NextDouble() * w,
                    Y = _random.NextDouble() * h,
                    Vx = (_random.NextDouble() - 0.5) * 0.4,
                    Vy = -(_random.NextDouble() * 0.4 + 0.1),
                    Radius = _random.NextDouble() * 3 + 1,
                    Alpha = _random.NextDouble() * 0.5 + 0.2,
                    AlphaSpeed = (_random.NextDouble() - 0.5) * 0.015,
                    Shape = new Ellipse
                    {
                        Width = 4,
                        Height = 4,
                        Fill = new SolidColorBrush(Color.FromArgb(100, 200, 230, 255)),
                        IsHitTestVisible = false
                    }
                };

                p.Shape.Width = p.Radius * 2;
                p.Shape.Height = p.Radius * 2;
                Canvas.SetLeft(p.Shape, p.X);
                Canvas.SetTop(p.Shape, p.Y);
                _ambientCanvas.Children.Add(p.Shape);
                _ambientParticles.Add(p);
            }
        }

        public void UpdateThemeColors(Color particleColor)
        {
            foreach (var p in _ambientParticles)
            {
                p.Shape.Fill = new SolidColorBrush(Color.FromArgb((byte)(p.Alpha * 255), particleColor.R, particleColor.G, particleColor.B));
            }
        }

        public void SpawnVictoryBurst(Point center, Color color1, Color color2, int count = 70)
        {
            for (int i = 0; i < count; i++)
            {
                double angle = _random.NextDouble() * Math.PI * 2;
                double speed = _random.NextDouble() * 9 + 3;
                bool isSquare = _random.Next(2) == 0;
                double size = _random.NextDouble() * 8 + 4;
                Color col = (_random.Next(2) == 0) ? color1 : color2;

                var brush = new SolidColorBrush(col);
                Shape shape = isSquare 
                    ? new Rectangle { Width = size, Height = size, Fill = brush }
                    : new Ellipse { Width = size, Height = size, Fill = brush };

                shape.IsHitTestVisible = false;

                var bp = new BurstParticle
                {
                    X = center.X,
                    Y = center.Y,
                    Vx = Math.Cos(angle) * speed,
                    Vy = Math.Sin(angle) * speed - 2.0, // Initial upward burst
                    Life = 1.0,
                    MaxLife = _random.NextDouble() * 0.6 + 0.7,
                    Size = size,
                    Rotation = _random.NextDouble() * 360,
                    RotSpeed = (_random.NextDouble() - 0.5) * 15,
                    Shape = shape,
                    Brush = brush
                };

                Canvas.SetLeft(shape, bp.X);
                Canvas.SetTop(shape, bp.Y);
                _effectsCanvas.Children.Add(shape);
                _burstParticles.Add(bp);
            }
        }

        private void Update(object? sender, EventArgs e)
        {
            double w = _ambientCanvas.ActualWidth > 0 ? _ambientCanvas.ActualWidth : 800;
            double h = _ambientCanvas.ActualHeight > 0 ? _ambientCanvas.ActualHeight : 600;

            // Ambient particles
            foreach (var p in _ambientParticles)
            {
                p.X += p.Vx;
                p.Y += p.Vy;
                p.Alpha += p.AlphaSpeed;

                if (p.Alpha is <= 0.1 or >= 0.7)
                    p.AlphaSpeed = -p.AlphaSpeed;

                if (p.Y < -10) { p.Y = h + 10; p.X = _random.NextDouble() * w; }
                if (p.X < -10) p.X = w + 10;
                if (p.X > w + 10) p.X = -10;

                Canvas.SetLeft(p.Shape, p.X);
                Canvas.SetTop(p.Shape, p.Y);
                p.Shape.Opacity = Math.Clamp(p.Alpha, 0.05, 0.8);
            }

            // Burst particles
            for (int i = _burstParticles.Count - 1; i >= 0; i--)
            {
                var bp = _burstParticles[i];
                bp.Life -= 0.02;
                bp.X += bp.Vx;
                bp.Y += bp.Vy;
                bp.Vy += 0.25; // Gravity
                bp.Vx *= 0.96; // Air resistance
                bp.Rotation += bp.RotSpeed;

                if (bp.Life <= 0)
                {
                    _effectsCanvas.Children.Remove(bp.Shape);
                    _burstParticles.RemoveAt(i);
                }
                else
                {
                    Canvas.SetLeft(bp.Shape, bp.X - bp.Size / 2);
                    Canvas.SetTop(bp.Shape, bp.Y - bp.Size / 2);
                    bp.Shape.Opacity = bp.Life;
                    bp.Shape.RenderTransform = new RotateTransform(bp.Rotation, bp.Size / 2, bp.Size / 2);
                }
            }
        }

        public void ClearEffects()
        {
            _effectsCanvas.Children.Clear();
            _burstParticles.Clear();
        }
    }
}

