using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALARm.Core.Report;

namespace ALARm.Core
{
    public class Sleepers : VideoObject
    {
        
    }

    /// <summary>
    /// Кустовая негодность и процент негодности шпал
    /// </summary>
    public class BadnessOfSleepers
    {
        private static int distanceBetweenSleepers = 54; //мм
        public static List<BadnessOfSleepers> GetBadnessOfSleepers(MainParametersProcess process, long trackId, IRdStructureRepository rdStructureRepository, IMainTrackStructureRepository mainTrackStructureRepository)
        {
            var badnessOfSleepers = new List<BadnessOfSleepers>();
            var badSleepers = rdStructureRepository.GetDigressions(process, new int[] { (int)DigressionName.LongitudinalCrack, (int)DigressionName.SplitsAtTheEnds, (int)DigressionName.PrickingOutPiecesOfWood, (int)DigressionName.FractureOfRCSleeper }).OrderBy(s => s.Km).ThenBy(s => s.Meter).ThenBy(s => s.Mm).ToList();
            int count = 0;
            double coords = 0;
            int meterMin = badSleepers.Min(s => s.Km * 10000 + s.Meter);
            int meterMax = badSleepers.Max(s => s.Km * 10000 + s.Meter);
            var speeds = new List<Speed>();
            var trackclasses = new List<TrackClass>();
            var braces = new List<RailsBrace>();
            var railstype = new List<RailsSections>();
            var curves = new List<StCurve>();
            var norma = new List<NormaWidth>();

            for (int i = 1; i < badSleepers.Count(); i++)
            {
                speeds = (List<Speed>)mainTrackStructureRepository.GetMtoObjectsByCoord(DateTime.Parse(process.Trip_date), badSleepers[i].Km, MainTrackStructureConst.MtoSpeed, trackId);
                trackclasses = (List<TrackClass>)mainTrackStructureRepository.GetMtoObjectsByCoord(DateTime.Parse(process.Trip_date), badSleepers[i].Km, MainTrackStructureConst.MtoTrackClass, trackId);
                braces = (List<RailsBrace>)mainTrackStructureRepository.GetMtoObjectsByCoord(DateTime.Parse(process.Trip_date), badSleepers[i].Km, MainTrackStructureConst.MtoRailsBrace, trackId);
                railstype = (List<RailsSections>)mainTrackStructureRepository.GetMtoObjectsByCoord(DateTime.Parse(process.Trip_date), badSleepers[i].Km, MainTrackStructureConst.MtoRailSection, trackId);
                curves = (List<StCurve>)mainTrackStructureRepository.GetMtoObjectsByCoord(DateTime.Parse(process.Trip_date), badSleepers[i].Km, MainTrackStructureConst.MtoStCurve, trackId);
                norma = (List<NormaWidth>)mainTrackStructureRepository.GetMtoObjectsByCoord(DateTime.Parse(process.Trip_date), badSleepers[i].Km, MainTrackStructureConst.MtoNormaWidth, trackId);

                if (badSleepers[i].Km == badSleepers[i - 1].Km)
                {
                    if (badSleepers[i].Meter == badSleepers[i - 1].Meter)
                    {
                        if (badSleepers[i].Mm - badSleepers[i - 1].Mm <= distanceBetweenSleepers)
                        {
                            coords = badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0;
                            count++;
                        }
                        else
                        {
                            if (count >= 6)
                            {
                                bool hasGap = rdStructureRepository.GetGapsBetweenCoords(process.Id, trackId, coords, badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0).Any();

                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                {
                                    if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Id > 3)
                                        hasGap = false;
                                }

                                if (hasGap)
                                {
                                    string speed, railtype, fastenertype, trackclass, tracktype, width;

                                    if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                    else
                                        speed = "нет данных";

                                    if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();
                                    else
                                        railtype = "нет данных";

                                    if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                    else
                                        fastenertype = "нет данных";

                                    if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                    else
                                        trackclass = "Нет данных (1)";

                                    if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                    else
                                        tracktype = "прямой";

                                    if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                    else
                                        width = "нет данных";

                                    badnessOfSleepers.Add(new BadnessOfSleepers {
                                        Km = Convert.ToInt32(coords) / 10000,
                                        Meter = Convert.ToInt32(coords) % 10000,
                                        digression = DigressionName.BushBadnessOfSleepers,
                                        Value = count,
                                        Speed = speed,
                                        AllowSpeed = "40/40",
                                        RailType = railtype,
                                        FastenerType = fastenertype,
                                        TrackClass = trackclass,
                                        TrackType = tracktype,
                                        Width = width,
                                        ItIsGap = true
                                    });
                                }

                                if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                {
                                    switch (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type_Id)
                                    {
                                        case 1:
                                        case 2:
                                        case 3:
                                            {
                                                string speed, railtype, fastenertype, trackclass, tracktype, width, allowspeed;

                                                if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                                else
                                                    speed = "нет данных";

                                                railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                                if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                                else
                                                    fastenertype = "нет данных";

                                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                                else
                                                    trackclass = "Нет данных (1)";

                                                if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                                else
                                                    tracktype = "прямой";

                                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                {
                                                    width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();

                                                    if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width >= 1545)
                                                        allowspeed = "0/0";
                                                    else
                                                        allowspeed = "15/15";
                                                }
                                                else
                                                {
                                                    width = "нет данных";
                                                    allowspeed = "15/15";
                                                }

                                                badnessOfSleepers.Add(new BadnessOfSleepers {
                                                    Km = Convert.ToInt32(coords) / 10000,
                                                    Meter = Convert.ToInt32(coords) % 10000,
                                                    digression = DigressionName.BushBadnessOfSleepers,
                                                    Value = count,
                                                    Speed = speed,
                                                    AllowSpeed = allowspeed,
                                                    RailType = railtype,
                                                    FastenerType = fastenertype,
                                                    TrackClass = trackclass,
                                                    TrackType = tracktype,
                                                    Width = width,
                                                    ItIsGap = true
                                                });
                                            }
                                            break;

                                        case 4:
                                        case 5:
                                            {
                                                string speed, railtype, fastenertype, trackclass, tracktype, width, allowspeed;

                                                if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                                else
                                                    speed = "нет данных";

                                                railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                                if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                                else
                                                    fastenertype = "нет данных";

                                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                                else
                                                    trackclass = "Нет данных (1)";

                                                if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                                else
                                                    tracktype = "прямой";

                                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                {
                                                    width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();

                                                    if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width >= 1545)
                                                        allowspeed = "0/0";
                                                    else
                                                        allowspeed = "15/15";
                                                }
                                                else
                                                {
                                                    width = "нет данных";
                                                    allowspeed = "15/15";
                                                }

                                                badnessOfSleepers.Add(new BadnessOfSleepers {
                                                    Km = Convert.ToInt32(coords) / 10000,
                                                    Meter = Convert.ToInt32(coords) % 10000,
                                                    digression = DigressionName.BushBadnessOfSleepers,
                                                    Value = count,
                                                    Speed = speed,
                                                    AllowSpeed = allowspeed,
                                                    RailType = railtype,
                                                    FastenerType = fastenertype,
                                                    TrackClass = trackclass,
                                                    TrackType = tracktype,
                                                    Width = width,
                                                    ItIsGap = true
                                                });
                                            }
                                            break;
                                    }
                                }

                                count = 0;
                                coords = 0.0;
                            }
                            else if (count >= 5)
                            {
                                bool hasGap = rdStructureRepository.GetGapsBetweenCoords(process.Id, trackId, coords, badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0).Any();

                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                {
                                    if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Id > 3)
                                        hasGap = false;
                                }

                                if (hasGap)
                                {
                                    string speed, railtype, fastenertype, trackclass, tracktype, width;

                                    if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                    else
                                        speed = "нет данных";

                                    if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();
                                    else
                                        railtype = "нет данных";

                                    if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                    else
                                        fastenertype = "нет данных";

                                    if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                    else
                                        trackclass = "Нет данных (1)";

                                    if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                    else
                                        tracktype = "прямой";

                                    if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                    else
                                        width = "нет данных";

                                    badnessOfSleepers.Add(new BadnessOfSleepers {
                                        Km = Convert.ToInt32(coords) / 10000,
                                        Meter = Convert.ToInt32(coords) % 10000,
                                        digression = DigressionName.BushBadnessOfSleepers,
                                        Value = count,
                                        Speed = speed,
                                        AllowSpeed = "40/40",
                                        RailType = railtype,
                                        FastenerType = fastenertype,
                                        TrackClass = trackclass,
                                        TrackType = tracktype,
                                        Width = width,
                                        ItIsGap = true
                                    });
                                }

                                if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                {
                                    switch (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type_Id)
                                    {
                                        case 1:
                                        case 2:
                                        case 3:
                                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).Any())
                                            {
                                                string speed, railtype, fastenertype, trackclass, tracktype, width, allowspeed;

                                                if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                                else
                                                    speed = "нет данных";

                                                railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                                if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                                else
                                                    fastenertype = "нет данных";

                                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                                else
                                                    trackclass = "Нет данных (1)";

                                                if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).Any())
                                                    tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                                else
                                                    tracktype = "прямой";

                                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                {
                                                    width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();

                                                    if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width >= 1545)
                                                        allowspeed = "0/0";
                                                    else
                                                        allowspeed = "15/15";
                                                }
                                                else
                                                {
                                                    width = "нет данных";
                                                    allowspeed = "15/15";
                                                }

                                                badnessOfSleepers.Add(new BadnessOfSleepers {
                                                    Km = Convert.ToInt32(coords) / 10000,
                                                    Meter = Convert.ToInt32(coords) % 10000,
                                                    digression = DigressionName.BushBadnessOfSleepers,
                                                    Value = count,
                                                    Speed = speed,
                                                    AllowSpeed = allowspeed,
                                                    RailType = railtype,
                                                    FastenerType = fastenertype,
                                                    TrackClass = trackclass,
                                                    TrackType = tracktype,
                                                    Width = width,
                                                    ItIsGap = true
                                                });
                                            }
                                            else
                                            {
                                                string speed, railtype, fastenertype, trackclass, tracktype, width;

                                                if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                                else
                                                    speed = "нет данных";

                                                railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                                if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                                else
                                                    fastenertype = "нет данных";

                                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                                else
                                                    trackclass = "Нет данных (1)";

                                                if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                                else
                                                    tracktype = "прямой";

                                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                                else
                                                    width = "нет данных";

                                                badnessOfSleepers.Add(new BadnessOfSleepers {
                                                    Km = Convert.ToInt32(coords) / 10000,
                                                    Meter = Convert.ToInt32(coords) % 10000,
                                                    digression = DigressionName.BushBadnessOfSleepers,
                                                    Value = count,
                                                    Speed = speed,
                                                    AllowSpeed = "40/25",
                                                    RailType = railtype,
                                                    FastenerType = fastenertype,
                                                    TrackClass = trackclass,
                                                    TrackType = tracktype,
                                                    Width = width,
                                                    ItIsGap = true
                                                });
                                            }
                                            break;

                                        case 4:
                                        case 5:
                                            {
                                                string speed, railtype, fastenertype, trackclass, tracktype, width, allowspeed;

                                                if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                                else
                                                    speed = "нет данных";

                                                railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                                if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                                else
                                                    fastenertype = "нет данных";

                                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                                else
                                                    trackclass = "Нет данных (1)";

                                                if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                                else
                                                    tracktype = "прямой";

                                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                {
                                                    width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();

                                                    if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width >= 1545)
                                                        allowspeed = "0/0";
                                                    else
                                                        allowspeed = "15/15";
                                                }
                                                else
                                                {
                                                    width = "нет данных";
                                                    allowspeed = "15/15";
                                                }

