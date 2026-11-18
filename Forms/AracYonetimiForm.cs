using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AracKiralamaSistemi.Modeller;
using AracKiralamaSistemi.Servisler;

namespace AracKiralamaSistemi
{
    public class AracYonetimiForm : Form
    {
        private readonly KiralamaYoneticisi yonetici;
        private readonly DataGridView dgv;
        private readonly TextBox txtPlaka, txtMarka, txtModel, txtYil, txtUcret, txtAra;
        private readonly ComboBox cmbDurumFiltre, cmbTip, cmbYakit;
        private readonly CheckBox chkKask;
        private readonly Label lblYakit, lblKask;
        private int secilenAracId = 0;

        public AracYonetimiForm(KiralamaYoneticisi yonetici)
        {
            this.yonetici = yonetici;
            Text = "Araç Yönetimi (Ekle / Güncelle / Sil)";
            Width = 980;
            Height = 620;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 10F);
            BackColor = Color.FromArgb(242, 247, 252);

            var ustPanel = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(12, 12, 12, 8) };
            Controls.Add(ustPanel);

            ustPanel.Controls.Add(new Label { Text = "Arama:", Location = new Point(12, 16), AutoSize = true });
            txtAra = new TextBox { Location = new Point(75, 12), Width = 200, PlaceholderText = "Plaka / marka / model" };
            txtAra.TextChanged += (s, e) => Listele();
            ustPanel.Controls.Add(txtAra);

            ustPanel.Controls.Add(new Label { Text = "Durum:", Location = new Point(295, 16), AutoSize = true });
            cmbDurumFiltre = new ComboBox { Location = new Point(355, 12), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbDurumFiltre.Items.Add("Tümü");
            cmbDurumFiltre.Items.Add("Müsait");
            cmbDurumFiltre.Items.Add("Kirada");
            cmbDurumFiltre.SelectedIndex = 0;
            cmbDurumFiltre.SelectedIndexChanged += (s, e) => Listele();
            ustPanel.Controls.Add(cmbDurumFiltre);

            var btnYenile = new Button { Text = "Yenile", Location = new Point(495, 10), Width = 100, Height = 30, BackColor = Color.LightBlue };
            btnYenile.Click += (s, e) => Listele();
            ustPanel.Controls.Add(btnYenile);

            var formPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 170,
                BackColor = Color.White,
                Padding = new Padding(12)
            };
            Controls.Add(formPanel);

