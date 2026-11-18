namespace AracKiralamaSistemi.Modeller
{
    public class Motosiklet : Arac
    {
        private bool _kaskDahilMi;

        public bool KaskDahilMi
        {
            get => _kaskDahilMi;
            set => _kaskDahilMi = value;
        }

        public override string TipAdi => "Motosiklet";

        public Motosiklet(int id, string plaka, string marka, string modelAdi, int uretilisYili, decimal gunlukKiraUcreti, bool kaskDahilMi)
            : base(id, plaka, marka, modelAdi, uretilisYili, gunlukKiraUcreti)
        {
            KaskDahilMi = kaskDahilMi;
        }

        public override decimal KiralamaUcretiHesapla(int gunSayisi)
        {
            decimal temelTutar = TemelUcretiHesapla(gunSayisi);
            if (KaskDahilMi) temelTutar *= 1.15m;
            return temelTutar;
        }

        public override string UcretDokumu(int gunSayisi)
        {
            decimal toplam = KiralamaUcretiHesapla(gunSayisi);
            if (KaskDahilMi)
                return $"{GunlukKiraUcreti:C2} × {gunSayisi} gün + kask (%15) = {toplam:C2}";
            return base.UcretDokumu(gunSayisi);
        }
    }
}
