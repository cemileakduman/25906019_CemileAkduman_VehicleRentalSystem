using System;

namespace AracKiralamaSistemi.Modeller
{
    public class Musteri
    {
        private int _id;
        private string _ad = string.Empty;
        private string _soyad = string.Empty;
        private string _telefon = string.Empty;
        private string _ehliyetNumarasi = string.Empty;

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public string Ad
        {
            get => _ad;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Ad boş olamaz!");
                _ad = value.Trim();
            }
        }

        public string Soyad
        {
            get => _soyad;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Soyad boş olamaz!");
                _soyad = value.Trim();
            }
        }

        public string Telefon
        {
            get => _telefon;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Telefon numarası boş olamaz!");
                _telefon = value.Trim();
            }
        }

        public string EhliyetNumarasi
        {
            get => _ehliyetNumarasi;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Ehliyet numarası boş olamaz!");
                _ehliyetNumarasi = value.Trim();
            }
        }

        public string AdSoyad => $"{Ad} {Soyad}".Trim();

        public Musteri(int id, string ad, string soyad, string telefon, string ehliyetNumarasi)
        {
            Id = id;
            Ad = ad;
            Soyad = soyad;
            Telefon = telefon;
            EhliyetNumarasi = ehliyetNumarasi;
        }

        public override string ToString() => AdSoyad;
    }
}
