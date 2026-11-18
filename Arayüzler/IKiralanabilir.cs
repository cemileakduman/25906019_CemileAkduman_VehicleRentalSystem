namespace AracKiralamaSistemi.Arayuzler
{
    /// <summary>Kiralanabilir varlıklar için ortak sözleşme.</summary>
    public interface IKiralanabilir
    {
        bool MusaitMi { get; }
        void KirayaVer();
        void AraciIadeAl();
    }
}
