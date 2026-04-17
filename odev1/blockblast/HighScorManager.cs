using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BlockBlastGame.Logic
{
    public static class HighScoreManager
    {
        private static string filePath = "scores.txt";

        // Kullanıcının en yüksek skorunu getir
        public static int GetUserHighScore(string user, string pass)
        {
            if (!File.Exists(filePath)) return 0;

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var parts = line.Split('|'); // Format: İsim|Şifre|Skor
                if (parts.Length >= 3 && parts[0] == user && parts[1] == pass)
                {
                    return int.Parse(parts[2]);
                }
            }
            return 0; // Kayıt yoksa veya şifre yanlışsa 0 döner
        }

        // Yeni skoru kaydet (Eskisinden yüksekse günceller)
        public static void SaveScore(string user, string pass, int newScore)
        {
            List<string> lines = File.Exists(filePath) ? File.ReadAllLines(filePath).ToList() : new List<string>();
            bool found = false;

            for (int i = 0; i < lines.Count; i++)
            {
                var parts = lines[i].Split('|');
                if (parts[0] == user && parts[1] == pass)
                {
                    int currentHigh = int.Parse(parts[2]);
                    if (newScore > currentHigh)
                    {
                        lines[i] = $"{user}|{pass}|{newScore}";
                    }
                    found = true;
                    break;
                }
            }

            if (!found) lines.Add($"{user}|{pass}|{newScore}");

            File.WriteAllLines(filePath, lines);
        }
    }
}