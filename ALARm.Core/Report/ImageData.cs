using System;
using System.Collections.Generic;
using System.Text;

namespace ALARm.Core.Report
{
    public class ImageData
    {
        public long FileId { get; set; } = 3287;
        public long Ms { get; set; } = 2620;
        public int Fnum { get; set; } = 1;
        public CarPosition CarPosition { get; set; } = CarPosition.NotDefined;
        public RepType RepType { get; set; } = RepType.Undefined;


        public FrontState State { get; set; } = FrontState.Undefined;
        public int Id { get; set; } = -1;
        public double X { get; set; }
        public double Y { get; set; }
        public double Zazor { get; set; }
    }

    public enum FrontState
    {
        Add = 0,
        Edit = 1,
        Delete = 2,
        Undefined = -1
    }
}
