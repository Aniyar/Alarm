using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ALARm.Core
{
    public class AppParameters
    {
        public string Direction { get; set; }
        public string TrackNumber { get; set; }
        public int KilometrNumber { get; set; }
        public int Legnth { get; set; }
        public CarParameters Car { get; set; }
        public Direction TravelDirection { get; set; }
        public DateTime TravelDate { get; set; }
        public List<double> ShortProminency { get; set; }
        public List<int> Meters { get; set; }
        public List<int> Meters2 { get; set; }
        public List<double> ShortRight { get; set; }
        public List<double> ShortLeft { get; set; }
        public void Parse(string line)
        {
            line = Regex.Replace(line, @"\s+", " ");
            var parameters = line.Split(new char[] { ' ', '\t' });
            {
                
                Meters.Add((int.Parse(parameters[0]) - 1) * 100 + int.Parse(parameters[1]));
                ShortRight.Add(float.Parse(parameters[2], CultureInfo.InvariantCulture.NumberFormat));
         
            }
        }
        public AppParameters()
        {
            Meters = new List<int>();
            Meters2 = new List<int>();
            ShortRight = new List<double>();
            ShortLeft = new List<double>();
        }

        public void Correct()
        {
           
        }

        public void Parse2(string line)
        {
            line = Regex.Replace(line, @"\s+", " ");
            var parameters = line.Split(new char[] { ' ', '\t' });
            {

                Meters2.Add((int.Parse(parameters[0]) - 1) * 100 + int.Parse(parameters[1]));
                ShortLeft.Add(float.Parse(parameters[2], CultureInfo.InvariantCulture.NumberFormat));

            }
        }
    }
}
