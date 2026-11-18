using System;
using System.Windows.Forms;
using AracKiralamaSistemi.Servisler;

namespace AracKiralamaSistemi
{
    /// <summary>Formlarda ortak mesaj ve hata yakalama yardımcısı.</summary>
    public static class ArayuzYardimcisi
    {
        public static void UyariGoster(string mesaj) =>
            MessageBox.Show(mesaj, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        public static void BilgiGoster(string mesaj) =>
            MessageBox.Show(mesaj, "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

        public static void HataGoster(Exception hata) =>
            MessageBox.Show(hata.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

        /// <summary>ArgumentException ve IsKuraliException'ı kullanıcıya uyarı olarak gösterir.</summary>
        public static void GuvenliCalistir(Action islem)
        {
            try
            {
                islem();
            }
            catch (ArgumentException ex)
            {
                UyariGoster(ex.Message);
            }
            catch (IsKuraliException ex)
            {
                UyariGoster(ex.Message);
            }
            catch (Exception ex)
            {
                HataGoster(ex);
            }
        }

        public static bool OnayIste(string mesaj, string baslik = "Onay") =>
            MessageBox.Show(mesaj, baslik, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
    }
}
