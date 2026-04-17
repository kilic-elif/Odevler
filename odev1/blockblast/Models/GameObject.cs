using System.Drawing;

namespace BlockBlastGame.Models
{
    // OOP: Abstraction (Soyutlama) - Temel sınıf
    public abstract class GameObject
    {
        public Point Position { get; set; }
        public Color Color { get; set; }
        public abstract void Draw(Graphics g); // Polymorphism için soyut metot
    }
}