

namespace ALARm.Core
{
    /// <summary>
    /// Уровень кривой
    /// </summary>
    public class ElCurve : MainTrackObject
    {
        /// <summary>
        /// уровень
        /// </summary>
        public float Lvl { get; set; }
        public int Transition_1 { get; set; }
        public int Transition_2 { get; set; }

        public float Radius { get; set; }
        public float Wear { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }
        public int Lvl_Length { get; set; }
        public int Lvl_start_km { get; set; }
        public int Lvl_start_m { get; set; }
        public int Lvl_final_km { get; set; } 
        public int Lvl_final_m { get; set; }
        public double FirstTransitionEnd { get; set; }
        public double SecondTransitionStart { get; set; }

        public float Altitude_start { get; set; }
        public float Altitude_final { get; set; }    
        
    }
}