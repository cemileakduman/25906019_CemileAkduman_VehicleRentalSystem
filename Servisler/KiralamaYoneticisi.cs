using System;
using System.Collections.Generic;
using System.Linq;
using AracKiralamaSistemi.Arayuzler;
using AracKiralamaSistemi.Modeller;

namespace AracKiralamaSistemi.Servisler
{
    public class KiralamaYoneticisi
    {
        private readonly List<Arac> _araclar = new List<Arac>();
        private readonly List<Musteri> _musteriler = new List<Musteri>();
        private readonly List<Kiralama> _kiralamalar = new List<Kiralama>();

        public IReadOnlyList<Arac> Araclar => _araclar;
        public IReadOnlyList<Musteri> Musteriler => _musteriler;
        public IReadOnlyList<Kiralama> Kiralamalar => _kiralamalar;

        public event EventHandler? VeriDegisti;

        protected virtual void OnVeriDegisti() => VeriDegisti?.Invoke(this, EventArgs.Empty);

        public void OrnekVerileriYukle()
        {
            // Araçlar: hem müsait hem kirada senaryosu için
            var corolla = new Otomobil(1, "34ABC123", "Toyota", "Corolla", 2022, 1000m, "Benzin");
            var clio = new Otomobil(2, "06XYZ789", "Renault", "Clio", 2023, 800m, "Dizel");
            var cbr = new Motosiklet(3, "34MTR01", "Honda", "CBR500", 2021, 450m, true);
            var focus = new Otomobil(4, "35FCS45", "Ford", "Focus", 2020, 900m, "Benzin");
            _araclar.Add(corolla);
            _araclar.Add(clio);
            _araclar.Add(cbr);
            _araclar.Add(focus);

            var ahmet = new Musteri(1, "Ahmet", "Yılmaz", "5551234567", "A1234567");
            var ayse = new Musteri(2, "Ayşe", "Demir", "5559876543", "B7654321");
            var mehmet = new Musteri(3, "Mehmet", "Kaya", "5321112233", "C9988776");
            _musteriler.Add(ahmet);
            _musteriler.Add(ayse);
            _musteriler.Add(mehmet);

            // Aktif kiralamalar (dashboard + iade/uzatma dolu görünsün)
            // Corolla → Ahmet (kirada)
            _kiralamalar.Add(new Kiralama(1, ahmet, corolla, DateTime.Today.AddDays(-3), DateTime.Today.AddDays(2), corolla.KiralamaUcretiHesapla(5)));
            corolla.KirayaVer();

            // CBR → Ayşe (kirada)
            _kiralamalar.Add(new Kiralama(2, ayse, cbr, DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1), cbr.KiralamaUcretiHesapla(2)));
            cbr.KirayaVer();

            // Tamamlanmış kiralama (geçmiş kayıt; Clio yeniden müsait)
            var eski = new Kiralama(3, mehmet, clio, DateTime.Today.AddDays(-10), DateTime.Today.AddDays(-7), clio.KiralamaUcretiHesapla(3));
            eski.Tamamla();
            _kiralamalar.Add(eski);
            // Clio ve Focus müsait kalır → kiralama ekranında seçilebilir
        }

        public int SonrakiAracId()
        {
            int aday = 1;
            while (_araclar.Any(a => a.Id == aday))
                aday++;
            return aday;
        }

        public int SonrakiMusteriId()
        {
            int aday = 1;
            while (_musteriler.Any(m => m.Id == aday))
                aday++;
            return aday;
        }

        public int SonrakiKiralamaId()
        {
            int aday = 1;
            while (_kiralamalar.Any(k => k.Id == aday))
                aday++;
            return aday;
        }

        public void AracEkle(Arac arac)
        {
            if (arac == null) throw new ArgumentNullException(nameof(arac));
            PlakaBenzersizMi(arac.Plaka, null);
            _araclar.Add(arac);
            OnVeriDegisti();
        }

