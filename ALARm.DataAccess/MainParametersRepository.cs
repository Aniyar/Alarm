using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
using ALARm.DataAccess.Properties;
using Dapper;
using Npgsql;

namespace ALARm.DataAccess
{
    public class MainParametersRepository : IMainParametersRepository
    {
		public MainParameters DevPlan(int kilometer)
        {
            var result = new MainParameters();
            string line;
            //чтение измерительных данных
            using (var file = new StreamReader(@"E:/Users/admin/Desktop/archive/Новая папка/work_shifrovka/new/km_" + kilometer.ToString() + ".svgpdat", Encoding.GetEncoding(1251)))
            {

                string start = file.ReadLine();
                string final = file.ReadLine();

                line = file.ReadLine();
                result.Car = new CarParameters { ChiefName = line, CurrentPosition = CarPosition.Base  };
                file.ReadLine();
                line = file.ReadLine();
                result.TrackNumber = line != null && line.Length <= 3 ? line : line != null && line.Equals(Resources.even) ? "2" : "1";
                line = file.ReadLine();
                result.TravelDirection = line != null && line.Equals(Resources.reverse) ? Direction.Reverse : Direction.Direct;
                result.Direction = result.TravelDirection == Direction.Direct ? start + "-" + final : final + "-" + start;
                line = file.ReadLine();
                //result.TravelDate = DateTime.ParseExact(line, "dd.mm.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                result.TravelDate = DateTime.Parse(line);
                result.Car.CarNumber = file.ReadLine();
                file.ReadLine();
                result.KilometrNumber = int.Parse(file.ReadLine() ?? throw new InvalidOperationException());
                result.KilometrNumber = kilometer;
                while ((line = file.ReadLine()) != null) result.ParseDevPlan(line);

                if (result.Strl.Count() != 0)
                {
                    result.NerovPlana.Add(result.Strl.Average() - result.Strr.Average());
                    //result.Strl.Clear();
                }
                if (result.Strr.Count() != 0)
                {
                    result.Krivizna.Add((result.Strl.Average() + result.Strr.Average()) / 2);
                    //result.Strr.Clear();
                }
            }
            return result;
        }
        public MainParameters GetMainParametersSKO(int kilometer)
        {
            var result = new MainParameters();
            string line;
            //чтение измерительных данных
            using (var file = new StreamReader("E:/Users/admin/Desktop/archive/Новая папка/work_shifrovka/new/km_" + kilometer.ToString() + ".svgpdat", Encoding.GetEncoding(1251)))
            {
                string start = file.ReadLine();
                string final = file.ReadLine();

                line = file.ReadLine();
                result.Car = new CarParameters { ChiefName = line, CurrentPosition = CarPosition.Base };
                file.ReadLine();
                line = file.ReadLine();
                result.TrackNumber = line != null && line.Length <= 3 ? line : line != null && line.Equals(Resources.even) ? "2" : "1";
                line = file.ReadLine();
                result.TravelDirection = line != null && line.Equals(Resources.reverse) ? Direction.Reverse : Direction.Direct;
                result.Direction = result.TravelDirection == Direction.Direct ? start + "-" + final : final + "-" + start;
                line = file.ReadLine();
                //result.TravelDate = DateTime.ParseExact(line, "dd.mm.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                result.TravelDate = DateTime.Parse(line);
                result.Car.CarNumber = file.ReadLine();
                file.ReadLine();
                result.KilometrNumber = int.Parse(file.ReadLine() ?? throw new InvalidOperationException());
                result.KilometrNumber = kilometer;
                while ((line = file.ReadLine()) != null) result.ParseSKO(line);

                if (result.CCCPspeedBYpiketGARB.Count() != 0)
                {
                    result.CCCPspeedBYpiket.Add(result.CCCPspeedBYpiketGARB.Average());
                    result.CCCPspeedBYpiketGARB.Clear();
                }

                if (result.Strl.Count() != 0)
                {
                    result.StraighteningLeft.Add(result.Strl.Average());
                    result.Strl.Clear();
                }
                if (result.Strr.Count() != 0)
                {
                    result.StraighteningRigth.Add(result.Strr.Average());
                    result.Strr.Clear();
                }
                if (result.LVL.Count() != 0)
                {
                    result.LevelNew.Add(result.LVL.Average());
                    result.LVL.Clear();
                }
                if (result.DrL.Count() != 0)
                {
                    result.DrawdownLeft.Add(result.DrL.Average());
                    result.DrL.Clear();
                }
                if (result.DrR.Count() != 0)
                {
                    result.DrawdoownRigth.Add(result.DrR.Average());
                    result.DrR.Clear();
                }
                if (result.TrW.Count() != 0)
                {
                    result.TrackWidth.Add(result.TrW.Average());
                    result.TrW.Clear();
                }
            }


            //чтение паспортных данных
            foreach (var unused in result.Meters)
            {
                double straighteningZero = 0;
                double levelZero = 0;
                result.GaugeNorm.Add(1520);
                result.ZeroGauge.Add(1520);
                result.ZeroStraightening.Add(straighteningZero);
                result.ZeroLevel.Add(levelZero);
                result.StraighteningSide.Add(1);
            }

            var mainTrackRepository = new MainTrackStructureRepository();
            result.Curves = mainTrackRepository.GetMtoObjectsByCoord(result.TravelDate, result.KilometrNumber,
                MainTrackStructureConst.MtoCurve, result.Direction, result.TrackNumber) as List<Curve>;
            result.CrossTies = mainTrackRepository.GetMtoObjectsByCoord(result.TravelDate, result.KilometrNumber,
                MainTrackStructureConst.MtoCrossTie, result.Direction, result.TrackNumber) as List<CrossTie>;
            result.ArtificialConstructions = mainTrackRepository.GetMtoObjectsByCoord(result.TravelDate,
                result.KilometrNumber, MainTrackStructureConst.MtoArtificialConstruction, result.Direction,
                result.TrackNumber) as List<ArtificialConstruction>;
            var prevKm = mainTrackRepository.GetMtoObjectsByCoord(result.TravelDate, result.KilometrNumber - 1,
                MainTrackStructureConst.MtoNonStandard, result.Direction, result.TrackNumber) as List<NonstandardKm>;
            var currentKm = mainTrackRepository.GetMtoObjectsByCoord(result.TravelDate, result.KilometrNumber,
                MainTrackStructureConst.MtoNonStandard, result.Direction, result.TrackNumber) as List<NonstandardKm>;

            var straighteningThreads = mainTrackRepository.GetMtoObjectsByCoord(result.TravelDate,
                result.KilometrNumber, MainTrackStructureConst.MtoStraighteningThread, result.Direction,
                result.TrackNumber) as List<StraighteningThread>;


            //рихтовочная нить
            foreach (var thread in straighteningThreads)
            {
                var x1 = 0;
                var x2 = 1000;
                if (thread.Start_Km == result.KilometrNumber)
                    x1 = thread.Start_M;
                if (thread.Final_Km == result.KilometrNumber)
                    x2 = thread.Final_M;

                for (var meter = x1; meter <= x2; meter++)
                {
                    var index = result.Meters.IndexOf(meter);
                    result.StraighteningSide[index] = thread.Side_Id * (int)result.TravelDirection;
                }
            }
            return result;
        }
        public MainParameters GetMainParametersSKOvedom(int kilometer)
        {
            var result = new MainParameters();
            string line;
            //чтение измерительных данных
            using (var file = new StreamReader("D:/work_shifrovka/new/km_" + kilometer.ToString() + ".svgpdat", Encoding.GetEncoding(1251)))
            {
                string start = file.ReadLine();
                string final = file.ReadLine();

                line = file.ReadLine();
                result.Car = new CarParameters { ChiefName = line, CurrentPosition = CarPosition.Base };
                file.ReadLine();
                line = file.ReadLine();
                result.TrackNumber = line != null && line.Length <= 3 ? line : line != null && line.Equals(Resources.even) ? "2" : "1";
                line = file.ReadLine();
                result.TravelDirection = line != null && line.Equals(Resources.reverse) ? Direction.Reverse : Direction.Direct;
                result.Direction = result.TravelDirection == Direction.Direct ? start + "-" + final : final + "-" + start;
                line = file.ReadLine();
                //result.TravelDate = DateTime.ParseExact(line, "dd.mm.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                result.TravelDate = DateTime.Parse(line);
                result.Car.CarNumber = file.ReadLine();
                file.ReadLine();
                result.KilometrNumber = int.Parse(file.ReadLine() ?? throw new InvalidOperationException());
                result.KilometrNumber = kilometer;
                while ((line = file.ReadLine()) != null) result.ParseSKOvedomost(line);

            }


            //чтение паспортных данных
            foreach (var unused in result.Meters)
            {
                double straighteningZero = 0;
                double levelZero = 0;
                result.GaugeNorm.Add(1520);
                result.ZeroGauge.Add(1520);
                result.ZeroStraightening.Add(straighteningZero);
                result.ZeroLevel.Add(levelZero);
                result.StraighteningSide.Add(1);
            }

            var mainTrackRepository = new MainTrackStructureRepository();
            result.Curves = mainTrackRepository.GetMtoObjectsByCoord(result.TravelDate, result.KilometrNumber,
                MainTrackStructureConst.MtoCurve, result.Direction, result.TrackNumber) as List<Curve>;
            result.CrossTies = mainTrackRepository.GetMtoObjectsByCoord(result.TravelDate, result.KilometrNumber,
                MainTrackStructureConst.MtoCrossTie, result.Direction, result.TrackNumber) as List<CrossTie>;
            result.ArtificialConstructions = mainTrackRepository.GetMtoObjectsByCoord(result.TravelDate,
                result.KilometrNumber, MainTrackStructureConst.MtoArtificialConstruction, result.Direction,
                result.TrackNumber) as List<ArtificialConstruction>;
            var prevKm = mainTrackRepository.GetMtoObjectsByCoord(result.TravelDate, result.KilometrNumber - 1,
                MainTrackStructureConst.MtoNonStandard, result.Direction, result.TrackNumber) as List<NonstandardKm>;
            var currentKm = mainTrackRepository.GetMtoObjectsByCoord(result.TravelDate, result.KilometrNumber,
                MainTrackStructureConst.MtoNonStandard, result.Direction, result.TrackNumber) as List<NonstandardKm>;

            var straighteningThreads = mainTrackRepository.GetMtoObjectsByCoord(result.TravelDate,
                result.KilometrNumber, MainTrackStructureConst.MtoStraighteningThread, result.Direction,
                result.TrackNumber) as List<StraighteningThread>;


            //рихтовочная нить
            foreach (var thread in straighteningThreads)
            {
                var x1 = 0;
                var x2 = 1000;
                if (thread.Start_Km == result.KilometrNumber)
                    x1 = thread.Start_M;
                if (thread.Final_Km == result.KilometrNumber)
                    x2 = thread.Final_M;

                for (var meter = x1; meter <= x2; meter++)
                {
                    var index = result.Meters.IndexOf(meter);
                    result.StraighteningSide[index] = thread.Side_Id * (int)result.TravelDirection;
                }
            }
            return result;
        }
        public ShortRoughness GetAppParameters(int km)
        {
            var result = new ShortRoughness();
            string line;
            //чтение измерительных данных
            using (var file = new StreamReader(km.ToString() + ".svgpdat", Encoding.GetEncoding(1251)))
            {
                line = file.ReadLine();
                result.Direction = "Обратный";
                result.Car = new CarParameters { ChiefName = "CheffName", CurrentPosition = CarPosition.Boiler };
                //line = file.ReadLine();
                result.TrackNumber = line != null && line.Length <= 3 ? line : line != null && line.Equals(Resources.even) ? "2" : "1";
                result.TravelDirection = line != null && line.Equals(Resources.reverse) ? Direction.Direct : Direction.Reverse;
                //line = file.ReadLine();
                //result.TravelDate = DateTime.ParseExact(line, "dd.mm.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                result.TravelDate = DateTime.Now;
                result.Car.CarNumber = "0001";
                result.KilometrNumber = int.Parse(line ?? throw new InvalidOperationException());
                while ((line = file.ReadLine()) != null) result.Parse(line);
            }
            using (var file = new StreamReader(km.ToString() + "_2.svgpdat", Encoding.GetEncoding(1251)))
            {
                line = file.ReadLine();
                result.Direction = "Обратный";
                result.Car = new CarParameters { ChiefName = "CheffName", CurrentPosition = CarPosition.Boiler };
                //line = file.ReadLine();
                result.TrackNumber = line != null && line.Length <= 3 ? line : line != null && line.Equals(Resources.even) ? "2" : "1";
                result.TravelDirection = line != null && line.Equals(Resources.reverse) ? Direction.Direct : Direction.Reverse;
                //line = file.ReadLine();
                //result.TravelDate = DateTime.ParseExact(line, "dd.mm.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                result.TravelDate = DateTime.Now;
                result.Car.CarNumber = "0001";
                result.KilometrNumber = int.Parse(line ?? throw new InvalidOperationException());
                while ((line = file.ReadLine()) != null) result.Parse2(line);
            }

            result.Correct();


            return result;
        }

