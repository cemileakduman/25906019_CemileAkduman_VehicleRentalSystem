using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AracKiralamaSistemi.Modeller;
using AracKiralamaSistemi.Servisler;

namespace AracKiralamaSistemi
{
    // --- ANA KABUK: SIDEBAR + NAVBAR + İÇERİK ---
    public class AnaForm : Form
    {
        private readonly KiralamaYoneticisi yonetici;
        private readonly Panel contentPanel;
        private readonly Label lblSayfaBaslik;
        private readonly System.Collections.Generic.List<Button> navButonlari = new();
        private string aktifSayfa = "dashboard";

        public AnaForm()
        {
            yonetici = new KiralamaYoneticisi();
            yonetici.OrnekVerileriYukle();
            yonetici.VeriDegisti += (s, e) =>
            {
                if (aktifSayfa == "dashboard" && !IsDisposed)
                    SayfaGoster("dashboard");
            };

            Text = "Araç Kiralama Yönetim Sistemi";
            Width = 1280;
            Height = 760;
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10F);
            BackColor = Color.FromArgb(242, 247, 252);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            var sidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 230,
                BackColor = Color.FromArgb(24, 48, 90)
            };
            Controls.Add(sidebar);

            sidebar.Controls.Add(new Label
            {
                Text = "Akduman Ticaret",
                Location = new Point(18, 24),
                AutoSize = true,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White
            });
            sidebar.Controls.Add(new Label
            {
                Text = "Araç Kiralama Sistemi",
                Location = new Point(18, 48),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(180, 200, 230)
            });

            int y = 100;
            sidebar.Controls.Add(OlusturNavButon("Ana Sayfa", "dashboard", ref y));
            sidebar.Controls.Add(OlusturNavButon("Araç Yönetimi", "arac", ref y));
            sidebar.Controls.Add(OlusturNavButon("Müşteri Yönetimi", "musteri", ref y));
            sidebar.Controls.Add(OlusturNavButon("Araç Kiralama", "kiralama", ref y));
            sidebar.Controls.Add(OlusturNavButon("İade / Uzatma", "iade", ref y));

