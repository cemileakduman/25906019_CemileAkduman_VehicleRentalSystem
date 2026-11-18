using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AracKiralamaSistemi.Modeller;
using AracKiralamaSistemi.Servisler;

namespace AracKiralamaSistemi
{
    public class KiralamaForm : Form
    {
        private readonly ComboBox cmbMusteri, cmbArac;
        private readonly DateTimePicker dtBaslangic, dtBitis;
        private readonly Label lblUcretBilgi;
        private readonly Button btnOnayla;
        private readonly KiralamaYoneticisi yonetici;

        public KiralamaForm(KiralamaYoneticisi yonetici)
        {
            this.yonetici = yonetici;
            Text = "Araç Kiralama ve Ücret Hesaplama";
            Width = 980;
            Height = 620;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(242, 247, 252);

            Controls.Add(new Label { Text = "Müşteri:", Location = new Point(20, 30), AutoSize = true });
            cmbMusteri = new ComboBox { Location = new Point(130, 27), Width = 290, DropDownStyle = ComboBoxStyle.DropDownList };
            Controls.Add(cmbMusteri);

            Controls.Add(new Label { Text = "Müsait Araç:", Location = new Point(20, 80), AutoSize = true });
            cmbArac = new ComboBox { Location = new Point(130, 77), Width = 290, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbArac.SelectedIndexChanged += HesaplaTahminiUcret;
            Controls.Add(cmbArac);

            Controls.Add(new Label { Text = "Başlangıç Tarihi:", Location = new Point(20, 130), AutoSize = true });
            dtBaslangic = new DateTimePicker { Location = new Point(130, 127), Width = 290 };
            dtBaslangic.ValueChanged += HesaplaTahminiUcret;
            Controls.Add(dtBaslangic);

            Controls.Add(new Label { Text = "Bitiş Tarihi:", Location = new Point(20, 180), AutoSize = true });
            dtBitis = new DateTimePicker { Location = new Point(130, 177), Width = 290 };
            dtBitis.Value = DateTime.Today.AddDays(1);
            dtBitis.ValueChanged += HesaplaTahminiUcret;
            Controls.Add(dtBitis);

            lblUcretBilgi = new Label
            {
                Text = "Toplam Ücret: 0 TL",
                Location = new Point(20, 225),
                AutoSize = true,
                MaximumSize = new Size(700, 0),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };
            Controls.Add(lblUcretBilgi);

            btnOnayla = new Button { Text = "Kiralamayı Onayla", Location = new Point(130, 280), Width = 290, Height = 35, BackColor = Color.LightGreen };
            btnOnayla.Click += (s, e) => ArayuzYardimcisi.GuvenliCalistir(() =>
            {
                var m = cmbMusteri.SelectedItem as Musteri;
                var a = cmbArac.SelectedItem as Arac;
                if (m == null || a == null)
                    throw new ArgumentException("Müşteri ve araç seçmelisiniz!");

                int id = yonetici.SonrakiKiralamaId();
                yonetici.KiralamaOlustur(id, m, a, dtBaslangic.Value.Date, dtBitis.Value.Date);
                ArayuzYardimcisi.BilgiGoster("Kiralama başarıyla yapıldı!");
                MusteriVeAracDoldur();
                HesaplaTahminiUcret(null, EventArgs.Empty);
            });
            Controls.Add(btnOnayla);

            MusteriVeAracDoldur();
            HesaplaTahminiUcret(null, EventArgs.Empty);
        }

        private void MusteriVeAracDoldur()
        {
            cmbMusteri.DataSource = null;
            cmbMusteri.DisplayMember = "AdSoyad";
            cmbMusteri.DataSource = yonetici.Musteriler.ToList();

            var musaitAraclar = yonetici.Araclar.Where(a => a.MusaitMi).ToList();
            cmbArac.DataSource = null;
            cmbArac.DisplayMember = nameof(Arac.Plaka);
            cmbArac.FormattingEnabled = true;
            cmbArac.Format -= CmbArac_Format;
            cmbArac.Format += CmbArac_Format;
            cmbArac.DataSource = musaitAraclar;

            bool hazir = yonetici.Musteriler.Count > 0 && musaitAraclar.Count > 0;
            btnOnayla.Enabled = hazir;
            if (!hazir)
                lblUcretBilgi.Text = "Kiralama için kayıtlı müşteri ve müsait araç gerekli.";
            else if (dtBitis.Value.Date < dtBaslangic.Value.Date)
                lblUcretBilgi.Text = "Bitiş tarihi başlangıç tarihinden önce olamaz!";
        }

        private void CmbArac_Format(object? sender, ListControlConvertEventArgs e)
        {
            if (e.ListItem is Arac arac)
                e.Value = arac.ToString();
        }

        private void HesaplaTahminiUcret(object? sender, EventArgs e)
        {
            if (dtBitis.Value.Date < dtBaslangic.Value.Date)
            {
                lblUcretBilgi.Text = "Bitiş tarihi başlangıç tarihinden önce olamaz!";
                btnOnayla.Enabled = false;
                return;
            }

            btnOnayla.Enabled = cmbMusteri.SelectedItem != null && cmbArac.SelectedItem != null;

            if (cmbArac.SelectedItem is Arac secilenArac)
            {
                int gun = Math.Max(1, (dtBitis.Value.Date - dtBaslangic.Value.Date).Days);
                // Polimorfik ücret dökümü (indirim / kask vb.)
                lblUcretBilgi.Text = $"Süre: {gun} gün | {secilenArac.UcretDokumu(gun)}";
            }
        }
    }
}
