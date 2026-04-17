using System;
using System.Windows.Forms;
using Microsoft.Win32; // Registry işlemleri için gerekli

namespace BlockBlastGame
{
    // Program.cs
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // 1. ADIM: İMZAYI KONTROL ET (Ama engelleme, sadece teknik raporda anlatacağız)
            string registryPath = @"Software\ElifKilicBlockBlast";

            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(registryPath))
            {
                // İlk açılışta tarih atar, diğer açılışlarda dokunmaz
                if (key.GetValue("FirstInstallDate") == null)
                {
                    key.SetValue("FirstInstallDate", DateTime.Now.ToString());
                }
            }

            // 2. ADIM: OYUNU HER ZAMAN BAŞLAT
            // Kullanıcı istediği kadar oynayabilir, Build yaparken de sana engel olmaz.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (LoginForm loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new Form1(loginForm.UserName, loginForm.Password));
                }
            }
        }
    }

}