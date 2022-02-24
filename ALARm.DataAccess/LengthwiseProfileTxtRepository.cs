using ALARm.Core;
using ALARm.DataAccess.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ALARm.DataAccess
{
    public class LengthwiseProfileTxtRepository : ILengthwiseProfileRepository
    {
        public List<LengthwiseProfile> GetLengthwiseProfile(string filename)
        {
            var result = new List<LengthwiseProfile>();
            string line;
            //чтение измерительных данных
            using (var file = new StreamReader(filename, Encoding.GetEncoding(1251)))
            {
                var lengthwiseProfile = new LengthwiseProfile();
                line = file.ReadLine().Replace(@"\s+", "");
                line += "-" + file.ReadLine().Replace(@"\s+", "");
                lengthwiseProfile.Direction = line;
                line = file.ReadLine().Replace(@"\s+", "");
                lengthwiseProfile.Car = new CarParameters { ChiefName = line, CurrentPosition = CarPosition.Boiler };
                lengthwiseProfile.KilometrNumber = int.Parse(file.ReadLine() ?? throw new InvalidOperationException());
                line = file.ReadLine().Replace(@"\s+", "");
                lengthwiseProfile.TrackNumber = line != null && line.Length <= 3 ? line : line != null && line.Equals(Resources.even) ? "2" : "1";
                line = file.ReadLine().Replace(@"\s+", "");
                lengthwiseProfile.TravelDirection = line != null && line.Equals(Resources.reverse) ? Direction.Direct : Direction.Reverse;
                line = file.ReadLine().Replace(@"\s+", "");
                lengthwiseProfile.TravelDate = DateTime.Parse(line);
                lengthwiseProfile.Car.CarNumber = file.ReadLine().Replace(@"\s+", "");
                file.ReadLine();
                
                while ((line = file.ReadLine()) != null) { 
                    lengthwiseProfile.Parse(line);
                    if (Math.Abs(lengthwiseProfile.Kms[0] - lengthwiseProfile.Kms[lengthwiseProfile.Kms.Count - 1]) == 10)
                    {
                        result.Add(lengthwiseProfile);
                        lengthwiseProfile = new LengthwiseProfile();
                        lengthwiseProfile.Direction = result[0].Direction;
                        lengthwiseProfile.Car = result[0].Car;
                        lengthwiseProfile.TrackNumber = result[0].TrackNumber;
                        lengthwiseProfile.TravelDirection = result[0].TravelDirection;
                        lengthwiseProfile.TravelDate = result[0].TravelDate;
                        lengthwiseProfile.Car.CarNumber = result[0].Car.CarNumber;

                    }
                }
                

            }

             return result;

        }
    }
}
