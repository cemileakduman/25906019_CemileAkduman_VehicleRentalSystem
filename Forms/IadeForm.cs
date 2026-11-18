using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AracKiralamaSistemi.Modeller;
using AracKiralamaSistemi.Servisler;

namespace AracKiralamaSistemi
{
    public class IadeForm : Form
    {
        private readonly DataGridView dgv;
        private readonly KiralamaYoneticisi yonetici;
        private readonly NumericUpDown nudEkGun;
        private readonly DateTimePicker dtYeniBitis;
        private readonly Label lblSecimBilgi;

        public IadeForm(KiralamaYoneticisi yonetici)
        {
            this.yonetici = yonetici;
            Text = "Araç İade ve Uzatma";
            Width = 980;
            Height = 620;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(242, 247, 252);

            var islemPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 150,
                BackColor = Color.White,
                Padding = new Padding(12)
            };
            Controls.Add(islemPanel);

            islemPanel.Controls.Add(new Label
            {
                Text = "Seçili kiralama işlemleri",
                Location = new Point(12, 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            });

            lblSecimBilgi = new Label
            {
                Text = "Listeden bir aktif kiralama seçin.",
                Location = new Point(12, 38),
                AutoSize = true,
                ForeColor = Color.FromArgb(80, 80, 80)
            };
            islemPanel.Controls.Add(lblSecimBilgi);

            islemPanel.Controls.Add(new Label { Text = "Ek gün:", Location = new Point(12, 72), AutoSize = true });
            nudEkGun = new NumericUpDown
            {
                Location = new Point(75, 68),
                Width = 70,
                Minimum = 1,
                Maximum = 365,
                Value = 1
            };
            islemPanel.Controls.Add(nudEkGun);

            islemPanel.Controls.Add(new Label { Text = "veya yeni bitiş:", Location = new Point(160, 72), AutoSize = true });
            dtYeniBitis = new DateTimePicker
            {
                Location = new Point(275, 68),
                Width = 160,
                Format = DateTimePickerFormat.Short
            };
            islemPanel.Controls.Add(dtYeniBitis);

            var btnUzatGun = new Button
            {
                Text = "Kiralamayı Uzat (Ek Gün)",
                Location = new Point(12, 105),
                Width = 190,
                Height = 36,
                BackColor = Color.FromArgb(200, 230, 255),
                FlatStyle = FlatStyle.Flat
            };
            btnUzatGun.Click += BtnUzatGun_Click;
            islemPanel.Controls.Add(btnUzatGun);

            var btnUzatTarih = new Button
            {
                Text = "Yeni Bitiş ile Uzat",
                Location = new Point(215, 105),
                Width = 170,
                Height = 36,
                BackColor = Color.FromArgb(220, 235, 255),
                FlatStyle = FlatStyle.Flat
            };
            btnUzatTarih.Click += BtnUzatTarih_Click;
            islemPanel.Controls.Add(btnUzatTarih);

            var btnIade = new Button
            {
                Text = "Aracı İade Al (Geri Al)",
                Location = new Point(400, 105),
                Width = 190,
                Height = 36,
                BackColor = Color.FromArgb(255, 220, 180),
                FlatStyle = FlatStyle.Flat
            };
            btnIade.Click += BtnIade_Click;
            islemPanel.Controls.Add(btnIade);

            var btnYenile = new Button
            {
                Text = "Listeyi Yenile",
                Location = new Point(605, 105),
                Width = 130,
                Height = 36,
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            btnYenile.Click += (s, e) => Listele();
            islemPanel.Controls.Add(btnYenile);

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
            dgv.SelectionChanged += Dgv_SelectionChanged;
            Controls.Add(dgv);
            dgv.BringToFront();
            Listele();
        }

        private int? SeciliKiralamaId()
        {
            if (dgv.SelectedRows.Count == 0) return null;
            return Convert.ToInt32(dgv.SelectedRows[0].Cells["Id"].Value);
        }

        private void Dgv_SelectionChanged(object? sender, EventArgs e)
        {
            var id = SeciliKiralamaId();
            if (id == null)
            {
                lblSecimBilgi.Text = "Listeden bir aktif kiralama seçin.";
                return;
            }

            var k = yonetici.Kiralamalar.FirstOrDefault(x => x.Id == id.Value);
            if (k == null) return;

            lblSecimBilgi.Text = $"Seçili: #{k.Id} | {k.MusteriBilgi} | {k.AracBilgi} | Bitiş: {k.BitisTarihi:d} | Ücret: {k.ToplamUcret:C2}";
            dtYeniBitis.Value = k.BitisTarihi.AddDays(1);
        }

        private void BtnUzatGun_Click(object? sender, EventArgs e)
        {
            ArayuzYardimcisi.GuvenliCalistir(() =>
            {
                var id = SeciliKiralamaId();
                if (id == null) throw new ArgumentException("Lütfen uzatılacak kiralama kaydını seçin!");

                int ekGun = (int)nudEkGun.Value;
                yonetici.KiralamaUzatGun(id.Value, ekGun);
                Listele();
                ArayuzYardimcisi.BilgiGoster($"Kiralama {ekGun} gün uzatıldı. Ücret yeniden hesaplandı.");
            });
        }

        private void BtnUzatTarih_Click(object? sender, EventArgs e)
        {
            ArayuzYardimcisi.GuvenliCalistir(() =>
            {
                var id = SeciliKiralamaId();
                if (id == null) throw new ArgumentException("Lütfen uzatılacak kiralama kaydını seçin!");

                yonetici.KiralamaUzat(id.Value, dtYeniBitis.Value.Date);
                Listele();
                ArayuzYardimcisi.BilgiGoster("Kiralama yeni bitiş tarihine uzatıldı. Ücret yeniden hesaplandı.");
            });
        }

        private void BtnIade_Click(object? sender, EventArgs e)
        {
            ArayuzYardimcisi.GuvenliCalistir(() =>
            {
                var id = SeciliKiralamaId();
                if (id == null) throw new ArgumentException("Lütfen iade edilecek kiralama kaydını seçin!");

                if (!ArayuzYardimcisi.OnayIste(
                        "Seçili araç iade alınacak ve Müsait duruma getirilecek. Onaylıyor musunuz?",
                        "İade Onayı"))
                    return;

                yonetici.AraciIadeEt(id.Value);
                Listele();
                ArayuzYardimcisi.BilgiGoster("Araç başarıyla iade alındı ve müsait duruma getirildi!");
            });
        }

        private void Listele()
        {
            dgv.DataSource = null;
            var aktifler = yonetici.Kiralamalar
                .Where(k => k.Durum == KiralamaDurumu.Aktif)
                .Select(k => new
                {
                    k.Id,
                    Musteri = string.IsNullOrWhiteSpace(k.MusteriBilgi) ? "Bilinmiyor" : k.MusteriBilgi,
                    Arac = string.IsNullOrWhiteSpace(k.AracBilgi) ? "Bilinmiyor" : k.AracBilgi,
                    Baslangic = k.BaslangicTarihi.ToShortDateString(),
                    Bitis = k.BitisTarihi.ToShortDateString(),
                    k.ToplamGun,
                    ToplamUcret = $"{k.ToplamUcret:C2}",
                    Durum = k.Durum.Turkce()
                }).ToList();

            dgv.DataSource = aktifler;
            lblSecimBilgi.Text = aktifler.Count == 0
                ? "Aktif kiralama yok."
                : "Listeden bir aktif kiralama seçin.";
        }
    }
}
