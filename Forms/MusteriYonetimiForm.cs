using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AracKiralamaSistemi.Modeller;
using AracKiralamaSistemi.Servisler;

namespace AracKiralamaSistemi
{
    public class MusteriYonetimiForm : Form
    {
        private readonly KiralamaYoneticisi yonetici;
        private readonly DataGridView dgv;
        private readonly TextBox txtAd, txtSoyad, txtTel, txtEhliyet;
        private int secilenMusteriId = 0;

        public MusteriYonetimiForm(KiralamaYoneticisi yonetici)
        {
            this.yonetici = yonetici;
            Text = "Müşteri Yönetimi (Ekle / Güncelle / Sil)";
            Width = 980;
            Height = 620;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(242, 247, 252);

            var formPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 160,
                BackColor = Color.White,
                Padding = new Padding(12)
            };
            Controls.Add(formPanel);

            formPanel.Controls.Add(new Label { Text = "Müşteri Bilgileri", Location = new Point(12, 8), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) });

            formPanel.Controls.Add(new Label { Text = "Ad:", Location = new Point(12, 42), AutoSize = true });
            txtAd = new TextBox { Location = new Point(55, 38), Width = 140 };
            formPanel.Controls.Add(txtAd);

            formPanel.Controls.Add(new Label { Text = "Soyad:", Location = new Point(220, 42), AutoSize = true });
            txtSoyad = new TextBox { Location = new Point(280, 38), Width = 140 };
            formPanel.Controls.Add(txtSoyad);

            formPanel.Controls.Add(new Label { Text = "Telefon:", Location = new Point(450, 42), AutoSize = true });
            txtTel = new TextBox { Location = new Point(520, 38), Width = 140 };
            formPanel.Controls.Add(txtTel);

            formPanel.Controls.Add(new Label { Text = "Ehliyet No:", Location = new Point(12, 82), AutoSize = true });
            txtEhliyet = new TextBox { Location = new Point(100, 78), Width = 160 };
            formPanel.Controls.Add(txtEhliyet);

            var btnEkle = new Button { Text = "Yeni Ekle", Location = new Point(12, 115), Width = 110, Height = 35, BackColor = Color.LightGreen };
            btnEkle.Click += (s, e) => ArayuzYardimcisi.GuvenliCalistir(() =>
            {
                int yeniId = yonetici.SonrakiMusteriId();
                yonetici.MusteriEkle(new Musteri(yeniId, txtAd.Text, txtSoyad.Text, txtTel.Text, txtEhliyet.Text));
                Listele();
                Temizle();
                ArayuzYardimcisi.BilgiGoster("Müşteri eklendi!");
            });
            formPanel.Controls.Add(btnEkle);

            var btnGuncelle = new Button { Text = "Güncelle", Location = new Point(135, 115), Width = 110, Height = 35, BackColor = Color.LightYellow };
            btnGuncelle.Click += (s, e) => ArayuzYardimcisi.GuvenliCalistir(() =>
            {
                if (secilenMusteriId == 0) throw new ArgumentException("Lütfen tablodan müşteri seçin!");
                yonetici.MusteriGuncelle(secilenMusteriId, txtAd.Text, txtSoyad.Text, txtTel.Text, txtEhliyet.Text);
                Listele();
                Temizle();
                ArayuzYardimcisi.BilgiGoster("Müşteri güncellendi!");
            });
            formPanel.Controls.Add(btnGuncelle);

            var btnSil = new Button { Text = "Seçileni Sil", Location = new Point(258, 115), Width = 110, Height = 35, BackColor = Color.MistyRose };
            btnSil.Click += (s, e) => ArayuzYardimcisi.GuvenliCalistir(() =>
            {
                if (secilenMusteriId == 0) throw new ArgumentException("Lütfen silinecek müşteri seçin!");
                if (!ArayuzYardimcisi.OnayIste("Seçili müşteri silinecek. Devam edilsin mi?", "Silme Onayı")) return;

                yonetici.MusteriSil(secilenMusteriId);
                Listele();
                Temizle();
                ArayuzYardimcisi.BilgiGoster("Müşteri silindi!");
            });
            formPanel.Controls.Add(btnSil);

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(245, 247, 250) }
            };
            dgv.CellClick += Dgv_CellClick;
            Controls.Add(dgv);
            dgv.BringToFront();
            Listele();
        }

        private void Listele()
        {
            dgv.DataSource = null;
            dgv.DataSource = yonetici.Musteriler
                .Select(m => new { m.Id, m.Ad, m.Soyad, m.Telefon, m.EhliyetNumarasi, m.AdSoyad })
                .ToList();
        }

        private void Dgv_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgv.Rows[e.RowIndex];
            secilenMusteriId = Convert.ToInt32(row.Cells["Id"].Value);
            txtAd.Text = row.Cells["Ad"].Value?.ToString() ?? "";
            txtSoyad.Text = row.Cells["Soyad"].Value?.ToString() ?? "";
            txtTel.Text = row.Cells["Telefon"].Value?.ToString() ?? "";
            txtEhliyet.Text = row.Cells["EhliyetNumarasi"].Value?.ToString() ?? "";
        }

        private void Temizle()
        {
            secilenMusteriId = 0;
            txtAd.Text = txtSoyad.Text = txtTel.Text = txtEhliyet.Text = "";
        }
    }
}
