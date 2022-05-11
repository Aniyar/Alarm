using ALARm.Core.Report;

namespace ALARm.Core.AdditionalParameteres
{
    public class Gap : RdObject
    {
        public double Nominal = -1;
        public int TempByRegion = -999; //todo
        public int IsAdditional = 2;
        public double RailLen = 25; //todo

        public int id { get; set; }
        //report gaps aslan
        public string Fastening { get; set; }
        public string Threat_id { get; set; }
        public string Temp { get; set; }
        //public string PCHU { get; set; }


        public int Km { get; set; }
        public int Meter { get; set; }


        //rd_MOVEMENT ASLAN

        public string Movement { get; set; }
        public int Temperature { get; set; }
        public string Sector { get; set; }

        //left gap
        public int Oid { get; set; }
        public string Name { get; set; }
        public string Next_name { get; set; }
        public int Razn { get; set; }
        public Threat Threat { get; set; }

        public int Ball { get; set; }
        public int Zazor { get; set; }
        public int Y { get; set; }
        public int Y2 { get; set; }
        public string FullSpeed { get; set; }


        public int Fnum { get; set; }
        public int Local_fnum { get; set; }
        public int File_Id { get; set; }
        public DigName Dname { get; set; }
        public string AllowSpeed { get; set; }
        public string Next_otst { get; set; }

        //right gap
        public int R_zazor { get; set; }

        public int R_km { get; set; }
        public int R_meter { get; set; }
        public int R_oid { get; set; }
        public string R_name { get; set; }
        public string R_next_name { get; set; }
        public int R_razn { get; set; }
        public Threat R_threat { get; set; }

        /// <summary>
        /// Импульсы
        /// </summary>
        /// 

    




        public int R_y { get; set; }
        public int R_y2 { get; set; }
        public string R_ms { get; set; }
        public string R_fnum { get; set; }
        public string R_local_fnum { get; set; }
        public string R_file_id { get; set; }
        public DigName R_Dname { get; set; }
        public string R_allowSpeed { get; set; }
        public string R_fullSpeed { get; set; }

        //забег стыка
        public int Zabeg { get; set; }

        public int PassSpeed { get; set; }
        public int FreightSpeed { get; set; }

        public int Length { get; set; }
        public int Direction { get; set; }
        public int Picket { get; set; }
        public int Thread { get; set; }

        //aniyar
      
        public string Date { get; set; }
        public string EditReason { get; set; }
        public string Editor { get; set; }
        //
        public string Primech { get; set; }
        public string Pdb_section { get; set; }
        public string Fragment { get; set; }
        public string Vdop { get; set; }
        public string Vpz { get; set; }
        public string Otst { get; set; }
        public string Otst_l { get; set; }
        public string Otst_r { get; set; }
        


        public int Start { get; set; }
        public string Code { get; set; }
 
        public string Put { get; set; }
        public int Pch { get; set; }
        
        public int Frame_Number { get; set; }
        public int X { get; set; }
        public int W { get; set; }
        public int H { get; set; }

        public int X_r { get; set; }
        public int Y_r { get; set; }
        public int H_r { get; set; }

        public int Length2 { get; set; }
        public int File_id { get; set; }
       
        

