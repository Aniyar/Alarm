using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlarmPP.Web.Components.Diagram;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using ALARm.Core;
using ALARm.Core.Report;
using MatBlazor;
using System.Globalization;
using System.Drawing;
using System.Threading;
using System.Text;
using System.ServiceProcess;
using ALARm.DataAccess;
using AlarmPP.Web.Services;

namespace AlarmPP.Web.Components.Diagram
{
    public partial class TrackPanel : ComponentBase
    {
        [Parameter]
        public List<Kilometer> Kilometers { get; set; }
        [Parameter]
        public double CurrentPosition { get; set; } = 0;
        // double SliderXPosition = 0, SliderCenterXPosition = 25, SliderYPosition=0;
        private int Width { get; set; } = 4000;
        double ScrollLeft = 0;
        double ScrollTop = 0;

        private DigressionTable digressionTable;
        private bool StopDialog { get; set; } = false;
        private bool loading = false;
        private Kilometer CurrKm { get; set; } = null;
        public bool MousePressed { get; set; } = false;
        public async Task OnMouseMove(MouseEventArgs args)
        {
            if (args.Buttons == 1)
            {
                MousePressed = true;
                AppData.SliderXPosition = Math.Round(ScrollLeft + args.ClientX - 75);
                AppData.SliderCenterXPosition = AppData.SliderXPosition + 25;
                AppData.SliderYPosition = AppData.SliderXPosition * 10 + 240;
                object[] paramss = new object[] { AppData.SliderXPosition * 10 };
                await JSRuntime.InvokeVoidAsync("ScrollMainSvg", paramss);
                MousePressed = false;
            }
        }

        public async Task OnMainMouseMove(MouseEventArgs args)
        {
            if (args.Buttons == 1)
            {
                MousePressed = true;
                object[] paramss = new object[] { "" };
                ScrollTop = Math.Round(await JSRuntime.InvokeAsync<double>("ScrollHeadSvg", paramss));
                AppData.SliderYPosition = Math.Round(ScrollTop + args.ClientY - 200);
                AppData.SliderXPosition = Math.Round(AppData.SliderYPosition / 10);
                AppData.SliderCenterXPosition = AppData.SliderXPosition + 25;
                MousePressed = false;
            }
        }

