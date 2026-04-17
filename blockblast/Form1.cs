using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BlockBlastGame.Models;
using BlockBlastGame.Logic;

namespace BlockBlastGame
{
    public partial class Form1 : Form
    {
        // --- AYARLAR VE DEĞİŞKENLER ---
        private GameEngine engine = new GameEngine();
        private Block[,] grid = new Block[8, 8];
        private List<Shape> availableShapes = new List<Shape>();
        private Shape? selectedShape = null;
        private Point dragOffset;
        private Point originalPos;
        private string userName;
        private string userPass;
        private int highScore;
        private bool isGameOver = false;

        private const int GridSize = 8;
        private const int TileSize = 48;
        private const int GridOffsetX = 50;
        private const int GridOffsetY = 130;

        // --- EFEKT DEĞİŞKENLERİ ---
        private List<Point> effectPoints = new List<Point>();
        private System.Windows.Forms.Timer effectTimer = new System.Windows.Forms.Timer();
        private float effectScale = 0f;
        private string comboText = "";
        private int comboAlpha = 0;
        private Point comboPos = new Point(100, 320);

        public Form1(string userName, string password)
        {
            InitializeComponent();
            this.userName = userName ?? "OYUNCU";
            this.userPass = password;
            // Rekoru dosyadan çek
            this.highScore = HighScoreManager.GetUserHighScore(this.userName, this.userPass);
            SetupForm();
        }

        private void SetupForm()
        {
            this.Text = "Block Blast - Elite Edition";
            this.Size = new Size(500, 850);
            this.BackColor = Color.FromArgb(18, 20, 32);
            this.DoubleBuffered = true;
            this.StartPosition = FormStartPosition.CenterScreen;

            ResetGame();

            // Efekt Ayarları
            effectTimer.Interval = 15;
            effectTimer.Tick += EffectTimer_Tick;
            engine.OnLinesCleared = (points) => {
                effectPoints = points;
                effectScale = 0f;
                string[] praises = { "HARİKA!", "MÜKEMMEL!", "İNANILMAZ!", "BOMBA!", "SÜPER!" };
                comboText = praises[new Random().Next(praises.Length)];
                comboAlpha = 255;
                effectTimer.Start();
            };

            SetupEvents();
        }

        private void ResetGame()
        {
            engine.Score = 0;
            isGameOver = false;
            for (int x = 0; x < GridSize; x++)
                for (int y = 0; y < GridSize; y++)
                    grid[x, y] = new Block { Position = new Point(x, y), IsActive = false };

            GenerateNewShapes();
            this.Invalidate();
        }

        private void SetupEvents()
        {
            this.MouseDown += (s, e) => {
                if (isGameOver)
                {
                    if (e.X >= 125 && e.X <= 375 && e.Y >= 480 && e.Y <= 540) ResetGame();
                    return;
                }
                foreach (var sh in availableShapes)
                {
                    if (!sh.IsPlaced && e.X >= sh.Position.X && e.X <= sh.Position.X + 150 && e.Y >= sh.Position.Y && e.Y <= sh.Position.Y + 150)
                    {
                        selectedShape = sh;
                        originalPos = sh.Position;
                        dragOffset = new Point(e.X - sh.Position.X, e.Y - sh.Position.Y);
                        break;
                    }
                }
            };

            this.MouseMove += (s, e) => {
                if (selectedShape != null)
                {
                    selectedShape.Position = new Point(e.X - dragOffset.X, e.Y - dragOffset.Y);
                    this.Invalidate();
                }
            };

            this.MouseUp += (s, e) => {
                if (selectedShape != null)
                {
                    int gx = (int)Math.Round((selectedShape.Position.X - GridOffsetX) / (double)TileSize);
                    int gy = (int)Math.Round((selectedShape.Position.Y - GridOffsetY) / (double)TileSize);
                    bool ok = true;

                    foreach (var p in selectedShape.Blocks)
                    {
                        int tx = gx + p.X, ty = gy + p.Y;
                        if (tx < 0 || tx >= GridSize || ty < 0 || ty >= GridSize || grid[tx, ty].IsActive) { ok = false; break; }
                    }

                    if (ok)
                    {
                        engine.Score += selectedShape.Blocks.Count * 10;
                        foreach (var p in selectedShape.Blocks)
                        {
                            grid[gx + p.X, gy + p.Y].IsActive = true;
                            grid[gx + p.X, gy + p.Y].Color = selectedShape.Color;
                        }
                        selectedShape.IsPlaced = true;
                        engine.CheckLines(grid);
                        if (availableShapes.TrueForAll(sh => sh.IsPlaced)) GenerateNewShapes();
                        CheckGameOver();
                    }
                    else
                    {
                        selectedShape.Position = originalPos;
                    }
                    selectedShape = null;
                    this.Invalidate();
                }
            };
        }

        private void CheckGameOver()
        {
            bool anyShapeFits = false;
            foreach (var sh in availableShapes)
            {
                if (sh.IsPlaced) continue;
                for (int x = 0; x < GridSize; x++)
                {
                    for (int y = 0; y < GridSize; y++)
                    {
                        bool fits = true;
                        foreach (var p in sh.Blocks)
                        {
                            int tx = x + p.X, ty = y + p.Y;
                            if (tx < 0 || tx >= GridSize || ty < 0 || ty >= GridSize || grid[tx, ty].IsActive) { fits = false; break; }
                        }
                        if (fits) { anyShapeFits = true; break; }
                    }
                    if (anyShapeFits) break;
                }
                if (anyShapeFits) break;
            }
            if (!anyShapeFits)
            {
                isGameOver = true;
                HighScoreManager.SaveScore(userName, userPass, engine.Score);
                highScore = HighScoreManager.GetUserHighScore(userName, userPass);
            }
        }

