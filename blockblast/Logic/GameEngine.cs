using System;
using System.Collections.Generic;
using System.Drawing;
using BlockBlastGame.Models;

namespace BlockBlastGame.Logic
{
    public class GameEngine
    {
        public int Score { get; set; } = 0;
        private Color[] themes = { Color.DeepSkyBlue, Color.MediumPurple, Color.HotPink, Color.Orange };
        private int currentTheme = 0;

        // Patlama efekti için Action (Koordinat listesi gönderir)
        // Action tanımlamasını nullable yap:
        public Action<List<Point>>? OnLinesCleared { get; set; }
        public Color GetCurrentThemeColor() => themes[currentTheme];

        public void CheckLines(Block[,] grid)
        {
            // 8x8 ızgara olduğu için sınırları 8 yapıyoruz
            int size = 8;
            List<int> r = new List<int>();
            List<int> c = new List<int>();
            List<Point> clearedPoints = new List<Point>();

            // Satır Kontrolü (8'e kadar)
            for (int y = 0; y < size; y++)
            {
                bool f = true;
                for (int x = 0; x < size; x++) if (!grid[x, y].IsActive) f = false;
                if (f)
                {
                    r.Add(y);
                    for (int x = 0; x < size; x++) clearedPoints.Add(new Point(x, y));
                }
            }

            // Sütun Kontrolü (8'e kadar)
            for (int x = 0; x < size; x++)
            {
                bool f = true;
                for (int y = 0; y < size; y++) if (!grid[x, y].IsActive) f = false;
                if (f)
                {
                    c.Add(x);
                    for (int y = 0; y < size; y++) clearedPoints.Add(new Point(x, y));
                }
            }

            // Patlatma
            foreach (Point p in clearedPoints) grid[p.X, p.Y].IsActive = false;

            if (clearedPoints.Count > 0)
            {
                Score += (r.Count + c.Count) * 100;
                currentTheme = (currentTheme + 1) % themes.Length;
                OnLinesCleared?.Invoke(clearedPoints);
            }
        }
    }
}