        private async Task OnTimedEventAsync()
        {
            Random rnd = new Random();

            AppData.Processing = !AppData.Processing;
            OutData prevData = null;

            while (!AppData.Processing)
            {
                //AppData.CurrentProfileLeft();
                //AppData.CurrentFrameIndex++;

                var outdatas     = RdStructureRepository.GetNextOutDatas(AppData.Meter, 100, AppData.Trip.Id);
                var profileDatas = RdStructureRepository.GetNextProfileDatas(AppData.ProfileMeter, 100, AppData.Trip.Id);

                foreach (var profileData in profileDatas)
                {
                    int length = 0;
                    foreach (var km in AppData.Kilometers)
                    {
                        length += km.GetLength();
                        if (AppData.ProfileMeter < length)
                        {
                            km.CrossRailProfile.ParseDB(profileData, km);
                            AppData.ProfileMeter += 1;
                            AppData.CalibrConstLeft = km.CrossRailProfile.DownhillLeft.Any() ? km.CrossRailProfile.DownhillLeft.Last().RadianToAngle().ToString("0.00").Replace(",", ".") : "0";
                            AppData.CalibrConstRight = km.CrossRailProfile.DownhillRight.Any() ? km.CrossRailProfile.DownhillRight.Last().RadianToAngle().ToString("0.00").Replace(",", ".") : "0   ";
                            break;
                        }
                    }
                }

                object[] paramss = new object[] { 
                    AppData.Meter + outdatas.Count ,
                    AppData.ProfileMeter + profileDatas.Count
                };
                if (OnlineModeData.AutoScroll)
                    await JSRuntime.InvokeVoidAsync("ScrollMainSvg2", paramss);

                var profileIndex = 0;
                foreach (var outdata in outdatas)
                {
                    var crashed = false;
                    if (prevData != null)
                    {
                        if (outdata._meters - prevData._meters > 1)
                        {

                            if (CurrKm != null)

                                CurrKm.Crashed = true;
                            crashed = true;
                        }

                    }
                    prevData = outdata;
                    if (OnlineModeData.AutoScroll)
                        AppData.SliderYPosition = AppData.Meter;
                    int length = 0;
                    foreach (var kilometer in AppData.Kilometers)
                    {

                        length += kilometer.GetLength();
                        if (AppData.Meter < length)
                        {
                            CurrKm = kilometer;

                            //Console.WriteLine($"{kilometer.Number}km - {kilometer.Meter} m");
                            //if (kilometer.Meter == 1027 && kilometer.Number ==712)
                            //    Console.WriteLine(kilometer.Number);

                            //Console.WriteLine(kilometer.Meter);

                            if (kilometer.Meter <= 0)
                            {

                                

                                if (crashed)
                                    kilometer.Meter = outdata.meter;
                                kilometer.Trip = AppData.Trip;
                                kilometer.Start_Index = AppData.Meter;
                                kilometer.Passage_time = DateTime.Now;
                                kilometer.Trip = AppData.Trip;
                            }
                            if (AppData.Kilometers.IndexOf(kilometer) >= 2)
                            // если километр свежий
                            {
                                //  GC.Collect();
                                int prevIndex = AppData.Kilometers.IndexOf(kilometer) - 2;
                                if (!AppData.Kilometers[prevIndex].IsPrinted)
                                {
                                    List<double> prevLevelAvgPart = new List<double>();
                                    List<double> nextLevelAvgPart = new List<double>();
                                    List<double> prevStrightAvgPart = new List<double>();
                                    List<double> nextStrightAvgPart = new List<double>();

                                    int n = 200;
                                    int prevN = 200;
                                    if (prevIndex > 0)
                                    {
                                        n = AppData.Kilometers[prevIndex - 1].LevelAvg.Count > prevN ? prevN : AppData.Kilometers[prevIndex - 1].LevelAvg.Count;
                                        
                                        prevLevelAvgPart = AppData.Kilometers[prevIndex - 1].LevelAvg.GetRange(AppData.Kilometers[prevIndex - 1].LevelAvg.Count - n, n);
                                        prevStrightAvgPart = AppData.Kilometers[prevIndex - 1].StrightAvg.GetRange(AppData.Kilometers[prevIndex - 1].StrightAvg.Count - n, n);

                                    }
                                    n = AppData.Kilometers[prevIndex + 1].LevelAvg.Count > prevN ? prevN : AppData.Kilometers[prevIndex + 1].LevelAvg.Count;
                                    nextLevelAvgPart = AppData.Kilometers[prevIndex + 1].LevelAvg.GetRange(0, n);

                                    //var nextGetNext = RdStructureRepository.GetNextPart(AppData.Kilometers[prevIndex].RealKm.Last, AppData.Kilometers[prevIndex]._Meters.Last, n);

                                    nextStrightAvgPart = AppData.Kilometers[prevIndex + 1].StrightAvg.GetRange(0, n);
                                    try
                                    {
                                        if (AppData.Kilometers[prevIndex].WriteData(AppData.Trip, prevLevelAvgPart, nextLevelAvgPart, prevStrightAvgPart, nextStrightAvgPart, MainTrackStructureRepository, RdStructureRepository, KMS: AppData.Kilometers))
                                        {
                                            RdStructureRepository.InsertKilometer(AppData.Kilometers[prevIndex]);
                                            var mainParameters = new ALARm.Core.Report.MainParameters(RdStructureRepository, MainTrackStructureRepository, AdmStructureRepository);
                                            var template = RdStructureRepository.GetReportTemplate("MainParametersOnline");
                                            try
                                            {
                                                mainParameters.diagramName = "Оригинал";
                                                mainParameters.Process(template, AppData.Kilometers[prevIndex], AppData.Trip, OnlineModeData.AutoPrint);
                                                //RdStructureRepository.SendEkasuiData(AppData.Trip, AppData.Kilometers[prevIndex].Number);

                                                AppData.Kilometers[prevIndex].Digressions = RdStructureRepository.GetDigressionMarks(AppData.Trip.Id, AppData.Kilometers[prevIndex].Number, AppData.Kilometers[prevIndex].Track_id, new int[] { 3, 4 });
                                              


                                                if (AppData.Kilometers[prevIndex].Digressions != null)
                                                {
                                                    var dangerouses = AppData.Kilometers[prevIndex].Digressions.Where(digression => digression.Degree == 4).ToList();
                                                    StringBuilder dangers = new StringBuilder();
                                                    foreach (var dangerous in dangerouses)
                                                    {
                                                        if (!AppData.Kilometers[prevIndex].Crashed)
                                                        {
                                                            dangers.Append($@"{ dangerous.DigName } 4 степени (знач.: {dangerous.Value}) на {dangerous.Km} км {dangerous.Meter} метре. Ограничение скорости: {dangerous.LimitSpeed}" + Environment.NewLine);
                                                        }
                                                    }
                                                    if (dangers.Length != 0)
                                                    {
                                                        Toaster.Add(dangers.ToString(), MatToastType.Danger, "Внимание!!!", "");
                                                        await JSRuntime.InvokeVoidAsync("beep", paramss);
                                                    }
                                                    dangers = null;
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine("AppData ошибка обработки:" + e.Message);
                                            }
                                        }
                                    }catch (Exception e) {

                                        Console.WriteLine("WRITE ERROR " + e.Message);
                                    }
                                }
                            }
                            if (outdata.correction != 0)
                            {
                                if ((OnlineModeData.Corrections.Count < 1) || ((Math.Abs(OnlineModeData.Corrections.Last() - AppData.Meter) > 50) && (AppData.Meter > OnlineModeData.Corrections.Last())))
                                {
                                    OnlineModeData.Corrections.Add(AppData.Meter);
                                    var kmforchange = AppData.Kilometers.IndexOf(kilometer) > 0 ? AppData.Kilometers[AppData.Kilometers.IndexOf(kilometer) - 1] : kilometer;
                                    //если первая прибивка
                                    if ((AppData.Kilometers.IndexOf(kilometer) < 2) && (OnlineModeData.Corrections.Count == 1))
                                    {
                                        //если два разных путей на одном километре
                                        if ((kmforchange != kilometer) && (kilometer.Number == kmforchange.Number))
                                        {
                                            if (kmforchange.Direction == Direction.Reverse)
                                            {
                                                kmforchange.Final_m -= (kilometer.GetLength() - kilometer.Meter);
                                            }
                                            else
                                            {
                                                kmforchange.Start_m += (kilometer.GetLength() - kilometer.Meter);
                                            }
                                            AppData.Meter = AppData.Meter - (kilometer.GetLength() - kilometer.Meter);
                                            kilometer.CorrectionValue = -(kilometer.GetLength() - kilometer.Meter);
                                            kilometer.CorrectionMeter = kilometer.Meter;
                                            kilometer.CorrectionType = (CorrectionType)outdata.correction;
                                            Toaster.Add($"Коррекция на: {kilometer.GetLength() - kilometer.Meter} м.   ", MatToastType.Info, "Поездка в режиме онлайн");
                                            kmforchange.Clear(null, kilometer.GetLength() - kilometer.Meter);
                                            kilometer.Clear();
                                            break;
                                        }
                                        else
                                        if (kmforchange != kilometer)
                                        {
                                            if (kilometer.RealMeterInOnline < 500)
                                            {
                                                if (kmforchange.Direction == Direction.Direct)
                                                    kmforchange.Start_m -= kilometer.Meter;
                                                else
                                                    kmforchange.Final_m += kilometer.RealMeterInOnline;
                                                kilometer.Clear(kmforchange);
                                                kilometer.CorrectionValue = kilometer.Meter;
                                                Toaster.Add($"Коррекция на: {kilometer.CorrectionValue} м.", MatToastType.Info, "Поездка в режиме онлайн");
                                                kilometer.CorrectionMeter = kilometer.Meter;
                                                kilometer.CorrectionType = (CorrectionType)outdata.correction;
                                                break;
                                            }
                                            else
                                            {
                                                kilometer.Final_m = kilometer.RealMeterInOnline;
                                                kilometer.CorrectionValue = -(kilometer.GetLength() - kilometer.Meter);
                                                Toaster.Add($"Коррекция на: {kilometer.CorrectionValue} м.", MatToastType.Info, "Поездка в режиме онлайн");
                                                kilometer.CorrectionMeter = kilometer.Meter;
                                                kilometer.CorrectionType = (CorrectionType)outdata.correction;
                                            }
                                        }
                                    }
                                    //если последующая прибивка
                                    else
                                    {

                                        if (kilometer.RealMeterInOnline < 500)
                                        {
                                            kmforchange.Final_m += kilometer.RealMeterInOnline;
                                            AppData.Meter = AppData.Meter - kilometer.Meter;
                                            kilometer.CorrectionValue = kilometer.Meter;
                                            kilometer.CorrectionMeter = kilometer.Meter;
                                            Toaster.Add($"{kilometer.Number} Коррекция на: {kilometer.CorrectionValue} м. ", MatToastType.Info, "Поездка в режиме онлайн");
                                            kilometer.CorrectionType = (CorrectionType)outdata.correction;
                                            kilometer.Clear(kmforchange);
                                        }
                                        else
                                        {
                                            int prevLength = kilometer.GetLength();
                                            kilometer.Final_m = kilometer.RealMeterInOnline;
                                            kilometer.CorrectionValue = -(prevLength - kilometer.GetLength());
                                            Toaster.Add($"{kilometer.Number} км коррекция на:   {kilometer.CorrectionValue} ", MatToastType.Info, "Поездка в режиме онлайн");
                                            kilometer.CorrectionMeter = kilometer.Meter;
                                            kilometer.CorrectionType = (CorrectionType)outdata.correction;
                                        }
                                    }
                                }
                                outdata.correction = 0;
                            }
                            if (Math.Abs(OnlineModeData.GlobalMeter - outdata._meters) != 1)
                            {
                                // AppData.Meter = GetCurrentCoordinate(AppData.Kilometers, outdata.km, outdata.meter);
                            }
                            //AppData.Meter = GetCurrentCoordinate(AppData.Kilometers, outdata.km, outdata.meter);
                            //System.Console.WriteLine($"{AppData.Meter}-{OnlineModeData.GlobalMeter}");
                            OnlineModeData.GlobalMeter = outdata._meters;

                            //List<CamBitmap> bitmaps = new List<CamBitmap>()
                            //{
                            //    new CamBitmap(@"H:\059M\SKH-2-4\2020_06",".s10"),
                            //    new CamBitmap(@"H:\059M\SKH-2-2\2020_06", ".s9"),
                            //    new CamBitmap(@"H:\059M\Mid_Shpala\2020_06",".s11"),
                            //    new CamBitmap(@"G:\059m\SKH-1-1\2020_06",".s7"),
                            //    new CamBitmap(@"G:\059m\SKH-1-3\2020_06",".s8")

                            //};
                            //Parallel.ForEach(bitmaps, (bitmap) =>
                            //{
                            //   var result = OnlineModeData.GetBitmapAsync(bitmap.FilePath, AppData.Meter*5);
                            //    if (result != null)
                            //    {
                            //        bitmap.Bitmap = result;
                            //        bitmap.Bitmap = new Bitmap(bitmap.Bitmap, new Size(bitmap.Bitmap.Width / 4, bitmap.Bitmap.Height / 4));
                            //    }

                            //});
                            //AppData.CurrentFrame = new Bitmap(bitmaps[0].Bitmap.Width + bitmaps[1].Bitmap.Width + bitmaps[2].Bitmap.Width + bitmaps[3].Bitmap.Width + bitmaps[4].Bitmap.Width, bitmaps[0].Bitmap.Height);

                            //    Graphics g = Graphics.FromImage(AppData.CurrentFrame);
                            //    g.DrawImageUnscaled(bitmaps[0].Bitmap, 0, 0);
                            //    g.DrawImageUnscaled(bitmaps[1].Bitmap, bitmaps[0].Bitmap.Width, 0);
                            //    g.DrawImageUnscaled(bitmaps[2].Bitmap, bitmaps[0].Bitmap.Width + bitmaps[1].Bitmap.Width, 0);
                            //    g.DrawImageUnscaled(bitmaps[3].Bitmap, bitmaps[0].Bitmap.Width + bitmaps[1].Bitmap.Width + bitmaps[2].Bitmap.Width, 0);
                            //    g.DrawImageUnscaled(bitmaps[4].Bitmap, bitmaps[0].Bitmap.Width + bitmaps[1].Bitmap.Width + bitmaps[2].Bitmap.Width + bitmaps[3].Bitmap.Width, 0);

                            int currentMetre = kilometer.Direction == Direction.Direct ? (AppData.Meter - (length - kilometer.GetLength()) + kilometer.Start_m) : kilometer.Final_m - (AppData.Meter - (length - kilometer.GetLength()));
                            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
                            kilometer.AddData(outdata, currentMetre, AppData.Koefs);

                            kilometer.SideWearLeft_.Add(OnlineModeData.SideWearLeft);
                            kilometer.SideWearRight_.Add(OnlineModeData.SideWearRight);

                            AppData.Meter += 1;
                            OnlineModeData.CurrentFrameIndex = AppData.Meter;
                            OnlineModeData.GetBitmapAsync();

                            kilometer.DownHillLeft += (OnlineModeData.DownhillLeft.ToRadians() * kilometer.DegKoef).ToString().Replace(",", ".") + "," + kilometer.Meter.ToString().Replace(",", ".") + " ";
                            kilometer.DownHillRight += (OnlineModeData.DownhillRight.ToRadians() * kilometer.DegKoef).ToString().Replace(",", ".") + "," + kilometer.Meter.ToString().Replace(",", ".") + " ";
                            kilometer.VertWearLeft += (OnlineModeData.VertWearLeft * kilometer.WearKoef).ToString().Replace(",", ".") + "," + kilometer.Meter.ToString().Replace(",", ".") + " ";
                            kilometer.VertWearRight += (OnlineModeData.VertWearRight * kilometer.WearKoef).ToString().Replace(",", ".") + "," + kilometer.Meter.ToString().Replace(",", ".") + " ";
                            kilometer.SideWearLeft += (OnlineModeData.SideWearLeft * kilometer.WearKoef).ToString().Replace(",", ".") + "," + kilometer.Meter.ToString().Replace(",", ".") + " ";
                            kilometer.SideWearRight += (OnlineModeData.SideWearRight * kilometer.WearKoef).ToString().Replace(",", ".") + "," + kilometer.Meter.ToString().Replace(",", ".") + " ";
                            kilometer.HeadWear45Left += (OnlineModeData.Wear45Left * kilometer.WearKoef).ToString().Replace(",", ".") + "," + kilometer.Meter.ToString().Replace(",", ".") + " ";
                            kilometer.HeadWear45Right += (OnlineModeData.Wear45Right * kilometer.WearKoef).ToString().Replace(",", ".") + "," + kilometer.Meter.ToString().Replace(",", ".") + " ";
                            break;
                        }
                    }
                    profileIndex++;
                }

                if (AppData.Meter % 10 == 0)
                {
                    await Task.Delay(AppData.Speed);
                    StateHasChanged();
                }
            }
        }

        public async Task OnScroll()
        {
            object[] paramss = new object[] { "dgscroll" };
            ScrollLeft = await JSRuntime.InvokeAsync<double>("GetDgScrollLeft", paramss);
        }

        public async Task OnMainScroll()
        {
            object[] paramss = new object[] { "" };
            ScrollTop = Math.Round(await JSRuntime.InvokeAsync<double>("ScrollHeadSvg", paramss));
        }

        protected override void OnInitialized()
        {
            AppData.OnChange += StateHasChanged;
            StateHasChanged();
            if (AppData.Kilometers == null)
                return;
            Width = AppData.Kilometers.Count() * 1000;
        }

        private int GetXByDigressionName(DigressionMark digression)
        {
            switch (digression.Digression.Value)
            {
                case int v when v == DigressionName.Level.Value
                || v == DigressionName.Sag.Value:
                    return AppData.MainParamsPosition + AppData.LevelZero;
                case int v when v == DigressionName.DrawdownLeft.Value:
                    return AppData.DrawdownLeftPosition + AppData.DrawdownZero;
                case int v when v == DigressionName.DrawdownRight.Value:
                    return AppData.DrawdownRightPosition + AppData.DrawdownZero;
                case int v when v == DigressionName.StrighteningLeft.Value:
                    return AppData.StrightLeftPosition + AppData.StrightLeftZero;
                case int v when v == DigressionName.StrighteningRight.Value:
                    return AppData.StrgihtRightPosition + AppData.StrightRightZero;
                case int v when v == DigressionName.Broadening.Value:
                    return AppData.GaugePosition + AppData.GaugeZero;
                case int v when v == DigressionName.Constriction.Value:
                    return AppData.GaugePosition + AppData.GaugeZero;
                ///
                case int v when v == DigressionName.NoneStrightening.Value:
                    return AppData.NoneStrgihtLeftPosition + AppData.NoneStrgihtRightPosition;
                ///
                default:
                    return 0;
            }
        }
        public void Refresh()
        {
            StateHasChanged();
            if (AppData.WorkMode == Services.WorkMode.Online)
            {
                _ = OnTimedEventAsync();
            }
            try
            {
                OnlineModeData.ViewBoxLeft = "-100 -30 200 300"; // Уакытша
                OnlineModeData.ViewBoxRight = "-100 -30 200 300"; // Уакытша
                OnlineModeData.NominalTranslateLeft = "-10px,-10px"; // Уакытша
                OnlineModeData.NominalTranslateRight = "-10px,-10px"; // Уакытша
                OnlineModeData.NominalRotateLeft = "0deg"; // Уакытша
                OnlineModeData.NominalRotateRight = "0deg"; // Уакытша
                                                            //OnlineModeData.CalibrConstLeft = 0; // Уакытша
                                                            //OnlineModeData.CalibrConstRight = 0; // Уакытша
                OnlineModeData.NominalRailScheme = OnlineModeData.GetNominalRailScheme(Rails.r50);
                digressionTable.Refresh();
            }
            catch (Exception e)
            {
                Console.WriteLine("djfkshkf" + e.Message);
            }
        }

        public void RouteEditableChange(Trips trip)
        {
            OnlineModeData.RouteEditable = !OnlineModeData.RouteEditable;
            if (!OnlineModeData.RouteEditable)
            {
                MainTrackStructureRepository.InsertRoute(trip.Route, trip.Id);
                OnlineModeData.FirstLoad = true;
                AppData.ReloadKilometers();
                Toaster.Add("Маршрут успешно изменен", MatToastType.Info, "ALARmProcess");
            }
            _ = OnTimedEventAsync();
        }

        public async Task StopTripAsync()
        {
            loading = true;
            await Task.Delay(1);

            StateHasChanged();
            StopDialog = false;
            if (CurrKm != null)
            {
                int prevIndex = AppData.Kilometers.IndexOf(CurrKm) - 1;
                await PrintKm(prevIndex);
                await PrintKm(prevIndex + 1);
            }
            RdStructureRepository.CloseTrip();
            Toaster.Add("Проезд завершен", MatToastType.Success, "ALARMDK", "");
            await Task.Delay(3000);
            loading = false;
            NavManager.NavigateTo("/", true);
        }

        private async Task PrintKm(int prevIndex)
        {
            if (!AppData.Kilometers[prevIndex].IsPrinted)
            {
                List<double> prevLevelAvgPart = new List<double>();
                List<double> nextLevelAvgPart = new List<double>();
                List<double> prevStrightAvgPart = new List<double>();
                List<double> nextStrightAvgPart = new List<double>();

                int n = 200;
                int prevN = 200;
                if (prevIndex > 0)
                {
                    n = AppData.Kilometers[prevIndex - 1].LevelAvg.Count > prevN ? prevN : AppData.Kilometers[prevIndex - 1].LevelAvg.Count;
                    prevLevelAvgPart = AppData.Kilometers[prevIndex - 1].LevelAvg.GetRange(AppData.Kilometers[prevIndex - 1].LevelAvg.Count - n - 1, n);
                    prevStrightAvgPart = AppData.Kilometers[prevIndex - 1].StrightAvg.GetRange(AppData.Kilometers[prevIndex - 1].StrightAvg.Count - n - 1, n);
                }
                if ((prevIndex + 1) < AppData.Kilometers.Count)
                {
                    n = AppData.Kilometers[prevIndex + 1].LevelAvg.Count > prevN ? prevN : AppData.Kilometers[prevIndex + 1].LevelAvg.Count;

                    nextLevelAvgPart = AppData.Kilometers[prevIndex + 1].LevelAvg.GetRange(0, n);
                    nextStrightAvgPart = AppData.Kilometers[prevIndex + 1].StrightAvg.GetRange(0, n);
                } else
                {
                    nextLevelAvgPart = new List<double>();
                    nextStrightAvgPart = new List<double>();
                }


                if (AppData.Kilometers[prevIndex].WriteData(AppData.Trip, prevLevelAvgPart, nextLevelAvgPart, prevStrightAvgPart, nextStrightAvgPart, MainTrackStructureRepository, RdStructureRepository))
                {
                    RdStructureRepository.InsertKilometer(AppData.Kilometers[prevIndex]);
                    var mainParameters = new ALARm.Core.Report.MainParameters(RdStructureRepository, MainTrackStructureRepository, AdmStructureRepository);
                    var template = RdStructureRepository.GetReportTemplate("MainParametersOnline");
                    try
                    {
                        mainParameters.diagramName = "Оригинал";
                        mainParameters.Process(template, AppData.Kilometers[prevIndex], AppData.Trip, OnlineModeData.AutoPrint);
                        //if (AppData.EmailCount < 3)
                        //{
                        //    RdStructureRepository.SendEkasuiData(AppData.Trip, AppData.Kilometers[prevIndex].Number);
                        //    AppData.EmailCount++;
                        //}
                        //List<CrosProf> GetCrossRailProfileFromDBbyKm(int kilometer, long trip_id);
                        AppData.Kilometers[prevIndex].Digressions = RdStructureRepository.GetDigressionMarks(AppData.Trip.Id, AppData.Kilometers[prevIndex].Number, AppData.Kilometers[prevIndex].Track_id, new int[] { 3, 4 });
                        //AppData.Kilometers[prevIndex].AdditionalDigressions = AdditionalParametersRepository.GetCrossRailProfileFromDBbyKm(AppData.Kilometers[prevIndex].Number, AppData.Trip.Id);
                        //AppData.Kilometers[prevIndex].Digressions.Add( AdditionalParametersRepository.GetCrossRailProfileFromDBbyKm(AppData.Kilometers[prevIndex].Number, AppData.Trip.Id));
                        var dangerouses = AppData.Kilometers[prevIndex].Digressions.Where(digression => digression.Degree == 4).ToList();
                        StringBuilder dangers = new StringBuilder();
                        foreach (var dangerous in dangerouses)
                        {
                            if (!AppData.Kilometers[prevIndex].Crashed)
                            {
                                dangers.Append($@"{ dangerous.DigName } 4 степени (знач.: {dangerous.Value}) на {dangerous.Km} км {dangerous.Meter} метре. Ограничение скорости: {dangerous.LimitSpeed}" + Environment.NewLine);

                            }
                        }
                        if (dangers.Length != 0)
                        {
                            object[] paramss = new object[] { AppData.Meter };
                            Toaster.Add(dangers.ToString(), MatToastType.Danger, "Внимание!!!", "");
                            await JSRuntime.InvokeVoidAsync("beep", paramss);
                        }
                        dangers = null;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Ошибка обработки:" + e.Message);
                    }
                }
            }
        }

        public async Task PrintCurrentKm()
        {
            await JSRuntime.InvokeVoidAsync("loader", true);
            List<double> prevLevelAvgPart = new List<double>();
            List<double> nextLevelAvgPart = new List<double>();
            List<double> prevStrightAvgPart = new List<double>();
            List<double> nextStrightAvgPart = new List<double>();
            int n = 500;
            int prevIndex = AppData.Kilometers.IndexOf(AppData.CurrentKilometer);
            int prevN = 500;

            if (prevIndex > 0)
            {
                n = AppData.Kilometers[prevIndex - 1].LevelAvg.Count > prevN ? prevN : AppData.Kilometers[prevIndex - 1].LevelAvg.Count;
                prevLevelAvgPart = AppData.Kilometers[prevIndex - 1].LevelAvg.GetRange(AppData.Kilometers[prevIndex - 1].LevelAvg.Count - n - 1, n);
                prevStrightAvgPart = AppData.Kilometers[prevIndex - 1].StrightAvg.GetRange(AppData.Kilometers[prevIndex - 1].StrightAvg.Count - n - 1, n);

            }
            n = AppData.Kilometers[prevIndex + 1].LevelAvg.Count > prevN ? prevN : AppData.Kilometers[prevIndex + 1].LevelAvg.Count;

            nextLevelAvgPart = AppData.Kilometers[prevIndex + 1].LevelAvg.GetRange(0, n);
            nextStrightAvgPart = AppData.Kilometers[prevIndex + 1].StrightAvg.GetRange(0, n);

            if (AppData.Kilometers[prevIndex].WriteCurrentData(AppData.Trip, prevLevelAvgPart, nextLevelAvgPart, prevStrightAvgPart, nextStrightAvgPart, MainTrackStructureRepository, RdStructureRepository))
            {
                AppData.Kilometers[prevIndex].Trip = AppData.Trip;
                var mainParameters = new ALARm.Core.Report.MainParameters(RdStructureRepository, MainTrackStructureRepository, AdmStructureRepository);
                var template = RdStructureRepository.GetReportTemplate("MainParametersOnline");
                mainParameters.diagramName = "Дубликат";
                mainParameters.Process(template, AppData.Kilometers[prevIndex], AppData.Trip, true);
            }
            await JSRuntime.InvokeVoidAsync("loader", false);
        }

        public async Task PrintRegion()
        {
            await JSRuntime.InvokeVoidAsync("loader", true);
            var prevLevelAvgPart = new List<double>();
            var nextLevelAvgPart = new List<double>();
            var prevStrightAvgPart = new List<double>();
            var nextStrightAvgPart = new List<double>();
            int n = 200;
            //int n = 200;
            int prevIndex = AppData.Kilometers.IndexOf(AppData.CurrentKilometer);

            int prevN = 200;
            //int prevN = 200;
            if (prevIndex > 0)
            {
                n = AppData.Kilometers[prevIndex - 1].LevelAvg.Count > prevN ? prevN : AppData.Kilometers[prevIndex - 1].LevelAvg.Count;
                prevLevelAvgPart = AppData.Kilometers[prevIndex - 1].LevelAvg.GetRange(AppData.Kilometers[prevIndex - 1].LevelAvg.Count - n - 1, n);
                prevStrightAvgPart = AppData.Kilometers[prevIndex - 1].StrightAvg.GetRange(AppData.Kilometers[prevIndex - 1].StrightAvg.Count - n - 1, n);

            }
            n = AppData.Kilometers[prevIndex + 1].LevelAvg.Count > prevN ? prevN : AppData.Kilometers[prevIndex + 1].LevelAvg.Count;

            nextLevelAvgPart = AppData.Kilometers[prevIndex + 1].LevelAvg.GetRange(0, n);
            nextStrightAvgPart = AppData.Kilometers[prevIndex + 1].StrightAvg.GetRange(0, n);

            if (AppData.Kilometers[prevIndex].WriteCurrentData(AppData.Trip, prevLevelAvgPart, nextLevelAvgPart, prevStrightAvgPart, nextStrightAvgPart, MainTrackStructureRepository, RdStructureRepository))
            {
                AppData.Kilometers[prevIndex].Trip = AppData.Trip;
                var mainParameters = new ALARm.Core.Report.MainParameters(RdStructureRepository, MainTrackStructureRepository, AdmStructureRepository);
                var template = RdStructureRepository.GetReportTemplate("MainParametersOnline");
                mainParameters.diagramName = "Дубликат";
                mainParameters.ProcessRegion(template, AppData.Kilometers[prevIndex], AppData.Trip, false, AppData.CurrentKmMeter);
            }
            await JSRuntime.InvokeVoidAsync("loader", false);
        }
    }
}
