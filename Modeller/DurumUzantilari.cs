namespace AracKiralamaSistemi.Modeller
{
    /// <summary>Enum değerlerini ekranda Türkçe göstermek için extension metotlar.</summary>
    public static class DurumUzantilari
    {
        public static string Turkce(this AracDurumu durum)
        {
            switch (durum)
            {
                case AracDurumu.Musait: return "Müsait";
                case AracDurumu.Kirada: return "Kirada";
                default: return durum.ToString();
            }
        }

        public static string Turkce(this KiralamaDurumu durum)
        {
            switch (durum)
            {
                case KiralamaDurumu.Aktif: return "Aktif";
                case KiralamaDurumu.Tamamlandi: return "Tamamlandı";
                default: return durum.ToString();
            }
        }
    }
}
