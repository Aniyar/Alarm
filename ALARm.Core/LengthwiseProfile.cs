using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ALARm.Core
{
    public class LengthwiseProfile
    {
        public string Direction { get; set; }
        public string TrackNumber { get; set; }
        public int KilometrNumber { get; set; }
        public int Legnth { get; set; }
        public CarParameters Car { get; set; }
        public Direction TravelDirection { get; set; }
        public DateTime TravelDate { get; set; }
        public List<int> Kms { get; set; }
        public List<int> Pickets { get; set; }
        public List<double> Altitudes { get; set; }
        public List<double> Skews { get; set; }
        public List<double> SkewsL { get; set; }

        public void Parse(string line)
        {
            line = Regex.Replace(line, @"\s+", " ");
            var parameters = line.Split(new char[] { ' ', '\t' });
            {
                double altitude = 0;
                double skew = 0;
                double skewl = 0;
                if (double.TryParse(parameters[3], out altitude) &&  double.TryParse(parameters[4], out skew) && double.TryParse(parameters[5], out skewl))
                { 
                    Kms.Add(int.Parse(parameters[0]));
                    Pickets.Add(int.Parse(parameters[1]));
                    Altitudes.Add(altitude);
                    Skews.Add(skew);
                    SkewsL.Add(skewl);
                }
            }
        }

        public LengthwiseProfile()
        {
            Kms = new List<int>();
            Pickets = new List<int>();
            Altitudes = new List<double>();
            Skews = new List<double>();
            SkewsL = new List<double>();
        }

    }
}
