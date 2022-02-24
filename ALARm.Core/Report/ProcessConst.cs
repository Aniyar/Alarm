using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core.Report
{
    public static class ProcessConst
    {
        public static int StrightMark = 1;
        // Коеффиценты поправки для параметров рельсовой колеи
        public static double koefUrob = 1; 
        public static double kfPro =  0.9; 
        public static double kfShab = 1.0; 
        public static double kfUrb = 1.0;
        public static double kfRih = 2;
    }
}