        public MainParameters GetMainParameters(int kilometer)
        {
            var result = new MainParameters();
            string line;
            //чтение измерительных данных
            using (var file = new StreamReader("D:/work_shifrovka/km_" + kilometer.ToString() + ".svgpdat", Encoding.GetEncoding(1251)))
            {
                string start = file.ReadLine();
                string final = file.ReadLine();
                
                line = file.ReadLine();
                result.Car = new CarParameters {ChiefName = line, CurrentPosition = CarPosition.Boiler};
                file.ReadLine();
                line = file.ReadLine();
                result.TrackNumber = line != null && line.Length <= 3 ? line : line != null && line.Equals(Resources.even) ? "2" : "1";
                line = file.ReadLine();
                result.TravelDirection = line != null && line.Equals(Resources.reverse) ? Direction.Direct : Direction.Reverse;
                result.Direction = result.TravelDirection == Direction.Reverse ? start + "-" + final : final + "-" + start;
                line = file.ReadLine();
                //result.TravelDate = DateTime.ParseExact(line, "dd.mm.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                result.TravelDate = DateTime.Parse(line);
                result.Car.CarNumber = file.ReadLine();
                file.ReadLine();
                result.KilometrNumber = int.Parse(file.ReadLine() ?? throw new InvalidOperationException());
                result.KilometrNumber = kilometer;
                while ((line = file.ReadLine()) != null) result.Parse(line);
            }

            //чтение паспортных данных
            foreach (var unused in result.Meters)
            {
                double straighteningZero = 0;
                double levelZero = 0;
                result.GaugeNorm.Add(1520);
                result.ZeroGauge.Add(1520);
                result.ZeroStraightening.Add(straighteningZero);
                result.ZeroLevel.Add(levelZero);
                result.StraighteningSide.Add(1);
            }

            var mainTrackRepository = new MainTrackStructureRepository();
            result.Curves = mainTrackRepository.GetMtoObjectsByCoord(result.TravelDate, result.KilometrNumber,
                MainTrackStructureConst.MtoCurve, result.Direction, result.TrackNumber) as List<Curve>;
            result.CrossTies = mainTrackRepository.GetMtoObjectsByCoord(result.TravelDate, result.KilometrNumber,
                MainTrackStructureConst.MtoCrossTie, result.Direction, result.TrackNumber) as List<CrossTie>;
            result.ArtificialConstructions = mainTrackRepository.GetMtoObjectsByCoord(result.TravelDate,
                result.KilometrNumber, MainTrackStructureConst.MtoArtificialConstruction, result.Direction,
                result.TrackNumber) as List<ArtificialConstruction>;
            var prevKm = mainTrackRepository.GetMtoObjectsByCoord(result.TravelDate, result.KilometrNumber - 1,
                MainTrackStructureConst.MtoNonStandard, result.Direction, result.TrackNumber) as List<NonstandardKm>;
            var currentKm = mainTrackRepository.GetMtoObjectsByCoord(result.TravelDate, result.KilometrNumber,
                MainTrackStructureConst.MtoNonStandard, result.Direction, result.TrackNumber) as List<NonstandardKm>;

            var straighteningThreads = mainTrackRepository.GetMtoObjectsByCoord(result.TravelDate,
                result.KilometrNumber, MainTrackStructureConst.MtoStraighteningThread, result.Direction,
                result.TrackNumber) as List<StraighteningThread>;


            //рихтовочная нить
            foreach (var thread in straighteningThreads)
            {
                var x1 = 0;
                var x2 = 1000;
                if (thread.Start_Km == result.KilometrNumber)
                    x1 = thread.Start_M;
                if (thread.Final_Km == result.KilometrNumber)
                    x2 = thread.Final_M;

                for (var meter = x1; meter <= x2; meter++)
                {
                    var index = result.Meters.IndexOf(meter);
                    result.StraighteningSide[index] = thread.Side_Id * (int) result.TravelDirection;
                }
            }

            return result;
        }

