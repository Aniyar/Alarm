using System;

namespace ALARm.Core
{
    public class Switch : MainTrackObject {
        public Int64 Station_Id { get; set; }
        public string Station { get; set; }
        public Side Side_Id { get; set; }
        public string Side { get; set; }
        public int Point_Id { get; set; }
        public string Point { get; set; }
        public SwitchDirection Dir_Id { get; set; }
        public string Dir { get; set; }
        public int Mark_Id { get; set; }
        public string Mark { get; set; }
        public int Length { get; set; }
        public int Km { get; set; }
        public int Meter { get; set; }
        public string Num { get; set; }
        /// <summary>
        /// координата в виде числа с плавающей запятой,
        /// до запятой номер километра
        /// первая цифра после запятой тысячный метр
        /// вторая цифрв сотый метр
        /// третья цифра десятый метр
        /// четвертая цифра единичный метр
        /// например: 15 км и 10 метр будет 15.0010
        /// </summary>
        /// <returns></returns>
        public double RealCoordinate => Km + Meter / 10000.0;
        

    }
}