        public void AracGuncelle(int id, string plaka, string marka, string model, int yil, decimal ucret, string? yakitTipi = null, bool? kaskDahilMi = null)
        {
            var arac = _araclar.FirstOrDefault(a => a.Id == id)
                ?? throw new IsKuraliException("Araç bulunamadı!");

            string normalizePlaka = Arac.NormalizePlaka(plaka);
            PlakaBenzersizMi(normalizePlaka, id);

            arac.BilgileriGuncelle(normalizePlaka, marka, model, yil, ucret);

            switch (arac)
            {
                case Otomobil otomobil when yakitTipi != null:
                    otomobil.YakitTipi = yakitTipi;
                    break;
                case Motosiklet motosiklet when kaskDahilMi.HasValue:
                    motosiklet.KaskDahilMi = kaskDahilMi.Value;
                    break;
            }

            OnVeriDegisti();
        }

        public void AracSil(int aracId)
        {
            var arac = _araclar.FirstOrDefault(a => a.Id == aracId)
                ?? throw new IsKuraliException("Silinecek araç bulunamadı!");

            // Daha önce kiralanmış araç silinmez
            if (_kiralamalar.Any(k => k.Arac != null && k.Arac.Id == aracId))
                throw new IsKuraliException("Kiralama kaydı bulunan araç silinemez!");

            _araclar.Remove(arac);
            OnVeriDegisti();
        }

        public void MusteriEkle(Musteri musteri)
        {
            ZorunluMusteriAlanlariniKontrolEt(musteri.Ad, musteri.Soyad, musteri.Telefon, musteri.EhliyetNumarasi);

            if (_musteriler.Any(m => m.EhliyetNumarasi.Equals(musteri.EhliyetNumarasi, StringComparison.OrdinalIgnoreCase)))
                throw new IsKuraliException("Bu ehliyet numarası zaten kayıtlı!");

            _musteriler.Add(musteri);
            OnVeriDegisti();
        }

        public void MusteriGuncelle(int id, string ad, string soyad, string tel, string ehliyet)
        {
            var musteri = _musteriler.FirstOrDefault(m => m.Id == id)
                ?? throw new IsKuraliException("Müşteri bulunamadı!");

            ZorunluMusteriAlanlariniKontrolEt(ad, soyad, tel, ehliyet);

            if (_musteriler.Any(m => m.Id != id && m.EhliyetNumarasi.Equals(ehliyet, StringComparison.OrdinalIgnoreCase)))
                throw new IsKuraliException("Bu ehliyet numarası başka bir müşteriye ait!");

            musteri.Ad = ad;
            musteri.Soyad = soyad;
            musteri.Telefon = tel;
            musteri.EhliyetNumarasi = ehliyet;
            OnVeriDegisti();
        }

        public void MusteriSil(int musteriId)
        {
            var musteri = _musteriler.FirstOrDefault(m => m.Id == musteriId)
                ?? throw new IsKuraliException("Silinecek müşteri bulunamadı!");

            if (_kiralamalar.Any(k => k.Musteri != null && k.Musteri.Id == musteriId && k.Durum == KiralamaDurumu.Aktif))
                throw new IsKuraliException("Aktif kiralaması bulunan müşteri silinemez!");

            _musteriler.Remove(musteri);
            OnVeriDegisti();
        }

        public void KiralamaOlustur(int id, Musteri musteri, Arac arac, DateTime baslangic, DateTime bitis)
        {
            if (musteri == null || arac == null)
                throw new ArgumentException("Müşteri ve araç seçilmelidir!");
            if (bitis.Date < baslangic.Date)
                throw new ArgumentException("Bitiş tarihi başlangıç tarihinden önce olamaz!");
            if (!arac.MusaitMi)
                throw new IsKuraliException("Seçilen araç şu anda müsait değil! Yalnızca müsait araçlar kiralanabilir.");

            int gunSayisi = Math.Max(1, (bitis.Date - baslangic.Date).Days);
            decimal tutar = arac.KiralamaUcretiHesapla(gunSayisi);
            _kiralamalar.Add(new Kiralama(id, musteri, arac, baslangic.Date, bitis.Date, tutar));
            KirayaVer(arac);
            OnVeriDegisti();
        }

