using System;
using System.Drawing;
using System.Windows.Forms;

namespace BlockBlastGame
{
    public partial class LoginForm : Form
    {
        // OOP: Encapsulation - Dışarıdan erişilecek bilgiler
        public string UserName { get; private set; } = "";
        public string Password { get; private set; } = "";

        private TextBox txtName;
        private TextBox txtPass;

        public LoginForm()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            // Form Ayarları
            this.Text = "Block Blast - Giriş";
            this.Size = new Size(360, 320);
            this.BackColor = Color.FromArgb(30, 35, 60); // Oyunla uyumlu koyu ton
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Başlık
            Label lblTitle = new Label()
            {
                Text = "BLOCK BLAST",
                Font = new Font("Impact", 24, FontStyle.Bold),
                ForeColor = Color.Gold,
                Location = new Point(80, 20),
                AutoSize = true
            };

            // Kullanıcı Adı Alanı
            Label lblName = new Label() { Text = "Kullanıcı Adı:", Location = new Point(50, 80), ForeColor = Color.White, AutoSize = true };
            txtName = new TextBox() { Location = new Point(50, 105), Size = new Size(240, 30), Font = new Font("Arial", 12) };

            // Şifre Alanı
            Label lblPass = new Label() { Text = "Şifre:", Location = new Point(50, 145), ForeColor = Color.White, AutoSize = true };
            txtPass = new TextBox() { Location = new Point(50, 170), Size = new Size(240, 30), PasswordChar = '*', Font = new Font("Arial", 12) };

            // Başlat Butonu
            Button btnStart = new Button()
            {
                Text = "OYUNA BAŞLA",
                Location = new Point(100, 220),
                Size = new Size(140, 45),
                BackColor = Color.Gold,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnStart.Click += (s, e) => {
                if (!string.IsNullOrWhiteSpace(txtName.Text) && !string.IsNullOrWhiteSpace(txtPass.Text))
                {
                    UserName = txtName.Text.Trim();
                    Password = txtPass.Text.Trim();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Lütfen tüm alanları doldur kanka!", "Eksik Bilgi");
                }
            };

            // Kontrolleri Ekle
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblPass);
            this.Controls.Add(txtPass);
            this.Controls.Add(btnStart);

            this.AcceptButton = btnStart;
        }
    }
}