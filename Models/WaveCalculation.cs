using System.ComponentModel.DataAnnotations;

namespace WavePowerApp.Models
{
    public class WaveCalculation
    {
        public int Id { get; set; }
        public double WaveHeight { get; set; } // H_s (meters)
        public double WavePeriod { get; set; } // T_e (seconds)
        public double ArrayLength { get; set; } // L (meters)
        public double Efficiency { get; set; } // Conversion efficiency (%)
        public double RawPowerKw { get; set; }
        public double NetPowerKw { get; set; }
        public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
    }
}