using System;
using System.Collections.Generic;
using System.Drawing;

namespace BlockBlastGame.Models
{
    public class Shape : GameObject
    {
        public List<Point> Blocks { get; set; } = new List<Point>();
        public bool IsPlaced { get; set; } = false;

        public Shape(int type, Color themeColor)
        {
            this.Color = themeColor;
            GenerateShape(type);
        }

        private void GenerateShape(int type)
        {
            Blocks.Clear();
            switch (type)
            {
                case 0: // 2x2 Kare
                    Blocks.Add(new Point(0, 0)); Blocks.Add(new Point(1, 0));
                    Blocks.Add(new Point(0, 1)); Blocks.Add(new Point(1, 1));
                    break;
                case 1: // 3x1 Çizgi
                    Blocks.Add(new Point(0, 0)); Blocks.Add(new Point(0, 1)); Blocks.Add(new Point(0, 2));
                    break;
                case 2: // L Şekli (Küçük)
                    Blocks.Add(new Point(0, 0)); Blocks.Add(new Point(0, 1)); Blocks.Add(new Point(1, 1));
                    break;
                case 3: // 3x3 DEV KARE (Hocayı şaşırtacak olan bu)
                    for (int i = 0; i < 3; i++) for (int j = 0; j < 3; j++) Blocks.Add(new Point(i, j));
                    break;
                case 4: // T Şekli
                    Blocks.Add(new Point(1, 0)); Blocks.Add(new Point(0, 1));
                    Blocks.Add(new Point(1, 1)); Blocks.Add(new Point(2, 1));
                    break;
                case 5: // 5'li Uzun Çizgi
                    for (int i = 0; i < 5; i++) Blocks.Add(new Point(i, 0));
                    break;
                case 6: // Büyük L
                    Blocks.Add(new Point(0, 0)); Blocks.Add(new Point(0, 1));
                    Blocks.Add(new Point(0, 2)); Blocks.Add(new Point(1, 2)); Blocks.Add(new Point(2, 2));
                    break;
                default: // Tekli Blok
                    Blocks.Add(new Point(0, 0));
                    break;
            }
        }

        public override void Draw(Graphics g)
        {
            if (!IsPlaced)
            {
                using (SolidBrush brush = new SolidBrush(Color))
                {
                    foreach (var p in Blocks)
                    {
                        g.FillRectangle(brush, Position.X + (p.X * 38), Position.Y + (p.Y * 38), 36, 36);
                    }
                }
            }
        }
    }
}