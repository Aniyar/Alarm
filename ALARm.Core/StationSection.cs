namespace ALARm.Core
{
    public class StationSection : MainTrackObject
    {
        public long Nod_Id { get; set; }
        public string Road { get; set; }
        public string Nod { get; set; }
        public long Station_Id { get; set; }
        public string Station { get; set; }
        public string PrevStation { get; set; }
        public string NextStation { get; set; }
        public int Side_Id { get; set; }
        public string Side { get; set; }
        public int Axis_Km { get; set; }
        public int Axis_M { get; set; }
       
        public int Point_Id { get; set; }
        public string Point { get; set; }
        public double RealCoordinate()
        {
            return Axis_Km + Axis_M / 1000.0;
        }
    }
}