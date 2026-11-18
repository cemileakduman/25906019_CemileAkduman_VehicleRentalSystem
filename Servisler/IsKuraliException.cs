using System;

namespace AracKiralamaSistemi.Servisler
{
    /// <summary>İş kuralı ihlallerinde fırlatılır (ör. plaka tekrarı).</summary>
    public class IsKuraliException : Exception
    {
        public IsKuraliException(string message) : base(message)
        {
        }
    }
}
