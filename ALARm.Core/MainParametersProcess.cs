using ALARm.Core.Report;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core
{
    public class MainParametersProcess
    {
        public Int64 Id { get; set; }
        public DateTime Date_Vrem { get; set; }
        public long Trip_id { get; set; }

        public int Process_Type { get; set; }

        public string Chief { get; set; }
        public string Car { get; set; }
        public string Trip_date  { get; set; }

        public Direction Direction { get; set; }
        public Direction Travel_Direction { get; set; }
     
        public CarPosition Car_Position { get; set; }
       
        public CarPosition CarPosition { get; set; }
        public string DistanceName { get; set; }
        public long DirectionID { get; set; }
        public string DirectionName { get; set; }
        public string DirectionCode { get; set; }
        public string TrackName { get; set; }
        public Int64 TrackID { get; set; }

        public TripType Trip_Type { get; set; }
        public string GetProcessTypeName
        {
            get
            {
                switch (Process_Type)
                {
                    //case 0: return "Рабочая";
                    //case 0: return "Контрольная 1";
                    case 1: return "Контрольная";
                    case 2: return "Дополнительная";
                    case 3: return "Колибровочная";
                    case 4: return "Доппараметры";
                    default: return string.Empty;
                }
            }
        }
    }
}