        public void AraciIadeEt(int kiralamaId)
        {
            var kiralama = _kiralamalar.FirstOrDefault(k => k.Id == kiralamaId && k.Durum == KiralamaDurumu.Aktif)
                ?? throw new IsKuraliException("Aktif kiralama kaydı bulunamadı!");
            if (kiralama.Arac == null)
                throw new IsKuraliException("Kiralama kaydına bağlı araç bulunamadı!");

            kiralama.Tamamla();
            IadeAl(kiralama.Arac);
            OnVeriDegisti();
        }

        public void KiralamaUzat(int kiralamaId, DateTime yeniBitisTarihi)
        {
            var kiralama = _kiralamalar.FirstOrDefault(k => k.Id == kiralamaId && k.Durum == KiralamaDurumu.Aktif)
                ?? throw new IsKuraliException("Aktif kiralama kaydı bulunamadı!");
            if (kiralama.Arac == null)
                throw new IsKuraliException("Kiralama kaydına bağlı araç bulunamadı!");

            if (yeniBitisTarihi.Date <= kiralama.BitisTarihi.Date)
                throw new ArgumentException("Yeni bitiş tarihi, mevcut bitiş tarihinden sonra olmalıdır!");

            int gunSayisi = Math.Max(1, (yeniBitisTarihi.Date - kiralama.BaslangicTarihi.Date).Days);
            kiralama.BitisTarihi = yeniBitisTarihi.Date;
            kiralama.ToplamUcret = kiralama.Arac.KiralamaUcretiHesapla(gunSayisi);
            OnVeriDegisti();
        }

        public void KiralamaUzatGun(int kiralamaId, int ekGun)
        {
            if (ekGun < 1) throw new ArgumentException("Uzatma süresi en az 1 gün olmalıdır!");
            var kiralama = _kiralamalar.FirstOrDefault(k => k.Id == kiralamaId && k.Durum == KiralamaDurumu.Aktif)
                ?? throw new IsKuraliException("Aktif kiralama kaydı bulunamadı!");
            KiralamaUzat(kiralamaId, kiralama.BitisTarihi.AddDays(ekGun));
        }

        public void KirayaVer(IKiralanabilir kiralanabilir)
        {
            if (kiralanabilir == null) throw new ArgumentNullException(nameof(kiralanabilir));
            if (!kiralanabilir.MusaitMi)
                throw new IsKuraliException("Araç zaten kirada!");
            kiralanabilir.KirayaVer();
        }

        public void IadeAl(IKiralanabilir kiralanabilir)
        {
            if (kiralanabilir == null) throw new ArgumentNullException(nameof(kiralanabilir));
            kiralanabilir.AraciIadeAl();
        }

        public List<Arac> DurumaGoreFiltrele(string filtre)
        {
            switch (filtre)
            {
                case "Müsait":
                    return _araclar.Where(a => a.MusaitMi).ToList();
                case "Kirada":
                    return _araclar.Where(a => !a.MusaitMi).ToList();
                default:
                    return _araclar.ToList();
            }
        }

        private void PlakaBenzersizMi(string plaka, int? haricId)
        {
            string n = Arac.NormalizePlaka(plaka);
            if (_araclar.Any(a => a.Plaka.Equals(n, StringComparison.OrdinalIgnoreCase) && (!haricId.HasValue || a.Id != haricId.Value)))
                throw new IsKuraliException("Bu plakaya sahip araç zaten kayıtlı! Plaka tekrarına izin verilmez.");
        }

        private static void ZorunluMusteriAlanlariniKontrolEt(string ad, string soyad, string telefon, string ehliyet)
        {
            if (string.IsNullOrWhiteSpace(ad) ||
                string.IsNullOrWhiteSpace(soyad) ||
                string.IsNullOrWhiteSpace(telefon) ||
                string.IsNullOrWhiteSpace(ehliyet))
                throw new ArgumentException("Ad, soyad, telefon ve ehliyet numarası zorunludur; boş bırakılamaz!");
        }
    }
}
