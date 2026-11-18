using System;

namespace AracKiralamaSistemi.Modeller
{
    public class Otomobil : Arac
    {
        private string _yakitTipi = "Benzin";

        public string YakitTipi
        {
            get => _yakitTipi;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Yakıt tipi boş olamaz!");
                _yakitTipi = value.Trim();
            }
        }

        public override string TipAdi => "Otomobil";

        public Otomobil(int id, string plaka, string marka, string modelAdi, int uretilisYili, decimal gunlukKiraUcreti, string yakitTipi)
            : base(id, plaka, marka, modelAdi, uretilisYili, gunlukKiraUcreti)
        {
            YakitTipi = yakitTipi;
        }

        public override decimal KiralamaUcretiHesapla(int gunSayisi)
        {
            decimal temelTutar = TemelUcretiHesapla(gunSayisi);
            // Polimorfizm: 7 gün üzeri %10 indirim
            if (gunSayisi > 7) temelTutar *= 0.90m;
            return temelTutar;
        }

        public override string UcretDokumu(int gunSayisi)
        {
            decimal toplam = KiralamaUcretiHesapla(gunSayisi);
            if (gunSayisi > 7)
                return $"{GunlukKiraUcreti:C2} × {gunSayisi} gün (−%10 indirim) = {toplam:C2}";
            return base.UcretDokumu(gunSayisi);
        }
    }
}