                                                badnessOfSleepers.Add(new BadnessOfSleepers {
                                                    Km = Convert.ToInt32(coords) / 10000,
                                                    Meter = Convert.ToInt32(coords) % 10000,
                                                    digression = DigressionName.BushBadnessOfSleepers,
                                                    Value = count,
                                                    Speed = speed,
                                                    AllowSpeed = allowspeed,
                                                    RailType = railtype,
                                                    FastenerType = fastenertype,
                                                    TrackClass = trackclass,
                                                    TrackType = tracktype,
                                                    Width = width,
                                                    ItIsGap = true
                                                });
                                            }
                                            break;
                                    }
                                }

                                count = 0;
                                coords = 0.0;
                            }
                            else if (count >= 4)
                            {
                                bool hasGap = rdStructureRepository.GetGapsBetweenCoords(process.Id, trackId, coords, badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0).Any();

                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                {
                                    if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Id > 3)
                                        hasGap = false;
                                }

                                if (hasGap)
                                {
                                    string speed, railtype, fastenertype, trackclass, tracktype, width;

                                    if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                    else
                                        speed = "нет данных";

                                    if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();
                                    else
                                        railtype = "нет данных";

                                    if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                    else
                                        fastenertype = "нет данных";

                                    if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                    else
                                        trackclass = "Нет данных (1)";

                                    if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                    else
                                        tracktype = "прямой";

                                    if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                    else
                                        width = "нет данных";

                                    badnessOfSleepers.Add(new BadnessOfSleepers {
                                        Km = Convert.ToInt32(coords) / 10000,
                                        Meter = Convert.ToInt32(coords) % 10000,
                                        digression = DigressionName.BushBadnessOfSleepers,
                                        Value = count,
                                        Speed = speed,
                                        AllowSpeed = "40/40",
                                        RailType = railtype,
                                        FastenerType = fastenertype,
                                        TrackClass = trackclass,
                                        TrackType = tracktype,
                                        Width = width,
                                        ItIsGap = true
                                    });
                                }

                                if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                {
                                    switch (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type_Id)
                                    {
                                        case 1:
                                        case 2:
                                        case 3:
                                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).Any())
                                            {
                                                string speed, railtype, fastenertype, trackclass, tracktype, width;

                                                if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                                else
                                                    speed = "нет данных";

                                                railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                                if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                                else
                                                    fastenertype = "нет данных";

                                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                                else
                                                    trackclass = "Нет данных (1)";

                                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).First().Radius).ToString();

                                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                                else
                                                    width = "нет данных";

                                                badnessOfSleepers.Add(new BadnessOfSleepers {
                                                    Km = Convert.ToInt32(coords) / 10000,
                                                    Meter = Convert.ToInt32(coords) % 10000,
                                                    digression = DigressionName.BushBadnessOfSleepers,
                                                    Value = count,
                                                    Speed = speed,
                                                    AllowSpeed = "40/25",
                                                    RailType = railtype,
                                                    FastenerType = fastenertype,
                                                    TrackClass = trackclass,
                                                    TrackType = tracktype,
                                                    Width = width,
                                                    ItIsGap = true
                                                });
                                            }
                                            else
                                            {
                                                string speed, railtype, fastenertype, trackclass, tracktype, width;

                                                if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                                else
                                                    speed = "нет данных";

                                                railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                                if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                                else
                                                    fastenertype = "нет данных";

                                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                                else
                                                    trackclass = "Нет данных (1)";

                                                if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                                else
                                                    tracktype = "прямой";

                                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                                else
                                                    width = "нет данных";

                                                badnessOfSleepers.Add(new BadnessOfSleepers {
                                                    Km = Convert.ToInt32(coords) / 10000,
                                                    Meter = Convert.ToInt32(coords) % 10000,
                                                    digression = DigressionName.BushBadnessOfSleepers,
                                                    Value = count,
                                                    Speed = speed,
                                                    AllowSpeed = "60/40",
                                                    RailType = railtype,
                                                    FastenerType = fastenertype,
                                                    TrackClass = trackclass,
                                                    TrackType = tracktype,
                                                    Width = width,
                                                    ItIsGap = true
                                                });
                                            }
                                            break;

                                        case 4:
                                        case 5:
                                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).Any())
                                            {
                                                string speed, railtype, fastenertype, trackclass, tracktype, width, allowspeed;

                                                if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                                else
                                                    speed = "нет данных";

                                                railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                                if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                                else
                                                    fastenertype = "нет данных";

                                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                                else
                                                    trackclass = "Нет данных (1)";

                                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).First().Radius).ToString();

                                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                {
                                                    width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();

                                                    if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width >= 1545)
                                                        allowspeed = "0/0";
                                                    else
                                                        allowspeed = "15/15";
                                                }
                                                else
                                                {
                                                    width = "нет данных";
                                                    allowspeed = "15/15";
                                                }

                                                badnessOfSleepers.Add(new BadnessOfSleepers {
                                                    Km = Convert.ToInt32(coords) / 10000,
                                                    Meter = Convert.ToInt32(coords) % 10000,
                                                    digression = DigressionName.BushBadnessOfSleepers,
                                                    Value = count,
                                                    Speed = speed,
                                                    AllowSpeed = allowspeed,
                                                    RailType = railtype,
                                                    FastenerType = fastenertype,
                                                    TrackClass = trackclass,
                                                    TrackType = tracktype,
                                                    Width = width,
                                                    ItIsGap = true
                                                });
                                            }
                                            else
                                            {
                                                string speed, railtype, fastenertype, trackclass, tracktype, width;

                                                if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                                else
                                                    speed = "нет данных";

                                                railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                                if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                                else
                                                    fastenertype = "нет данных";

                                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                                else
                                                    trackclass = "Нет данных (1)";

                                                if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                                else
                                                    tracktype = "прямой";

                                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                                else
                                                    width = "нет данных";

                                                badnessOfSleepers.Add(new BadnessOfSleepers {
                                                    Km = Convert.ToInt32(coords) / 10000,
                                                    Meter = Convert.ToInt32(coords) % 10000,
                                                    digression = DigressionName.BushBadnessOfSleepers,
                                                    Value = count,
                                                    Speed = speed,
                                                    AllowSpeed = "40/25",
                                                    RailType = railtype,
                                                    FastenerType = fastenertype,
                                                    TrackClass = trackclass,
                                                    TrackType = tracktype,
                                                    Width = width,
                                                    ItIsGap = true
                                                });
                                            }
                                            break;
                                    }
                                }

                                count = 0;
                                coords = 0.0;
                            }
                            else if (count >= 3)
                            {
                                bool hasGap = rdStructureRepository.GetGapsBetweenCoords(process.Id, trackId, coords, badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0).Any();

                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                {
                                    if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Id > 3)
                                        hasGap = false;
                                }

                                if (hasGap)
                                {
                                    string speed, railtype, fastenertype, trackclass, tracktype, width;

                                    if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                    else
                                        speed = "нет данных";

                                    if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();
                                    else
                                        railtype = "нет данных";

                                    if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                    else
                                        fastenertype = "нет данных";

                                    if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                    else
                                        trackclass = "Нет данных (1)";

                                    if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                    else
                                        tracktype = "прямой";

                                    if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                    else
                                        width = "нет данных";

                                    badnessOfSleepers.Add(new BadnessOfSleepers {
                                        Km = Convert.ToInt32(coords) / 10000,
                                        Meter = Convert.ToInt32(coords) % 10000,
                                        digression = DigressionName.BushBadnessOfSleepers,
                                        Value = count,
                                        Speed = speed,
                                        AllowSpeed = "40/40",
                                        RailType = railtype,
                                        FastenerType = fastenertype,
                                        TrackClass = trackclass,
                                        TrackType = tracktype,
                                        Width = width,
                                        ItIsGap = true
                                    });
                                }

                                if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                {
                                    switch (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type_Id)
                                    {
                                        case 4:
                                        case 5:
                                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).Any())
                                            {
                                                string speed, railtype, fastenertype, trackclass, tracktype, width;

                                                if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                                else
                                                    speed = "нет данных";

                                                railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                                if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                                else
                                                    fastenertype = "нет данных";

                                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                                else
                                                    trackclass = "Нет данных (1)";

                                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).First().Radius).ToString();

                                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                                else
                                                    width = "нет данных";

                                                badnessOfSleepers.Add(new BadnessOfSleepers {
                                                    Km = Convert.ToInt32(coords) / 10000,
                                                    Meter = Convert.ToInt32(coords) % 10000,
                                                    digression = DigressionName.BushBadnessOfSleepers,
                                                    Value = count,
                                                    Speed = speed,
                                                    AllowSpeed = "40/25",
                                                    RailType = railtype,
                                                    FastenerType = fastenertype,
                                                    TrackClass = trackclass,
                                                    TrackType = tracktype,
                                                    Width = width,
                                                    ItIsGap = true
                                                });
                                            }
                                            else
                                            {
                                                string speed, railtype, fastenertype, trackclass, tracktype, width;

                                                if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                                else
                                                    speed = "нет данных";
                                                    
                                                railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                                if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                                else
                                                    fastenertype = "нет данных";

                                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                                else
                                                    trackclass = "Нет данных (1)";

                                                if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                                else
                                                    tracktype = "прямой";

                                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                    width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                                else
                                                    width = "нет данных";

                                                badnessOfSleepers.Add(new BadnessOfSleepers {
                                                    Km = Convert.ToInt32(coords) / 10000,
                                                    Meter = Convert.ToInt32(coords) % 10000,
                                                    digression = DigressionName.BushBadnessOfSleepers,
                                                    Value = count,
                                                    Speed = speed,
                                                    AllowSpeed = "50/40",
                                                    RailType = railtype,
                                                    FastenerType = fastenertype,
                                                    TrackClass = trackclass,
                                                    TrackType = tracktype,
                                                    Width = width,
                                                    ItIsGap = true
                                                });
                                            }
                                            break;
                                    }
                                }

                                count = 0;
                                coords = 0.0;
                            }
                            else if (count >= 2)
                            {
                                bool hasGap = rdStructureRepository.GetGapsBetweenCoords(process.Id, trackId, coords, badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0).Any();
                                
                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                {
                                    if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Id > 3)
                                        hasGap = false;
                                }

                                if (hasGap)
                                {
                                    string speed, railtype, fastenertype, trackclass, tracktype, width;

                                    if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                    else
                                        speed = "нет данных";

                                    if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();
                                    else
                                        railtype = "нет данных";

                                    if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                    else
                                        fastenertype = "нет данных";

                                    if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                    else
                                        trackclass = "Нет данных (1)";

                                    if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                    else
                                        tracktype = "прямой";

                                    if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                    else
                                        width = "нет данных";

                                    badnessOfSleepers.Add(new BadnessOfSleepers {
                                        Km = Convert.ToInt32(coords) / 10000,
                                        Meter = Convert.ToInt32(coords) % 10000,
                                        digression = DigressionName.BushBadnessOfSleepers,
                                        Value = count,
                                        Speed = speed,
                                        AllowSpeed = "40/40",
                                        RailType = railtype,
                                        FastenerType= fastenertype,
                                        TrackClass = trackclass,
                                        TrackType = tracktype,
                                        Width = width,
                                        ItIsGap = true
                                    });
                                }

                                count = 0;
                                coords = 0.0;
                            }
                        }
                    }
                    else if (badSleepers[i].Meter - badSleepers[i - 1].Meter == 1 && badSleepers[i - 1].Mm >= 940 && badSleepers[i - 1].Mm <= 60 && 1000 - badSleepers[i - 1].Mm + badSleepers[i - 1].Mm <= distanceBetweenSleepers)
                    {
                        if (count >= 6)
                        {
                            bool hasGap = rdStructureRepository.GetGapsBetweenCoords(process.Id, trackId, coords, badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0).Any();

                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                            {
                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Id > 3)
                                    hasGap = false;
                            }

                            if (hasGap)
                            {
                                string speed, railtype, fastenertype, trackclass, tracktype, width;

                                if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                else
                                    speed = "нет данных";

                                if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();
                                else
                                    railtype = "нет данных";

                                if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                else
                                    fastenertype = "нет данных";

                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                else
                                    trackclass = "Нет данных (1)";

                                if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                else
                                    tracktype = "прямой";

                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                else
                                    width = "нет данных";

                                badnessOfSleepers.Add(new BadnessOfSleepers {
                                    Km = Convert.ToInt32(coords) / 10000,
                                    Meter = Convert.ToInt32(coords) % 10000,
                                    digression = DigressionName.BushBadnessOfSleepers,
                                    Value = count,
                                    Speed = speed,
                                    AllowSpeed = "40/40",
                                    RailType = railtype,
                                    FastenerType = fastenertype,
                                    TrackClass = trackclass,
                                    TrackType = tracktype,
                                    Width = width,
                                    ItIsGap = true
                                });
                            }

                            if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                            {
                                switch (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type_Id)
                                {
                                    case 1:
                                    case 2:
                                    case 3:
                                        {
                                            string speed, railtype, fastenertype, trackclass, tracktype, width, allowspeed;

                                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                            else
                                                speed = "нет данных";

                                            railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                            else
                                                fastenertype = "нет данных";

                                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                            else
                                                trackclass = "Нет данных (1)";

                                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                            else
                                                tracktype = "прямой";

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            {
                                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();

                                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width >= 1545)
                                                    allowspeed = "0/0";
                                                else
                                                    allowspeed = "15/15";
                                            }
                                            else
                                            {
                                                width = "нет данных";
                                                allowspeed = "15/15";
                                            }

                                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                                Km = Convert.ToInt32(coords) / 10000,
                                                Meter = Convert.ToInt32(coords) % 10000,
                                                digression = DigressionName.BushBadnessOfSleepers,
                                                Value = count,
                                                Speed = speed,
                                                AllowSpeed = allowspeed,
                                                RailType = railtype,
                                                FastenerType = fastenertype,
                                                TrackClass = trackclass,
                                                TrackType = tracktype,
                                                Width = width,
                                                ItIsGap = true
                                            });
                                        }
                                        break;

                                    case 4:
                                    case 5:
                                        {
                                            string speed, railtype, fastenertype, trackclass, tracktype, width, allowspeed;

                                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                            else
                                                speed = "нет данных";

                                            railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                            else
                                                fastenertype = "нет данных";

                                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                            else
                                                trackclass = "Нет данных (1)";

                                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                            else
                                                tracktype = "прямой";

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            {
                                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();

                                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width >= 1545)
                                                    allowspeed = "0/0";
                                                else
                                                    allowspeed = "15/15";
                                            }
                                            else
                                            {
                                                width = "нет данных";
                                                allowspeed = "15/15";
                                            }

                                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                                Km = Convert.ToInt32(coords) / 10000,
                                                Meter = Convert.ToInt32(coords) % 10000,
                                                digression = DigressionName.BushBadnessOfSleepers,
                                                Value = count,
                                                Speed = speed,
                                                AllowSpeed = allowspeed,
                                                RailType = railtype,
                                                FastenerType = fastenertype,
                                                TrackClass = trackclass,
                                                TrackType = tracktype,
                                                Width = width,
                                                ItIsGap = true
                                            });
                                        }
                                        break;
                                }
                            }

                            count = 0;
                            coords = 0.0;
                        }
                        else if (count >= 5)
                        {
                            bool hasGap = rdStructureRepository.GetGapsBetweenCoords(process.Id, trackId, coords, badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0).Any();

                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                            {
                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Id > 3)
                                    hasGap = false;
                            }

                            if (hasGap)
                            {
                                string speed, railtype, fastenertype, trackclass, tracktype, width;

                                if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                else
                                    speed = "нет данных";

                                if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();
                                else
                                    railtype = "нет данных";

                                if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                else
                                    fastenertype = "нет данных";

                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                else
                                    trackclass = "Нет данных (1)";

                                if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                else
                                    tracktype = "прямой";

                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                else
                                    width = "нет данных";

                                badnessOfSleepers.Add(new BadnessOfSleepers {
                                    Km = Convert.ToInt32(coords) / 10000,
                                    Meter = Convert.ToInt32(coords) % 10000,
                                    digression = DigressionName.BushBadnessOfSleepers,
                                    Value = count,
                                    Speed = speed,
                                    AllowSpeed = "40/40",
                                    RailType = railtype,
                                    FastenerType = fastenertype,
                                    TrackClass = trackclass,
                                    TrackType = tracktype,
                                    Width = width,
                                    ItIsGap = true
                                });
                            }

                            if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                            {
                                switch (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type_Id)
                                {
                                    case 1:
                                    case 2:
                                    case 3:
                                        if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).Any())
                                        {
                                            string speed, railtype, fastenertype, trackclass, tracktype, width, allowspeed;

                                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                            else
                                                speed = "нет данных";

                                            railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                            else
                                                fastenertype = "нет данных";

                                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                            else
                                                trackclass = "Нет данных (1)";

                                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).Any())
                                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                            else
                                                tracktype = "прямой";

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            {
                                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();

                                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width >= 1545)
                                                    allowspeed = "0/0";
                                                else
                                                    allowspeed = "15/15";
                                            }
                                            else
                                            {
                                                width = "нет данных";
                                                allowspeed = "15/15";
                                            }

                                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                                Km = Convert.ToInt32(coords) / 10000,
                                                Meter = Convert.ToInt32(coords) % 10000,
                                                digression = DigressionName.BushBadnessOfSleepers,
                                                Value = count,
                                                Speed = speed,
                                                AllowSpeed = allowspeed,
                                                RailType = railtype,
                                                FastenerType = fastenertype,
                                                TrackClass = trackclass,
                                                TrackType = tracktype,
                                                Width = width,
                                                ItIsGap = true
                                            });
                                        }
                                        else
                                        {
                                            string speed, railtype, fastenertype, trackclass, tracktype, width;

                                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                            else
                                                speed = "нет данных";

                                            railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                            else
                                                fastenertype = "нет данных";

                                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                            else
                                                trackclass = "Нет данных (1)";

                                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                            else
                                                tracktype = "прямой";

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                            else
                                                width = "нет данных";

                                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                                Km = Convert.ToInt32(coords) / 10000,
                                                Meter = Convert.ToInt32(coords) % 10000,
                                                digression = DigressionName.BushBadnessOfSleepers,
                                                Value = count,
                                                Speed = speed,
                                                AllowSpeed = "40/25",
                                                RailType = railtype,
                                                FastenerType = fastenertype,
                                                TrackClass = trackclass,
                                                TrackType = tracktype,
                                                Width = width,
                                                ItIsGap = true
                                            });
                                        }
                                        break;

                                    case 4:
                                    case 5:
                                        {
                                            string speed, railtype, fastenertype, trackclass, tracktype, width, allowspeed;

                                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                            else
                                                speed = "нет данных";

                                            railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                            else
                                                fastenertype = "нет данных";

                                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                            else
                                                trackclass = "Нет данных (1)";

                                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                            else
                                                tracktype = "прямой";

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            {
                                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();

                                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width >= 1545)
                                                    allowspeed = "0/0";
                                                else
                                                    allowspeed = "15/15";
                                            }
                                            else
                                            {
                                                width = "нет данных";
                                                allowspeed = "15/15";
                                            }

                                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                                Km = Convert.ToInt32(coords) / 10000,
                                                Meter = Convert.ToInt32(coords) % 10000,
                                                digression = DigressionName.BushBadnessOfSleepers,
                                                Value = count,
                                                Speed = speed,
                                                AllowSpeed = allowspeed,
                                                RailType = railtype,
                                                FastenerType = fastenertype,
                                                TrackClass = trackclass,
                                                TrackType = tracktype,
                                                Width = width,
                                                ItIsGap = true
                                            });
                                        }
                                        break;
                                }
                            }

                            count = 0;
                            coords = 0.0;
                        }
                        else if (count >= 4)
                        {
                            bool hasGap = rdStructureRepository.GetGapsBetweenCoords(process.Id, trackId, coords, badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0).Any();

                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                            {
                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Id > 3)
                                    hasGap = false;
                            }

                            if (hasGap)
                            {
                                string speed, railtype, fastenertype, trackclass, tracktype, width;

                                if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                else
                                    speed = "нет данных";

                                if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();
                                else
                                    railtype = "нет данных";

                                if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                else
                                    fastenertype = "нет данных";

                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                else
                                    trackclass = "Нет данных (1)";

                                if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                else
                                    tracktype = "прямой";

                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                else
                                    width = "нет данных";

                                badnessOfSleepers.Add(new BadnessOfSleepers {
                                    Km = Convert.ToInt32(coords) / 10000,
                                    Meter = Convert.ToInt32(coords) % 10000,
                                    digression = DigressionName.BushBadnessOfSleepers,
                                    Value = count,
                                    Speed = speed,
                                    AllowSpeed = "40/40",
                                    RailType = railtype,
                                    FastenerType = fastenertype,
                                    TrackClass = trackclass,
                                    TrackType = tracktype,
                                    Width = width,
                                    ItIsGap = true
                                });
                            }

                            if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                            {
                                switch (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type_Id)
                                {
                                    case 1:
                                    case 2:
                                    case 3:
                                        if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).Any())
                                        {
                                            string speed, railtype, fastenertype, trackclass, tracktype, width;

                                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                            else
                                                speed = "нет данных";

                                            railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                            else
                                                fastenertype = "нет данных";

                                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                            else
                                                trackclass = "Нет данных (1)";

                                            tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).First().Radius).ToString();

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                            else
                                                width = "нет данных";

                                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                                Km = Convert.ToInt32(coords) / 10000,
                                                Meter = Convert.ToInt32(coords) % 10000,
                                                digression = DigressionName.BushBadnessOfSleepers,
                                                Value = count,
                                                Speed = speed,
                                                AllowSpeed = "40/25",
                                                RailType = railtype,
                                                FastenerType = fastenertype,
                                                TrackClass = trackclass,
                                                TrackType = tracktype,
                                                Width = width,
                                                ItIsGap = true
                                            });
                                        }
                                        else
                                        {
                                            string speed, railtype, fastenertype, trackclass, tracktype, width;

                                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                            else
                                                speed = "нет данных";

                                            railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                            else
                                                fastenertype = "нет данных";

                                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                            else
                                                trackclass = "Нет данных (1)";

                                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                            else
                                                tracktype = "прямой";

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                            else
                                                width = "нет данных";

                                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                                Km = Convert.ToInt32(coords) / 10000,
                                                Meter = Convert.ToInt32(coords) % 10000,
                                                digression = DigressionName.BushBadnessOfSleepers,
                                                Value = count,
                                                Speed = speed,
                                                AllowSpeed = "60/40",
                                                RailType = railtype,
                                                FastenerType = fastenertype,
                                                TrackClass = trackclass,
                                                TrackType = tracktype,
                                                Width = width,
                                                ItIsGap = true
                                            });
                                        }
                                        break;

                                    case 4:
                                    case 5:
                                        if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).Any())
                                        {
                                            string speed, railtype, fastenertype, trackclass, tracktype, width, allowspeed;

                                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                            else
                                                speed = "нет данных";

                                            railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                            else
                                                fastenertype = "нет данных";

                                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                            else
                                                trackclass = "Нет данных (1)";

                                            tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).First().Radius).ToString();

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            {
                                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();

                                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width >= 1545)
                                                    allowspeed = "0/0";
                                                else
                                                    allowspeed = "15/15";
                                            }
                                            else
                                            {
                                                width = "нет данных";
                                                allowspeed = "15/15";
                                            }

                                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                                Km = Convert.ToInt32(coords) / 10000,
                                                Meter = Convert.ToInt32(coords) % 10000,
                                                digression = DigressionName.BushBadnessOfSleepers,
                                                Value = count,
                                                Speed = speed,
                                                AllowSpeed = allowspeed,
                                                RailType = railtype,
                                                FastenerType = fastenertype,
                                                TrackClass = trackclass,
                                                TrackType = tracktype,
                                                Width = width,
                                                ItIsGap = true
                                            });
                                        }
                                        else
                                        {
                                            string speed, railtype, fastenertype, trackclass, tracktype, width;

                                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                            else
                                                speed = "нет данных";

                                            railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                            else
                                                fastenertype = "нет данных";

                                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                            else
                                                trackclass = "Нет данных (1)";

                                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                            else
                                                tracktype = "прямой";

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                            else
                                                width = "нет данных";

                                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                                Km = Convert.ToInt32(coords) / 10000,
                                                Meter = Convert.ToInt32(coords) % 10000,
                                                digression = DigressionName.BushBadnessOfSleepers,
                                                Value = count,
                                                Speed = speed,
                                                AllowSpeed = "40/25",
                                                RailType = railtype,
                                                FastenerType = fastenertype,
                                                TrackClass = trackclass,
                                                TrackType = tracktype,
                                                Width = width,
                                                ItIsGap = true
                                            });
                                        }
                                        break;
                                }
                            }

                            count = 0;
                            coords = 0.0;
                        }
                        else if (count >= 3)
                        {
                            bool hasGap = rdStructureRepository.GetGapsBetweenCoords(process.Id, trackId, coords, badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0).Any();

                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                            {
                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Id > 3)
                                    hasGap = false;
                            }

                            if (hasGap)
                            {
                                string speed, railtype, fastenertype, trackclass, tracktype, width;

                                if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                else
                                    speed = "нет данных";

                                if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();
                                else
                                    railtype = "нет данных";

                                if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                else
                                    fastenertype = "нет данных";

                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                else
                                    trackclass = "Нет данных (1)";

                                if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                else
                                    tracktype = "прямой";

                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                else
                                    width = "нет данных";

                                badnessOfSleepers.Add(new BadnessOfSleepers {
                                    Km = Convert.ToInt32(coords) / 10000,
                                    Meter = Convert.ToInt32(coords) % 10000,
                                    digression = DigressionName.BushBadnessOfSleepers,
                                    Value = count,
                                    Speed = speed,
                                    AllowSpeed = "40/40",
                                    RailType = railtype,
                                    FastenerType = fastenertype,
                                    TrackClass = trackclass,
                                    TrackType = tracktype,
                                    Width = width,
                                    ItIsGap = true
                                });
                            }

                            if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                            {
                                switch (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type_Id)
                                {
                                    case 4:
                                    case 5:
                                        if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).Any())
                                        {
                                            string speed, railtype, fastenertype, trackclass, tracktype, width;

                                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                            else
                                                speed = "нет данных";

                                            railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                            else
                                                fastenertype = "нет данных";

                                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                            else
                                                trackclass = "Нет данных (1)";

                                            tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).First().Radius).ToString();

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                            else
                                                width = "нет данных";

                                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                                Km = Convert.ToInt32(coords) / 10000,
                                                Meter = Convert.ToInt32(coords) % 10000,
                                                digression = DigressionName.BushBadnessOfSleepers,
                                                Value = count,
                                                Speed = speed,
                                                AllowSpeed = "40/25",
                                                RailType = railtype,
                                                FastenerType = fastenertype,
                                                TrackClass = trackclass,
                                                TrackType = tracktype,
                                                Width = width,
                                                ItIsGap = true
                                            });
                                        }
                                        else
                                        {
                                            string speed, railtype, fastenertype, trackclass, tracktype, width;

                                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                            else
                                                speed = "нет данных";

                                            railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                            else
                                                fastenertype = "нет данных";

                                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                            else
                                                trackclass = "Нет данных (1)";

                                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                            else
                                                tracktype = "прямой";

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                            else
                                                width = "нет данных";

                                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                                Km = Convert.ToInt32(coords) / 10000,
                                                Meter = Convert.ToInt32(coords) % 10000,
                                                digression = DigressionName.BushBadnessOfSleepers,
                                                Value = count,
                                                Speed = speed,
                                                AllowSpeed = "50/40",
                                                RailType = railtype,
                                                FastenerType = fastenertype,
                                                TrackClass = trackclass,
                                                TrackType = tracktype,
                                                Width = width,
                                                ItIsGap = true
                                            });
                                        }
                                        break;
                                }
                            }

                            count = 0;
                            coords = 0.0;
                        }
                        else if (count >= 2)
                        {
                            bool hasGap = rdStructureRepository.GetGapsBetweenCoords(process.Id, trackId, coords, badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0).Any();

                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                            {
                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Id > 3)
                                    hasGap = false;
                            }

                            if (hasGap)
                            {
                                string speed, railtype, fastenertype, trackclass, tracktype, width;

                                if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                else
                                    speed = "нет данных";

                                if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();
                                else
                                    railtype = "нет данных";

                                if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                else
                                    fastenertype = "нет данных";

                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                else
                                    trackclass = "Нет данных (1)";

                                if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                else
                                    tracktype = "прямой";

                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                else
                                    width = "нет данных";

                                badnessOfSleepers.Add(new BadnessOfSleepers {
                                    Km = Convert.ToInt32(coords) / 10000,
                                    Meter = Convert.ToInt32(coords) % 10000,
                                    digression = DigressionName.BushBadnessOfSleepers,
                                    Value = count,
                                    Speed = speed,
                                    AllowSpeed = "40/40",
                                    RailType = railtype,
                                    FastenerType = fastenertype,
                                    TrackClass = trackclass,
                                    TrackType = tracktype,
                                    Width = width,
                                    ItIsGap = true
                                });
                            }

                            count = 0;
                            coords = 0.0;
                        }
                    }
                    else
                    {
                        if (count >= 6)
                        {
                            bool hasGap = rdStructureRepository.GetGapsBetweenCoords(process.Id, trackId, coords, badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0).Any();

                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                            {
                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Id > 3)
                                    hasGap = false;
                            }

                            if (hasGap)
                            {
                                string speed, railtype, fastenertype, trackclass, tracktype, width;

                                if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                else
                                    speed = "нет данных";

                                if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();
                                else
                                    railtype = "нет данных";

                                if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                else
                                    fastenertype = "нет данных";

                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                else
                                    trackclass = "Нет данных (1)";

                                if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                else
                                    tracktype = "прямой";

                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                else
                                    width = "нет данных";

                                badnessOfSleepers.Add(new BadnessOfSleepers {
                                    Km = Convert.ToInt32(coords) / 10000,
                                    Meter = Convert.ToInt32(coords) % 10000,
                                    digression = DigressionName.BushBadnessOfSleepers,
                                    Value = count,
                                    Speed = speed,
                                    AllowSpeed = "40/40",
                                    RailType = railtype,
                                    FastenerType = fastenertype,
                                    TrackClass = trackclass,
                                    TrackType = tracktype,
                                    Width = width,
                                    ItIsGap = true
                                });
                            }

                            if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                            {
                                switch (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type_Id)
                                {
                                    case 1:
                                    case 2:
                                    case 3:
                                        {
                                            string speed, railtype, fastenertype, trackclass, tracktype, width, allowspeed;

                                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                            else
                                                speed = "нет данных";

                                            railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                            else
                                                fastenertype = "нет данных";

                                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                            else
                                                trackclass = "Нет данных (1)";

                                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                            else
                                                tracktype = "прямой";

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            {
                                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();

                                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width >= 1545)
                                                    allowspeed = "0/0";
                                                else
                                                    allowspeed = "15/15";
                                            }
                                            else
                                            {
                                                width = "нет данных";
                                                allowspeed = "15/15";
                                            }

                                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                                Km = Convert.ToInt32(coords) / 10000,
                                                Meter = Convert.ToInt32(coords) % 10000,
                                                digression = DigressionName.BushBadnessOfSleepers,
                                                Value = count,
                                                Speed = speed,
                                                AllowSpeed = allowspeed,
                                                RailType = railtype,
                                                FastenerType = fastenertype,
                                                TrackClass = trackclass,
                                                TrackType = tracktype,
                                                Width = width,
                                                ItIsGap = true
                                            });
                                        }
                                        break;

                                    case 4:
                                    case 5:
                                        {
                                            string speed, railtype, fastenertype, trackclass, tracktype, width, allowspeed;

                                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                            else
                                                speed = "нет данных";

                                            railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                            else
                                                fastenertype = "нет данных";

                                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                            else
                                                trackclass = "Нет данных (1)";

                                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                            else
                                                tracktype = "прямой";

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            {
                                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();

                                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width >= 1545)
                                                    allowspeed = "0/0";
                                                else
                                                    allowspeed = "15/15";
                                            }
                                            else
                                            {
                                                width = "нет данных";
                                                allowspeed = "15/15";
                                            }

                                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                                Km = Convert.ToInt32(coords) / 10000,
                                                Meter = Convert.ToInt32(coords) % 10000,
                                                digression = DigressionName.BushBadnessOfSleepers,
                                                Value = count,
                                                Speed = speed,
                                                AllowSpeed = allowspeed,
                                                RailType = railtype,
                                                FastenerType = fastenertype,
                                                TrackClass = trackclass,
                                                TrackType = tracktype,
                                                Width = width,
                                                ItIsGap = true
                                            });
                                        }
                                        break;
                                }
                            }

                            count = 0;
                            coords = 0.0;
                        }
                        else if (count >= 5)
                        {
                            bool hasGap = rdStructureRepository.GetGapsBetweenCoords(process.Id, trackId, coords, badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0).Any();

                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                            {
                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Id > 3)
                                    hasGap = false;
                            }

                            if (hasGap)
                            {
                                string speed, railtype, fastenertype, trackclass, tracktype, width;

                                if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                else
                                    speed = "нет данных";

                                if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();
                                else
                                    railtype = "нет данных";

                                if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                else
                                    fastenertype = "нет данных";

                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                else
                                    trackclass = "Нет данных (1)";

                                if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                else
                                    tracktype = "прямой";

                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                else
                                    width = "нет данных";

                                badnessOfSleepers.Add(new BadnessOfSleepers {
                                    Km = Convert.ToInt32(coords) / 10000,
                                    Meter = Convert.ToInt32(coords) % 10000,
                                    digression = DigressionName.BushBadnessOfSleepers,
                                    Value = count,
                                    Speed = speed,
                                    AllowSpeed = "40/40",
                                    RailType = railtype,
                                    FastenerType = fastenertype,
                                    TrackClass = trackclass,
                                    TrackType = tracktype,
                                    Width = width,
                                    ItIsGap = true
                                });
                            }

                            if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                            {
                                switch (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type_Id)
                                {
                                    case 1:
                                    case 2:
                                    case 3:
                                        if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).Any())
                                        {
                                            string speed, railtype, fastenertype, trackclass, tracktype, width, allowspeed;

                                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                            else
                                                speed = "нет данных";

                                            railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                            else
                                                fastenertype = "нет данных";

                                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                            else
                                                trackclass = "Нет данных (1)";

                                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).Any())
                                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                            else
                                                tracktype = "прямой";

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            {
                                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();

                                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width >= 1545)
                                                    allowspeed = "0/0";
                                                else
                                                    allowspeed = "15/15";
                                            }
                                            else
                                            {
                                                width = "нет данных";
                                                allowspeed = "15/15";
                                            }

                                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                                Km = Convert.ToInt32(coords) / 10000,
                                                Meter = Convert.ToInt32(coords) % 10000,
                                                digression = DigressionName.BushBadnessOfSleepers,
                                                Value = count,
                                                Speed = speed,
                                                AllowSpeed = allowspeed,
                                                RailType = railtype,
                                                FastenerType = fastenertype,
                                                TrackClass = trackclass,
                                                TrackType = tracktype,
                                                Width = width,
                                                ItIsGap = true
                                            });
                                        }
                                        else
                                        {
                                            string speed, railtype, fastenertype, trackclass, tracktype, width;

                                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                            else
                                                speed = "нет данных";

                                            railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                            else
                                                fastenertype = "нет данных";

                                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                            else
                                                trackclass = "Нет данных (1)";

                                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                            else
                                                tracktype = "прямой";

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                            else
                                                width = "нет данных";

                                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                                Km = Convert.ToInt32(coords) / 10000,
                                                Meter = Convert.ToInt32(coords) % 10000,
                                                digression = DigressionName.BushBadnessOfSleepers,
                                                Value = count,
                                                Speed = speed,
                                                AllowSpeed = "40/25",
                                                RailType = railtype,
                                                FastenerType = fastenertype,
                                                TrackClass = trackclass,
                                                TrackType = tracktype,
                                                Width = width,
                                                ItIsGap = true
                                            });
                                        }
                                        break;

                                    case 4:
                                    case 5:
                                        {
                                            string speed, railtype, fastenertype, trackclass, tracktype, width, allowspeed;

                                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                            else
                                                speed = "нет данных";

                                            railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                            else
                                                fastenertype = "нет данных";

                                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                            else
                                                trackclass = "Нет данных (1)";

                                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                            else
                                                tracktype = "прямой";

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            {
                                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();

                                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width >= 1545)
                                                    allowspeed = "0/0";
                                                else
                                                    allowspeed = "15/15";
                                            }
                                            else
                                            {
                                                width = "нет данных";
                                                allowspeed = "15/15";
                                            }

                                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                                Km = Convert.ToInt32(coords) / 10000,
                                                Meter = Convert.ToInt32(coords) % 10000,
                                                digression = DigressionName.BushBadnessOfSleepers,
                                                Value = count,
                                                Speed = speed,
                                                AllowSpeed = allowspeed,
                                                RailType = railtype,
                                                FastenerType = fastenertype,
                                                TrackClass = trackclass,
                                                TrackType = tracktype,
                                                Width = width,
                                                ItIsGap = true
                                            });
                                        }
                                        break;
                                }
                            }

                            count = 0;
                            coords = 0.0;
                        }
                        else if (count >= 4)
                        {
                            bool hasGap = rdStructureRepository.GetGapsBetweenCoords(process.Id, trackId, coords, badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0).Any();

                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                            {
                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Id > 3)
                                    hasGap = false;
                            }

                            if (hasGap)
                            {
                                string speed, railtype, fastenertype, trackclass, tracktype, width;

                                if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                else
                                    speed = "нет данных";

                                if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();
                                else
                                    railtype = "нет данных";

                                if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                else
                                    fastenertype = "нет данных";

                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                else
                                    trackclass = "Нет данных (1)";

                                if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                else
                                    tracktype = "прямой";

                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                else
                                    width = "нет данных";

                                badnessOfSleepers.Add(new BadnessOfSleepers {
                                    Km = Convert.ToInt32(coords) / 10000,
                                    Meter = Convert.ToInt32(coords) % 10000,
                                    digression = DigressionName.BushBadnessOfSleepers,
                                    Value = count,
                                    Speed = speed,
                                    AllowSpeed = "40/40",
                                    RailType = railtype,
                                    FastenerType = fastenertype,
                                    TrackClass = trackclass,
                                    TrackType = tracktype,
                                    Width = width,
                                    ItIsGap = true
                                });
                            }

                            if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                            {
                                switch (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type_Id)
                                {
                                    case 1:
                                    case 2:
                                    case 3:
                                        if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).Any())
                                        {
                                            string speed, railtype, fastenertype, trackclass, tracktype, width;

                                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                            else
                                                speed = "нет данных";

                                            railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                            else
                                                fastenertype = "нет данных";

                                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                            else
                                                trackclass = "Нет данных (1)";

                                            tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).First().Radius).ToString();

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                            else
                                                width = "нет данных";

                                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                                Km = Convert.ToInt32(coords) / 10000,
                                                Meter = Convert.ToInt32(coords) % 10000,
                                                digression = DigressionName.BushBadnessOfSleepers,
                                                Value = count,
                                                Speed = speed,
                                                AllowSpeed = "40/25",
                                                RailType = railtype,
                                                FastenerType = fastenertype,
                                                TrackClass = trackclass,
                                                TrackType = tracktype,
                                                Width = width,
                                                ItIsGap = true
                                            });
                                        }
                                        else
                                        {
                                            string speed, railtype, fastenertype, trackclass, tracktype, width;

                                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                            else
                                                speed = "нет данных";

                                            railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                            else
                                                fastenertype = "нет данных";

                                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                            else
                                                trackclass = "Нет данных (1)";

                                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                            else
                                                tracktype = "прямой";

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                            else
                                                width = "нет данных";

                                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                                Km = Convert.ToInt32(coords) / 10000,
                                                Meter = Convert.ToInt32(coords) % 10000,
                                                digression = DigressionName.BushBadnessOfSleepers,
                                                Value = count,
                                                Speed = speed,
                                                AllowSpeed = "60/40",
                                                RailType = railtype,
                                                FastenerType = fastenertype,
                                                TrackClass = trackclass,
                                                TrackType = tracktype,
                                                Width = width,
                                                ItIsGap = true
                                            });
                                        }
                                        break;

                                    case 4:
                                    case 5:
                                        if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).Any())
                                        {
                                            string speed, railtype, fastenertype, trackclass, tracktype, width, allowspeed;

                                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                            else
                                                speed = "нет данных";

                                            railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                            else
                                                fastenertype = "нет данных";

                                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                            else
                                                trackclass = "Нет данных (1)";

                                            tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).First().Radius).ToString();

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            {
                                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();

                                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width >= 1545)
                                                    allowspeed = "0/0";
                                                else
                                                    allowspeed = "15/15";
                                            }
                                            else
                                            {
                                                width = "нет данных";
                                                allowspeed = "15/15";
                                            }

                                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                                Km = Convert.ToInt32(coords) / 10000,
                                                Meter = Convert.ToInt32(coords) % 10000,
                                                digression = DigressionName.BushBadnessOfSleepers,
                                                Value = count,
                                                Speed = speed,
                                                AllowSpeed = allowspeed,
                                                RailType = railtype,
                                                FastenerType = fastenertype,
                                                TrackClass = trackclass,
                                                TrackType = tracktype,
                                                Width = width,
                                                ItIsGap = true
                                            });
                                        }
                                        else
                                        {
                                            string speed, railtype, fastenertype, trackclass, tracktype, width;

                                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                            else
                                                speed = "нет данных";

                                            railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                            else
                                                fastenertype = "нет данных";

                                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                            else
                                                trackclass = "Нет данных (1)";

                                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                            else
                                                tracktype = "прямой";

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                            else
                                                width = "нет данных";

                                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                                Km = Convert.ToInt32(coords) / 10000,
                                                Meter = Convert.ToInt32(coords) % 10000,
                                                digression = DigressionName.BushBadnessOfSleepers,
                                                Value = count,
                                                Speed = speed,
                                                AllowSpeed = "40/25",
                                                RailType = railtype,
                                                FastenerType = fastenertype,
                                                TrackClass = trackclass,
                                                TrackType = tracktype,
                                                Width = width,
                                                ItIsGap = true
                                            });
                                        }
                                        break;
                                }
                            }

                            count = 0;
                            coords = 0.0;
                        }
                        else if (count >= 3)
                        {
                            bool hasGap = rdStructureRepository.GetGapsBetweenCoords(process.Id, trackId, coords, badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0).Any();

                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                            {
                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Id > 3)
                                    hasGap = false;
                            }

                            if (hasGap)
                            {
                                string speed, railtype, fastenertype, trackclass, tracktype, width;

                                if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                else
                                    speed = "нет данных";

                                if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();
                                else
                                    railtype = "нет данных";

                                if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                else
                                    fastenertype = "нет данных";

                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                else
                                    trackclass = "Нет данных (1)";

                                if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                else
                                    tracktype = "прямой";

                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                else
                                    width = "нет данных";

                                badnessOfSleepers.Add(new BadnessOfSleepers {
                                    Km = Convert.ToInt32(coords) / 10000,
                                    Meter = Convert.ToInt32(coords) % 10000,
                                    digression = DigressionName.BushBadnessOfSleepers,
                                    Value = count,
                                    Speed = speed,
                                    AllowSpeed = "40/40",
                                    RailType = railtype,
                                    FastenerType = fastenertype,
                                    TrackClass = trackclass,
                                    TrackType = tracktype,
                                    Width = width,
                                    ItIsGap = true
                                });
                            }

                            if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                            {
                                switch (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type_Id)
                                {
                                    case 4:
                                    case 5:
                                        if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).Any())
                                        {
                                            string speed, railtype, fastenertype, trackclass, tracktype, width;

                                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                            else
                                                speed = "нет данных";

                                            railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                            else
                                                fastenertype = "нет данных";

                                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                            else
                                                trackclass = "Нет данных (1)";

                                            tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).First().Radius).ToString();

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                            else
                                                width = "нет данных";

                                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                                Km = Convert.ToInt32(coords) / 10000,
                                                Meter = Convert.ToInt32(coords) % 10000,
                                                digression = DigressionName.BushBadnessOfSleepers,
                                                Value = count,
                                                Speed = speed,
                                                AllowSpeed = "40/25",
                                                RailType = railtype,
                                                FastenerType = fastenertype,
                                                TrackClass = trackclass,
                                                TrackType = tracktype,
                                                Width = width,
                                                ItIsGap = true
                                            });
                                        }
                                        else
                                        {
                                            string speed, railtype, fastenertype, trackclass, tracktype, width;

                                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                            else
                                                speed = "нет данных";

                                            railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                            else
                                                fastenertype = "нет данных";

                                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                            else
                                                trackclass = "Нет данных (1)";

                                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                            else
                                                tracktype = "прямой";

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                            else
                                                width = "нет данных";

                                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                                Km = Convert.ToInt32(coords) / 10000,
                                                Meter = Convert.ToInt32(coords) % 10000,
                                                digression = DigressionName.BushBadnessOfSleepers,
                                                Value = count,
                                                Speed = speed,
                                                AllowSpeed = "50/40",
                                                RailType = railtype,
                                                FastenerType = fastenertype,
                                                TrackClass = trackclass,
                                                TrackType = tracktype,
                                                Width = width,
                                                ItIsGap = true
                                            });
                                        }
                                        break;
                                }
                            }

                            count = 0;
                            coords = 0.0;
                        }
                        else if (count >= 2)
                        {
                            bool hasGap = rdStructureRepository.GetGapsBetweenCoords(process.Id, trackId, coords, badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0).Any();

                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                            {
                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Id > 3)
                                    hasGap = false;
                            }

                            if (hasGap)
                            {
                                string speed, railtype, fastenertype, trackclass, tracktype, width;

                                if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                else
                                    speed = "нет данных";

                                if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();
                                else
                                    railtype = "нет данных";

                                if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                else
                                    fastenertype = "нет данных";

                                if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                else
                                    trackclass = "Нет данных (1)";

                                if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                else
                                    tracktype = "прямой";

                                if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                    width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                else
                                    width = "нет данных";

                                badnessOfSleepers.Add(new BadnessOfSleepers {
                                    Km = Convert.ToInt32(coords) / 10000,
                                    Meter = Convert.ToInt32(coords) % 10000,
                                    digression = DigressionName.BushBadnessOfSleepers,
                                    Value = count,
                                    Speed = speed,
                                    AllowSpeed = "40/40",
                                    RailType = railtype,
                                    FastenerType = fastenertype,
                                    TrackClass = trackclass,
                                    TrackType = tracktype,
                                    Width = width,
                                    ItIsGap = true
                                });
                            }

                            count = 0;
                            coords = 0.0;
                        }
                    }
                }
                else
                {
                    if (count >= 6)
                    {
                        bool hasGap = rdStructureRepository.GetGapsBetweenCoords(process.Id, trackId, coords, badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0).Any();

                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                        {
                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Id > 3)
                                hasGap = false;
                        }

                        if (hasGap)
                        {
                            string speed, railtype, fastenertype, trackclass, tracktype, width;

                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                            else
                                speed = "нет данных";

                            if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();
                            else
                                railtype = "нет данных";

                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                            else
                                fastenertype = "нет данных";

                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                            else
                                trackclass = "Нет данных (1)";

                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                            else
                                tracktype = "прямой";

                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                            else
                                width = "нет данных";

                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                Km = Convert.ToInt32(coords) / 10000,
                                Meter = Convert.ToInt32(coords) % 10000,
                                digression = DigressionName.BushBadnessOfSleepers,
                                Value = count,
                                Speed = speed,
                                AllowSpeed = "40/40",
                                RailType = railtype,
                                FastenerType = fastenertype,
                                TrackClass = trackclass,
                                TrackType = tracktype,
                                Width = width,
                                ItIsGap = true
                            });
                        }

                        if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                        {
                            switch (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type_Id)
                            {
                                case 1:
                                case 2:
                                case 3:
                                    {
                                        string speed, railtype, fastenertype, trackclass, tracktype, width, allowspeed;

                                        if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                        else
                                            speed = "нет данных";

                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                        if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                        else
                                            fastenertype = "нет данных";

                                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                        else
                                            trackclass = "Нет данных (1)";

                                        if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                        else
                                            tracktype = "прямой";

                                        if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        {
                                            width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width >= 1545)
                                                allowspeed = "0/0";
                                            else
                                                allowspeed = "15/15";
                                        }
                                        else
                                        {
                                            width = "нет данных";
                                            allowspeed = "15/15";
                                        }

                                        badnessOfSleepers.Add(new BadnessOfSleepers {
                                            Km = Convert.ToInt32(coords) / 10000,
                                            Meter = Convert.ToInt32(coords) % 10000,
                                            digression = DigressionName.BushBadnessOfSleepers,
                                            Value = count,
                                            Speed = speed,
                                            AllowSpeed = allowspeed,
                                            RailType = railtype,
                                            FastenerType = fastenertype,
                                            TrackClass = trackclass,
                                            TrackType = tracktype,
                                            Width = width,
                                            ItIsGap = true
                                        });
                                    }
                                    break;

                                case 4:
                                case 5:
                                    {
                                        string speed, railtype, fastenertype, trackclass, tracktype, width, allowspeed;

                                        if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                        else
                                            speed = "нет данных";

                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                        if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                        else
                                            fastenertype = "нет данных";

                                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                        else
                                            trackclass = "Нет данных (1)";

                                        if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                        else
                                            tracktype = "прямой";

                                        if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        {
                                            width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width >= 1545)
                                                allowspeed = "0/0";
                                            else
                                                allowspeed = "15/15";
                                        }
                                        else
                                        {
                                            width = "нет данных";
                                            allowspeed = "15/15";
                                        }

                                        badnessOfSleepers.Add(new BadnessOfSleepers {
                                            Km = Convert.ToInt32(coords) / 10000,
                                            Meter = Convert.ToInt32(coords) % 10000,
                                            digression = DigressionName.BushBadnessOfSleepers,
                                            Value = count,
                                            Speed = speed,
                                            AllowSpeed = allowspeed,
                                            RailType = railtype,
                                            FastenerType = fastenertype,
                                            TrackClass = trackclass,
                                            TrackType = tracktype,
                                            Width = width,
                                            ItIsGap = true
                                        });
                                    }
                                    break;
                            }
                        }

                        count = 0;
                        coords = 0.0;
                    }
                    else if (count >= 5)
                    {
                        bool hasGap = rdStructureRepository.GetGapsBetweenCoords(process.Id, trackId, coords, badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0).Any();

                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                        {
                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Id > 3)
                                hasGap = false;
                        }

                        if (hasGap)
                        {
                            string speed, railtype, fastenertype, trackclass, tracktype, width;

                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                            else
                                speed = "нет данных";

                            if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();
                            else
                                railtype = "нет данных";

                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                            else
                                fastenertype = "нет данных";

                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                            else
                                trackclass = "Нет данных (1)";

                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                            else
                                tracktype = "прямой";

                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                            else
                                width = "нет данных";

                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                Km = Convert.ToInt32(coords) / 10000,
                                Meter = Convert.ToInt32(coords) % 10000,
                                digression = DigressionName.BushBadnessOfSleepers,
                                Value = count,
                                Speed = speed,
                                AllowSpeed = "40/40",
                                RailType = railtype,
                                FastenerType = fastenertype,
                                TrackClass = trackclass,
                                TrackType = tracktype,
                                Width = width,
                                ItIsGap = true
                            });
                        }

                        if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                        {
                            switch (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type_Id)
                            {
                                case 1:
                                case 2:
                                case 3:
                                    if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).Any())
                                    {
                                        string speed, railtype, fastenertype, trackclass, tracktype, width, allowspeed;

                                        if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                        else
                                            speed = "нет данных";

                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                        if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                        else
                                            fastenertype = "нет данных";

                                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                        else
                                            trackclass = "Нет данных (1)";

                                        if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).Any())
                                            tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                        else
                                            tracktype = "прямой";

                                        if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        {
                                            width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width >= 1545)
                                                allowspeed = "0/0";
                                            else
                                                allowspeed = "15/15";
                                        }
                                        else
                                        {
                                            width = "нет данных";
                                            allowspeed = "15/15";
                                        }

                                        badnessOfSleepers.Add(new BadnessOfSleepers {
                                            Km = Convert.ToInt32(coords) / 10000,
                                            Meter = Convert.ToInt32(coords) % 10000,
                                            digression = DigressionName.BushBadnessOfSleepers,
                                            Value = count,
                                            Speed = speed,
                                            AllowSpeed = allowspeed,
                                            RailType = railtype,
                                            FastenerType = fastenertype,
                                            TrackClass = trackclass,
                                            TrackType = tracktype,
                                            Width = width,
                                            ItIsGap = true
                                        });
                                    }
                                    else
                                    {
                                        string speed, railtype, fastenertype, trackclass, tracktype, width;

                                        if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                        else
                                            speed = "нет данных";

                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                        if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                        else
                                            fastenertype = "нет данных";

                                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                        else
                                            trackclass = "Нет данных (1)";

                                        if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                        else
                                            tracktype = "прямой";

                                        if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                        else
                                            width = "нет данных";

                                        badnessOfSleepers.Add(new BadnessOfSleepers {
                                            Km = Convert.ToInt32(coords) / 10000,
                                            Meter = Convert.ToInt32(coords) % 10000,
                                            digression = DigressionName.BushBadnessOfSleepers,
                                            Value = count,
                                            Speed = speed,
                                            AllowSpeed = "40/25",
                                            RailType = railtype,
                                            FastenerType = fastenertype,
                                            TrackClass = trackclass,
                                            TrackType = tracktype,
                                            Width = width,
                                            ItIsGap = true
                                        });
                                    }
                                    break;

                                case 4:
                                case 5:
                                    {
                                        string speed, railtype, fastenertype, trackclass, tracktype, width, allowspeed;

                                        if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                        else
                                            speed = "нет данных";

                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                        if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                        else
                                            fastenertype = "нет данных";

                                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                        else
                                            trackclass = "Нет данных (1)";

                                        if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                        else
                                            tracktype = "прямой";

                                        if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        {
                                            width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width >= 1545)
                                                allowspeed = "0/0";
                                            else
                                                allowspeed = "15/15";
                                        }
                                        else
                                        {
                                            width = "нет данных";
                                            allowspeed = "15/15";
                                        }

                                        badnessOfSleepers.Add(new BadnessOfSleepers {
                                            Km = Convert.ToInt32(coords) / 10000,
                                            Meter = Convert.ToInt32(coords) % 10000,
                                            digression = DigressionName.BushBadnessOfSleepers,
                                            Value = count,
                                            Speed = speed,
                                            AllowSpeed = allowspeed,
                                            RailType = railtype,
                                            FastenerType = fastenertype,
                                            TrackClass = trackclass,
                                            TrackType = tracktype,
                                            Width = width,
                                            ItIsGap = true
                                        });
                                    }
                                    break;
                            }
                        }

                        count = 0;
                        coords = 0.0;
                    }
                    else if (count >= 4)
                    {
                        bool hasGap = rdStructureRepository.GetGapsBetweenCoords(process.Id, trackId, coords, badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0).Any();

                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                        {
                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Id > 3)
                                hasGap = false;
                        }

                        if (hasGap)
                        {
                            string speed, railtype, fastenertype, trackclass, tracktype, width;

                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                            else
                                speed = "нет данных";

                            if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();
                            else
                                railtype = "нет данных";

                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                            else
                                fastenertype = "нет данных";

                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                            else
                                trackclass = "Нет данных (1)";

                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                            else
                                tracktype = "прямой";

                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                            else
                                width = "нет данных";

                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                Km = Convert.ToInt32(coords) / 10000,
                                Meter = Convert.ToInt32(coords) % 10000,
                                digression = DigressionName.BushBadnessOfSleepers,
                                Value = count,
                                Speed = speed,
                                AllowSpeed = "40/40",
                                RailType = railtype,
                                FastenerType = fastenertype,
                                TrackClass = trackclass,
                                TrackType = tracktype,
                                Width = width,
                                ItIsGap = true
                            });
                        }

                        if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                        {
                            switch (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type_Id)
                            {
                                case 1:
                                case 2:
                                case 3:
                                    if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).Any())
                                    {
                                        string speed, railtype, fastenertype, trackclass, tracktype, width;

                                        if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                        else
                                            speed = "нет данных";

                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                        if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                        else
                                            fastenertype = "нет данных";

                                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                        else
                                            trackclass = "Нет данных (1)";

                                        tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).First().Radius).ToString();

                                        if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                        else
                                            width = "нет данных";

                                        badnessOfSleepers.Add(new BadnessOfSleepers {
                                            Km = Convert.ToInt32(coords) / 10000,
                                            Meter = Convert.ToInt32(coords) % 10000,
                                            digression = DigressionName.BushBadnessOfSleepers,
                                            Value = count,
                                            Speed = speed,
                                            AllowSpeed = "40/25",
                                            RailType = railtype,
                                            FastenerType = fastenertype,
                                            TrackClass = trackclass,
                                            TrackType = tracktype,
                                            Width = width,
                                            ItIsGap = true
                                        });
                                    }
                                    else
                                    {
                                        string speed, railtype, fastenertype, trackclass, tracktype, width;

                                        if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                        else
                                            speed = "нет данных";

                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                        if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                        else
                                            fastenertype = "нет данных";

                                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                        else
                                            trackclass = "Нет данных (1)";

                                        if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                        else
                                            tracktype = "прямой";

                                        if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                        else
                                            width = "нет данных";

                                        badnessOfSleepers.Add(new BadnessOfSleepers {
                                            Km = Convert.ToInt32(coords) / 10000,
                                            Meter = Convert.ToInt32(coords) % 10000,
                                            digression = DigressionName.BushBadnessOfSleepers,
                                            Value = count,
                                            Speed = speed,
                                            AllowSpeed = "60/40",
                                            RailType = railtype,
                                            FastenerType = fastenertype,
                                            TrackClass = trackclass,
                                            TrackType = tracktype,
                                            Width = width,
                                            ItIsGap = true
                                        });
                                    }
                                    break;

                                case 4:
                                case 5:
                                    if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).Any())
                                    {
                                        string speed, railtype, fastenertype, trackclass, tracktype, width, allowspeed;

                                        if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                        else
                                            speed = "нет данных";

                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                        if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                        else
                                            fastenertype = "нет данных";

                                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                        else
                                            trackclass = "Нет данных (1)";

                                        tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).First().Radius).ToString();

                                        if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        {
                                            width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width >= 1545)
                                                allowspeed = "0/0";
                                            else
                                                allowspeed = "15/15";
                                        }
                                        else
                                        {
                                            width = "нет данных";
                                            allowspeed = "15/15";
                                        }

                                        badnessOfSleepers.Add(new BadnessOfSleepers {
                                            Km = Convert.ToInt32(coords) / 10000,
                                            Meter = Convert.ToInt32(coords) % 10000,
                                            digression = DigressionName.BushBadnessOfSleepers,
                                            Value = count,
                                            Speed = speed,
                                            AllowSpeed = allowspeed,
                                            RailType = railtype,
                                            FastenerType = fastenertype,
                                            TrackClass = trackclass,
                                            TrackType = tracktype,
                                            Width = width,
                                            ItIsGap = true
                                        });
                                    }
                                    else
                                    {
                                        string speed, railtype, fastenertype, trackclass, tracktype, width;

                                        if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                        else
                                            speed = "нет данных";

                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                        if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                        else
                                            fastenertype = "нет данных";

                                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                        else
                                            trackclass = "Нет данных (1)";

                                        if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                        else
                                            tracktype = "прямой";

                                        if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                        else
                                            width = "нет данных";

                                        badnessOfSleepers.Add(new BadnessOfSleepers {
                                            Km = Convert.ToInt32(coords) / 10000,
                                            Meter = Convert.ToInt32(coords) % 10000,
                                            digression = DigressionName.BushBadnessOfSleepers,
                                            Value = count,
                                            Speed = speed,
                                            AllowSpeed = "40/25",
                                            RailType = railtype,
                                            FastenerType = fastenertype,
                                            TrackClass = trackclass,
                                            TrackType = tracktype,
                                            Width = width,
                                            ItIsGap = true
                                        });
                                    }
                                    break;
                            }
                        }

                        count = 0;
                        coords = 0.0;
                    }
                    else if (count >= 3)
                    {
                        bool hasGap = rdStructureRepository.GetGapsBetweenCoords(process.Id, trackId, coords, badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0).Any();

                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                        {
                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Id > 3)
                                hasGap = false;
                        }

                        if (hasGap)
                        {
                            string speed, railtype, fastenertype, trackclass, tracktype, width;

                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                            else
                                speed = "нет данных";

                            if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();
                            else
                                railtype = "нет данных";

                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                            else
                                fastenertype = "нет данных";

                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                            else
                                trackclass = "Нет данных (1)";

                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                            else
                                tracktype = "прямой";

                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                            else
                                width = "нет данных";

                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                Km = Convert.ToInt32(coords) / 10000,
                                Meter = Convert.ToInt32(coords) % 10000,
                                digression = DigressionName.BushBadnessOfSleepers,
                                Value = count,
                                Speed = speed,
                                AllowSpeed = "40/40",
                                RailType = railtype,
                                FastenerType = fastenertype,
                                TrackClass = trackclass,
                                TrackType = tracktype,
                                Width = width,
                                ItIsGap = true
                            });
                        }

                        if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                        {
                            switch (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type_Id)
                            {
                                case 4:
                                case 5:
                                    if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).Any())
                                    {
                                        string speed, railtype, fastenertype, trackclass, tracktype, width;

                                        if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                        else
                                            speed = "нет данных";

                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                        if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                        else
                                            fastenertype = "нет данных";

                                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                        else
                                            trackclass = "Нет данных (1)";

                                        tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).First().Radius).ToString();

                                        if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                        else
                                            width = "нет данных";

                                        badnessOfSleepers.Add(new BadnessOfSleepers {
                                            Km = Convert.ToInt32(coords) / 10000,
                                            Meter = Convert.ToInt32(coords) % 10000,
                                            digression = DigressionName.BushBadnessOfSleepers,
                                            Value = count,
                                            Speed = speed,
                                            AllowSpeed = "40/25",
                                            RailType = railtype,
                                            FastenerType = fastenertype,
                                            TrackClass = trackclass,
                                            TrackType = tracktype,
                                            Width = width,
                                            ItIsGap = true
                                        });
                                    }
                                    else
                                    {
                                        string speed, railtype, fastenertype, trackclass, tracktype, width;

                                        if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                        else
                                            speed = "нет данных";

                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                        if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                        else
                                            fastenertype = "нет данных";

                                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                        else
                                            trackclass = "Нет данных (1)";

                                        if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                        else
                                            tracktype = "прямой";

                                        if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                        else
                                            width = "нет данных";

                                        badnessOfSleepers.Add(new BadnessOfSleepers {
                                            Km = Convert.ToInt32(coords) / 10000,
                                            Meter = Convert.ToInt32(coords) % 10000,
                                            digression = DigressionName.BushBadnessOfSleepers,
                                            Value = count,
                                            Speed = speed,
                                            AllowSpeed = "50/40",
                                            RailType = railtype,
                                            FastenerType = fastenertype,
                                            TrackClass = trackclass,
                                            TrackType = tracktype,
                                            Width = width,
                                            ItIsGap = true
                                        });
                                    }
                                    break;
                            }
                        }

                        count = 0;
                        coords = 0.0;
                    }
                    else if (count >= 2)
                    {
                        bool hasGap = rdStructureRepository.GetGapsBetweenCoords(process.Id, trackId, coords, badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0).Any();

                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                        {
                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Id > 3)
                                hasGap = false;
                        }

                        if (hasGap)
                        {
                            string speed, railtype, fastenertype, trackclass, tracktype, width;

                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                            else
                                speed = "нет данных";

                            if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();
                            else
                                railtype = "нет данных";

                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                            else
                                fastenertype = "нет данных";

                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                            else
                                trackclass = "Нет данных (1)";

                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                            else
                                tracktype = "прямой";

                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                            else
                                width = "нет данных";

                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                Km = Convert.ToInt32(coords) / 10000,
                                Meter = Convert.ToInt32(coords) % 10000,
                                digression = DigressionName.BushBadnessOfSleepers,
                                Value = count,
                                Speed = speed,
                                AllowSpeed = "40/40",
                                RailType = railtype,
                                FastenerType = fastenertype,
                                TrackClass = trackclass,
                                TrackType = tracktype,
                                Width = width,
                                ItIsGap = true
                            });
                        }

                        count = 0;
                        coords = 0.0;
                    }

                    count = 0;
                    coords = 0;
                }

                if (i == badSleepers.Count - 1)
                {
                    if (count >= 6)
                    {
                        bool hasGap = rdStructureRepository.GetGapsBetweenCoords(process.Id, trackId, coords, badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0).Any();

                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                        {
                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Id > 3)
                                hasGap = false;
                        }

                        if (hasGap)
                        {
                            string speed, railtype, fastenertype, trackclass, tracktype, width;

                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                            else
                                speed = "нет данных";

                            if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();
                            else
                                railtype = "нет данных";

                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                            else
                                fastenertype = "нет данных";

                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                            else
                                trackclass = "Нет данных (1)";

                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                            else
                                tracktype = "прямой";

                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                            else
                                width = "нет данных";

                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                Km = Convert.ToInt32(coords) / 10000,
                                Meter = Convert.ToInt32(coords) % 10000,
                                digression = DigressionName.BushBadnessOfSleepers,
                                Value = count,
                                Speed = speed,
                                AllowSpeed = "40/40",
                                RailType = railtype,
                                FastenerType = fastenertype,
                                TrackClass = trackclass,
                                TrackType = tracktype,
                                Width = width,
                                ItIsGap = true
                            });
                        }

                        if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                        {
                            switch (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type_Id)
                            {
                                case 1:
                                case 2:
                                case 3:
                                    {
                                        string speed, railtype, fastenertype, trackclass, tracktype, width, allowspeed;

                                        if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                        else
                                            speed = "нет данных";

                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                        if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                        else
                                            fastenertype = "нет данных";

                                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                        else
                                            trackclass = "Нет данных (1)";

                                        if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                        else
                                            tracktype = "прямой";

                                        if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        {
                                            width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width >= 1545)
                                                allowspeed = "0/0";
                                            else
                                                allowspeed = "15/15";
                                        }
                                        else
                                        {
                                            width = "нет данных";
                                            allowspeed = "15/15";
                                        }

                                        badnessOfSleepers.Add(new BadnessOfSleepers {
                                            Km = Convert.ToInt32(coords) / 10000,
                                            Meter = Convert.ToInt32(coords) % 10000,
                                            digression = DigressionName.BushBadnessOfSleepers,
                                            Value = count,
                                            Speed = speed,
                                            AllowSpeed = allowspeed,
                                            RailType = railtype,
                                            FastenerType = fastenertype,
                                            TrackClass = trackclass,
                                            TrackType = tracktype,
                                            Width = width,
                                            ItIsGap = true
                                        });
                                    }
                                    break;

                                case 4:
                                case 5:
                                    {
                                        string speed, railtype, fastenertype, trackclass, tracktype, width, allowspeed;

                                        if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                        else
                                            speed = "нет данных";

                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                        if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                        else
                                            fastenertype = "нет данных";

                                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                        else
                                            trackclass = "Нет данных (1)";

                                        if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                        else
                                            tracktype = "прямой";

                                        if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        {
                                            width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width >= 1545)
                                                allowspeed = "0/0";
                                            else
                                                allowspeed = "15/15";
                                        }
                                        else
                                        {
                                            width = "нет данных";
                                            allowspeed = "15/15";
                                        }

                                        badnessOfSleepers.Add(new BadnessOfSleepers {
                                            Km = Convert.ToInt32(coords) / 10000,
                                            Meter = Convert.ToInt32(coords) % 10000,
                                            digression = DigressionName.BushBadnessOfSleepers,
                                            Value = count,
                                            Speed = speed,
                                            AllowSpeed = allowspeed,
                                            RailType = railtype,
                                            FastenerType = fastenertype,
                                            TrackClass = trackclass,
                                            TrackType = tracktype,
                                            Width = width,
                                            ItIsGap = true
                                        });
                                    }
                                    break;
                            }
                        }

                        count = 0;
                        coords = 0.0;
                    }
                    else if (count >= 5)
                    {
                        bool hasGap = rdStructureRepository.GetGapsBetweenCoords(process.Id, trackId, coords, badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0).Any();

                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                        {
                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Id > 3)
                                hasGap = false;
                        }

                        if (hasGap)
                        {
                            string speed, railtype, fastenertype, trackclass, tracktype, width;

                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                            else
                                speed = "нет данных";

                            if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();
                            else
                                railtype = "нет данных";

                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                            else
                                fastenertype = "нет данных";

                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                            else
                                trackclass = "Нет данных (1)";

                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                            else
                                tracktype = "прямой";

                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                            else
                                width = "нет данных";

                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                Km = Convert.ToInt32(coords) / 10000,
                                Meter = Convert.ToInt32(coords) % 10000,
                                digression = DigressionName.BushBadnessOfSleepers,
                                Value = count,
                                Speed = speed,
                                AllowSpeed = "40/40",
                                RailType = railtype,
                                FastenerType = fastenertype,
                                TrackClass = trackclass,
                                TrackType = tracktype,
                                Width = width,
                                ItIsGap = true
                            });
                        }

                        if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                        {
                            switch (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type_Id)
                            {
                                case 1:
                                case 2:
                                case 3:
                                    if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).Any())
                                    {
                                        string speed, railtype, fastenertype, trackclass, tracktype, width, allowspeed;

                                        if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                        else
                                            speed = "нет данных";

                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                        if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                        else
                                            fastenertype = "нет данных";

                                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                        else
                                            trackclass = "Нет данных (1)";

                                        if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).Any())
                                            tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                        else
                                            tracktype = "прямой";

                                        if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        {
                                            width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width >= 1545)
                                                allowspeed = "0/0";
                                            else
                                                allowspeed = "15/15";
                                        }
                                        else
                                        {
                                            width = "нет данных";
                                            allowspeed = "15/15";
                                        }

                                        badnessOfSleepers.Add(new BadnessOfSleepers {
                                            Km = Convert.ToInt32(coords) / 10000,
                                            Meter = Convert.ToInt32(coords) % 10000,
                                            digression = DigressionName.BushBadnessOfSleepers,
                                            Value = count,
                                            Speed = speed,
                                            AllowSpeed = allowspeed,
                                            RailType = railtype,
                                            FastenerType = fastenertype,
                                            TrackClass = trackclass,
                                            TrackType = tracktype,
                                            Width = width,
                                            ItIsGap = true
                                        });
                                    }
                                    else
                                    {
                                        string speed, railtype, fastenertype, trackclass, tracktype, width;

                                        if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                        else
                                            speed = "нет данных";

                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                        if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                        else
                                            fastenertype = "нет данных";

                                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                        else
                                            trackclass = "Нет данных (1)";

                                        if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                        else
                                            tracktype = "прямой";

                                        if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                        else
                                            width = "нет данных";

                                        badnessOfSleepers.Add(new BadnessOfSleepers {
                                            Km = Convert.ToInt32(coords) / 10000,
                                            Meter = Convert.ToInt32(coords) % 10000,
                                            digression = DigressionName.BushBadnessOfSleepers,
                                            Value = count,
                                            Speed = speed,
                                            AllowSpeed = "40/25",
                                            RailType = railtype,
                                            FastenerType = fastenertype,
                                            TrackClass = trackclass,
                                            TrackType = tracktype,
                                            Width = width,
                                            ItIsGap = true
                                        });
                                    }
                                    break;

                                case 4:
                                case 5:
                                    {
                                        string speed, railtype, fastenertype, trackclass, tracktype, width, allowspeed;

                                        if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                        else
                                            speed = "нет данных";

                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                        if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                        else
                                            fastenertype = "нет данных";

                                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                        else
                                            trackclass = "Нет данных (1)";

                                        if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                        else
                                            tracktype = "прямой";

                                        if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        {
                                            width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width >= 1545)
                                                allowspeed = "0/0";
                                            else
                                                allowspeed = "15/15";
                                        }
                                        else
                                        {
                                            width = "нет данных";
                                            allowspeed = "15/15";
                                        }

                                        badnessOfSleepers.Add(new BadnessOfSleepers {
                                            Km = Convert.ToInt32(coords) / 10000,
                                            Meter = Convert.ToInt32(coords) % 10000,
                                            digression = DigressionName.BushBadnessOfSleepers,
                                            Value = count,
                                            Speed = speed,
                                            AllowSpeed = allowspeed,
                                            RailType = railtype,
                                            FastenerType = fastenertype,
                                            TrackClass = trackclass,
                                            TrackType = tracktype,
                                            Width = width,
                                            ItIsGap = true
                                        });
                                    }
                                    break;
                            }
                        }

                        count = 0;
                        coords = 0.0;
                    }
                    else if (count >= 4)
                    {
                        bool hasGap = rdStructureRepository.GetGapsBetweenCoords(process.Id, trackId, coords, badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0).Any();

                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                        {
                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Id > 3)
                                hasGap = false;
                        }

                        if (hasGap)
                        {
                            string speed, railtype, fastenertype, trackclass, tracktype, width;

                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                            else
                                speed = "нет данных";

                            if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();
                            else
                                railtype = "нет данных";

                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                            else
                                fastenertype = "нет данных";

                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                            else
                                trackclass = "Нет данных (1)";

                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                            else
                                tracktype = "прямой";

                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                            else
                                width = "нет данных";

                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                Km = Convert.ToInt32(coords) / 10000,
                                Meter = Convert.ToInt32(coords) % 10000,
                                digression = DigressionName.BushBadnessOfSleepers,
                                Value = count,
                                Speed = speed,
                                AllowSpeed = "40/40",
                                RailType = railtype,
                                FastenerType = fastenertype,
                                TrackClass = trackclass,
                                TrackType = tracktype,
                                Width = width,
                                ItIsGap = true
                            });
                        }

                        if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                        {
                            switch (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type_Id)
                            {
                                case 1:
                                case 2:
                                case 3:
                                    if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).Any())
                                    {
                                        string speed, railtype, fastenertype, trackclass, tracktype, width;

                                        if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                        else
                                            speed = "нет данных";

                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                        if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                        else
                                            fastenertype = "нет данных";

                                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                        else
                                            trackclass = "Нет данных (1)";

                                        tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).First().Radius).ToString();

                                        if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                        else
                                            width = "нет данных";

                                        badnessOfSleepers.Add(new BadnessOfSleepers {
                                            Km = Convert.ToInt32(coords) / 10000,
                                            Meter = Convert.ToInt32(coords) % 10000,
                                            digression = DigressionName.BushBadnessOfSleepers,
                                            Value = count,
                                            Speed = speed,
                                            AllowSpeed = "40/25",
                                            RailType = railtype,
                                            FastenerType = fastenertype,
                                            TrackClass = trackclass,
                                            TrackType = tracktype,
                                            Width = width,
                                            ItIsGap = true
                                        });
                                    }
                                    else
                                    {
                                        string speed, railtype, fastenertype, trackclass, tracktype, width;

                                        if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                        else
                                            speed = "нет данных";

                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                        if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                        else
                                            fastenertype = "нет данных";

                                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                        else
                                            trackclass = "Нет данных (1)";

                                        if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                        else
                                            tracktype = "прямой";

                                        if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                        else
                                            width = "нет данных";

                                        badnessOfSleepers.Add(new BadnessOfSleepers {
                                            Km = Convert.ToInt32(coords) / 10000,
                                            Meter = Convert.ToInt32(coords) % 10000,
                                            digression = DigressionName.BushBadnessOfSleepers,
                                            Value = count,
                                            Speed = speed,
                                            AllowSpeed = "60/40",
                                            RailType = railtype,
                                            FastenerType = fastenertype,
                                            TrackClass = trackclass,
                                            TrackType = tracktype,
                                            Width = width,
                                            ItIsGap = true
                                        });
                                    }
                                    break;

                                case 4:
                                case 5:
                                    if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).Any())
                                    {
                                        string speed, railtype, fastenertype, trackclass, tracktype, width, allowspeed;

                                        if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                        else
                                            speed = "нет данных";

                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                        if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                        else
                                            fastenertype = "нет данных";

                                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                        else
                                            trackclass = "Нет данных (1)";

                                        tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).First().Radius).ToString();

                                        if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        {
                                            width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();

                                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width >= 1545)
                                                allowspeed = "0/0";
                                            else
                                                allowspeed = "15/15";
                                        }
                                        else
                                        {
                                            width = "нет данных";
                                            allowspeed = "15/15";
                                        }

                                        badnessOfSleepers.Add(new BadnessOfSleepers {
                                            Km = Convert.ToInt32(coords) / 10000,
                                            Meter = Convert.ToInt32(coords) % 10000,
                                            digression = DigressionName.BushBadnessOfSleepers,
                                            Value = count,
                                            Speed = speed,
                                            AllowSpeed = allowspeed,
                                            RailType = railtype,
                                            FastenerType = fastenertype,
                                            TrackClass = trackclass,
                                            TrackType = tracktype,
                                            Width = width,
                                            ItIsGap = true
                                        });
                                    }
                                    else
                                    {
                                        string speed, railtype, fastenertype, trackclass, tracktype, width;

                                        if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                        else
                                            speed = "нет данных";

                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                        if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                        else
                                            fastenertype = "нет данных";

                                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                        else
                                            trackclass = "Нет данных (1)";

                                        if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                        else
                                            tracktype = "прямой";

                                        if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                        else
                                            width = "нет данных";

                                        badnessOfSleepers.Add(new BadnessOfSleepers {
                                            Km = Convert.ToInt32(coords) / 10000,
                                            Meter = Convert.ToInt32(coords) % 10000,
                                            digression = DigressionName.BushBadnessOfSleepers,
                                            Value = count,
                                            Speed = speed,
                                            AllowSpeed = "40/25",
                                            RailType = railtype,
                                            FastenerType = fastenertype,
                                            TrackClass = trackclass,
                                            TrackType = tracktype,
                                            Width = width,
                                            ItIsGap = true
                                        });
                                    }
                                    break;
                            }
                        }

                        count = 0;
                        coords = 0.0;
                    }
                    else if (count >= 3)
                    {
                        bool hasGap = rdStructureRepository.GetGapsBetweenCoords(process.Id, trackId, coords, badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0).Any();

                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                        {
                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Id > 3)
                                hasGap = false;
                        }

                        if (hasGap)
                        {
                            string speed, railtype, fastenertype, trackclass, tracktype, width;

                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                            else
                                speed = "нет данных";

                            if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();
                            else
                                railtype = "нет данных";

                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                            else
                                fastenertype = "нет данных";

                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                            else
                                trackclass = "Нет данных (1)";

                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                            else
                                tracktype = "прямой";

                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                            else
                                width = "нет данных";

                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                Km = Convert.ToInt32(coords) / 10000,
                                Meter = Convert.ToInt32(coords) % 10000,
                                digression = DigressionName.BushBadnessOfSleepers,
                                Value = count,
                                Speed = speed,
                                AllowSpeed = "40/40",
                                RailType = railtype,
                                FastenerType = fastenertype,
                                TrackClass = trackclass,
                                TrackType = tracktype,
                                Width = width,
                                ItIsGap = true
                            });
                        }

                        if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                        {
                            switch (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type_Id)
                            {
                                case 4:
                                case 5:
                                    if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).Any())
                                    {
                                        string speed, railtype, fastenertype, trackclass, tracktype, width;

                                        if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                        else
                                            speed = "нет данных";

                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                        if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                        else
                                            fastenertype = "нет данных";

                                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                        else
                                            trackclass = "Нет данных (1)";

                                        tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords) && Convert.ToInt32(s.Radius) < 650).First().Radius).ToString();

                                        if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                        else
                                            width = "нет данных";

                                        badnessOfSleepers.Add(new BadnessOfSleepers {
                                            Km = Convert.ToInt32(coords) / 10000,
                                            Meter = Convert.ToInt32(coords) % 10000,
                                            digression = DigressionName.BushBadnessOfSleepers,
                                            Value = count,
                                            Speed = speed,
                                            AllowSpeed = "40/25",
                                            RailType = railtype,
                                            FastenerType = fastenertype,
                                            TrackClass = trackclass,
                                            TrackType = tracktype,
                                            Width = width,
                                            ItIsGap = true
                                        });
                                    }
                                    else
                                    {
                                        string speed, railtype, fastenertype, trackclass, tracktype, width;

                                        if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                        else
                                            speed = "нет данных";

                                        railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                        if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                        else
                                            fastenertype = "нет данных";

                                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                        else
                                            trackclass = "Нет данных (1)";

                                        if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                        else
                                            tracktype = "прямой";

                                        if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                            width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                        else
                                            width = "нет данных";

                                        badnessOfSleepers.Add(new BadnessOfSleepers {
                                            Km = Convert.ToInt32(coords) / 10000,
                                            Meter = Convert.ToInt32(coords) % 10000,
                                            digression = DigressionName.BushBadnessOfSleepers,
                                            Value = count,
                                            Speed = speed,
                                            AllowSpeed = "50/40",
                                            RailType = railtype,
                                            FastenerType = fastenertype,
                                            TrackClass = trackclass,
                                            TrackType = tracktype,
                                            Width = width,
                                            ItIsGap = true
                                        });
                                    }
                                    break;
                            }
                        }

                        count = 0;
                        coords = 0.0;
                    }
                    else if (count >= 2)
                    {
                        bool hasGap = rdStructureRepository.GetGapsBetweenCoords(process.Id, trackId, coords, badSleepers[i - 1].Km * 10000 + badSleepers[i - 1].Meter + badSleepers[i - 1].Mm / 1000.0).Any();

                        if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                        {
                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Id > 3)
                                hasGap = false;
                        }

                        if (hasGap)
                        {
                            string speed, railtype, fastenertype, trackclass, tracktype, width;

                            if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                            else
                                speed = "нет данных";

                            if (railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();
                            else
                                railtype = "нет данных";

                            if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                            else
                                fastenertype = "нет данных";

                            if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                            else
                                trackclass = "Нет данных (1)";

                            if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                tracktype = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                            else
                                tracktype = "прямой";

                            if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                            else
                                width = "нет данных";

                            badnessOfSleepers.Add(new BadnessOfSleepers {
                                Km = Convert.ToInt32(coords) / 10000,
                                Meter = Convert.ToInt32(coords) % 10000,
                                digression = DigressionName.BushBadnessOfSleepers,
                                Value = count,
                                Speed = speed,
                                AllowSpeed = "40/40",
                                RailType = railtype,
                                FastenerType = fastenertype,
                                TrackClass = trackclass,
                                TrackType = tracktype,
                                Width = width,
                                ItIsGap = true
                            });
                        }

                        count = 0;
                        coords = 0.0;
                    }
                }
            }

            foreach (var km in badSleepers.GroupBy(s => s.Km).Select(s => s.Key))
            {
                if (badSleepers.Where(s => s.Km == km).Count() >= 350)
                {
                    int tracktype = 1;
                    if (trackclasses.Where(s => s.Start_Km <= km && s.Final_Km >= km).Any())
                        tracktype = trackclasses.Where(s => s.Start_Km <= km && s.Final_Km >= km).First().Class_Id;

                    switch (tracktype)
                    {
                        case 1:
                        case 2:
                            {
                                int percentage;
                                string speed, railtype, fastenertype, trackclass, tracktypeStr, width, allowspeed = String.Empty;
                                if (tracktype == 1)
                                    percentage = badSleepers.Where(s => s.Km == km).Count() * 100 / 2000;
                                else
                                    percentage = badSleepers.Where(s => s.Km == km).Count() * 100 / 1840;

                                if (railstype.Where(s => s.Start_Km <= km && s.Final_Km >= km).Any())
                                {
                                    switch (railstype.Where(s => s.Start_Km <= km && s.Final_Km >= km).First().Type_Id)
                                    {
                                        case 1:
                                        case 2:
                                        case 3:
                                            {
                                                if (curves.Where(s => s.Start_Km <= km && s.Final_Km >= km && Convert.ToInt32(s.Radius) < 650).Any())
                                                {
                                                    if (percentage >= 20 && percentage <= 24)
                                                    {
                                                        allowspeed = "60/50";
                                                    }
                                                    else if (percentage >= 25 && percentage <= 29)
                                                    {
                                                        allowspeed = "50/40";
                                                    }
                                                    else if (percentage >= 30 && percentage <= 35)
                                                    {
                                                        allowspeed = "40/25";
                                                    }
                                                    else if (percentage > 35)
                                                    {
                                                        allowspeed = "25/15";
                                                    }
                                                }
                                                else
                                                {
                                                    if (percentage >= 20 && percentage <= 24)
                                                    {
                                                        allowspeed = "70/60";
                                                    }
                                                    else if (percentage >= 25 && percentage <= 29)
                                                    {
                                                        allowspeed = "60/50";
                                                    }
                                                    else if (percentage >= 30 && percentage <= 35)
                                                    {
                                                        allowspeed = "50/40";
                                                    }
                                                    else if (percentage > 35)
                                                    {
                                                        allowspeed = "40/25";
                                                    }
                                                }
                                            }
                                            break;

                                        case 4:
                                        case 5:
                                            {
                                                if (curves.Where(s => s.Start_Km <= km && s.Final_Km >= km && Convert.ToInt32(s.Radius) < 650).Any())
                                                {
                                                    if (percentage >= 20 && percentage <= 24)
                                                    {
                                                        allowspeed = "50/40";
                                                    }
                                                    else if (percentage >= 25 && percentage <= 29)
                                                    {
                                                        allowspeed = "40/25";
                                                    }
                                                    else if (percentage > 30)
                                                    {
                                                        allowspeed = "25/15";
                                                    }
                                                }
                                                else
                                                {
                                                    if (percentage >= 20 && percentage <= 24)
                                                    {
                                                        allowspeed = "60/50";
                                                    }
                                                    else if (percentage >= 25 && percentage <= 29)
                                                    {
                                                        allowspeed = "50/40";
                                                    }
                                                    else if (percentage > 30)
                                                    {
                                                        allowspeed = "40/25";
                                                    }
                                                }
                                            }
                                            break;
                                    }
                                }

                                if (String.IsNullOrEmpty(allowspeed))
                                {
                                    if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                    else
                                        speed = "нет данных";

                                    railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                    if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                    else
                                        fastenertype = "нет данных";

                                    if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                    else
                                        trackclass = "Нет данных (1)";

                                    if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        tracktypeStr = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                    else
                                        tracktypeStr = "прямой";

                                    if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                    else
                                        width = "нет данных";

                                    badnessOfSleepers.Add(new BadnessOfSleepers {
                                        Km = km,
                                        digression = DigressionName.PercentageBadnessOfSleepers,
                                        Value = percentage,
                                        Speed = speed,
                                        AllowSpeed = allowspeed,
                                        RailType = railtype,
                                        FastenerType = fastenertype,
                                        TrackClass = trackclass,
                                        TrackType = tracktypeStr,
                                        Width = width,
                                        ItIsGap = false
                                    });
                                }
                            }
                            break;

                        case 3:
                            {
                                int percentage = badSleepers.Where(s => s.Km == km).Count() * 100 / 1840;
                                string speed, railtype, fastenertype, trackclass, tracktypeStr, width, allowspeed = String.Empty;

                                if (railstype.Where(s => s.Start_Km <= km && s.Final_Km >= km).Any())
                                {
                                    switch (railstype.Where(s => s.Start_Km <= km && s.Final_Km >= km).First().Type_Id)
                                    {
                                        case 1:
                                        case 2:
                                        case 3:
                                            {
                                                if (curves.Where(s => s.Start_Km <= km && s.Final_Km >= km && Convert.ToInt32(s.Radius) < 650).Any())
                                                {
                                                    if (percentage >= 25 && percentage <= 29)
                                                    {
                                                        allowspeed = "60/50";
                                                    }
                                                    else if (percentage >= 30 && percentage <= 39)
                                                    {
                                                        allowspeed = "50/40";
                                                    }
                                                    else if (percentage >= 40 && percentage <= 45)
                                                    {
                                                        allowspeed = "40/25";
                                                    }
                                                    else if (percentage > 45)
                                                    {
                                                        allowspeed = "25/15";
                                                    }
                                                }
                                                else
                                                {
                                                    if (percentage >= 25 && percentage <= 29)
                                                    {
                                                        allowspeed = "70/60";
                                                    }
                                                    else if (percentage >= 35 && percentage <= 39)
                                                    {
                                                        allowspeed = "60/50";
                                                    }
                                                    else if (percentage >= 40 && percentage <= 45)
                                                    {
                                                        allowspeed = "50/40";
                                                    }
                                                    else if (percentage > 45)
                                                    {
                                                        allowspeed = "40/25";
                                                    }
                                                }
                                            }
                                            break;

                                        case 4:
                                        case 5:
                                            {
                                                if (curves.Where(s => s.Start_Km <= km && s.Final_Km >= km && Convert.ToInt32(s.Radius) < 650).Any())
                                                {
                                                    if (percentage >= 25 && percentage <= 29)
                                                    {
                                                        allowspeed = "50/40";
                                                    }
                                                    else if (percentage >= 30 && percentage <= 39)
                                                    {
                                                        allowspeed = "40/25";
                                                    }
                                                    else if (percentage >= 40)
                                                    {
                                                        allowspeed = "25/15";
                                                    }
                                                }
                                                else
                                                {
                                                    if (percentage >= 25 && percentage <= 29)
                                                    {
                                                        allowspeed = "60/50";
                                                    }
                                                    else if (percentage >= 30 && percentage <= 39)
                                                    {
                                                        allowspeed = "50/40";
                                                    }
                                                    else if (percentage >= 40)
                                                    {
                                                        allowspeed = "40/25";
                                                    }
                                                }
                                            }
                                            break;
                                    }
                                }

                                if (String.IsNullOrEmpty(allowspeed))
                                {
                                    if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                    else
                                        speed = "нет данных";

                                    railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                    if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                    else
                                        fastenertype = "нет данных";

                                    if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                    else
                                        trackclass = "Нет данных (1)";

                                    if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        tracktypeStr = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                    else
                                        tracktypeStr = "прямой";

                                    if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                    else
                                        width = "нет данных";

                                    badnessOfSleepers.Add(new BadnessOfSleepers {
                                        Km = km,
                                        digression = DigressionName.PercentageBadnessOfSleepers,
                                        Value = percentage,
                                        Speed = speed,
                                        AllowSpeed = allowspeed,
                                        RailType = railtype,
                                        FastenerType = fastenertype,
                                        TrackClass = trackclass,
                                        TrackType = tracktypeStr,
                                        Width = width,
                                        ItIsGap = false
                                    });
                                }
                            }
                            break;

                        case 4:
                        case 5:
                            {
                                int percentage = badSleepers.Where(s => s.Km == km).Count() * 100 / 1840;
                                string speed, railtype, fastenertype, trackclass, tracktypeStr, width, allowspeed = String.Empty;

                                if (railstype.Where(s => s.Start_Km <= km && s.Final_Km >= km).Any())
                                {
                                    switch (railstype.Where(s => s.Start_Km <= km && s.Final_Km >= km).First().Type_Id)
                                    {
                                        case 1:
                                        case 2:
                                        case 3:
                                            {
                                                if (curves.Where(s => s.Start_Km <= km && s.Final_Km >= km && Convert.ToInt32(s.Radius) < 650).Any())
                                                {
                                                    if (percentage >= 30 && percentage <= 34)
                                                    {
                                                        allowspeed = "60/50";
                                                    }
                                                    else if (percentage >= 35 && percentage <= 44)
                                                    {
                                                        allowspeed = "50/40";
                                                    }
                                                    else if (percentage >= 45 && percentage <= 50)
                                                    {
                                                        allowspeed = "40/25";
                                                    }
                                                    else if (percentage > 50)
                                                    {
                                                        allowspeed = "25/15";
                                                    }
                                                }
                                                else
                                                {
                                                    if (percentage >= 30 && percentage <= 34)
                                                    {
                                                        allowspeed = "70/60";
                                                    }
                                                    else if (percentage >= 35 && percentage <= 44)
                                                    {
                                                        allowspeed = "60/50";
                                                    }
                                                    else if (percentage >= 45 && percentage <= 50)
                                                    {
                                                        allowspeed = "50/40";
                                                    }
                                                    else if (percentage > 50)
                                                    {
                                                        allowspeed = "40/25";
                                                    }
                                                }
                                            }
                                            break;

                                        case 4:
                                        case 5:
                                            {
                                                if (curves.Where(s => s.Start_Km <= km && s.Final_Km >= km && Convert.ToInt32(s.Radius) < 650).Any())
                                                {
                                                    if (percentage >= 30 && percentage <= 34)
                                                    {
                                                        allowspeed = "50/40";
                                                    }
                                                    else if (percentage >= 35 && percentage <= 44)
                                                    {
                                                        allowspeed = "40/25";
                                                    }
                                                    else if (percentage >= 45)
                                                    {
                                                        allowspeed = "25/15";
                                                    }
                                                }
                                                else
                                                {
                                                    if (percentage >= 30 && percentage <= 34)
                                                    {
                                                        allowspeed = "60/50";
                                                    }
                                                    else if (percentage >= 35 && percentage <= 44)
                                                    {
                                                        allowspeed = "50/40";
                                                    }
                                                    else if (percentage >= 45)
                                                    {
                                                        allowspeed = "40/25";
                                                    }
                                                }
                                            }
                                            break;
                                    }
                                }

                                if (String.IsNullOrEmpty(allowspeed))
                                {
                                    if (speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        speed = speeds.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Select(s => s.Passenger.ToString() + "/" + s.Freight.ToString()).First();
                                    else
                                        speed = "нет данных";

                                    railtype = railstype.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Type.ToUpper();

                                    if (braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        fastenertype = braces.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Brace_Type;
                                    else
                                        fastenertype = "нет данных";

                                    if (trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        trackclass = trackclasses.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Class_Type;
                                    else
                                        trackclass = "Нет данных (1)";

                                    if (curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        tracktypeStr = "кривая R-" + Convert.ToInt32(curves.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Radius).ToString();
                                    else
                                        tracktypeStr = "прямой";

                                    if (norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).Any())
                                        width = norma.Where(s => s.Start_Km * 10000 + s.Start_M <= Convert.ToInt32(coords) && s.Final_Km * 10000 + s.Final_M >= Convert.ToInt32(coords)).First().Norma_Width.ToString();
                                    else
                                        width = "нет данных";

                                    badnessOfSleepers.Add(new BadnessOfSleepers {
                                        Km = km,
                                        digression = DigressionName.PercentageBadnessOfSleepers,
                                        Value = percentage,
                                        Speed = speed,
                                        AllowSpeed = allowspeed,
                                        RailType = railtype,
                                        FastenerType = fastenertype,
                                        TrackClass = trackclass,
                                        TrackType = tracktypeStr,
                                        Width = width,
                                        ItIsGap = false
                                    });
                                }
                            }
                            break;
                    }
                }
            }

            return badnessOfSleepers;
        }

        public int Km { get; set; }
        public int Meter { get; set; }
        public int Picket { get { return Meter / 100 + 1; } }
        public DigName digression { get; set; }
        public int Value { get; set; }

        public string Speed { get; set; }
        public string AllowSpeed { get; set; }
        public string RailType { get; set; }
        public string FastenerType { get; set; }
        public string TrackClass { get; set; }
        public string TrackType { get; set; }
        public string Width { get; set; }
        public bool ItIsGap { get; set; }
    }
}