        /// <summary>
        ///     екі нүкте арқылы түзудің бойындағы x-нүктесіндегі y-тің мәнін табу
        /// </summary>
        /// <param name="x">y-тің мәні ізделіп отырған нүкте</param>
        /// <param name="x1">бірінші нүктенің x бойынша координатасы</param>
        /// <param name="x2">екінші нүктенің x бойынша координатасы</param>
        /// <param name="y1">бірінші нүктенің y бойынша координатасы</param>
        /// <param name="y2">екінші нүктенің y бойынша координатасы</param>
        /// <returns></returns>
        public double GetY(double x, double x1, double x2, double y1, double y2)
        {
            return (x - x1) / (x2 - x1) * (y2 - y1) + y1;
        }

        public List<Sssp> GetMainParametersSKOvedomDB(int kilometer, long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    return db.Query<Sssp>($@"SELECT
	                                            meter,
	                                            skewness_SKO,
	                                            drawdown_left_SKO,
	                                            drawdown_right_SKO,
	                                            level_sko as gauge,
	                                            stright_left,
	                                            stright_right,
	                                            SSSP_speed,
	                                            speed,
	                                            level_sko,
	                                            ( skewness_SKO / 2.0 + drawdown_left_SKO / 2.1 + drawdown_right_SKO / 2.1 + level_sko / 3.0 ) / 4.0 AS cv,
	                                            skewness_SKO AS cr,--sko надо поменять на ско рихтовки на 100м
	                                            ( skewness_SKO / 2.0 + drawdown_left_SKO / 2.1 + drawdown_right_SKO / 2.1 + level_sko / 3.0 ) / 4.0 + skewness_SKO AS co                                    
                                            FROM 
	                                            outdata_{ trip_id }
                                            where
	                                            km = { kilometer }
                                            ORDER BY 
	                                            km, meter ASC  ").ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetMainParametersSKOvedomDB error: " + e.Message);
                    return null;
                }
            }
        }
        public List<Sssp> GetMainParametersSkoFP52(int kilometer, long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    return db.Query<Sssp>($@"SELECT
	                                            km,
	                                            (meter-10)/100+1 piket,
	                                            avg(gauge) gauge,
	                                            avg(stright_avg) stright_avg, 	
	                                            avg(level) lvl, 	
	                                            avg(level) gauge_SKO, 
	                                            avg(skewness_SKO) skewness_SKO,
	                                            avg(drawdown_left_SKO) drawdown_left_SKO,
	                                            avg(drawdown_right_SKO) drawdown_right_SKO,
	                                            avg(stright_right) stright_right,
	                                            avg(SSSP_speed) sssp_vert,
	                                            avg(speed) sssp_gor
                                            FROM 
	                                            outdata_{ trip_id }
                                            where
	                                            km = { kilometer }
                                            group by 
                                                km, piket
                                            order by 
                                                km, piket").ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetMainParametersSkoFP52 error: " + e.Message);
                    return null;
                }
            }
        }

        public List<Sssp> GetMainParametersFromDB(long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    //var query = $@" SELECT
	                   //                 km,
	                   //                 AVG ( gauge ) gauge_avg,
	                   //                 stddev( gauge ) gauge_sko,
	                   //                 AVG ( LEVEL - level_avg ) level_avg,
	                   //                 stddev( LEVEL - level_avg ) level_sko,
	                   //                 AVG ( stright_left - stright_avg ) stright_left_avg,
	                   //                 stddev( stright_left - stright_avg ) stright_left_sko,
	                   //                 AVG ( stright_right - stright_avg ) stright_right_avg,
	                   //                 stddev( stright_right - stright_avg ) stright_right_sko,
	                   //                 AVG ( drawdown_left ) drawdown_left_avg,
	                   //                 stddev( drawdown_left ) drawdown_left_sko,
	                   //                 AVG ( drawdown_right ) drawdown_right_avg,
	                   //                 stddev( drawdown_right ) drawdown_right_sko,
	                   //                 AVG ( drawdown_right ) drawdown_right_avg,
	                   //                 stddev( drawdown_right ) drawdown_right_sko,
	                   //                 AVG ( skewness_PXI ) skewness_avg,
	                   //                 stddev( skewness_PXI ) skewness_sko,
	                   //                 AVG ( SSSP_speed ) sssp_speed,
	                   //                 AVG ( SSSP_speed ) sssp_vert,
	                   //                 AVG ( speed ) sssp_gor,
	                   //                 (
		                  //                  stddev( skewness_PXI ) / 2.0 + stddev( drawdown_left ) / 2.1 + stddev( drawdown_right ) / 2.1 + stddev( LEVEL ) / 3.0 
	                   //                 ) / 4.0 AS cv,
	                   //                 ( stddev( stright_left ) + stddev( stright_right ) ) / 2.0 AS cr,
	                   //                 (
		                  //                  stddev( skewness_PXI ) / 2.0 + stddev( drawdown_left ) / 2.1 + stddev( drawdown_right ) / 2.1 + stddev( LEVEL ) / 3.0 
	                   //                 ) / 4.0 + ( stddev( stright_left ) + stddev( stright_right ) ) / 2.0 AS co 
                    //                FROM
	                   //                 outdata_{trip_id}
                    //                GROUP BY
	                   //                 km
                    //                ORDER BY
	                   //                 km ";
                    var query = $@" SELECT
	                                    km,
                                         avg (meter) meter, 
	                                    AVG ( gauge ) gauge_avg,
	                                    stddev( gauge ) gauge_sko,
	                                    AVG ( LEVEL - level_avg ) level_avg,
	                                    stddev( LEVEL - level_avg ) level_sko,
	                                    AVG ( stright_left - stright_avg ) stright_left_avg,
	                                    stddev( stright_left - stright_avg ) stright_left_sko,
	                                    AVG ( stright_right - stright_avg ) stright_right_avg,
	                                    stddev( stright_right - stright_avg ) stright_right_sko,
	                                    AVG ( drawdown_left ) drawdown_left_avg,
	                                    stddev( drawdown_left ) drawdown_left_sko,
	                                    AVG ( drawdown_right ) drawdown_right_avg,
	                                    stddev( drawdown_right ) drawdown_right_sko,
	                                    AVG ( drawdown_right ) drawdown_right_avg,
	                                    stddev( drawdown_right ) drawdown_right_sko,
	                                    AVG ( skewness_PXI ) skewness_avg,
	                                    stddev( skewness_PXI ) skewness_sko,
	                                    AVG ( SSSP_speed ) sssp_speed,
	                                    AVG ( SSSP_speed ) sssp_vert,
	                                    AVG ( speed ) sssp_gor,
	                                    	( stddev( skewness_PXI ) / 2.0 + stddev( drawdown_left ) / 2.1 + stddev( drawdown_right ) / 2.1 ) / 3.0 AS cv,
	                                        ( stddev( stright_left- stright_avg ) + stddev( stright_right- stright_avg ) ) / 2.0 AS cr,
	                                        ( stddev( skewness_PXI ) / 2.0 + stddev( drawdown_left ) / 2.1 + stddev( drawdown_right ) / 2.1 ) / 3.0
	                                        + ( stddev( stright_left- stright_avg ) + stddev( stright_right- stright_avg ) ) / 2.0 AS co 
                                    FROM
	                                    outdata_{trip_id}
                                    GROUP BY
	                                    km 
                                    ORDER BY
	                                    km   ";

                    return db.Query<Sssp>(query).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetMainParametersFromDB error: " + e.Message);
                    return null;
                }
            }
        }
        public List<Sssp> GetMainParametersFromDBMeter(long trip_id)
        {
            using (IDbConnection db = new NpgsqlConnection(Helper.ConnectionString()))
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();
                try
                {
                    var query = $@" SELECT
	                                    km,
	                                    meter,
	                                    stright_left-stright_avg ner_l,
	                                    stright_right-stright_avg ner_r,
	                                    LEVEL lvl,
	                                    level_avg lvl_avg,
	                                    level_sko lvl0,
	                                    stright_left,
	                                    stright_right,
	                                    stright_avg,
	                                    gauge,
	                                    drawdown_left,
	                                    drawdown_right
                                    FROM
	                                    outdata_{trip_id} 
                                    ORDER BY
	                                    km,
	                                    meter ";

                    return db.Query<Sssp>(query).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetMainParametersFromDB error: " + e.Message);
                    return null;
                }
            }
        }
    }
}