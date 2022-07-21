using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core.Report
{
    public class Digression : RdObject
    {
        //public int PassengerSpeedLimit { get; set; }
        //public int FreightSpeedLimit { get; set; }
        public long Trip_id { get; set; }
        public int impulses { get; set; }
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }
        public int Hight { get; set; }

        public string Tripplan { get; set; }
        public string Vpz { get; set; }
        public int Total_NPK_l { get; set; }
        public int Total_b_fastener { get; set; }

        public int Total_shpala { get; set; }

        public int Total_gaps { get; set; }
        public string Joint { get; set; }
        public string Sleeper { get; set; }
        public string Pchu { get; set; }
        public string Meropr { get; set; }


        public string Vdop { get; set; }
        //  public string PdbSection { get; set; }

        //public string Station { get; set; }
        public int Km { get; set; }
        public int Meter { get; set; }
        public string Otst { get; set; }
        public string Threat_id { get; set; }
        public string Fastener { get; set; }
        //   public string Notice { get; set; }
        //
        //aniyar
        public string Date { get; set; }
        public string EditReason { get; set; }
        public string Editor { get; set; }
        public string Put { get; set; }
        public int Direction_num { get; set; }
        //


        public int Next_koord { get; set; }
        public string Next_note { get; set; }
        public int Razn { get; set; }
        public int Next_oid { get; set; }
        public int Koord { get; set; }
        public string Note { get; set; }
        public string Station { get; set; }
        public string Speed { get; set; }
        public string Overlay { get; set; }
        public string Fastening { get; set; }
        public string Before { get; set; }
        public string After { get; set; }
        public string Speed2 { get; set; }
        public string Notice { get; set; }
        public int Fileid { get; set; }
        public int Fnum { get; set; }
        public int Mtr { get; set; }
        public int Oid { get; set; }
        public int R_meter { get; set; }
        public int R_y { get; set; }
        public int R_oid { get; set; }
        public int R_km { get; set; }
        public int Local_fnum { get; set; }
        public int R_local_fnum { get; set; }
        public int Mm { get; set; }
        public string Ots { get; set; }
        public int Otkl { get; set; }
        public int Len { get; set; }
        public int Velich { get; set; }
        public string Kol { get; set; }
        public string Direction { get; set; }
        public string Track { get; set; }
        public string PCHU { get; set; }
        public string PD { get; set; }
        public string PDB { get; set; }
        public DateTime FoundDate { get; set; }
        public string Name { get; set; }
        public float Value { get; set; }
        public int Length { get; set; } 
        
        public string dig_rim { get; set; }
        
        public int Count { get; set; } = 1;
        public object Typ { get; set; }
        public string FullSpeed { get; set; }
        public string R_FullSpeed { get; set; }
        public string AllowSpeed { get; set; } = "";
        public string R_AllowSpeed { get; set; }

        public string Norma { get; set; } = "1520";
        public string Primech { get; set; } = "";
        public Int64 Process_id { get; set; }
        public string Pch { get; set; }
        public int AvgBall { get; set; }
        public string PdCount { get; set; }
        public int Kmetr { get; set; }
        public float Lkm { get; set; }

        public string prim { get; set; } = "";
        
        public DigName DigName { get; set; } = DigressionName.Undefined;
        public DigName R_DigName { get; set; } = DigressionName.Undefined;
        public Threat Threat { get; set; }
        public Threat R_threat { get; set; }
        public string StationName { get; set; }
        public string BraceType { get; set; }
        public int CurveRadius { get; set; }
        public string ThreadSide { get; set; }
        public string Description { get; set; }
        public string RailType { get; set; }
        public string TrackClass { get; set; }
        public long File_id { get; set; }
        public string File_name { get; set; }
        public long R_file_id { get; set; }
        public double Angle { get; set; }
        public double Intensity_ra { get; set; }

        public double Left_data { get; set; }
        public double Right_data { get; set; }

        public int R_ms { get; set; }
        public int R_fnum { get; set; }

        public override string ToString()
        {
            return $"{Km}, {Meter}, {Oid}";
        }
        public string GetName()
        {
            return DigName.Name;
        }
        public Location Location { get; set; }
        public int Cn { get; set; }
        public int file_id { get; set; }

        public int PassSpeed { get; set; }
        public int FreightSpeed { get; set; }
        public int Zazor { get; set; }
        public int Zazor_L { get; set; }
        public int Zazor_R { get; set; }
       
           
        public DigressionType DigressionType { get; set; } = DigressionType.Additional;
        public string Pdb_section { get; set; }
        public string Fragment { get; set; }
        public string temp { get; set; }
        public int PassengerSpeedLimit { get; set; }
        public int FreightSpeedLimit { get; set; }

        public void GetDigressions()
        {
            FullSpeed = PassSpeed + "/" + FreightSpeed;
            switch (Zazor)
            {
                case 0:
                    AllowSpeed = "";
                    DigName = DigressionName.FusingGap;
                    break;
                case int gap when gap > 24 && gap <= 26:
                    AllowSpeed = (PassSpeed > 100 ? "100" : "-") + "/" + (FreightSpeed > 100 ? "100" : "-");
                    DigName = AllowSpeed.Equals("-/-") ? DigressionName.AnomalisticGap : DigressionName.Gap;
                    break;
                case int gap when gap > 26 && gap <= 30:
                    AllowSpeed = (PassSpeed > 60 ? "60" : "-") + "/" + (FreightSpeed > 60 ? "60" : "-");
                    DigName = AllowSpeed.Equals("-/-") ? DigressionName.AnomalisticGap : DigressionName.Gap;
                    break;
                case int gap when gap > 30 && gap <= 35:
                    AllowSpeed = (PassSpeed > 25 ? "25" : "-") + "/" + (FreightSpeed > 25 ? "25" : "-");
                    DigName = AllowSpeed.Equals("-/-") ? DigressionName.AnomalisticGap : DigressionName.Gap;
                    break;
                case int gap when gap > 35:
                    AllowSpeed = "0/0";
                    DigName = DigressionName.Gap;
                    break;
            }
        }
        public void GetDigressions436()
        {
            FullSpeed = PassSpeed + "/" + FreightSpeed;
            Zazor = (int)(Zazor * Helper.GetGapKoef());
      

            if (61 <= PassSpeed && PassSpeed <= 100)
            {
                
                switch (Zazor)
                {

                    case var value when 25 <= value && value <= 30 :
                        Kmetr = Kmetr;
                        AllowSpeed = "";
                        DigName = DigressionName.GapSimbol;
                        break;
                    case var value when 30 < value && value <= 35:
                        AllowSpeed = "25/25";
                        DigName = DigressionName.Gap;
                        break;
                    case var value when 35 < value:
                        AllowSpeed = "0/0";
                        DigName = DigressionName.Gap;
                        break;
                    case var value when value == 0:
                        DigName = DigressionName.FusingGap;
                        break;
                }
            }
            else if (101 <= PassSpeed && PassSpeed <= 200)
            {
                switch (Zazor)
                {
                    case var value when 27 <= value && value <= 30:
                        AllowSpeed = "";
                        DigName = DigressionName.GapSimbol;
                        break;
                    case var value when 30 < value && value <= 35:
                        AllowSpeed = "25/25";
                        DigName = DigressionName.Gap;
                        break;
                    case var value when 35 < value:
                        AllowSpeed = "0/0";
                        DigName = DigressionName.Gap;
                        break;
                }
            }
        }


    }


    public class DigressionMark
    {
        public DigName Digression { get; set; } = DigressionName.Undefined;
        public int ID { get; set; }
        public int Id { get; set; }
        public int lvl { get; set; }
        public int Meter { get; set; }
        public int Km { get; set; }
        public int Degree { get; set; }
        public int Length { get; set; }
        public string Prim { get; set; }
        public string NewBedemostComment { get; set; }
        public double Radius { get; set; } = 10000;
        public CrosTieType CrosTieType { get; set; } = CrosTieType.Before96;
        public float Dlina { get; set; }
        public float Value { get; set; }
        public int Count { get; set; }
        public string DigName { get; set; }
        public string LimitSpeed { get; set; }
        public bool Is2to3 { get; set; }
        public bool IsEqualTo4 { get; set; }
        public bool IsEqualTo3 { get; set; }
        public int IsAdditional { get; set; }
        public long FileId { get; set; }
        public long Ms { get; set; }
        public int FNum { get; set; }
        public CarPosition CarPosition { get; set; } = CarPosition.NotDefined;
        public RepType RepType { get; set; } = RepType.Undefined;
        public Dictionary<String, Object> DigressionImage { get; set; }
        public string DigImage { get; set; }
        public string DirectionName { get; set; }
        public string TrackName { get; set; }

        public string primech { get; set; }
        public bool IsLong { get; set; }
        public int PassengerSpeedLimit { get; set; }
        public int FreightSpeedLimit { get; set; }
        public int PassengerSpeedAllow { get; set; }
        public int FreightSpeedAllow { get; set; }
        public long TrackId { get; set; }
        public long TripId { get; set; }
        public string Comment { get; set; }
        public bool gotprim { get; set; } = false;
        public string EditReason { get; set; }
        public string Editor { get; set; }
        public string CNI { get; set; } = "";

        public string GetDigName(string dig="")
        {
            return DigName;
        }
        public override string ToString()
        {
            return Note();
            //return Properties.Resources.ResourceManager.GetString(Digression.ToString());
            //return $"{Meter} {DigName} {Comment} {Degree} {Value} {Length} ";
        }
        public bool NotMoveAlert { get; set; } = false;

        public int GetCurvePoint(Kilometer km)
        {
            int result = 0;
            Digression = (DigName)DigName;
            switch (Digression.Value)
            {
                case int v when v == DigressionName.Ramp.Value: result = 50; break;
                case int v when v == DigressionName.Psi.Value: result = 50; break;
                case int v when v == DigressionName.SpeedUp.Value: result = 50; break;
                case int v when v == DigressionName.Pru.Value: result = 50; break;

            }
            return result;
        }

        public string Alert { get; set; } = string.Empty;
        public int DeltaM { get; set; }
        public int DeltaKM { get; set; }

        public string Note()
        {
            if (Alert != string.Empty)
                return Alert;
            
            static string is2to3String(int degree, bool is2to3)
            {
                return is2to3 ? degree.ToString() + "b" : degree.ToString();
            }
            
            if (DigName == DigressionName.Gauge10.Name || Digression == DigressionName.Degree3)
                return string.Empty;
            if (Degree < 4)
            {
                return $"{Meter} {DigName} {is2to3String(Degree, Is2to3)} {Value} {Length} {(PassengerSpeedLimit > 0 || FreightSpeedLimit > 0 ? LimitSpeedToString() : "")} {Comment}";
            }
            if (Degree == 5)
                return $"{Meter} {DigName} {Value} {(((Digression == DigressionName.Ramp || Digression == DigressionName.RampNear || Digression == DigressionName.Psi || Digression == DigressionName.PsiNear) && Length > 20) ? ">20" : Length.ToString())} {LimitSpeedToString()}";
            if (Degree == 4 || IsEqualTo4)
            {
                var result = $"{Meter} {DigName} {Degree} {Value} {Length} " + (Comment.Contains("Стр.") ? Comment : LimitSpeedToString());
                if (Comment.Contains("ис") || Comment.Contains("t+"))
                    result += " " + Comment;
                return result;
            }
            return string.Empty;


        }


        public void DigNameToDigression(string digName)
        {
            Digression = (DigName)digName;
        }
        public bool OnSwitch { get; set; } = false;
        public int finish_meter { get; set; }
        public string AllowSpeed { get; set; }
        public string Diagram_type { get; set; }

        public string LimitSpeedToString()
        {
            var v1 = PassengerSpeedLimit >= 0 ? PassengerSpeedLimit.ToString() : "-";
            var v2 = FreightSpeedLimit >= 0 ? PassengerSpeedLimit.ToString() : "-";
            return $"{v1}/{v2}";
        }

        internal object FontStyle()
        {
            var addStyle = DigName != null && (DigName.Contains("Нпк.л") || DigName.Contains("Нпк.п") || DigName.Contains("Пу.л") || DigName.Contains("Пу.п")) && Degree == 4;
            var psyStyle = DigName != null && (DigName.Contains("Пси") || DigName.Contains("?Пси"));
            var mainStyle = Degree == 3 || Degree == 4;

            return (addStyle || psyStyle) ? "bold" : mainStyle  ? "bold" : "normal";

            

        }

        public int GetCount()
        {
          
            int count = -1;
            switch (Digression.Name)
            {
                case string digname when 
                   digname == DigressionName.DrawdownLeft.Name
                || digname == DigressionName.DrawdownRight.Name
                || digname == DigressionName.Sag.Name
                || digname == DigressionName.StrighteningLeft.Name
                || digname == DigressionName.StrighteningRight.Name
                ||( digname == DigressionName.NoneStrightening.Name && Degree == 4)

              //  || digname == DigressionName.NoneStrighteningST.Name
                || digname == DigressionName.StrighteningOnSwitch.Name

                || digname == DigressionName.Strightening.Name:
                    count = 1;
                    break;
                case string digname when digname == DigressionName.Constriction.Name://суж
               
                    count = Length / 4;
                    count += Length % 4 > 0 ? 1 : 0;
                    break;
                case string digname when digname == DigressionName.Level.Name && Degree <3: //ур
                    count = Length / 20;
                    count += Length % 20 > 0 ? 1 : 0;
                    break;
                case string digname when digname == DigressionName.Level.Name && Degree >2: //ур
                    count = Length / 10;
                    count += Length % 10 > 0 ? 1 : 0;
                    break;
                case string digname when digname == DigressionName.Broadening.Name: //уш
                    count = Length / 4;
                    count += Length % 4 > 0 ? 1 : 0;
                    break;
                case string digname when 
                digname == DigressionName.TreadTiltLeft.Name || digname == DigressionName.TreadTiltRight.Name ||
                digname == DigressionName.DownhillLeft.Name || digname == DigressionName.DownhillRight.Name ||
                digname == DigressionName.SideWearLeft.Name || digname == DigressionName.SideWearRight.Name ||
                digname == DigressionName.VertIznosL.Name || digname == DigressionName.VertIznosR.Name ||
                digname == DigressionName.ReducedWearLeft.Name || digname == DigressionName.ReducedWearRight.Name ||
                digname == DigressionName.HeadWearLeft.Name || digname == DigressionName.HeadWearRight.Name:
                    count = Length / 4;
                    count += Length % 4 > 0 ? 1 : 0;
                    break;
            }
            return count;
        }
        public int Speed { get; set; }
        public int Norma { get; set; } = 1520;
        public int Width { get; set; }
        public bool InArtificialSection { get; set; } = false;
        public DigressionType Digtype { get; set; } = DigressionType.Main;
        public string Pch { get; internal set; }

        public int GetPoint(Kilometer kilometer)
        {
            var noteCoord = kilometer.Number.ToDoubleCoordinate(Meter);
            foreach (var speed in kilometer.Speeds)
            {
                if (speed.RealStartCoordinate <= noteCoord && noteCoord <= speed.RealFinalCoordinate)
                {
                    Speed = speed.Passenger;
                    FreightSpeedAllow = speed.Freight;
                    break;
                }
            }

            foreach (var norma in kilometer.Normas)
            {
                if (norma.RealStartCoordinate <= noteCoord && noteCoord <= norma.RealFinalCoordinate)
                {
                    Norma = norma.Norma_Width;
                    break;
                }
            }
            foreach (var curve in kilometer.Curves)
                foreach (var stright in curve.Straightenings)
                {
                    if (stright.RealStartCoordinate <= noteCoord && noteCoord <= stright.RealFinalCoordinate)
                    {
                        Norma = stright.Width;
                        Radius = stright.Radius;
                        break;
                    }
                }

            foreach (var tie in kilometer.CrossTies)
            {
                if (tie.RealStartCoordinate <= noteCoord && noteCoord <= tie.RealFinalCoordinate)
                {
                    CrosTieType = (CrosTieType)tie.Crosstie_type_id;
                }
            }

            foreach (var arr in kilometer.Artificials)
                if (noteCoord.Between(arr.Entrance_Start_km.ToDoubleCoordinate(arr.Entrance_Start_m), arr.Entrance_Final_km.ToDoubleCoordinate(arr.Entrance_Final_m)))
                {
                    InArtificialSection = true;
                }
            var point = 0;
            Digression = (DigName)DigName;

            
            switch (Digression.Value)
            {
                case int v when v == DigressionName.Broadening.Value: point = GetBroadeningPoint(); break;
                case int v when v == DigressionName.Constriction.Value: point = GetConstrisctionPoint(); break;
                case int v when v == DigressionName.Sag.Value: point = GetSagPoint(); break;
                case int v when v == DigressionName.Level.Value: point = GetLevelPoint(); break;
                case int v when v == DigressionName.DrawdownLeft.Value || v == DigressionName.DrawdownRight.Value: point = GetDrawdownPoint(); break;
                case int v when v == DigressionName.Strightening.Value || v == DigressionName.StrighteningLeft.Value || v == DigressionName.StrighteningRight.Value
                             || v == DigressionName.StrighteningOnSwitch.Value: //Алтанбек ага тексеру керек
                    if (Speed <= 140)
                        point = GetStrighteningPointTo140();
                    else
                        point = GetStrighteningPointFrom140();
                    break;
                case int v when v == DigressionName.StrgihtPlusSag.Value || v == DigressionName.StrgihtPlusDrawdown.Value || v == DigressionName.PatternRetraction.Value
                             || v == DigressionName.NoneStrightening.Value:
                    if (!LimitSpeedToString().Equals("-/-"))
                    {
                        point = 100;
                    }
                    break;
            }
            return point;
        }
        /// <summary>
        /// Величина балла за расширение (Таблицы П.2.1, П.2.2)
        /// </summary>
        /// <returns></returns>
        public int GetBroadeningPoint()
        {
            if (Km == 709)
            {
                Km = Km;
            }

            int point = 0;
            //Величина балла за единичное отступление длиной до 4 м
            if (Speed > 140)
                switch (Speed)
                {
                    case int speed when speed > 200 && speed <= 250:
                        switch ((int)Value)
                        {
                            case 1527: point = 1; break;
                            case 1528: point = 2; break;
                            case 1529: point = 3; break;
                            case 1530: point = 5; break;
                            case 1531: point = 10; break;
                            case 1532: point = 18; break;
                            case 1533: point = 24; break;
                            case 1534: point = 32; break;
                            case int n when n >= 1535: point = 100; break;
                        }
                        break;
                    case int speed when speed > 160 && speed <= 200:
                        if (Radius >= 3000)
                            switch ((int)Value)
                            {
                                case 1528: point = 1; break;
                                case 1529: point = 2; break;
                                case 1530: point = 2; break;
                                case 1531: point = 3; break;
                                case 1532: point = 5; break;
                                case 1533: point = 18; break;
                                case 1534: point = 24; break;
                                case int n when n >= 1535: point = 100; break;
                            }
                        else
                            switch ((int)Value)
                            {
                                case 1530: point = 1; break;
                                case 1531: point = 1; break;
                                case 1532: point = 2; break;
                                case 1533: point = 3; break;
                                case 1534: point = 5; break;
                                case 1535: point = 18; break;
                                case 1536: point = 24; break;
                                case int n when n >= 1537: point = 100; break;
                            }
                        break;
                    case int speed when speed > 140 && speed <= 160:
                        if (Radius >= 3000)
                            switch ((int)Value)
                            {
                                case 1529: point = 1; break;
                                case 1530: point = 1; break;
                                case 1531: point = 2; break;
                                case 1532: point = 3; break;
                                case 1533: point = 5; break;
                                case 1534: point = 18; break;
                                case 1535: point = 24; break;
                                case 1536: point = 32; break;
                                case int n when n >= 1537: point = 100; break;
                            }
                        else
                            switch ((int)Value)
                            {
                                case 1531: point = 1; break;
                                case 1532: point = 1; break;
                                case 1533: point = 2; break;
                                case 1534: point = 2; break;
                                case 1535: point = 3; break;
                                case 1536: point = 5; break;
                                case 1537: point = 18; break;
                                case 1538: point = 24; break;
                                case int n when n >= 1539: point = 100; break;
                            }
                        break;
                }
            else
                switch (Norma)
                {
                    case 1520:
                        switch (Speed)
                        {
                            case int speed when speed > 120 && speed <= 140:
                                switch ((int)Value)
                                {
                                    case 1535: point = 1; break;
                                    case 1536: point = 2; break;
                                    case 1537: point = 18; break;
                                    case 1538: point = 24; break;
                                    case 1539: point = 24; break;
                                    case 1540: point = 32; break;
                                    case int n when n >= 1541: point = 100; break;
                                }
                                break;
                            case int speed when speed > 100 && speed <= 120:
                                switch ((int)Value)
                                {
                                    case 1537: point = 1; break;
                                    case 1538: point = 2; break;
                                    case 1539: point = 5; break;
                                    case 1540: point = 18; break;
                                    case 1541: point = 24; break;
                                    case 1542: point = 32; break;
                                    case int n when n >= 1543: point = 100; break;
                                }
                                break;
                            case int speed when speed > 60 && speed <= 100:
                                switch ((int)Value)
                                {
                                    case 1539: point = 1; break;
                                    case 1540: point = 2; break;
                                    case 1541: point = 5; break;
                                    case 1542: point = 18; break;
                                    case 1543: point = 24; break;
                                    case 1544: point = 32; break;
                                    case int n when n >= 1545: point = 100; break;
                                }
                                break;
                            case int speed when speed > 25 && speed <= 60:
                                switch ((int)Value)
                                {
                                    case 1539: point = 1; break;
                                    case 1540: point = 1; break;
                                    case 1541: point = 2; break;
                                    case 1542: point = 3; break;
                                    case 1543: point = 5; break;
                                    case 1544: point = 18; break;
                                    case 1545: point = 24; break;
                                    case 1546: point = 32; break;
                                    case int n when n >= 1547: point = 100; break;
                                }
                                break;
                            case int speed when speed <= 25:
                                switch ((int)Value)
                                {
                                    case 1539: point = 1; break;
                                    case 1540: point = 1; break;
                                    case 1541: point = 1; break;
                                    case 1542: point = 2; break;
                                    case 1543: point = 2; break;
                                    case 1544: point = 3; break;
                                    case 1545: point = 3; break;
                                    case 1546: point = 5; break;
                                    case 1547: point = 18; break;
                                    case 1548: point = 24; break;
                                    case int n when n >= 1549: point = 100; break;
                                }
                                break;
                        }
                        break;
                    case 1524:
                        switch (Speed)
                        {
                            case int speed when speed > 120 && speed <= 140:
                                switch ((int)Value)
                                {
                                    case 1535: point = 1; break;
                                    case 1536: point = 1; break;
                                    case 1537: point = 2; break;
                                    case 1538: point = 5; break;
                                    case 1539: point = 18; break;
                                    case 1540: point = 24; break;
                                    case int n when n >= 1541: point = 100; break;
                                }
                                break;
                            case int speed when speed > 100 && speed <= 120:
                                switch ((int)Value)
                                {
                                    case 1537: point = 1; break;
                                    case 1538: point = 2; break;
                                    case 1539: point = 5; break;
                                    case 1540: point = 18; break;
                                    case 1541: point = 24; break;
                                    case 1542: point = 32; break;
                                    case int n when n >= 1543: point = 100; break;
                                }
                                break;
                            case int speed when speed > 60 && speed <= 100:
                                switch ((int)Value)
                                {
                                    case 1539: point = 1; break;
                                    case 1540: point = 2; break;
                                    case 1541: point = 5; break;
                                    case 1542: point = 18; break;
                                    case 1543: point = 24; break;
                                    case 1544: point = 32; break;
                                    case int n when n >= 1545: point = 100; break;
                                }
                                break;
                            case int speed when speed > 25 && speed <= 60:
                                switch ((int)Value)
                                {
                                    case 1539: point = 1; break;
                                    case 1540: point = 1; break;
                                    case 1541: point = 2; break;
                                    case 1542: point = 5; break;
                                    case 1543: point = 18; break;
                                    case 1544: point = 18; break;
                                    case 1545: point = 24; break;
                                    case 1546: point = 32; break;
                                    case int n when n >= 1547: point = 100; break;
                                }
                                break;
                            case int speed when speed <= 25:
                                switch ((int)Value)
                                {
                                    case 1539: point = 1; break;
                                    case 1540: point = 1; break;
                                    case 1541: point = 1; break;
                                    case 1542: point = 2; break;
                                    case 1543: point = 2; break;
                                    case 1544: point = 3; break;
                                    case 1545: point = 3; break;
                                    case 1546: point = 5; break;
                                    case 1547: point = 18; break;
                                    case 1548: point = 24; break;
                                    case int n when n >= 1549: point = 100; break;
                                }
                                break;
                        }
                        break;
                    case 1530:
                        switch (Speed)
                        {
                            case int speed when speed > 120 && speed <= 140:
                                switch ((int)Value)
                                {
                                    case 1541: point = 1; break;
                                    case 1542: point = 32; break;
                                    case int n when n >= 1543: point = 100; break;
                                }
                                break;
                            case int speed when speed > 25 && speed <= 120:
                                switch ((int)Value)
                                {
                                    case 1543: point = 1; break;
                                    case 1544: point = 2; break;
                                    case 1545: point = 18; break;
                                    case 1546: point = 24; break;
                                    case int n when n >= 1547: point = 100; break;
                                }
                                break;
                            case int speed when speed <= 25:
                                switch ((int)Value)
                                {
                                    case 1545: point = 1; break;
                                    case 1546: point = 2; break;
                                    case 1547: point = 18; break;
                                    case 1548: point = 24; break;
                                    case int n when n >= 1549: point = 100; break;
                                }
                                break;
                        }
                        break;
                    case 1535:
                        switch (Speed)
                        {
                            case int speed when speed > 60 && speed <= 120:
                                switch ((int)Value)
                                {
                                    case 1545: point = 1; break;
                                    case 1546: point = 24; break;
                                    case int n when n >= 1547: point = 100; break;
                                }
                                break;
                            case int speed when speed > 25 && speed <= 60:
                                switch ((int)Value)
                                {
                                    case 1546: point = 1; break;
                                    case 1547: point = 18; break;
                                    case 1548: point = 24; break;
                                    case int n when n >= 1549: point = 100; break;
                                }
                                break;
                            case int speed when speed <= 25:
                                switch ((int)Value)
                                {
                                    case 1546: point = 1; break;
                                    case 1547: point = 18; break;
                                    case 1548: point = 24; break;
                                    case int n when n >= 1549: point = 100; break;
                                }
                                break;
                        }
                        break;
                    case 1540:
                    case int speed when speed <= 60:
                        switch ((int)Value)
                        {
                            case 1548: point = 1; break;
                            case int n when n >= 1549: point = 100; break;
                        }
                        break;

                }
            // величина балла в зависимости от длины отступления, м
            if (Length <= 4)
                return point;
            if (Length > 40)
            {
                point = 150;
                return point;
            }
            int count = Length / 4 + (Length % 4 > 0 ? 1 : 0);
            switch (point)
            {
                case int b when b >= 1 && b <= 5:
                case 10:
                    point = point + (count - 1);
                    break;
                case 18:
                case 24:
                case 32:
                    point = point + 2 * (count - 1);
                    break;
                case 100:
                    point = point + 5 * (count - 1);
                    break;
            }
            return point;
        }
        /// <summary>
        /// Балловая оценка сужения колеи - Таблица П.2.3
        /// </summary>
        /// <returns></returns>
        public int GetConstrisctionPoint()
        {
            int point = 0;
            //Величина балла за единичное отступление длиной до 4 м
            switch (Norma)
            {
                case 1520:
                    switch (Speed)
                    {
                        case int speed when speed > 200 && speed <= 250:
                            switch ((int)Value)
                            {
                                case 1516: point = 1; break;
                                case 1515: point = 2; break;
                                case 1514: point = 18; break;
                                case int n when n <= 1513: point = 100; break;
                            }
                            break;
                        case int speed when speed > 160 && speed <= 200:
                            if (CrosTieType != CrosTieType.Before96)
                                switch ((int)Value)
                                {
                                    case 1515: point = 1; break;
                                    case 1514: point = 2; break;
                                    case 1513: point = 18; break;
                                    case 1512: point = 24; break;
                                    case int n when n <= 1511: point = 100; break;
                                }
                            else
                                switch ((int)Value)
                                {
                                    case 1514: point = 1; break;
                                    case 1513: point = 5; break;
                                    case 1512: point = 18; break;
                                    case int n when n <= 1511: point = 100; break;
                                }
                            break;
                        case int speed when speed > 140 && speed <= 160:
                            if (CrosTieType != CrosTieType.Before96)
                                switch ((int)Value)
                                {
                                    case 1514: point = 1; break;
                                    case 1513: point = 5; break;
                                    case 1512: point = 18; break;
                                    case int n when n <= 1511: point = 100; break;
                                }
                            else
                                switch ((int)Value)
                                {
                                    case 1513: point = 1; break;
                                    case 1512: point = 5; break;
                                    case 1511: point = 18; break;
                                    case 1510: point = 24; break;
                                    case int n when n <= 1509: point = 100; break;
                                }
                            break;
                        case int speed when speed > 120 && speed <= 140:
                            switch ((int)Value)
                            {
                                case 1513: point = 1; break;
                                case 1512: point = 18; break;
                                case int n when n <= 1511: point = 100; break;
                            }
                            break;
                        case int speed when speed <= 120:
                            if (CrosTieType != CrosTieType.Before96)
                                switch ((int)Value)
                                {
                                    case 1513: point = 1; break;
                                    case 1512: point = 18; break;
                                    case int n when n <= 1511: point = 100; break;
                                }
                            else
                                switch ((int)Value)
                                {
                                    case 1513: point = 1; break;
                                    case 1512: point = 5; break;
                                    case 1511: point = 18; break;
                                    case 1510: point = 24; break;
                                    case int n when n <= 1509: point = 100; break;
                                }
                            break;
                    }
                    break;
                case 1524:
                    switch (Speed)
                    {
                        case int speed when speed > 120 && speed <= 140:
                            switch ((int)Value)
                            {
                                case 1515: point = 1; break;
                                case 1514: point = 2; break;
                                case 1513: point = 18; break;
                                case 1512: point = 24; break;
                                case int n when n <= 1511: point = 100; break;
                            }
                            break;
                        case int speed when speed > 100 && speed <= 120:
                            switch ((int)Value)
                            {
                                case 1514: point = 1; break;
                                case 1513: point = 5; break;
                                case 1512: point = 18; break;
                                case int n when n <= 1511: point = 100; break;
                            }
                            break;
                        case int speed when speed <= 100 && !(speed <= 25 && CrosTieType == CrosTieType.Before96):
                            switch ((int)Value)
                            {
                                case 1513: point = 1; break;
                                case 1512: point = 18; break;
                                case int n when n <= 1511: point = 100; break;
                            }
                            break;
                        case int speed when speed <= 25 && CrosTieType == CrosTieType.Before96:
                            switch ((int)Value)
                            {
                                case 1513: point = 1; break;
                                case 1512: point = 18; break;
                                case 1511: point = 24; break;
                                case 1510: point = 32; break;
                                case int n when n <= 1509: point = 100; break;
                            }
                            break;
                    }
                    break;
                case 1530:
                    switch (Speed)
                    {
                        case int speed when speed > 25 && speed <= 140:
                            switch ((int)Value)
                            {
                                case 1519: point = 1; break;
                                case 1518: point = 2; break;
                                case 1517: point = 3; break;
                                case 1516: point = 5; break;
                                case 1515: point = 24; break;
                                case int n when n <= 1514: point = 100; break;
                            }
                            break;
                        case int speed when speed <= 25:
                            switch ((int)Value)
                            {
                                case 1519: point = 1; break;
                                case 1518: point = 1; break;
                                case 1517: point = 2; break;
                                case 1516: point = 2; break;
                                case 1515: point = 3; break;
                                case 1514: point = 5; break;
                                case 1513: point = 18; break;
                                case 1512: point = 32; break;
                                case int n when n <= 1511: point = 100; break;
                            }
                            break;
                    }
                    break;
                case 1535:
                    switch (Speed)
                    {
                        case int speed when speed > 60 && speed <= 100:
                            switch ((int)Value)
                            {
                                case 1522: point = 1; break;
                                case 1521: point = 2; break;
                                case 1520: point = 3; break;
                                case 1519: point = 5; break;
                                case 1518: point = 18; break;
                                case 1517: point = 24; break;
                                case int n when n <= 1516: point = 100; break;
                            }
                            break;
                        case int speed when speed > 25 && speed <= 60:
                            switch ((int)Value)
                            {
                                case 1520: point = 1; break;
                                case 1519: point = 2; break;
                                case 1518: point = 18; break;
                                case 1517: point = 24; break;
                                case int n when n >= 1516: point = 100; break;
                            }
                            break;
                        case int speed when speed <= 25:
                            switch ((int)Value)
                            {
                                case 1520: point = 1; break;
                                case 1519: point = 1; break;
                                case 1518: point = 2; break;
                                case 1517: point = 3; break;
                                case 1516: point = 5; break;
                                case 1515: point = 18; break;
                                case 1514: point = 24; break;
                                case 1513: point = 32; break;
                                case 1512: point = 32; break;
                                case int n when n <= 1511: point = 100; break;
                            }
                            break;
                    }
                    break;
                case 1540:
                case int speed when speed > 25 && speed <= 120:
                    switch ((int)Value)
                    {
                        case 1523: point = 1; break;
                        case 1522: point = 2; break;
                        case 1519: point = 18; break;
                        case 1518: point = 32; break;
                        case int n when n <= 1517: point = 100; break;
                    }
                    break;
                case int speed when speed <= 25:
                    switch ((int)Value)
                    {
                        case 1523: point = 1; break;
                        case 1522: point = 1; break;
                        case 1521: point = 2; break;
                        case 1520: point = 2; break;
                        case 1519: point = 5; break;
                        case 1518: point = 5; break;
                        case 1517: point = 18; break;
                        case 1516: point = 18; break;
                        case 1515: point = 24; break;
                        case 1514: point = 24; break;
                        case 1513: point = 32; break;
                        case 1512: point = 32; break;
                        case int n when n <= 1511: point = 100; break;
                    }
                    break;

            }
            // величина балла в зависимости от длины отступления, м
            if (Length <= 4)
                return point;
            int count = Length / 4 + (Length % 4 > 0 ? 1 : 0);
            switch (point)
            {
                case int b when b >= 1 && b <= 5:
                    point = point + 1 * (count - 1);
                    break;
                case 18:
                case 24:
                case 32:
                    point = point + 2 * (count - 1);
                    break;
                case 100:
                    point = point + 5 * (count - 1);
                    break;
            }
            return point;
        }
        /// <summary>
        /// Балловая оценка перекоса
        /// </summary>
        /// <returns></returns>
        public int GetSagPoint() {
            int[] to10 = { 1, 2, 3, 4, 6, 22, 22, 28, 40, 60, 100 };
            int[] to20 = { 1, 2, 3, 4, 6, 19, 22, 26, 36, 55, 100 };
            var point = 0;
            int index = -1;
            //table 8.4 
            if (InArtificialSection)
            {
                if (Speed > 120 && Value > 12)
                    index = 10;
                if (Speed > 60 && Value > 14)
                    index = 10;
                if (Speed > 40 && Value > 20)
                    index = 10;
                if (Speed > 15 && Value > 25)
                    index = 10;
                if (Value > 34)
                    index = 10;
            }
            if (index<0)
            switch (Speed)
            {
                case int speed when speed.Between(201, 250):

                    switch ((int)Value)
                    {
                        case 9: index = 0; break;
                        case 10: index = 1; break;
                        case 11: index = 2; break;
                        case 12: index = 4; break;
                        case 13: index = 5; break;
                        case 14: index = 8; break;
                        case int sag when sag >= 15: index = 10; break;
                    }
                    break;

                case int speed when speed.Between(161,200):
                    switch ((int)Value)
                    {
                        case 10: index = 0; break;
                        case 11: index = 2; break;
                        case 12: index = 4; break;
                        case 13: index = 5; break;
                        case 14: index = 7; break;
                        case 15: index = 9; break;
                        case int sag when sag >= 16: index = 10; break;
                    }
                    break;
                case int speed when speed.Between(141, 160):
                    switch ((int)Value)
                    {
                        case 10: index = 0; break;
                        case 11: index = 2; break;
                        case 12: index = 4; break;
                        case 13: index = 5; break;
                        case 14: index = 6; break;
                        case 15: index = 8; break;
                        case 16: index = 9; break;
                        case int sag when sag >= 17: index = 10; break;
                    }
                    break;
                case int speed when speed.Between(121, 140):
                    switch ((int)Value)
                    {
                        case 10: index = 0; break;
                        case 11: index = 2; break;
                        case 12: index = 4; break;
                        case 13: index = 5; break;
                        case 14: index = 6; break;
                        case 15: index = 7; break;
                        case 16: index = 9; break;
                        case int sag when sag >= 17: index = 10; break;
                    }
                    break;
                case int speed when speed.Between(61, 120) && !(FreightSpeedAllow.Between(61,90) && Length <11):
                    switch ((int)Value)
                    {
                        case 11: index = 0; break;
                        case 12: index = 1; break;
                        case 13: index = 3; break;
                        case 14: index = 4; break;
                        case 15: index = 5; break;
                        case 16: index = 6; break;
                        case 17:
                        case 18:
                            index = 7; break;
                        case 19: index = 8; break;
                        case 20: index = 9; break;
                        case int sag when sag >= 21: index = 10; break;
                    }
                    break;
                case int speed when speed.Between(61, 120) && (FreightSpeedAllow.Between(61, 90) && Length < 11):
                    switch ((int)Value)
                    {
                        case 11: index = 0; break;
                        case 12: index = 1; break;
                        case 13: index = 3; break;
                        case 14: index = 4; break;
                        case 15: index = 6; break;
                        case 16: index = 7; break;
                        case 17: index = 8; break;
                        case 18: index = 9; break;
                        case int sag when sag >= 19: index = 10; break;
                    }
                    break;
                case int speed when speed.Between(41, 60):
                    switch ((int)Value)
                    {
                        case int val when val.Between(15,17): index = 0; break;
                        case 18: index = 2; break;
                        case 19: index = 3; break;
                        case 20: index = 4; break;
                        case 21: index = 5; break;
                        case 22: index = 6; break;
                        case 23: index = 7; break;
                        case 24: index = 8; break;
                        case 25: index = 9; break;
                        case int sag when sag >= 26: index = 10; break;
                    }
                    break;
                case int speed when speed.Between(16, 40):
                    switch ((int)Value)
                    {
                        case int val when val.Between(17, 18): index = 0; break;
                        case int val when val.Between(19, 20): index = 1; break;
                        case int val when val.Between(21, 22): index = 2; break;
                        case int val when val.Between(23, 24): index = 3; break;
                        case 25: index = 4; break;
                        case 26: index = 5; break;
                        case 27: index = 6; break;
                        case 28: index = 7; break;
                        case 29: index = 8; break;
                        case 30: index = 9; break;
                        case int sag when sag >= 31: index = 10; break;
                    }
                    break;
                case 15:
                    switch ((int)Value)
                    {
                        case int val when val.Between(17, 20): index = 0; break;
                        case int val when val.Between(21, 25): index = 1; break;
                        case int val when val.Between(26, 29): index = 2; break;
                        case int val when val.Between(30, 23): index = 3; break;
                        case 34: index = 4; break;
                        case int val when val.Between(35, 38): index = 5; break;
                        case int val when val.Between(39, 42): index = 6; break;
                        case int val when val.Between(43, 46): index = 7; break;
                        case int val when val.Between(47, 49): index = 8; break;
                        case 50: index = 9; break;
                        case int sag when sag >= 31: index = 10; break;
                    }
                    break;
            }
            if (index > -1)
            {
                if (Length <= 10 && FreightSpeedAllow.Between(61, 90))
                    point = to10[index];
                else
                    point = to20[index];
            }
            return point;
        }

        public void GetAllowSpeedAddParam(Speed speed, double radius, float value)
        {
            try
            {
                int ust = -1;
                int pass = -1;
                int freig = -1;

                if (13.1f <= value && value <= 15)
                {
                    ust = 140;
                }
                else if (15.1f <= value && value <= 20 && 350 < radius)
                {
                    ust = 70;
                }
                else if (15.1f <= value && value <= 20 && 350 >= radius)
                {
                    ust = 50;
                }
                else if (20.1f <= value )
                {
                    ust = 50;
                }

                if(ust != -1)
                {
                    if (speed.Passenger <= ust)
                        pass = -1;
                    else
                        pass = ust;

                    if (speed.Freight <= ust)
                        freig = -1;
                    else
                        freig = ust;
                }

                PassengerSpeedLimit = pass;
                FreightSpeedLimit = freig;

                AllowSpeed = $"{pass}/{freig}";
            }
            catch
            {
                AllowSpeed = "-/-";
            }
        }


        /// <summary>
        /// Балловая оценка просадка
        /// </summary>
        /// <returns></returns>
        public int GetDrawdownPoint()
        {
            int[] points = { 1, 2, 3, 5, 7, 12, 20, 30, 45, 60, 100 };
            var point = 0;
            int index = -1;
            //table 8.4 
            if (InArtificialSection)
            {
                if (Speed > 120 && Value > 15)
                    index = 10;
                if (Speed > 60 && Value > 20)
                    index = 10;
                if (Speed > 40 && Value > 25)
                    index = 10;
                if (Speed > 15 && Value > 30)
                    index = 10;
                if (Value > 34)
                    index = 10;
            }
            if (index < 0)
                switch (Speed)
                {
                    case int speed when speed.Between(201,250):

                        switch ((int)Value)
                        {
                            case 9: index = 0; break;
                            case 10: index = 1; break;
                            case 11: index = 2; break;
                            case 12: index = 3; break;
                            case 13: index = 4; break;
                            case 14: index = 5; break;
                            case 15: index = 6; break;
                            case 16: index = 7; break;
                            case 17: index = 8; break;
                            case 18: index = 9; break;
                            case int sag when sag >= 19: index = 10; break;
                        }
                        break;

                   case int speed when speed.Between(161, 200):
                        switch ((int)Value)
                        {
                            case 11: index = 2; break;
                            case 12: index = 1; break;
                            case 13: index = 3; break;
                            case 14: index = 4; break;
                            case 15: index = 5; break;
                            case 16: index = 6; break;
                            case 17: index = 7; break;
                            case 18: index = 9; break;
                            case int sag when sag >= 19: index = 10; break;
                        }
                        break;
                    case int speed when speed.Between(141, 160):
                        switch ((int)Value)
                        {
                            case 12: index = 0; break;
                            case 13: index = 1; break;
                            case 14: index = 2; break;
                            case 15: index = 4; break;
                            case 16: index = 5; break;
                            case 17: index = 7; break;
                            case 18: index = 9; break;
                            case int sag when sag >= 19: index = 10; break;
                        }
                        break;
                    case int speed when speed.Between(121, 140):
                        switch ((int)Value)
                        {
                            case 12: index = 0; break;
                            case 13: index = 1; break;
                            case 14: index = 2; break;
                            case 15: index = 4; break;
                            case 16: index = 5; break;
                            case 17: index = 6; break;
                            case 18: index = 7; break;
                            case 19: index = 8; break;
                            case 20: index = 9; break;
                            case int sag when sag >= 21: index = 10; break;
                        }
                        break;
                    case int speed when speed.Between(61, 120):
                        switch ((int)Value)
                        {
                            case int val when val.Between(13, 14): index = 0; break;
                            case 15: index = 1; break;
                            case 16: index = 2; break;
                            case int val when val.Between(17, 18): index = 3; break;
                            case int val when val.Between(19, 20): index = 4; break;
                            case 21: index = 5; break;
                            case 22: index = 6; break;
                            case 23: index = 7; break;
                            case 24: index = 8; break;
                            case 25: index = 9; break;
                            case int sag when sag >= 26: index = 10; break;
                        }
                        break;
                    case int speed when speed.Between(41, 60):
                        switch ((int)Value)
                        {
                            case int val when val.Between(15, 18): index = 0; break;
                            case int val when val.Between(19, 20): index = 1; break;
                            case int val when val.Between(21, 22): index = 2; break;
                            case int val when val.Between(23, 24): index = 3; break;
                            case 25: index = 4; break;
                            case 26: index = 5; break;
                            case 27: index = 6; break;
                            case 28: index = 7; break;
                            case 29: index = 8; break;
                            case 30: index = 9; break;
                            case int sag when sag >= 31: index = 10; break;
                        }
                        break;
                    case int speed when speed.Between(16, 40):
                        switch ((int)Value)
                        {
                            case int val when val.Between(17, 19): index = 0; break;
                            case int val when val.Between(20, 22): index = 1; break;
                            case int val when val.Between(23, 25): index = 2; break;
                            case int val when val.Between(26, 28): index = 3; break;
                            case int val when val.Between(29, 30): index = 4; break;
                            case 31: index = 5; break;
                            case 32: index = 6; break;
                            case 33: index = 7; break;
                            case 34: index = 8; break;
                            case 35: index = 9; break;
                            case int sag when sag >= 31: index = 10; break;
                        }
                        break;
                    case 15:
                        switch ((int)Value)
                        {
                            case int val when val.Between(17, 19): index = 0; break;
                            case int val when val.Between(20, 23): index = 1; break;
                            case int val when val.Between(24, 27): index = 2; break;
                            case int val when val.Between(28, 31): index = 3; break;
                            case int val when val.Between(32, 35): index = 4; break;
                            case int val when val.Between(36, 37): index = 5; break;
                            case int val when val.Between(38, 39): index = 6; break;
                            case int val when val.Between(40, 41): index = 7; break;
                            case int val when val.Between(42, 43): index = 8; break;
                            case int val when val.Between(44, 45): index = 8; break;
                            case int sag when sag >= 46: index = 10; break;
                        }
                        break;
                }
            if (index > -1)
            {
                point = points[index];
            }
            return point;
        }
        /// <summary>
        /// Балловая оценка отступлений в плане при скоростях движения до 140 км/час
        /// </summary>
        /// <returns></returns>
        public int GetStrighteningPointTo140()
        {
            int point = 0, index=-1;
            
            int[] to20 =    { 1, 2, 3, 4, 6, 19, 22, 28, 40, 60, 100 };
            int[] f20to40 = { 1, 2, 3, 4, 6, 19, 22, 26, 36, 55, 100 };
            switch (Speed)
            {
                case int speed when speed.Between(121, 140):
                    if (Length <= 20)
                        switch ((int)Value)
                        {
                            case 13: index = 0; break;
                            case 14: index = 2; break;
                            case 15: index = 4; break;
                            case int val when val.Between(16, 17): index = 5; break;
                            case int val when val.Between(18, 19): index = 6; break;
                            case int val when val.Between(20, 21): index = 7; break;
                            case int val when val.Between(22, 23): index = 8; break;
                            case int val when val.Between(24, 25): index = 9; break;
                            case int val when val >= 26: index = 10; break;
                        }
                    else if (Length.Between(21, 40))
                        switch ((int)Value)
                        {
                            case 21: index = 0; break;
                            case 22: index = 1; break;
                            case 23: index = 2; break;
                            case 24: index = 3; break;
                            case 25: index = 4; break;
                            case int val when val.Between(26, 27): index = 5; break;
                            case int val when val.Between(28, 29): index = 6; break;
                            case int val when val.Between(30, 31): index = 7; break;
                            case int val when val.Between(32, 33): index = 8; break;
                            case int val when val.Between(34, 35): index = 9; break;
                            case int val when val >= 36: index = 10; break;
                        }
                    break;
                case int speed when speed.Between(61, 120):
                    if (Length <= 20)
                        switch ((int)Value)
                        {
                            case 16: index = 0; break;
                            case int val when val.Between(17, 18): index = 1; break;
                            case int val when val.Between(19, 20): index = 2; break;
                            case int val when val.Between(21, 22): index = 3; break;
                            case int val when val.Between(23, 25): index = 4; break;
                            case int val when val.Between(26, 27): index = 5; break;
                            case int val when val.Between(28, 29): index = 6; break;
                            case int val when val.Between(30, 31): index = 7; break;
                            case int val when val.Between(32, 33): index = 8; break;
                            case int val when val.Between(34, 35): index = 9; break;
                            case int val when val >= 36: index = 10; break;
                        }
                    else if (Length.Between(21, 40))
                        switch ((int)Value)
                        {
                            case int val when val.Between(21, 23): index = 0; break;
                            case int val when val.Between(24, 26): index = 1; break;
                            case int val when val.Between(27, 29): index = 2; break;
                            case int val when val.Between(30, 32): index = 3; break;
                            case int val when val.Between(33, 35): index = 4; break;
                            case 36: index = 5; break;
                            case 37: index = 6; break;
                            case 38: index = 7; break;
                            case 39: index = 8; break;
                            case 40: index = 9; break;
                            case int val when val >= 41: index = 10; break;
                        }
                    break;
                case int speed when speed.Between(41, 60):
                    if (Length <= 20)
                        switch ((int)Value)
                        {
                            case int val when val.Between(21, 23): index = 0; break;
                            case int val when val.Between(24, 26): index = 1; break;
                            case int val when val.Between(27, 29): index = 2; break;
                            case int val when val.Between(30, 32): index = 3; break;
                            case int val when val.Between(33, 35): index = 4; break;
                            case 36: index = 5; break;
                            case 37: index = 6; break;
                            case 38: index = 7; break;
                            case 39: index = 8; break;
                            case 40: index = 9; break;
                            case int val when val >= 41: index = 10; break;
                        }
                    else if (Length.Between(21, 40))
                        switch ((int)Value)
                        {
                            case int val when val.Between(31, 32): index = 0; break;
                            case int val when val.Between(33, 34): index = 1; break;
                            case int val when val.Between(35, 36): index = 2; break;
                            case int val when val.Between(37, 38): index = 3; break;
                            case int val when val.Between(39, 40): index = 4; break;
                            case int val when val.Between(41, 42): index = 5; break;
                            case int val when val.Between(43, 44): index = 6; break;
                            case int val when val.Between(45, 46): index = 7; break;
                            case int val when val.Between(47, 48): index = 8; break;
                            case int val when val.Between(49, 50): index = 9; break;
                            case int val when val >= 51: index = 10; break;
                        }
                    break;
                case int speed when speed.Between(16, 40):
                    if (Length <= 20)
                        switch ((int)Value)
                        {
                            case int val when val.Between(21, 24): index = 0; break;
                            case int val when val.Between(25, 28): index = 1; break;
                            case int val when val.Between(29, 32): index = 2; break;
                            case int val when val.Between(33, 36): index = 3; break;
                            case int val when val.Between(37, 40): index = 4; break;
                            case int val when val.Between(41, 42): index = 5; break;
                            case int val when val.Between(43, 44): index = 6; break;
                            case int val when val.Between(45, 46): index = 7; break;
                            case int val when val.Between(47, 48): index = 8; break;
                            case int val when val.Between(49, 50): index = 9; break;
                            case int val when val >= 51: index = 10; break;
                        }
                    else if (Length.Between(21, 40))
                        switch ((int)Value)
                        {
                            case int val when val.Between(31, 34): index = 0; break;
                            case int val when val.Between(35, 39): index = 1; break;
                            case int val when val.Between(40, 44): index = 2; break;
                            case int val when val.Between(45, 47): index = 3; break;
                            case int val when val.Between(48, 50): index = 4; break;
                            case int val when val.Between(51, 53): index = 5; break;
                            case int val when val.Between(54, 56): index = 6; break;
                            case int val when val.Between(57, 59): index = 7; break;
                            case int val when val.Between(60, 62): index = 8; break;
                            case int val when val.Between(63, 65): index = 9; break;
                            case int val when val >= 66: index = 10; break;
                        }
                    break;
                case 15:
                    if (Length <= 20)
                        switch ((int)Value)
                        {
                            case int val when val.Between(31, 34): index = 0; break;
                            case int val when val.Between(35, 39): index = 1; break;
                            case int val when val.Between(40, 44): index = 2; break;
                            case int val when val.Between(45, 47): index = 3; break;
                            case int val when val.Between(48, 50): index = 4; break;
                            case int val when val.Between(51, 53): index = 5; break;
                            case int val when val.Between(54, 56): index = 6; break;
                            case int val when val.Between(57, 59): index = 7; break;
                            case int val when val.Between(60, 62): index = 8; break;
                            case int val when val.Between(63, 65): index = 9; break;
                            case int val when val >= 66: index = 10; break;
                        }
                    else if (Length.Between(21, 40))
                        switch ((int)Value)
                        {
                            case int val when val.Between(41, 45): index = 0; break;
                            case int val when val.Between(46, 50): index = 1; break;
                            case int val when val.Between(51, 55): index = 2; break;
                            case int val when val.Between(56, 60): index = 3; break;
                            case int val when val.Between(61, 65): index = 4; break;
                            case int val when val.Between(66, 70): index = 5; break;
                            case int val when val.Between(71, 75): index = 6; break;
                            case int val when val.Between(76, 80): index = 7; break;
                            case int val when val.Between(81, 85): index = 8; break;
                            case int val when val.Between(86, 90): index = 8; break;
                            case int val when val >= 91: index = 10; break;
                        }
                    break;
            }
            if (index >= 0)
            {
                if (Length <= 20)
                    point = to20[index];
                else if (Length.Between(21,40))
                    point = f20to40[index];
            }
            return point;
        }
        /// <summary>
        /// Балловая оценка отступлений в плане при скоростях движения более 140 км/час
        /// </summary>
        /// <returns></returns>
        public int GetStrighteningPointFrom140()
        {
            int point = 0, index = -1;

            int[] to20 = { 1, 2, 3, 5, 7, 20, 24, 30, 44, 65, 100 };
            int[] f20to40 = { 1, 2, 3, 4, 6, 19, 22, 26, 36, 55, 100 };
            int[] f40to60 = { 1, 2, 3, 4, 6, 18, 20, 24, 32, 50, 100 };
            switch (Speed)
            {
                case int speed when speed.Between(141, 160):
                    switch (Length) {
                        case int len when len <= 20:
                            switch ((int)Value)
                            {
                                case 11: index = 0; break;
                                case 12: index = 1; break;
                                case 13: index = 2; break;
                                case 14: index = 3; break;
                                case 15: index = 4; break;
                                case int val when val.Between(16, 17): index = 5; break;
                                case int val when val.Between(18, 19): index = 6; break;
                                case 20: index = 7; break;
                                case 21: index = 8; break;
                                case 22: index = 9; break;
                                case int val when val >= 23: index = 10; break;
                            }
                            break;
                        case int len when len.Between(21, 40):
                        switch ((int)Value)
                            {
                                case int val when val.Between(16, 17): index = 0; break;
                                case int val when val.Between(18, 19): index = 1; break;
                                case int val when val.Between(20, 21): index = 2; break;
                                case 22: index = 3; break;
                                case 23: index = 4; break;
                                case 24: index = 5; break;
                                case 25: index = 6; break;
                                case 26: index = 7; break;
                                case int val when val.Between(27, 28): index = 8; break;
                                case int val when val.Between(29, 30): index = 9; break;
                                case int val when val >= 31: index = 10; break;
                            }
                            break;
                        case int len when len.Between(41, 60):
                            switch ((int)Value)
                            {
                                case 21: index = 0; break;
                                case 22: index = 1; break;
                                case 23: index = 2; break;
                                case 24: index = 3; break;
                                case 25: index = 4; break;
                                case int val when val.Between(26, 27): index = 5; break;
                                case int val when val.Between(28, 29): index = 6; break;
                                case int val when val.Between(30, 31): index = 7; break;
                                case int val when val.Between(32, 33): index = 8; break;
                                case int val when val.Between(34, 35): index = 9; break;
                                case int val when val >= 36: index = 10; break;
                            }
                            break;

                    }
                    break;
                case int speed when speed.Between(161, 200):
                    switch (Length)
                    {
                        case int len when len <= 20:
                            switch ((int)Value)
                            {
                                case 11: index = 0; break;
                                case 12: index = 1; break;
                                case 13: index = 2; break;
                                case 14: index = 3; break;
                                case 15: index = 4; break;
                                case 16: index = 5; break;
                                case 17: index = 6; break;
                                case 18: index = 7; break;
                                case 19: index = 8; break;
                                case 20: index = 9; break;
                                case int val when val >= 21: index = 10; break;
                            }
                            break;
                        case int len when len.Between(21, 40):
                            switch ((int)Value)
                            {
                                case 16: index = 0; break;
                                case 17: index = 1; break;
                                case 18: index = 2; break;
                                case 19: index = 3; break;
                                case 20: index = 4; break;
                                case 21: index = 5; break;
                                case 22: index = 6; break;
                                case 23: index = 7; break;
                                case int val when val.Between(24, 25): index = 8; break;
                                case int val when val.Between(26, 27): index = 9; break;
                                case int val when val >= 28: index = 10; break;
                            }
                            break;
                        case int len when len.Between(41, 60):
                            switch ((int)Value)
                            {
                                case 21: index = 0; break;
                                case 22: index = 2; break;
                                case 23: index = 4; break;
                                case int val when val.Between(24, 25): index = 5; break;
                                case int val when val.Between(26, 27): index = 6; break;
                                case 28: index = 7; break;
                                case 29: index = 8; break;
                                case 30: index = 9; break;
                                case int val when val >= 31: index = 10; break;
                            }
                            break;

                    }
                    break;
                case int speed when speed.Between(201, 250):
                    switch (Length)
                    {
                        case int len when len <= 20:
                            switch ((int)Value)
                            {
                                case 11: index = 0; break;
                                case 12: index = 1; break;
                                case 13: index = 3; break;
                                case 14: index = 4; break;
                                case 15: index = 5; break;
                                case 16: index = 6; break;
                                case 17: index = 8; break;
                                case 18: index = 9; break;
                                case int val when val >= 19: index = 10; break;
                            }
                            break;
                        case int len when len.Between(21, 40):
                            switch ((int)Value)
                            {
                                case int val when val.Between(13, 14): index = 0; break;
                                case 15: index = 1; break;
                                case 16: index = 2; break;
                                case 17: index = 3; break;
                                case 18: index = 4; break;
                                case int val when val.Between(19, 20): index = 5; break;
                                case 21: index = 6; break;
                                case 22: index = 7; break;
                                case 23: index = 8; break;
                                case 24: index = 9; break;
                                case int val when val >= 25: index = 10; break;
                            }
                            break;
                        case int len when len.Between(41, 60):
                            switch ((int)Value)
                            {
                                case 16: index = 0; break;
                                case 17: index = 1; break;
                                case 18: index = 3; break;
                                case 19: index = 4; break;
                                case int val when val.Between(20, 21): index = 5; break;
                                case int val when val.Between(22, 23): index = 6; break;
                                case int val when val.Between(24, 25): index = 7; break;
                                case 26: index = 8; break;
                                case 27: index = 9; break;
                                case int val when val >= 28: index = 10; break;
                            }
                            break;
                    }
                    break;
            }
            if (index >= 0)
            {
                if (Length <= 20)
                    point = to20[index];
                else if (Length.Between(21, 40))
                    point = f20to40[index];
                else if(Length.Between(21, 40))
                    point = f40to60[index];
            }
            return point;
        }
        /// <summary>
        /// Балловая оценка отступления по уровню
        /// </summary>
        /// <returns></returns>
        public int GetLevelPoint()
        {
            int point = 0;
            int[] f20t30 = { 1, 2, 3, 4, 6, 18, 20, 24, 32, 50, 100 };
            int[] f31t40 = { 2, 3, 4, 5, 7, 20, 22, 26, 34, 54, 110 };
            int[] f41t50 = { 3, 4, 5, 6, 8, 22, 24, 28, 36, 58, 120 };
            int[] f51t60 = { 4, 5, 6, 7, 9, 24, 26, 30, 38, 62, 130 };
            int[] f61t80 = { 5, 6, 7, 8, 10, 26, 28, 32, 40, 66, 140 };
            int[] f81t100 = { 6, 7, 8, 9, 11, 28, 30, 34, 42, 70, 150 };
            int[] f100 = { 8, 9, 11, 13, 15, 34, 40, 50, 58, 80, 150 };

            if ((Value >150) || (Value >75 && OnSwitch))
                return 100;
            int index = -1;

            switch (Speed)
            {
                case int speed when speed.Between(201, 250): 
                    switch((int)Value)
                    {
                        case int val when val.Between(8, 9): index = 0; break;
                        case 10: index = 1; break;
                        case 11: index = 2; break;
                        case 12: index = 3; break;
                        case 13: index = 4; break;
                        case 14: index = 5; break;
                        case 15: index = 7; break;
                        case 16: index = 9; break;
                        case int val when val >= 17: index = 10; break;
                    }
                    break;
                case int speed when speed.Between(141, 200):
                    switch ((int)Value)
                    {
                        case 10: index = 0; break;
                        case 11: index = 1; break;
                        case 12: index = 2; break;
                        case 13: index = 3; break;
                        case 14: index = 4; break;
                        case 15: index = 5; break;
                        case 16: index = 6; break;
                        case 17: index = 7; break;
                        case 18: index = 8; break;
                        case int val when val >= 19: index = 10; break;
                    }
                    break;
                case int speed when speed.Between(121, 140):
                    switch ((int)Value)
                    {
                        case int val when val.Between(11, 12): index = 0; break;
                        case 13: index = 1; break;
                        case 14: index = 2; break;
                        case 15: index = 3; break;
                        case 16: index = 4; break;
                        case 17: index = 5; break;
                        case 18: index = 6; break;
                        case 19: index = 7; break;
                        case 20: index = 9; break;
                        case int val when val >= 21: index = 10; break;
                    }
                    break;
                case int speed when speed.Between(61, 120):
                    switch ((int)Value)
                    {
                        case int val when val.Between(13, 14): index = 0; break;
                        case int val when val.Between(15, 16): index = 1; break;
                        case int val when val.Between(17, 18): index = 2; break;
                        case 19: index = 3; break;
                        case 20: index = 4; break;
                        case 21: index = 5; break;
                        case 22: index = 6; break;
                        case 23: index = 7; break;
                        case 24: index = 8; break;
                        case 25: index = 9; break;
                        case int val when val >= 21: index = 10; break;
                    }
                    break;
                case int speed when speed.Between(41, 60):
                    switch ((int)Value)
                    {
                        case int val when val.Between(15, 18): index = 0; break;
                        case int val when val.Between(19, 20): index = 1; break;
                        case int val when val.Between(21, 22): index = 2; break;
                        case int val when val.Between(23, 24): index = 3; break;
                        case 25: index = 4; break;
                        case 26: index = 5; break;
                        case 27: index = 6; break;
                        case 28: index = 7; break;
                        case 29: index = 8; break;
                        case 30: index = 9; break;
                        case int val when val >= 31: index = 10; break;
                    }
                    break;
                case int speed when speed.Between(16, 40):
                    switch ((int)Value)
                    {
                        case int val when val.Between(17, 19): index = 0; break;
                        case int val when val.Between(20, 22): index = 1; break;
                        case int val when val.Between(23, 25): index = 2; break;
                        case int val when val.Between(26, 28): index = 3; break;
                        case int val when val.Between(29, 30): index = 4; break;
                        case 26: index = 5; break;
                        case 27: index = 6; break;
                        case 28: index = 7; break;
                        case 29: index = 8; break;
                        case 30: index = 9; break;
                        case int val when val >= 31: index = 10; break;
                    }
                    break;
                case 15:
                    switch ((int)Value)
                    {
                        case int val when val.Between(17, 20): index = 0; break;
                        case int val when val.Between(21, 24): index = 1; break;
                        case int val when val.Between(25, 29): index = 2; break;
                        case int val when val.Between(30, 32): index = 3; break;
                        case int val when val.Between(33, 34): index = 4; break;
                        case int val when val.Between(35, 38): index = 5; break;
                        case int val when val.Between(39, 42): index = 6; break;
                        case int val when val.Between(43, 46): index = 7; break;
                        case int val when val.Between(47, 48): index = 8; break;
                        case int val when val.Between(49, 50): index = 9; break;
                        case int val when val >= 31: index = 10; break;
                    }
                    break;
            }
            if (index>-1)
                switch(Length)
                {
                    case int len when len.Between(17, 19): point = f20t30[index]; break;
                    case int len when len.Between(20, 30): point = f20t30[index]; break;
                    case int len when len.Between(31, 40): point = f20t30[index]; break;
                    case int len when len.Between(41, 50): point = f20t30[index]; break;
                    case int len when len.Between(51, 60): point = f20t30[index]; break;
                    case int len when len.Between(61, 70): point = f20t30[index]; break;
                    case int len when len.Between(81, 100): point = f20t30[index]; break;
                    case int len when len > 100 : point = f100[index]; break;
                }
            
            return point;

        }
    }
    public enum Location
    {
        OnStrightSection = 0, //прямой участок или кривой радиусом более 650 метров
        OnCurveSection = 1, // на кривом участке с радиусом 650 метров или менее
        OnSwitchSection = 2 // на рамном рельсе, в крестовине или контррельсовом рельсе стрелочного перевода
    }
    public enum DigressionType
    {
        Additional = 0, //доп
        Main = 1, // основные
        Video = 2 // видео
    }
    public enum Rating
    {
        О = 5,
        Х = 4,
        У = 3,
        Н = -5,
   
    }

}