            var navbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 58,
                BackColor = Color.White
            };
            Controls.Add(navbar);
            navbar.BringToFront();

            lblSayfaBaslik = new Label
            {
                Text = "Ana Sayfa",
                Location = new Point(20, 16),
                AutoSize = true,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 50, 80)
            };
            navbar.Controls.Add(lblSayfaBaslik);

            var btnYenile = new Button
            {
                Text = "Yenile",
                Location = new Point(920, 12),
                Width = 100,
                Height = 34,
                BackColor = Color.FromArgb(82, 168, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnYenile.Click += (s, e) => SayfaGoster(aktifSayfa);
            navbar.Controls.Add(btnYenile);

            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(242, 247, 252),
                Padding = new Padding(16)
            };
            Controls.Add(contentPanel);
            contentPanel.BringToFront();

            SayfaGoster("dashboard");
        }

        private Button OlusturNavButon(string metin, string sayfaKodu, ref int y)
        {
            var btn = new Button
            {
                Text = "  " + metin,
                Tag = sayfaKodu,
                Location = new Point(12, y),
                Width = 206,
                Height = 42,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(24, 48, 90),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 70, 120);
            btn.Click += (s, e) => SayfaGoster(sayfaKodu);
            navButonlari.Add(btn);
            y += 48;
            return btn;
        }

        private void NavAktifYap(string sayfaKodu)
        {
            aktifSayfa = sayfaKodu;
            foreach (var btn in navButonlari)
            {
                bool aktif = (btn.Tag?.ToString() ?? "") == sayfaKodu;
                btn.BackColor = aktif ? Color.FromArgb(56, 100, 170) : Color.FromArgb(24, 48, 90);
                btn.Font = new Font("Segoe UI", 10F, aktif ? FontStyle.Bold : FontStyle.Regular);
            }
        }

        private void SayfaGoster(string sayfaKodu)
        {
            NavAktifYap(sayfaKodu);

            foreach (Control c in contentPanel.Controls)
                c.Dispose();
            contentPanel.Controls.Clear();

            switch (sayfaKodu)
            {
                case "arac":
                    lblSayfaBaslik.Text = "Araç Yönetimi";
                    IcerigeGom(new AracYonetimiForm(yonetici));
                    break;
                case "musteri":
                    lblSayfaBaslik.Text = "Müşteri Yönetimi";
                    IcerigeGom(new MusteriYonetimiForm(yonetici));
                    break;
                case "kiralama":
                    lblSayfaBaslik.Text = "Araç Kiralama";
                    IcerigeGom(new KiralamaForm(yonetici));
                    break;
                case "iade":
                    lblSayfaBaslik.Text = "Araç İade / Uzatma";
                    IcerigeGom(new IadeForm(yonetici));
                    break;
                default:
                    lblSayfaBaslik.Text = "Ana Sayfa (Dashboard)";
                    contentPanel.Controls.Add(OlusturDashboard());
                    break;
            }
        }

        private void IcerigeGom(Form sayfa)
        {
            sayfa.TopLevel = false;
            sayfa.FormBorderStyle = FormBorderStyle.None;
            sayfa.Dock = DockStyle.Fill;
            sayfa.BackColor = Color.FromArgb(242, 247, 252);
            contentPanel.Controls.Add(sayfa);
            sayfa.Show();
        }

        private Panel OlusturDashboard()
        {
            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };

            panel.Controls.Add(new Label
            {
                Text = "Özet İstatistikler",
                Location = new Point(8, 8),
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 60, 90)
            });

            var kartPanel = new FlowLayoutPanel
            {
                Location = new Point(4, 40),
                Size = new Size(980, 95),
                FlowDirection = FlowDirection.LeftToRight
            };
            panel.Controls.Add(kartPanel);

            kartPanel.Controls.Add(OlusturIstatistikKart("Toplam Araç", yonetici.Araclar.Count.ToString(), Color.FromArgb(76, 154, 255)));
            kartPanel.Controls.Add(OlusturIstatistikKart("Müsait Araç", yonetici.Araclar.Count(a => a.MusaitMi).ToString(), Color.FromArgb(56, 178, 116)));
            kartPanel.Controls.Add(OlusturIstatistikKart("Kiradaki Araç", yonetici.Araclar.Count(a => a.Durum == AracDurumu.Kirada).ToString(), Color.FromArgb(232, 93, 93)));
            kartPanel.Controls.Add(OlusturIstatistikKart("Toplam Müşteri", yonetici.Musteriler.Count.ToString(), Color.FromArgb(255, 170, 70)));
            kartPanel.Controls.Add(OlusturIstatistikKart("Aktif Kiralama", yonetici.Kiralamalar.Count(k => k.Durum == KiralamaDurumu.Aktif).ToString(), Color.FromArgb(120, 100, 220)));

            panel.Controls.Add(new Label
            {
                Text = "Aktif Kiralamalar",
                Location = new Point(8, 150),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 60, 90)
            });
            panel.Controls.Add(new Label
            {
                Text = "Araç Durumu Özeti",
                Location = new Point(510, 150),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 60, 90)
            });

            var dgvAktif = new DataGridView
            {
                Location = new Point(8, 180),
                Width = 480,
                Height = 420,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(245, 247, 250) },
                DataSource = yonetici.Kiralamalar
                    .Where(k => k.Durum == KiralamaDurumu.Aktif)
                    .Select(k => new
                    {
                        k.Id,
                        Musteri = string.IsNullOrWhiteSpace(k.MusteriBilgi) ? "-" : k.MusteriBilgi,
                        Arac = string.IsNullOrWhiteSpace(k.AracBilgi) ? "-" : k.AracBilgi,
                        Baslangic = k.BaslangicTarihi.ToShortDateString(),
                        Bitis = k.BitisTarihi.ToShortDateString(),
                        Gun = k.ToplamGun,
                        Ucret = $"{k.ToplamUcret:C2}"
                    })
                    .ToList()
            };
            panel.Controls.Add(dgvAktif);

            var dgvArac = new DataGridView
            {
                Location = new Point(510, 180),
                Width = 470,
                Height = 420,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(245, 247, 250) },
                DataSource = yonetici.Araclar
                    .Select(a => new
                    {
                        Tip = a.TipAdi,
                        a.Plaka,
                        a.Marka,
                        Model = a.ModelAdi,
                        Durum = a.Durum.Turkce()
                    })
                    .ToList()
            };
            dgvArac.CellFormatting += (s, e) =>
            {
                if (dgvArac.Columns[e.ColumnIndex].Name == "Durum" && e.Value is string durum)
                    UygulaDurumRengi(e.CellStyle, durum);
            };
            panel.Controls.Add(dgvArac);

            return panel;
        }

        private static Panel OlusturIstatistikKart(string baslik, string deger, Color renk)
        {
            var kart = new Panel
            {
                Width = 180,
                Height = 85,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(5)
            };
            kart.Controls.Add(new Panel { Width = 5, Height = 85, BackColor = renk, Dock = DockStyle.Left });
            kart.Controls.Add(new Label
            {
                Text = baslik,
                Location = new Point(14, 12),
                AutoSize = true,
                ForeColor = Color.FromArgb(90, 90, 90),
                Font = new Font("Segoe UI", 9F)
            });
            kart.Controls.Add(new Label
            {
                Text = deger,
                Location = new Point(14, 36),
                AutoSize = true,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = renk
            });
            return kart;
        }

        public static void UygulaDurumRengi(DataGridViewCellStyle? stil, string durum)
        {
            if (stil == null) return;

            if (durum == "Müsait")
            {
                stil.BackColor = Color.FromArgb(220, 255, 220);
                stil.ForeColor = Color.DarkGreen;
                stil.SelectionBackColor = Color.Green;
                stil.SelectionForeColor = Color.White;
            }
            else if (durum == "Kirada")
            {
                stil.BackColor = Color.FromArgb(255, 235, 230);
                stil.ForeColor = Color.DarkRed;
                stil.SelectionBackColor = Color.DarkRed;
                stil.SelectionForeColor = Color.White;
            }
        }
    }
}
