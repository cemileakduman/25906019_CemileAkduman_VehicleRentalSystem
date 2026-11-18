using System;

namespace AracKiralamaSistemi.Modeller
{
    public class Kiralama
    {
        private int _id;
        private Musteri _musteri = null!;
        private Arac _arac = null!;
        private DateTime _baslangicTarihi;
        private DateTime _bitisTarihi;
        private decimal _toplamUcret;
        private KiralamaDurumu _durum;

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public Musteri Musteri
        {
            get => _musteri;
            set => _musteri = value ?? throw new ArgumentNullException(nameof(value), "Müşteri boş olamaz!");
        }

        public Arac Arac
        {
            get => _arac;
            set => _arac = value ?? throw new ArgumentNullException(nameof(value), "Araç boş olamaz!");
        }

        public DateTime BaslangicTarihi
        {
            get => _baslangicTarihi;
            set => _baslangicTarihi = value;
        }

        public DateTime BitisTarihi
        {
            get => _bitisTarihi;
            set => _bitisTarihi = value;
        }

        public int ToplamGun => Math.Max(1, (BitisTarihi - BaslangicTarihi).Days);

        public decimal ToplamUcret
        {
            get => _toplamUcret;
            set
            {
                if (value < 0) throw new ArgumentException("Toplam ücret negatif olamaz!");
                _toplamUcret = value;
            }
        }

        public KiralamaDurumu Durum
        {
            get => _durum;
            private set => _durum = value;
        }

        public string MusteriBilgi => Musteri != null ? Musteri.AdSoyad : "";
        public string AracBilgi => Arac != null ? $"{Arac.Marka} {Arac.ModelAdi} ({Arac.Plaka})" : "";

        public Kiralama(int id, Musteri musteri, Arac arac, DateTime baslangicTarihi, DateTime bitisTarihi, decimal toplamUcret)
        {
            if (bitisTarihi.Date < baslangicTarihi.Date)
                throw new ArgumentException("Bitiş tarihi başlangıç tarihinden önce olamaz!");

            Id = id;
            Musteri = musteri;
            Arac = arac;
            BaslangicTarihi = baslangicTarihi.Date;
            BitisTarihi = bitisTarihi.Date;
            ToplamUcret = toplamUcret;
            Durum = KiralamaDurumu.Aktif;
        }

        public void Tamamla() => Durum = KiralamaDurumu.Tamamlandi;
    }
}
