using System;
using System.Text.RegularExpressions;
using AracKiralamaSistemi.Arayuzler;

namespace AracKiralamaSistemi.Modeller
{
    public abstract class Arac : IKiralanabilir
    {
        private int _id;
        private string _plaka = string.Empty;
        private string _marka = string.Empty;
        private string _model = string.Empty;
        private int _uretilisYili;
        private decimal _gunlukKiraUcreti;
        private AracDurumu _durum;

        public int Id { get => _id; set => _id = value; }

        public string Plaka
        {
            get => _plaka;
            set => _plaka = NormalizePlaka(value);
        }

        public string Marka
        {
            get => _marka;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Marka boş olamaz!");
                _marka = value.Trim();
            }
        }

        public string ModelAdi
        {
            get => _model;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Model boş olamaz!");
                _model = value.Trim();
            }
        }

        public int UretilisYili
        {
            get => _uretilisYili;
            set
            {
                if (value < 1900 || value > DateTime.Now.Year + 1)
                    throw new ArgumentException("Geçerli bir üretim yılı giriniz!");
                _uretilisYili = value;
            }
        }

        public decimal GunlukKiraUcreti
        {
            get => _gunlukKiraUcreti;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Günlük kira ücreti sıfırdan büyük olmalıdır!");
                _gunlukKiraUcreti = value;
            }
        }

        /// <summary>Durum yalnızca KirayaVer / AraciIadeAl ile değişir.</summary>
        public AracDurumu Durum
        {
            get => _durum;
            private set => _durum = value;
        }

        public bool MusaitMi => Durum == AracDurumu.Musait;

        public abstract string TipAdi { get; }

        protected Arac(int id, string plaka, string marka, string modelAdi, int uretilisYili, decimal gunlukKiraUcreti)
        {
            Id = id;
            Plaka = plaka;
            Marka = marka;
            ModelAdi = modelAdi;
            UretilisYili = uretilisYili;
            GunlukKiraUcreti = gunlukKiraUcreti;
            Durum = AracDurumu.Musait;
        }

        protected decimal TemelUcretiHesapla(int gunSayisi)
        {
            if (gunSayisi < 1) gunSayisi = 1;
            return GunlukKiraUcreti * gunSayisi;
        }

        public abstract decimal KiralamaUcretiHesapla(int gunSayisi);

        public virtual string UcretDokumu(int gunSayisi)
        {
            decimal toplam = KiralamaUcretiHesapla(gunSayisi);
            return $"{GunlukKiraUcreti:C2} × {gunSayisi} gün = {toplam:C2}";
        }

        public void BilgileriGuncelle(string plaka, string marka, string model, int yil, decimal ucret)
        {
            // Önce doğrula, sonra ata (yarım güncelleme olmasın)
            var nPlaka = NormalizePlaka(plaka);
            if (string.IsNullOrWhiteSpace(marka)) throw new ArgumentException("Marka boş olamaz!");
            if (string.IsNullOrWhiteSpace(model)) throw new ArgumentException("Model boş olamaz!");
            if (yil < 1900 || yil > DateTime.Now.Year + 1) throw new ArgumentException("Geçerli bir üretim yılı giriniz!");
            if (ucret <= 0) throw new ArgumentException("Günlük kira ücreti sıfırdan büyük olmalıdır!");

            _plaka = nPlaka;
            _marka = marka.Trim();
            _model = model.Trim();
            _uretilisYili = yil;
            _gunlukKiraUcreti = ucret;
        }

        public void KirayaVer()
        {
            if (Durum == AracDurumu.Kirada)
                throw new InvalidOperationException("Araç zaten kirada!");
            Durum = AracDurumu.Kirada;
        }

        public void AraciIadeAl() => Durum = AracDurumu.Musait;

        public override string ToString() => $"{Plaka} - {Marka} {ModelAdi} ({TipAdi})";

        /// <summary>Plakayı normalize eder (boşluksuz, büyük harf) ve Türkiye formatını kontrol eder.</summary>
        public static string NormalizePlaka(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Plaka boş olamaz!");

            string temiz = Regex.Replace(value.Trim().ToUpperInvariant(), @"\s+", "");
            // 34ABC123 veya 06A1234 benzeri
            if (!Regex.IsMatch(temiz, @"^(0[1-9]|[1-7][0-9]|8[01])[A-Z]{1,3}[0-9]{2,4}$"))
                throw new ArgumentException("Plaka formatı geçersiz! Örnek: 34ABC123");

            return temiz;
        }
    }
}