        private void EffectTimer_Tick(object? sender, EventArgs e)
        {
            effectScale += 0.1f;
            if (comboAlpha > 10) comboAlpha -= 15;
            if (effectScale >= 2.0f) { effectTimer.Stop(); effectPoints.Clear(); comboAlpha = 0; }
            this.Invalidate();
        }

        private void GenerateNewShapes()
        {
            Random r = new Random();
            availableShapes.Clear();
            for (int i = 0; i < 3; i++)
            {
                var s = new Shape(r.Next(0, 8), engine.GetCurrentThemeColor());
                s.Position = new Point(50 + (i * 140), 630);
                availableShapes.Add(s);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // 1. Üst Bilgiler
            g.DrawString($"SKOR", new Font("Impact", 16), Brushes.Gray, GridOffsetX, 15);
            g.DrawString($"{engine.Score}", new Font("Impact", 38), Brushes.Gold, GridOffsetX - 5, 35);
            g.DrawString($"REKOR: {highScore}", new Font("Impact", 18), Brushes.Orange, GridOffsetX + 220, 45);
            g.DrawString($"OYUNCU: {userName.ToUpper()}", new Font("Segoe UI", 9, FontStyle.Bold), Brushes.LightSteelBlue, GridOffsetX, 95);

            // 2. Izgara Arka Planı
            using (SolidBrush bgBrush = new SolidBrush(Color.FromArgb(32, 38, 58)))
            {
                for (int x = 0; x < GridSize; x++)
                    for (int y = 0; y < GridSize; y++)
                        g.FillRectangle(bgBrush, GridOffsetX + (x * TileSize), GridOffsetY + (y * TileSize), TileSize - 3, TileSize - 3);
            }

            // 3. Yerleşmiş Bloklar
            foreach (var b in grid)
            {
                if (b.IsActive)
                {
                    int dx = GridOffsetX + (b.Position.X * TileSize);
                    int dy = GridOffsetY + (b.Position.Y * TileSize);
                    using (SolidBrush sb = new SolidBrush(b.Color))
                    {
                        g.FillRectangle(sb, dx, dy, TileSize - 3, TileSize - 3);
                        g.FillRectangle(new SolidBrush(Color.FromArgb(50, Color.White)), dx, dy, TileSize - 3, 6); // Üst parlama
                    }
                }
            }

            // 4. Şekiller
            foreach (var sh in availableShapes)
            {
                if (!sh.IsPlaced)
                {
                    using (SolidBrush sb = new SolidBrush(sh.Color))
                    {
                        foreach (var p in sh.Blocks)
                        {
                            int size = (selectedShape == sh) ? TileSize - 3 : 32;
                            int step = (selectedShape == sh) ? TileSize : 34;
                            g.FillRectangle(sb, sh.Position.X + (p.X * step), sh.Position.Y + (p.Y * step), size, size);
                        }
                    }
                }
            }

            // 5. PATLAMA ANİMASYONU (Geri Gelen Kısım)
            if (effectPoints.Count > 0)
            {
                using (SolidBrush eb = new SolidBrush(Color.FromArgb((int)(200 * (1 - effectScale / 2.0f)), Color.White)))
                {
                    foreach (Point p in effectPoints)
                    {
                        int size = (int)(TileSize * effectScale);
                        g.FillRectangle(eb, GridOffsetX + (p.X * TileSize) + (TileSize / 2) - size / 2, GridOffsetY + (p.Y * TileSize) + (TileSize / 2) - size / 2, size, size);
                    }
                }
            }

            // 6. ÖVGÜ YAZISI
            if (comboAlpha > 0)
            {
                using (Font f = new Font("Impact", 52, FontStyle.Italic))
                using (SolidBrush b = new SolidBrush(Color.FromArgb(comboAlpha, Color.Gold)))
                {
                    g.DrawString(comboText, f, Brushes.Black, comboPos.X + 3, comboPos.Y + 3); // Gölge
                    g.DrawString(comboText, f, b, comboPos.X, comboPos.Y);
                }
            }

            // 7. GAME OVER EKRANI
            if (isGameOver)
            {
                using (SolidBrush overlay = new SolidBrush(Color.FromArgb(220, 15, 17, 28)))
                    g.FillRectangle(overlay, 0, 0, this.Width, this.Height);

                g.DrawString("OYUN BİTTİ", new Font("Impact", 45), new SolidBrush(Color.FromArgb(230, 80, 80)), 105, 220);
                g.FillRectangle(new SolidBrush(Color.FromArgb(40, 255, 255, 255)), 100, 310, 300, 80);
                g.DrawString($"SKOR: {engine.Score}", new Font("Impact", 28), Brushes.White, 160, 325);

                Rectangle btnRect = new Rectangle(125, 480, 250, 60);
                g.FillRectangle(Brushes.Gold, btnRect);
                g.DrawString("YENİDEN BAŞLAT", new Font("Segoe UI", 14, FontStyle.Bold), Brushes.Black, 165, 495);
            }
        }
    }
}