            formPanel.Controls.Add(new Label { Text = "Araç Bilgileri", Location = new Point(12, 8), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) });

            formPanel.Controls.Add(new Label { Text = "Tip:", Location = new Point(12, 40), AutoSize = true });
            cmbTip = new ComboBox { Location = new Point(55, 36), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTip.Items.Add("Otomobil");
            cmbTip.Items.Add("Motosiklet");
            cmbTip.SelectedIndex = 0;
            cmbTip.SelectedIndexChanged += (s, e) => TipAlanlariniGuncelle();
            formPanel.Controls.Add(cmbTip);

            formPanel.Controls.Add(new Label { Text = "Plaka:", Location = new Point(195, 40), AutoSize = true });
            txtPlaka = new TextBox { Location = new Point(250, 36), Width = 120, PlaceholderText = "34ABC123" };
            formPanel.Controls.Add(txtPlaka);

            formPanel.Controls.Add(new Label { Text = "Marka:", Location = new Point(390, 40), AutoSize = true });
            txtMarka = new TextBox { Location = new Point(450, 36), Width = 120 };
            formPanel.Controls.Add(txtMarka);

            formPanel.Controls.Add(new Label { Text = "Model:", Location = new Point(590, 40), AutoSize = true });
            txtModel = new TextBox { Location = new Point(655, 36), Width = 120 };
            formPanel.Controls.Add(txtModel);

            formPanel.Controls.Add(new Label { Text = "Üretim Yılı:", Location = new Point(12, 80), AutoSize = true });
            txtYil = new TextBox { Location = new Point(100, 76), Width = 100 };
            formPanel.Controls.Add(txtYil);

            formPanel.Controls.Add(new Label { Text = "Günlük Ücret:", Location = new Point(220, 80), AutoSize = true });
            txtUcret = new TextBox { Location = new Point(325, 76), Width = 90 };
            formPanel.Controls.Add(txtUcret);

            lblYakit = new Label { Text = "Yakıt:", Location = new Point(440, 80), AutoSize = true };
            formPanel.Controls.Add(lblYakit);
            cmbYakit = new ComboBox { Location = new Point(490, 76), Width = 110, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbYakit.Items.AddRange(new object[] { "Benzin", "Dizel", "Hibrit", "Elektrik" });
            cmbYakit.SelectedIndex = 0;
            formPanel.Controls.Add(cmbYakit);

            lblKask = new Label { Text = "Kask:", Location = new Point(440, 80), AutoSize = true, Visible = false };
            formPanel.Controls.Add(lblKask);
            chkKask = new CheckBox { Text = "Kask dahil", Location = new Point(490, 78), Width = 120, Visible = false };
            formPanel.Controls.Add(chkKask);

            TipAlanlariniGuncelle();

            var btnEkle = new Button { Text = "Yeni Ekle", Location = new Point(12, 120), Width = 110, Height = 35, BackColor = Color.LightGreen };
            btnEkle.Click += (s, e) => ArayuzYardimcisi.GuvenliCalistir(() =>
            {
                if (string.IsNullOrWhiteSpace(txtYil.Text) || string.IsNullOrWhiteSpace(txtUcret.Text))
                    throw new ArgumentException("Üretim yılı ve günlük ücret zorunludur!");

                int yeniId = yonetici.SonrakiAracId();
                int yil = Convert.ToInt32(txtYil.Text);
                decimal ucret = Convert.ToDecimal(txtUcret.Text);
                string tip = cmbTip.SelectedItem?.ToString() ?? "Otomobil";

                Arac yeniArac = tip == "Motosiklet"
                    ? new Motosiklet(yeniId, txtPlaka.Text, txtMarka.Text, txtModel.Text, yil, ucret, chkKask.Checked)
                    : new Otomobil(yeniId, txtPlaka.Text, txtMarka.Text, txtModel.Text, yil, ucret, cmbYakit.SelectedItem?.ToString() ?? "Benzin");

                yonetici.AracEkle(yeniArac);
                Listele();
                Temizle();
                ArayuzYardimcisi.BilgiGoster("Araç eklendi!");
            });
            formPanel.Controls.Add(btnEkle);

            var btnGuncelle = new Button { Text = "Güncelle", Location = new Point(135, 120), Width = 110, Height = 35, BackColor = Color.LightYellow };
            btnGuncelle.Click += (s, e) => ArayuzYardimcisi.GuvenliCalistir(() =>
            {
                if (secilenAracId == 0) throw new ArgumentException("Lütfen tablodan güncellenecek aracı seçin!");
                if (string.IsNullOrWhiteSpace(txtYil.Text) || string.IsNullOrWhiteSpace(txtUcret.Text))
                    throw new ArgumentException("Üretim yılı ve günlük ücret zorunludur!");

                int yil = Convert.ToInt32(txtYil.Text);
                decimal ucret = Convert.ToDecimal(txtUcret.Text);
                yonetici.AracGuncelle(
                    secilenAracId,
                    txtPlaka.Text,
                    txtMarka.Text,
                    txtModel.Text,
                    yil,
                    ucret,
                    cmbYakit.SelectedItem?.ToString(),
                    chkKask.Checked);
                Listele();
                Temizle();
                ArayuzYardimcisi.BilgiGoster("Araç güncellendi!");
            });
            formPanel.Controls.Add(btnGuncelle);

            var btnSil = new Button { Text = "Seçileni Sil", Location = new Point(258, 120), Width = 110, Height = 35, BackColor = Color.MistyRose };
            btnSil.Click += (s, e) => ArayuzYardimcisi.GuvenliCalistir(() =>
            {
                if (secilenAracId == 0) throw new ArgumentException("Lütfen silinecek aracı seçin!");
                if (!ArayuzYardimcisi.OnayIste("Seçili araç silinecek. Devam edilsin mi?", "Silme Onayı")) return;

                yonetici.AracSil(secilenAracId);
                Listele();
                Temizle();
                ArayuzYardimcisi.BilgiGoster("Araç silindi!");
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
            dgv.CellFormatting += Dgv_CellFormatting;
            dgv.CellClick += Dgv_CellClick;
            Controls.Add(dgv);
            dgv.BringToFront();
            Listele();
        }

        private void TipAlanlariniGuncelle()
        {
            bool motosiklet = cmbTip.SelectedItem?.ToString() == "Motosiklet";
            lblYakit.Visible = !motosiklet;
            cmbYakit.Visible = !motosiklet;
            lblKask.Visible = motosiklet;
            chkKask.Visible = motosiklet;
            cmbTip.Enabled = secilenAracId == 0;
        }

        private void Listele()
        {
            var arama = txtAra.Text.Trim();
            var filtre = cmbDurumFiltre.SelectedItem?.ToString() ?? "Tümü";

            var sonuc = yonetici.DurumaGoreFiltrele(filtre)
                .Where(a => string.IsNullOrWhiteSpace(arama) ||
                    a.Plaka.Contains(arama, StringComparison.OrdinalIgnoreCase) ||
                    a.Marka.Contains(arama, StringComparison.OrdinalIgnoreCase) ||
                    a.ModelAdi.Contains(arama, StringComparison.OrdinalIgnoreCase))
                .Select(a => new
                {
                    a.Id,
                    Tip = a.TipAdi,
                    a.Plaka,
                    a.Marka,
                    ModelAdi = a.ModelAdi,
                    UretilisYili = a.UretilisYili,
                    GunlukKiraUcreti = a.GunlukKiraUcreti,
                    Ozellik = a is Otomobil o ? o.YakitTipi : (a is Motosiklet m ? (m.KaskDahilMi ? "Kask dahil" : "Kask yok") : ""),
                    Durum = a.Durum.Turkce()
                })
                .ToList();

            dgv.DataSource = null;
            dgv.DataSource = sonuc;
        }

        private void Dgv_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgv.Columns[e.ColumnIndex].Name == "Durum" && e.Value is string durum)
                AnaForm.UygulaDurumRengi(e.CellStyle, durum);
        }

        private void Dgv_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgv.Rows[e.RowIndex];
            secilenAracId = Convert.ToInt32(row.Cells["Id"].Value);
            txtPlaka.Text = row.Cells["Plaka"].Value?.ToString() ?? "";
            txtMarka.Text = row.Cells["Marka"].Value?.ToString() ?? "";
            txtModel.Text = row.Cells["ModelAdi"].Value?.ToString() ?? "";
            txtYil.Text = row.Cells["UretilisYili"].Value?.ToString() ?? "";
            txtUcret.Text = row.Cells["GunlukKiraUcreti"].Value?.ToString() ?? "";

            cmbTip.SelectedItem = row.Cells["Tip"].Value?.ToString() ?? "Otomobil";
            TipAlanlariniGuncelle();

            var arac = yonetici.Araclar.FirstOrDefault(a => a.Id == secilenAracId);
            if (arac is Otomobil otomobil)
            {
                var idx = cmbYakit.Items.IndexOf(otomobil.YakitTipi);
                cmbYakit.SelectedIndex = idx >= 0 ? idx : 0;
            }
            else if (arac is Motosiklet motosiklet)
            {
                chkKask.Checked = motosiklet.KaskDahilMi;
            }
        }

        private void Temizle()
        {
            secilenAracId = 0;
            txtPlaka.Text = txtMarka.Text = txtModel.Text = txtYil.Text = txtUcret.Text = "";
            cmbTip.SelectedIndex = 0;
            cmbYakit.SelectedIndex = 0;
            chkKask.Checked = false;
            TipAlanlariniGuncelle();
        }
    }
}