        public Digression GetDigressions()
        {
            Digression digression = new Digression
            {
                FullSpeed = PassSpeed + "/" + FreightSpeed,
            };
            FullSpeed = PassSpeed + "/" + FreightSpeed;
            digression.Meter = Meter;
            digression.Kmetr = Km;
            digression.Threat = (Threat)Threat.Left;
            digression.Velich = Zazor;
            //Zazor = Zazor < 0 ? Zabeg == 0 ? -1 : Zazor : Zazor;
            switch (Zazor)
            {
                case 0:

                    digression.PassengerSpeedLimit = -1;
                    digression.FreightSpeedLimit = -1;

                    digression.AllowSpeed = "";
                    AllowSpeed = "";
                    digression.DigName = DigressionName.FusingGap;
                    Dname = digression.DigName;
                    break;
                case int gap when gap > 24 && gap <= 26:

                    digression.PassengerSpeedLimit = PassSpeed > 100 ? 100 : -1;
                    digression.FreightSpeedLimit = FreightSpeed > 100 ? 100 : -1;

                    digression.AllowSpeed = (PassSpeed > 100 ? "100" : "-") + "/" + (FreightSpeed > 100 ? "100" : "-");
                    AllowSpeed = digression.AllowSpeed;
                    digression.DigName = digression.AllowSpeed.Equals("-/-") ? DigressionName.AnomalisticGap : DigressionName.Gap;
                    Dname = digression.DigName;
                    break;
                case int gap when gap > 26 && gap <= 30:

                    digression.PassengerSpeedLimit = PassSpeed > 60 ? 60 : -1;
                    digression.FreightSpeedLimit = FreightSpeed > 60 ? 60 : -1;

                    digression.AllowSpeed = (PassSpeed > 60 ? "60" : "-") + "/" + (FreightSpeed > 60 ? "60" : "-");
                    AllowSpeed = digression.AllowSpeed;
                    digression.DigName = digression.AllowSpeed.Equals("-/-") ? DigressionName.AnomalisticGap : DigressionName.Gap;
                    Dname = digression.DigName;
                    break;
                case int gap when gap > 30 && gap <= 35:

                    digression.PassengerSpeedLimit = PassSpeed > 25 ? 25 : -1;
                    digression.FreightSpeedLimit = FreightSpeed > 25 ? 25 : -1;

                    digression.AllowSpeed = (PassSpeed > 25 ? "25" : "-") + "/" + (FreightSpeed > 25 ? "25" : "-");
                    AllowSpeed = digression.AllowSpeed;
                    digression.DigName = digression.AllowSpeed.Equals("-/-") ? DigressionName.AnomalisticGap : DigressionName.Gap;
                    Dname = digression.DigName;
                    break;
                case int gap when gap > 35:

                    digression.PassengerSpeedLimit = 0;
                    digression.FreightSpeedLimit = 0;

                    digression.AllowSpeed = "0/0";
                    AllowSpeed = digression.AllowSpeed;
                    digression.DigName = DigressionName.Gap;
                    Dname = digression.DigName;
                    break;
            }
            return digression;
        }
        public int GetPoint()
        {
            if (Otst == "З?")
            {
                return 20;
            }
            if (Otst == "З")
            {
                return 50;
            }
            return -1;
        }
        public Digression GetDigressions3()
        {
            Digression digression = new Digression
            {
                FullSpeed = PassSpeed + "/" + FreightSpeed
            };
            FullSpeed = PassSpeed + "/" + FreightSpeed;
            digression.Meter = Meter;
            digression.Kmetr = Km;
            digression.Threat = (Threat)Threat.Right;
            digression.Velich = R_zazor;
            //Zazor = Zazor < 0 ? Zabeg == 0 ? -1 : Zazor : Zazor;
            switch (R_zazor)
            {
                case 0:

                    digression.PassengerSpeedLimit = -1;
                    digression.FreightSpeedLimit = -1;

                    digression.AllowSpeed = "";
                    AllowSpeed = "";
                    digression.DigName = DigressionName.FusingGap;
                    Dname = digression.DigName;
                    break;
                case int gap when gap > 24 && gap <= 26:

                    digression.PassengerSpeedLimit = PassSpeed > 100 ? 100 : -1;
                    digression.FreightSpeedLimit = FreightSpeed > 100 ? 100 : -1;

                    digression.AllowSpeed = (PassSpeed > 100 ? "100" : "-") + "/" + (FreightSpeed > 100 ? "100" : "-");
                    AllowSpeed = digression.AllowSpeed;
                    digression.DigName = digression.AllowSpeed.Equals("-/-") ? DigressionName.AnomalisticGap : DigressionName.Gap;
                    Dname = digression.DigName;
                    break;
                case int gap when gap > 26 && gap <= 30:

                    digression.PassengerSpeedLimit = PassSpeed > 60 ? 60 : -1;
                    digression.FreightSpeedLimit = FreightSpeed > 60 ? 60 : -1;

                    digression.AllowSpeed = (PassSpeed > 60 ? "60" : "-") + "/" + (FreightSpeed > 60 ? "60" : "-");
                    AllowSpeed = digression.AllowSpeed;
                    digression.DigName = digression.AllowSpeed.Equals("-/-") ? DigressionName.AnomalisticGap : DigressionName.Gap;
                    Dname = digression.DigName;
                    break;
                case int gap when gap > 30 && gap <= 35:

                    digression.PassengerSpeedLimit = PassSpeed > 25 ? 25 : -1;
                    digression.FreightSpeedLimit = FreightSpeed > 25 ? 25 : -1;

                    digression.AllowSpeed = (PassSpeed > 25 ? "25" : "-") + "/" + (FreightSpeed > 25 ? "25" : "-");
                    AllowSpeed = digression.AllowSpeed;
                    digression.DigName = digression.AllowSpeed.Equals("-/-") ? DigressionName.AnomalisticGap : DigressionName.Gap;
                    Dname = digression.DigName;
                    break;
                case int gap when gap > 35:

                    digression.PassengerSpeedLimit = 0;
                    digression.FreightSpeedLimit = 0;

                    digression.AllowSpeed = "0/0";
                    AllowSpeed = digression.AllowSpeed;
                    digression.DigName = DigressionName.Gap;
                    Dname = digression.DigName;
                    break;
            }
            return digression;
        }
        public Digression GetDigressions2()
        {
            Digression digression = new Digression
            {
                R_FullSpeed = PassSpeed + "/" + FreightSpeed
            };
            R_fullSpeed = PassSpeed + "/" + FreightSpeed;
            digression.Meter = Meter;
            digression.Kmetr= Km;
            digression.Threat = (Threat)Threat.Right;
            digression.Velich = R_zazor;
            //R_zazor = R_zazor < 0 ? Zabeg == 0 ? -1 : R_zazor : R_zazor;
            switch (R_zazor)
            {
                case 0:
                    digression.R_AllowSpeed = "";
                    R_allowSpeed = digression.R_AllowSpeed;
                    digression.R_DigName = DigressionName.FusingGap;
                    R_Dname = digression.DigName;
                    break;
                case int gap when gap > 24 && gap <= 26:
                    digression.R_AllowSpeed = (PassSpeed > 100 ? "100" : "-") + "/" + (FreightSpeed > 100 ? "100" : "-");
                    R_allowSpeed = digression.R_AllowSpeed;
                    digression.R_DigName = digression.R_AllowSpeed.Equals("-/-") ? DigressionName.AnomalisticGap : DigressionName.Gap;
                    R_Dname = digression.DigName;
                    break;
                case int gap when gap > 26 && gap <= 30:
                    digression.R_AllowSpeed = (PassSpeed > 60 ? "60" : "-") + "/" + (FreightSpeed > 60 ? "60" : "-");
                    R_allowSpeed = digression.R_AllowSpeed;
                    digression.R_DigName = digression.R_AllowSpeed.Equals("-/-") ? DigressionName.AnomalisticGap : DigressionName.Gap;
                    R_Dname = digression.DigName;
                    break;
                case int gap when gap > 30 && gap <= 35:
                    digression.R_AllowSpeed = (PassSpeed > 25 ? "25" : "-") + "/" + (FreightSpeed > 25 ? "25" : "-");
                    R_allowSpeed = digression.R_AllowSpeed;
                    digression.R_DigName = digression.R_AllowSpeed.Equals("-/-") ? DigressionName.AnomalisticGap : DigressionName.Gap;
                    R_Dname = digression.DigName;
                    break;
                case int gap when gap > 35:
                    digression.R_AllowSpeed = "0/0";
                    R_allowSpeed = digression.R_AllowSpeed;
                    digression.R_DigName = DigressionName.Gap;
                    R_Dname = digression.DigName;
                    break;
            }
            return digression;
        }
        /// <summary>
        /// Температура рельсов, 0С, для климатических регионов с годовой амплитудой температуры рельсов *
        /// </summary>
        /// <returns></returns>
        public double GetNominalGapValueByTemp()
        {
            try
            {
                if (TempByRegion != null || TempByRegion != -999)
                {
                    switch (TempByRegion)
                    {
                        case int value when value > 100:
                            if (Temperature != null || Temperature != -999)
                            {
                                if (Temperature > 30)
                                    Nominal = 0;
                                else if (Temperature > 25 && Temperature <= 30)
                                    Nominal = 1.5;
                                else if (Temperature > 20 && Temperature <= 25)
                                    Nominal = 3.0;
                                else if (Temperature > 15 && Temperature <= 20)
                                    Nominal = 4.5;
                                else if (Temperature > 10 && Temperature <= 15)
                                    Nominal = 6;
                                else if (Temperature > 5 && Temperature <= 10)
                                    Nominal = 7.5;
                                else if (Temperature > 0 && Temperature <= 5)
                                    Nominal = 9;
                                else if (Temperature > -5 && Temperature <= 0)
                                    Nominal = 10.5;
                                else if (Temperature > -10 && Temperature <= -5)
                                    Nominal = 12;
                                else if (Temperature > -15 && Temperature <= -10)
                                    Nominal = 13.5;
                                else if (Temperature > -20 && Temperature <= -15)
                                    Nominal = 15;
                                else if (Temperature > -25 && Temperature <= -20)
                                    Nominal = 16.5;
                                else if (Temperature > -30 && Temperature <= -25)
                                    Nominal = 18;
                                else if (Temperature > -35 && Temperature <= -30)
                                    Nominal = 19.5;
                                else if (Temperature > -40 && Temperature <= -35)
                                    Nominal = 21;
                                else if (Temperature <= -40)
                                    Nominal = 22;
                            }
                            else
                            {
                                Nominal = -1;
                            }
                            return Nominal;

                        case int value when 80 <= value && value <= 100:
                            if (Temperature != null || Temperature != -999)
                            {
                                if (Temperature > 40)
                                    Nominal = 0;
                                else if (Temperature > 35 && Temperature <= 40)
                                    Nominal = 1.5;
                                else if (Temperature > 30 && Temperature <= 35)
                                    Nominal = 3;
                                else if (Temperature > 25 && Temperature <= 30)
                                    Nominal = 4.5;
                                else if (Temperature > 20 && Temperature <= 25)
                                    Nominal = 6;
                                else if (Temperature > 15 && Temperature <= 20)
                                    Nominal = 7.5;
                                else if (Temperature > 10 && Temperature <= 15)
                                    Nominal = 9;
                                else if (Temperature > 5 && Temperature <= 10)
                                    Nominal = 10.5;
                                else if (Temperature > 0 && Temperature <= 5)
                                    Nominal = 12;
                                else if (Temperature > -5 && Temperature <= 0)
                                    Nominal = 13.5;
                                else if (Temperature > -10 && Temperature <= -5)
                                    Nominal = 15;
                                else if (Temperature > -15 && Temperature <= -10)
                                    Nominal = 16.5;
                                else if (Temperature > -20 && Temperature <= -15)
                                    Nominal = 18;
                                else if (Temperature > -25 && Temperature <= -20)
                                    Nominal = 19.5;
                                else if (Temperature > -30 && Temperature <= -25)
                                    Nominal = 21;
                                else if (Temperature <= -30)
                                    Nominal = 22;
                            }
                            else
                            {
                                Nominal = -1;
                            }
                            return Nominal;
                        case int value when value < 80:
                            if (Temperature != null || Temperature != -999)
                            {
                                if (Temperature > 50)
                                    Nominal = 0;
                                else if (Temperature > 45 && Temperature <= 50)
                                    Nominal = 1.5;
                                else if (Temperature > 40 && Temperature <= 45)
                                    Nominal = 3;
                                else if (Temperature > 35 && Temperature <= 40)
                                    Nominal = 4.5;
                                else if (Temperature > 30 && Temperature <= 35)
                                    Nominal = 6;
                                else if (Temperature > 25 && Temperature <= 30)
                                    Nominal = 7.5;
                                else if (Temperature > 20 && Temperature <= 25)
                                    Nominal = 9;
                                else if (Temperature > 15 && Temperature <= 20)
                                    Nominal = 10.5;
                                else if (Temperature > 10 && Temperature <= 15)
                                    Nominal = 12;
                                else if (Temperature > 5 && Temperature <= 10)
                                    Nominal = 13.5;
                                else if (Temperature > 0 && Temperature <= 5)
                                    Nominal = 15;
                                else if (Temperature > -5 && Temperature <= 0)
                                    Nominal = 16.5;
                                else if (Temperature > -10 && Temperature <= -5)
                                    Nominal = 18;
                                else if (Temperature > -15 && Temperature <= -10)
                                    Nominal = 19.5;
                                else if (Temperature > -20 && Temperature <= -15)
                                    Nominal = 21;
                                else if (Temperature <= -20)
                                    Nominal = 22;
                            }
                            else
                            {
                                Nominal = -1;
                            }
                            return Nominal;
                        default:
                            return -1;
                    }
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }
        }
    }
    public class Heat : RdObject
    {
        public int Value { get; set; }
    }
}
