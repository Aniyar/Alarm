namespace ALARm.Core
{
    /// <summary>
    /// Искуственные соорожения (мост/тоннель)
    /// </summary>
    public class ArtificialConstruction : MainTrackObject
    {
        /// <summary>
        /// идентификатор типа
        /// </summary>
        public int Type_Id { get; set; }
        /// <summary>
        /// наименование типа
        /// </summary>
        public string Type { get; set; }
        public int Km { get; set; }
        public int Meter { get; set; }
        public int Len { get; set; }
        public int AxisKm { get; set; }
        public int AxisM { get; set; }
        public double startcoords { get; set; }
        public double finalcoords { get; set; }
        public int Entrance_Start_km { get; set; }
        public int Entrance_Final_km { get; set; }
        public int Entrance_Start_m { get; set; }
        public int Entrance_Final_m { get; set; }
        public int EntranceLength => Len>=25 && Len<= 100 ? 200 : Len > 100 ? 500 : 0;
    
    }
}