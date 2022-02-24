using System;

namespace ALARm.Core
{
    public class ImportListDirTrackID
    {
        public Int64 NewTrackID { get; set; }
        public int OldTrackID { get; set; }
        public Int64 NewDirectionID { get; set; }
        public int OldDirectionID { get; set; }
    }

    public class ImportListStationID
    {
        public string Station { get; set; }
        public string Code { get; set; }
        public Int64 NewStationID { get; set; }
        public int OldStationID { get; set; }
    }

    public class ImportListCurveID
    {
        public Int64 CurveID { get; set; }
        public string OldTrackID { get; set; }
        public string StartKM { get; set; }
        public string StartM { get; set; }
        public string FinalKM { get; set; }
        public string FinalM { get; set; }   
        public string SideID { get; set; }
    }

    public class ImportListStCurve
    {
        public string OldTrackID { get; set; }
        public string StartKM { get; set; }
        public string StartM { get; set; }
        public string FinalKM { get; set; }
        public string FinalM { get; set; }
        public string T1 { get; set; }
        public string T2 { get; set; }
        public string Radius { get; set; }
        public string Wear { get; set; }
        public string Width { get; set; }
    }

    public class ImportListElCurve
    {
        public string OldTrackID { get; set; }
        public string StartKM { get; set; }
        public string StartM { get; set; }
        public string FinalKM { get; set; }
        public string FinalM { get; set; }
        public string Lvl { get; set; }
        public string T1 { get; set; }
        public string T2 { get;set; }
    }

    public class ImportListPDBID
    {
        public string OldTrackID { get; set; }
        public Int64 DistanceID { get; set; }
        public string Distance { get; set; }
        public Int64 PCHUID { get; set; }
        public string PCHU { get; set; }
        public Int64 PDID { get; set; }
        public string PD { get; set; }
        public Int64 PDBID { get; set; }
        public string PDB { get; set; }
        public string FIO { get; set; }
        public int Start { get; set; }
        public int Final { get; set; }
    }
	
    public class ImportListStationSection
    {
        public string OldTrackID { get; set; }
        public string OldStationID { get; set; }
        public int Start_pos { get; set; }
        public int Axis_pos { get; set; }
        public int Final_pos { get; set; }
    }

    public class ImportListId
    {
        public long OldId { get; set; }
        public long NewId { get; set; }
    }
}
