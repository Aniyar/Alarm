using System;
using System.Collections.Generic;
using System.Text;

namespace ALARm.Core
{
    public enum CorrectionType
    {
        None = -1,  //авто прибивка, Игорь счетчик
        Manual = 1, //ручная прибивка
        GPS = 3     //сигнал прибивки gps
    }
}
