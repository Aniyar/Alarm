using ALARm.Core.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core.AdditionalParameteres
{
    //рельсовые скрепления
    public abstract class RailFastener : VideoObject
    {
        public abstract bool IsBroken();
        public List<Digression> Digressions { get; set; }
        //aslan
        public int Otstlperpendicular { get; set; }
        public string Pchu { get; set; }
        public string RailType { get; set; }
        public string Kns { get; set; }
        public string Station { get; set; }
        public string Fragment { get; set; }
        public string Otst { get; set; }
        public string Threat_id { get; set; }
        public int Type_id { get; set; }
        public string Fastening { get; set; }
        public string Notice { get; set; }

        public string Velich { get; set; }
        public string TrackClass { get; set; }
        public string Tripplan { get; set; }
        public string Vpz { get; set; }
        public string Vdop { get; set; }

        //
        public int Count { get; set; }
        public string Ots { get; set; }
        public string Norma { get; set; }
        public Location Location { get; set; }

        //public int Koord { get; set; }

        public abstract void AddDigression(Digression digression);

        public int GetMeter()
        {
            return (Pt - 1) * 100 + Mtr;
        }
        //Растояние между осями шпал (м)
        public static double distanceBetweenSleepers = 0.45;
        public static List<Digression> GetDigressions(MainParametersProcess videoProcess, long distanceId, IRdStructureRepository rdStructureRepository, IMainTrackStructureRepository mainTrackStructureRepository, string pch)
        {
            var digressions = new List<Digression>();
            void AddDigression(Digression digression)
            {
                var speed = mainTrackStructureRepository.GetMtoObjectsByCoord(videoProcess.Date_Vrem, digression.Km, MainTrackStructureConst.MtoSpeed, distanceId, videoProcess.TrackName) as List<Speed>;
                if (speed.Count > 0)
                {
                    if (speed[0].Passenger > int.Parse(digression.AllowSpeed.Split('/')[0]))
                        digressions.Add(digression);
                }
                else
                {
                    digressions.Add(digression);
                }
            }

            //получаем список негодных скреплений
            // GetBadRailFasteners(tripId, orderBySide, code, trackName, km);
            var badFasteners = rdStructureRepository.GetBadRailFasteners(videoProcess.Trip_id, true, videoProcess.TrackName);

            for (int i = 0; i < badFasteners.Count - 1; i++)
            {
                var found = false;
                //обработка узлов бесподкладочных скреплений
                for (int count = 5; count >= 3; count--)
                {
                    if (count < (badFasteners.Count - i))
                    {
                        Digression digression = GetKNSForGBR(distanceId, distanceBetweenSleepers, videoProcess, digressions, badFasteners, count, ref i, mainTrackStructureRepository);
                        if (digression != null)
                        {
                            AddDigression(digression);
                            found = true;
                            break;
                        }
                        if (found)
                            continue;
                    }
                }

                //обработка узлов подлкдаочных или анкерных скреплений
                //поумолчанию будем считать что скрепление расположено на прямом участке
                var fastenerLocation = Location.OnStrightSection;
                //проверяем лежит ли на кривом участке
                var curves = mainTrackStructureRepository.GetMtoObjectsByCoord(videoProcess.Date_Vrem, badFasteners[i].Km, MainTrackStructureConst.MtoStCurve, distanceId, videoProcess.TrackName, badFasteners[i].Mtr) as List<StCurve>;

                if (curves.Count > 0)
                {
                    if (curves[0].Radius > 650)
                        fastenerLocation = Location.OnCurveSection;
                }
                // или на стерлочном переводе, стрелочный перевод получаем по базе, может стоит доработать (ToDo) так чтобы определить фактический стрелочный перевода среди обнаруженных видеообъектов 
                var switches = mainTrackStructureRepository.GetMtoObjectsByCoord(videoProcess.Date_Vrem, badFasteners[i].Km, MainTrackStructureConst.MtoSwitch, distanceId, videoProcess.TrackName, badFasteners[i].Mtr) as List<Switch>;
                if (switches.Count > 0)
                    fastenerLocation = Location.OnSwitchSection;

                switch (fastenerLocation)
                {
                    case Location.OnStrightSection:
                        for (int count = 7; count >= 4; count--)
                        {
                            if (count < (badFasteners.Count - i))
                            {
                                var digression = GetKNSForStrightSection(distanceId, distanceBetweenSleepers, videoProcess, digressions, badFasteners, count, ref i, mainTrackStructureRepository);
                                if (digression != null)
                                {
                                    AddDigression(digression);
                                    found = true;
                                    break;
                                }
                            }
                        }
                        if (found)
                            continue;
                        break;
                    case Location.OnCurveSection:
                        for (int count = 6; count >= 4; count--)
                        {
                            if (count < (badFasteners.Count - i))
                            {
                                var digression = GetKNSForCurveSection(distanceId, distanceBetweenSleepers, digressions, badFasteners, count, ref i, curves[0]);
                                if (digression != null)
                                {
                                    AddDigression(digression);
                                    found = true;
                                    break;
                                }
                            }
                        }
                        if (found)
                            continue;
                        break;
                    case Location.OnSwitchSection:
                        for (int count = 5; count >= 2; count--)
                        {
                            if (count < (badFasteners.Count - i))
                            {
                                var digression = GetKNSForSwitchSection(distanceId, distanceBetweenSleepers, videoProcess, digressions, badFasteners, count, ref i, mainTrackStructureRepository, switches[0]);
                                if (digression != null)
                                {
                                    AddDigression(digression);
                                    found = true;
                                    break;
                                }
                            }
                        }
                        if (found)
                            continue;
                        break;
                }
            }
            return digressions;
        }
        /// <summary>
        ///  наличие негодных скреплений на стрелочном переводе
        /// </summary>
        /// <param name="distanceId">ПЧ</param>
        /// <param name="distanceBetweenSleepers">растояние между осями шпал</param>
        /// <param name="digressions">отступления</param>
        /// <param name="badFasteners">негодные скрепления</param>
        /// <param name="count">количество подряд негодных скреплений</param>
        /// <param name="i">количество подряд негодных скреплений</param>
        /// <returns>отступление или null</returns>
        private static Digression GetKNSForSwitchSection(long distanceId, double distanceBetweenSleepers, MainParametersProcess videoProcess, List<Digression> digressions, List<RailFastener> badFasteners, int count, ref int i, IMainTrackStructureRepository mainTrackStructureRepository, Switch @switch)
        {
            List<StCurve> curves = null;
            List<NormaWidth> normaWidths = null;
            Digression digression = null;
            if (Math.Abs(badFasteners[i].GetMeter() - badFasteners[i + count].GetMeter()) <= count * distanceBetweenSleepers)
            {
                digression = new Digression() {Primech = $"СП №{@switch.Num}", BraceType = badFasteners[i].ToString(), Location = Location.OnSwitchSection, Threat = badFasteners[i].Threat, Count = count, AllowSpeed = count == 5 ? "15/15" : (count == 4 ? "25/25" : (count == 3 ? "40/40" : "60/60")), DigName = DigressionName.KNS, Km = badFasteners[i].Km, Meter = badFasteners[i].Mtr };
                int degIndex = i;
                if (count == 5)
                {
                    i++;
                    while ((i < badFasteners.Count) && (Math.Abs(badFasteners[degIndex].GetMeter() - badFasteners[i].GetMeter()) <= Math.Abs(degIndex - i) * distanceBetweenSleepers))
                    {
                        digression.Count++;
                        i++;
                    }

                    curves = mainTrackStructureRepository.GetMtoObjectsByCoord(videoProcess.Date_Vrem, badFasteners[i].Km, MainTrackStructureConst.MtoStCurve, distanceId, videoProcess.TrackName, badFasteners[i].Mtr) as List<StCurve>;
                    if (curves.Count > 0)
                    {
                        digression.CurveRadius = (int)curves[0].Radius;
                        digression.Norma = curves[0].Width.ToString();
                        if (curves[0].Width >= 1545)
                        {
                            digression.AllowSpeed = "0/0";
                        }
                    }
                    else
                    {
                        normaWidths = mainTrackStructureRepository.GetMtoObjectsByCoord(videoProcess.Date_Vrem, badFasteners[i].Km, MainTrackStructureConst.MtoNormaWidth, distanceId, videoProcess.TrackName) as List<NormaWidth>;
                        if (normaWidths.Count > 0)
                        {
                            digression.Norma = normaWidths[0].Norma_Width.ToString();
                            digression.Location = Location.OnStrightSection;
                            if (normaWidths[0].Norma_Width >= 1545)
                            {
                                digression.AllowSpeed = "0/0";
                            }
                        }
                    }
                }
            }
            return digression;
        }

        /// <summary>
        /// наличие негодных скреплений на кривом участке радиусом 650 мм и менее
        /// </summary>
        /// <param name="distanceId">ПЧ</param>
        /// <param name="distanceBetweenSleepers">растояние между осями шпал</param>
        /// <param name="digressions">отступления</param>
        /// <param name="badFasteners">негодные скрепления</param>
        /// <param name="count">количество подряд негодных скреплений</param>
        /// <param name="i">количество подряд негодных скреплений</param>
        /// <param name="curve">кривой</param>
        /// <returns>отступление или null</returns>
        private static Digression GetKNSForCurveSection(long distanceId, double distanceBetweenSleepers, List<Digression> digressions, List<RailFastener> badFasteners, int count, ref int i, StCurve curve)
        {
            Digression digression = null;
            if (Math.Abs(badFasteners[i].GetMeter() - badFasteners[i + count].GetMeter()) <= count * distanceBetweenSleepers)
            {
                digression = new Digression() { BraceType = badFasteners[i].ToString(), CurveRadius = (int)curve.Radius, Location = Location.OnCurveSection, Threat = badFasteners[i].Threat, Count = count, AllowSpeed = count == 6 ? "15/15" : (count == 5 ? "25/25" : "40/40"), DigName = DigressionName.KNS, Km = badFasteners[i].Km, Meter = badFasteners[i].Mtr };
                int degIndex = i;
                if (count == 6)
                {
                    i++;
                    while ((i < badFasteners.Count) && (Math.Abs(badFasteners[degIndex].GetMeter() - badFasteners[i].GetMeter()) <= Math.Abs(degIndex - i) * distanceBetweenSleepers))
                    {
                        digression.Count++;
                        degIndex = i;
                        i++;
                    }
                    digression.Norma = curve.Width.ToString();
                    if (curve.Width >= 1545)
                    {
                        digression.AllowSpeed = "0/0";
                    }
                }
            }

            return digression;
        }

        /// <summary>
        /// наличие негодных скреплений на прямом или кривом участке радиусом более 650 мм
        /// </summary>
        /// <param name="distanceId">ПЧ</param>
        /// <param name="distanceBetweenSleepers">растояние между осями шпал</param>
        /// <param name="videoProcess">данные видеообработки</param>
        /// <param name="digressions">отступления</param>
        /// <param name="badFasteners">негодные скрепления</param>
        /// <param name="count">количество подряд негодных скреплений</param>
        /// <param name="i">глобальный идентификатор текущего скрепления</param>
        /// <returns>отступление или null</returns>
        private static Digression GetKNSForStrightSection(long distanceId, double distanceBetweenSleepers, MainParametersProcess videoProcess, List<Digression> digressions, List<RailFastener> badFasteners, int count, ref int i, IMainTrackStructureRepository mainTrackStructureRepository)
        {
            List<StCurve> curves = new List<StCurve>();
            List<NormaWidth> normaWidths = new List<NormaWidth>();
            Digression digression = null;
            if (Math.Abs(badFasteners[i].GetMeter() - badFasteners[i + count].GetMeter()) <= count * distanceBetweenSleepers)
            {
                digression = new Digression() { BraceType = badFasteners[i].ToString(), Location = Location.OnStrightSection, Threat = badFasteners[i].Threat, Count = count, AllowSpeed = count == 7 ? "15/15" : (count == 6 ? "25/25" : (count == 5 ? "40/40" : "60/60")), DigName = DigressionName.KNS, Km = badFasteners[i].Km, Meter = badFasteners[i].Mtr };
                int degIndex = i;
                if (count == 7)
                {
                    i++;
                    while ((i < badFasteners.Count-1) && (Math.Abs(badFasteners[degIndex].GetMeter() - badFasteners[i].GetMeter()) <= Math.Abs(degIndex - i) * distanceBetweenSleepers))
                    {
                        digression.Count++;
                        degIndex = i;
                        i++;
                    }
                    if ((i==0) || ((i>0) && (badFasteners[i].Km != badFasteners[i-1].Km)))
                        curves = mainTrackStructureRepository.GetMtoObjectsByCoord(videoProcess.Date_Vrem, badFasteners[i].Km, MainTrackStructureConst.MtoStCurve, badFasteners[i].TrackId) as List<StCurve>;
                    if (curves.Count > 0)
                    {
                        foreach (var curve in curves)
                        {
                            if (badFasteners[i].RealCoordinate.Between(curve.RealStartCoordinate,curve.RealFinalCoordinate))
                            {
                                digression.Norma = curve.Width.ToString();
                                digression.CurveRadius = (int)curve.Radius;
                                digression.Location = Location.OnCurveSection;
                                if (curve.Width >= 1545)
                                {
                                    digression.AllowSpeed = "0/0";
                                }
                            }
                        }
                    }
                    else
                    {
                        if ((i == 0) || ((i > 0) && (badFasteners[i].Km != badFasteners[i - 1].Km)))
                            normaWidths = mainTrackStructureRepository.GetMtoObjectsByCoord(videoProcess.Date_Vrem, badFasteners[i].Km, MainTrackStructureConst.MtoNormaWidth, badFasteners[i].TrackId) as List<NormaWidth>;
                        if (normaWidths.Count > 0)
                        {
                            foreach (var norma in normaWidths)
                            {
                                if (badFasteners[i].RealCoordinate.Between(norma.RealStartCoordinate,norma.RealFinalCoordinate))
                                {
                                    digression.Norma = norma.Norma_Width.ToString();
                                    digression.Location = Location.OnStrightSection;
                                    if (norma.Norma_Width >= 1545)
                                    {
                                        digression.AllowSpeed = "0/0";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return digression;
        }

        /// <summary>
        /// наличие негодных узлов на бесподкладочных скреплениях по одной нити 
        /// </summary>
        /// <param name="distanceId">ПЧ</param>
        /// <param name="distanceBetweenSleepers">растояние между осями шпал</param>
        /// <param name="videoProcess">данные видеообработки</param>
        /// <param name="digressions">отступления</param>
        /// <param name="badFasteners">негодные скрепления</param>
        /// <param name="count">количество подряд негодных скреплений</param>
        /// <param name="i">глобальный идентификатор текущего скрепления</param>
        /// <returns>отступление или null</returns>
        private static Digression GetKNSForGBR(long distanceId, double distanceBetweenSleepers, MainParametersProcess videoProcess, List<Digression> digressions, List<RailFastener> badFasteners, int count, ref int i, IMainTrackStructureRepository mainTrackStructureRepository)
        {
            List<StCurve> curves = null;
            List<NormaWidth> normaWidths = null;
            Digression digression = null;
            var allGBR = true;

            if (Math.Abs(badFasteners[i].GetMeter() - badFasteners[i + count].GetMeter()) <= count * distanceBetweenSleepers)
            {
                for (int j = i; j <= i + count; j++)
                {
                    if (badFasteners[j].Oid != (int)VideoObjectType.KB65_MissingClamp)
                        allGBR = false;
                }
                if (allGBR)
                {
                    digression = new Digression() { BraceType = badFasteners[i].ToString(), Threat = badFasteners[i].Threat, Count = count, AllowSpeed = count == 5 ? "15/15" : (count == 4 ? "25/25" : "40/40"), DigName = DigressionName.KNS, Km = badFasteners[i].Km, Meter = (badFasteners[i].Pt - 1) * 100 + badFasteners[i].Mtr };
                    int degIndex = i;
                    i = i + count;
                    if (count == 5)
                    {
                        i++;
                        while ((i < badFasteners.Count) && (Math.Abs(badFasteners[degIndex].GetMeter() - badFasteners[i].GetMeter()) <= Math.Abs(degIndex - i) * distanceBetweenSleepers))
                        {
                            digression.Count++;
                            degIndex = i;
                            i++;
                        }

                        curves = mainTrackStructureRepository.GetMtoObjectsByCoord(videoProcess.Date_Vrem, badFasteners[i].Km, MainTrackStructureConst.MtoStCurve, distanceId, videoProcess.TrackName, badFasteners[i].Mtr) as List<StCurve>;
                        if (curves.Count > 0)
                        {
                            digression.Norma = curves[0].Width.ToString();
                            digression.CurveRadius = (int)curves[0].Radius;
                            digression.Location = Location.OnCurveSection;
                            if (curves[0].Width >= 1545)
                            {
                                digression.AllowSpeed = "0/0";
                            }
                        }
                        else
                        {
                            normaWidths = mainTrackStructureRepository.GetMtoObjectsByCoord(videoProcess.Date_Vrem, badFasteners[i].Km, MainTrackStructureConst.MtoNormaWidth, distanceId, videoProcess.TrackName) as List<NormaWidth>;
                            if (normaWidths.Count > 0)
                            {
                                digression.Norma = normaWidths[0].Norma_Width.ToString();
                                digression.Location = Location.OnStrightSection;
                                if (normaWidths[0].Norma_Width >= 1545)
                                {
                                    digression.AllowSpeed = "0/0";
                                }
                            }
                        }
                    }
                }
            }
            return digression;
        }
    }

    public class D65 : RailFastener
    {
        public bool missingBaseplate { get; set; } = false;
        public bool brokenBaseplate { get; set; } = false;
        public bool missing2OrMoreMainSpikes { get; set; } = false;
        public override bool IsBroken()
        {
            if (missingBaseplate || brokenBaseplate || missing2OrMoreMainSpikes)
                return true;
            return IsMissingOrBrokenBaseplate() || IsOldBaseplateWhereMainSpikesDoNotReachTheRailBottom() || IsMissing2OrMoreMainSpikes();
        }
        public override string ToString()
        {
            return "Д65";
        }
        
        //отсутствие или излом подкладки
        private bool IsMissingOrBrokenBaseplate()
        {
            return false;
        }
        //при износе подкладки, при котором основные костыли не достают до подошвы рельса хотя бы с одной стороны
        private bool IsOldBaseplateWhereMainSpikesDoNotReachTheRailBottom()
        {
            return false;
        }
        //при отсутствии или изломе двух и более пришивочных (основных) костылей.
        private bool IsMissing2OrMoreMainSpikes()
        {
            return false;   
        }
        //при отсутствии обоих клемм или клеммных болтов
        private bool IsMissingBothRailClampsOrTensioningBolts()
        {
            return false;
        }

        public override void AddDigression(Digression digression)
        {
            if (Digressions == null)
                Digressions = new List<Digression>();
            Digressions.Add(digression);
            switch (digression.DigName.Name)
            {
                case string name when name == DigressionName.BrokenBasePlate.Name:
                    brokenBaseplate = true;
                    break;
                case string name when name == DigressionName.Missing2OrMoreMainSpikes.Name:
                    missing2OrMoreMainSpikes = true;
                    break;
            }
        }
    }
    public class KD65 : RailFastener
    {
        public bool missingClamp { get; set; } = false;
        public override bool IsBroken()
        {
            if (missingClamp)
                return true;
            return IsMissingOrBrokenBaseplate() || IsMissingOrBroken4Screws() || IsMissingBothRailClampsOrTensioningBolts();
        }
        //отсутствие или излом подкладки
        private bool IsMissingOrBrokenBaseplate()
        {
            return false;
        }
        //при отсутствии или изломе 4 шурупов КД
        private bool IsMissingOrBroken4Screws()
        {
            return false;
        }

        //при отсутствии обоих клемм или клеммных болтов
        private bool IsMissingBothRailClampsOrTensioningBolts()
        {
            return false;
        }

        public override void AddDigression(Digression digression)
        {
            if (Digressions == null)
                Digressions = new List<Digression>();
            Digressions.Add(digression);
            switch (digression.DigName.Name)
            {
                case string name when name == DigressionName.MissingClamp.Name:
                    missingClamp = true;
                    break;
            }
        }
        public override string ToString()
        {
            return "КД-65";
        }
    }
    public class KB65 : RailFastener
    {
        public bool missingClamp { get; set; } = false;
        public override bool IsBroken()
        {
            if (missingClamp)
                return true;
            return IsMissingOrBrokenBaseplate() || IsMissingBothRailClampsOrTensioningBolts() || IsMissingOrBrokenBothEmbededBolts();
        }
        //отсутствие или излом подкладки
        private bool IsMissingOrBrokenBaseplate()
        {
            return false;
        }
        //при отсутствии или изломе обоих закладных болтов КБ 4 шурупов КД
        private bool IsMissingOrBrokenBothEmbededBolts()
        {
            return false;
        }

        //при отсутствии обоих клемм или клеммных болтов
        private bool IsMissingBothRailClampsOrTensioningBolts()
        {
            return false;
        }

        public override void AddDigression(Digression digression)
        {
            if (Digressions == null)
                Digressions = new List<Digression>();
            Digressions.Add(digression);
            switch (digression.DigName.Name)
            {
                case string name when name == DigressionName.MissingClamp.Name:
                    missingClamp = true;
                    break;
            }
        }
        public override string ToString()
        {
            return "КБ-65";
        }
    }
    public class GBR : RailFastener
    {
        public bool missingArsClamp { get; set; } = false;
        public override void AddDigression(Digression digression)
        {
            if (Digressions == null)
                Digressions = new List<Digression>();
            Digressions.Add(digression);
            switch (digression.DigName.Name)
            {
                case string name when name == DigressionName.MissingClamp.Name:
                    missingArsClamp = true;
                    break;
            }
        }

        public override bool IsBroken()
        {
            if (missingArsClamp)
                return true;
            return isMissingOrBrokenClamp() || isMissingTensioningBoltsOrScrews();
        }
        //при отсутствии или изломе упругой клеммы
        private bool isMissingOrBrokenClamp()
        {
            return false;
        }
        private bool isMissingTensioningBoltsOrScrews()
        {
            return false;
        }
        public override string ToString()
        {
            return "ЖБР";
        }
    }
    public class NODDO2 : RailFastener
    {
        public override void AddDigression(Digression digression)
        {
            throw new NotImplementedException();
        }

        public override bool IsBroken()
        {
            return true;
        }
    }
    public class SKL : RailFastener
    {
        public bool missingArsClamp { get; set; } = false;
        public override bool IsBroken()
        {
            if (missingArsClamp)
                return true;
            return isMissingOrBrokenClamp() || isMissingTensioningBoltsOrScrews() || isMissingMonoRegulator();
        }
        private bool isMissingOrBrokenClamp()
        {
            return false;
        }
        private bool isMissingTensioningBoltsOrScrews()
        {
            return false;
        }
        private bool isMissingMonoRegulator()
        {
            return false;
        }

        public override void AddDigression(Digression digression)
        {
            if (Digressions == null)
                Digressions = new List<Digression>();
            Digressions.Add(digression);
            switch (digression.DigName.Name)
            {
                case string name when name == DigressionName.MissingClamp.Name:
                    missingArsClamp = true;
                    break;
            }
        }
        public override string ToString()
        {
            return "СКЛ";
        }
    }

    //Виды скреплений
    public enum FastenerEnum {
        GBR = 4, 
        SKL = 6, 
        D65 = 7, 
        KD65 = 8, 
        KB65 = 10, 
        DDO2=22, 
        NBDD02 = 26, 
        BROKEN_SKL = 28, 
        KB65WITHOUTCLAMP = 13,
        P350 = 29,
        kpp = 30
    }
}