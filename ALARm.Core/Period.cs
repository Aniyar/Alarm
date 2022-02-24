using System;

namespace ALARm.Core
{
    public class Period
    {
        public Int64 Id { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime Final_Date { get; set; }
        public DateTime Change_Date { get; set; }
        public int Mto_Type { get; set; }
        public long Track_Id { get; set; }
    }
}