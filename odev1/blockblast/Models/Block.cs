using System.Drawing;

namespace BlockBlastGame.Models
{
    public class Block : GameObject
    {
        public bool IsActive { get; set; } = false;

        public override void Draw(Graphics g)
        {
            if (IsActive)
            {
                int drawX = 40 + (Position.X * 38);
                int drawY = 80 + (Position.Y * 38);

                using (SolidBrush brush = new SolidBrush(Color))
                {
                    g.FillRectangle(brush, drawX, drawY, 36, 36);
                    // 3D Parlama efekti
                    using (Pen lightPen = new Pen(Color.FromArgb(100, Color.White), 2))
                        g.DrawLine(lightPen, drawX, drawY, drawX + 36, drawY);
                }
            }
        }
    }
}