namespace ALARm.Core
{
    public class DirectionList
    {
        
    }

    public class DLWidthNorma
    {
        public int Adm_Distance_Code { get; set; }
        public int Start_Km { get; set; }
        public int Start_M { get; set; }
        public int Final_Km { get; set; }
        public int Final_M { get; set; }
        public int Width { get; set; }
        public int Adm_Track_Code { get; set; }
        public string Side { get; set; }
    }

    public class DLElevation
    {
        public int Adm_Distance_Code { get; set; }
        public int Start_Km { get; set; }
        public int Start_M { get; set; }
        public int Final_Km { get; set; }
        public int Final_M { get; set; }
        public int Adm_Track_Code { get; set; }
        public string Level { get; set; }
    }

    public class DLElCurve
    {
        public int Adm_Distance_Code { get; set; }
        public int Start_Km { get; set; }
        public int Start_M { get; set; }
        public int Final_Km { get; set; }
        public int Final_M { get; set; }
        public int Lvl_Start_Km { get; set; }
        public int Lvl_Start_M { get; set; }
        public int Lvl_Final_Km { get; set; }
        public int Lvl_Final_M { get; set; }
        public int Radius { get; set; }
        public int Width { get; set; }
        public int Lvl { get; set; }
        public int Wear { get; set; }
        public int Adm_Track_Code { get; set; }
        public string Side { get; set; }
    }

    public class DLSwitch
    {
        public int Adm_Distance_Code { get; set; }
        public int Start_Km { get; set; }
        public int Start_M { get; set; }
        public int Final_Km { get; set; }
        public int Final_M { get; set; }
        public int Itype { get; set; }
        public int Len { get; set; }
        public int Dir_Id { get; set; }
        public int Side_id { get; set; }
        public int Adm_Track_Code { get; set; }
        public string Num { get; set; }
        //public int Point_Id { get; set; }
        public int Adm_Station_Code { get; set; }
        //public string Angl { get; set; }
        public string Rails_Type { get; set; }
    }

    public class DLNonExtKm
    {
        public int Adm_Distance_Code { get; set; }
        public int Km { get; set; }
        public int Adm_Track_Code { get; set; }
    }

    public class DLNstKm
    {
        public int Adm_Distance_Code { get; set; }
        public int Km { get; set; }
        public int Len { get; set; }
        public int Adm_Track_Code { get; set; }
    }

    public class DLStationSection
    {
        public int Adm_Distance_Code { get; set; }
        public int Start_Km { get; set; }
        public int Start_M { get; set; }
        public int Final_Km { get; set; }
        public int Final_M { get; set; }
        public string Station_Code { get; set; }
        public int Axis_Km { get; set; }
        public int Axis_M { get; set; }
        public string Station_Name { get; set; }
        public string Point { get; set; }
        public int Adm_Track_Code { get; set; }
    }

    public class DLPdbSection
    {
        public int Adm_Distance_Code { get; set; }
        public string Mex { get; set; }
        public string Pd_Code { get; set; }
        public string Code { get; set; }
        public int Start_Km { get; set; }
        public int Final_Km { get; set; }
        //public string nazvn { get; set; }
        //public string nazvk { get; set; }
        public string Distance_Name { get; set; }
        public string Pd_Name { get; set; }
        public int Adm_Track_Code { get; set; }
    }

    public class DLSpeed
    {
        public int Adm_Distance_Code { get; set; }
        public int Start_Km { get; set; }
        public int Start_M { get; set; }
        public int Final_Km { get; set; }
        public int Final_M { get; set; }
        public int Passenger { get; set; }
        public int Freight { get; set; }
        public int Adm_Track_Code { get; set; }
    }

    public class DLArtificialConstruction
    {
        public int Adm_Distance_Code { get; set; }
        public int Start_Km { get; set; }
        public int Start_M { get; set; }
        public int Final_Km { get; set; }
        public int Final_M { get; set; }
        public int Length { get; set; }
        public string Type { get; set; }
        public int Adm_Track_Code { get; set; }
    }

    public class DLCrosstie
    {
        public int Adm_Distance_Code { get; set; }
        public int Start_Km { get; set; }
        public int Start_M { get; set; }
        public int Final_Km { get; set; }
        public int Final_M { get; set; }
        public int Crosstie_Type_Id { get; set; }
        public int Adm_Track_Code { get; set; }
    }
}
