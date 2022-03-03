using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;
using AlarmPP.Web.Services;
using BlazorContextMenu;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

using System.Threading.Tasks;

namespace AlarmPP.Web.Components.Diagram
{
    public partial class DigressionTable : ComponentBase
    {
        private int digGapCurrentKm { get; set; }
        private int digGapCurrentIndex { get; set; }
        private int digType { get; set; }

        [Parameter]
        public List<Kilometer> Kilometers { get; set; }
        public List<PdbSection> PdbSection;
        [Parameter]
        public int CurrentRow { get; set; } = 0;

        private DigressionMark digression { get; set; } = new DigressionMark();
        private bool DigressionDeleteDialog { get; set; } = false;
        private bool DigressionEditDialog { get; set; } = false;
        private string DigressionEditor { get; set; }
        private string EditReason { get; set; }

        private Gap digressionGap { get; set; } = new Gap();
        private bool GapDeleteDialog { get; set; } = false;
        private bool GapEditDialog { get; set; } = false;
        private string GapEditor { get; set; }
        private string GapEditReason { get; set; }

/*
        private Digression digressionBolt { get; set; } = new Digression();*/
        private bool BoltDeleteDialog { get; set; } = false;
        private bool BoltEditDialog { get; set; } = false;

        /*private Digression digressionFastener { get; set; } = new Digression();*/
        private bool FastenerDeleteDialog { get; set; } = false;
        private bool FastenerEditDialog { get; set; } = false;

        /*private Digression PerShpal { get; set; } = new Digression();*/
        private bool PerShpalDeleteDialog { get; set; } = false;
        private bool PerShpalEditDialog { get; set; } = false;

        private bool DefShpalEditDialog { get; set; } = false;
        private bool DefShpalDeleteDialog { get; set; } = false;

        private Digression digressionO { get; set; } = new Digression();
        public bool DeleteModalState { get; set; } = false;
        private bool DigressionImageDialog { get; set; } = false;
        public FrontState State { get; set; } = FrontState.Undefined;
        public string ModalClass { get; set; } = "image-modal";

        public async Task GoToMark(int yposition, int rowIndex)
        {
            CurrentRow = rowIndex;
            AppData.SliderYPosition = yposition;
            AppData.SliderXPosition = Math.Round(AppData.SliderYPosition / 10);
            AppData.SliderCenterXPosition = AppData.SliderXPosition + 25;
            object[] paramss = new object[] { AppData.SliderYPosition - 200 };
            await JSRuntime.InvokeVoidAsync("ScrollMainSvg", paramss);


        }

        
        void DeleteClick(DigressionMark mark)
        {

            digression = mark;

            DigressionDeleteDialog = true;
        }
        void ModifyClick(DigressionMark mark)
        {
            digression = mark;
            DigressionEditDialog = true;
        }
        void PrintClick(DigressionMark mark)
        {
            digression = mark;
            DigressionEditDialog = true;
            //onclick = ($"window.print();return false;");

        }



        public void Refresh()
        {
            StateHasChanged();
            AppData.MainLoading = false;
        }
        void UpdateDigression(RdAction action)
        {

            if (DigressionEditor == null || EditReason == null || DigressionEditor.Equals("") || EditReason.Equals("") || DigressionEditor.Equals(string.Empty) || EditReason.Equals(string.Empty))
            {
                Toaster.Add($"Заполните все поля диалогового окна", MatBlazor.MatToastType.Warning, "Редактирование отступлений");
                return;
            }
            digression.EditReason = EditReason;
            digression.Editor = DigressionEditor;
            try
            {
                var kilometer = (from km in Kilometers where km.Number == digression.Km select km).First();
                if (AppData.RdStructureRepository.UpdateDigression(digression, kilometer, action) > 0)
                {
                    Toaster.Add($"Редактирование успешно завершено", MatBlazor.MatToastType.Success, "Редактирование отступлений");
                    if (action == RdAction.Delete)
                        DigressionDeleteDialog = false;
                    else
                        DigressionEditDialog = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Не уадлость завершить редактирование из за ошибки: " + e.Message);
            }
        }

        void UpdateGap(RdAction action)
        {

            if (GapEditor == null || GapEditReason == null || GapEditor.Equals("") || GapEditReason.Equals("") || GapEditor.Equals(string.Empty) || GapEditReason.Equals(string.Empty))
            {
                Toaster.Add($"Заполните все поля диалогового окна", MatBlazor.MatToastType.Warning, "Редактирование отступлений");
                return;
            }
            digressionGap.EditReason = GapEditReason;
            digressionGap.Editor = GapEditor;

            try
            {
                var kilometer = (from km in Kilometers where km.Number == digressionGap.Km select km).First();
                if (AppData.RdStructureRepository.UpdateGapBase(digressionGap, kilometer, action) > 0)
                {
                    Toaster.Add($"Редактирование успешно завершено", MatBlazor.MatToastType.Success, "Редактирование отступлений");
                    if (action == RdAction.Delete)
                    {
                        GapDeleteDialog = false;
                    }
                    else
                    {
                        GapEditDialog = false;
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Не уадлость завершить редактирование из за ошибки: " + e.Message);
            }
        }

        void EditDigression(RdAction action, int type, bool dialog)
        {

            if (DigressionEditor == null || EditReason == null || DigressionEditor.Equals("") || EditReason.Equals("") || DigressionEditor.Equals(string.Empty) || EditReason.Equals(string.Empty))
            {
                Toaster.Add($"Заполните все поля диалогового окна", MatBlazor.MatToastType.Warning, "Редактирование отступлений");
                return;
            }
            digressionO.EditReason = EditReason;
            digressionO.Editor = DigressionEditor;
            try
            {
                var kilometer = (from km in Kilometers where km.Number == digressionO.Km select km).First();
                if (AppData.RdStructureRepository.UpdateDigressionBase(digressionO, type, kilometer, action) > 0)
                {
                    Toaster.Add($"Редактирование успешно завершено", MatBlazor.MatToastType.Success, "Редактирование отступлений");
                    dialog = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Не уадлость завершить редактирование из за ошибки: " + e.Message);
            }
        }


        public void GetImage(DigressionMark data)
        {
            try
            {
                JSRuntime.InvokeVoidAsync("loader", true);
                digression = data;
                DigressionImageDialog = true;
                int upperKoef = 45;
                var result = new Dictionary<String, Object>();
                List<Object> shapes = new List<Object>();

                var carPosition = data.CarPosition;

                var topDic = AppData.AdditionalParametersRepository.getBitMaps(data.FileId, data.Ms - 200 * (int)carPosition, data.FNum - 1 * (int)carPosition, data.RepType);
                List<Bitmap> top = (List<Bitmap>)topDic["bitMaps"];
                var commonBitMap = new Bitmap(top[0].Width * 5 - 87, top[0].Height * 3 - 175);
                Graphics g = Graphics.FromImage(commonBitMap);

                int x1 = -7,
                    y1 = -46,
                    x2 = top[0].Width - 20,
                    y2 = -65,
                    x3 = top[1].Width + top[1].Width + top[2].Width - 60,
                    y3 = -24,
                    x4 = top[1].Width + top[1].Width + top[2].Width + top[3].Width - 120,
                    y4 = -24;

                g.DrawImageUnscaled(RotateImage(top[0], -1), x1, y1);
                g.DrawImageUnscaled(RotateImage(top[1], 3), x2, y2);
                g.DrawImageUnscaled(RotateImage(top[2], 0), top[0].Width + top[1].Width - 30, -35);
                g.DrawImageUnscaled(RotateImage(top[4], 3), x4 - 20, y4);
                g.DrawImageUnscaled(RotateImage(top[3], -1), x3, y3);

                var topShapes = (List<Dictionary<String, Object>>)topDic["drawShapes"];
                topShapes.ForEach(s => { shapes.Add(s); });

                var centerDic = AppData.AdditionalParametersRepository.getBitMaps(data.FileId, data.Ms, data.FNum, data.RepType);
                List<Bitmap> center = (List<Bitmap>)centerDic["bitMaps"];

                int topx1 = -10,
                    topy1 = top[0].Height + y1 - 55,
                    topx2 = center[0].Width - 25,
                    topy2 = top[1].Height + y2 - 51,
                    topx3 = top[1].Width + top[2].Width - 60,
                    topx4 = top[1].Width + top[2].Width + top[3].Width - 120;

                //center
                g.DrawImageUnscaled(center[0], topx1, topy1);
                g.DrawImageUnscaled(RotateImage(center[1], 1), topx2, topy2);
                g.DrawImageUnscaled(RotateImage(center[2], 1), center[0].Width + center[1].Width - 28, top[2].Height - upperKoef);
                g.DrawImageUnscaled(RotateImage(center[4], 4), center[1].Width + center[1].Width + center[2].Width + center[3].Width - 135, top[4].Height + y4 - 63);
                g.DrawImageUnscaled(RotateImage(center[3], -3), center[1].Width + center[1].Width + center[2].Width - 57, top[3].Height + y3 - 50);

                var centerShapes = (List<Dictionary<String, Object>>)centerDic["drawShapes"];
                centerShapes.ForEach(s => { shapes.Add(s); });

                var bottomDic = AppData.AdditionalParametersRepository.getBitMaps(data.FileId, data.Ms + 200 * (int)carPosition, data.FNum + 1 * (int)carPosition, data.RepType);
                List<Bitmap> bottom = (List<Bitmap>)bottomDic["bitMaps"];
                g.DrawImageUnscaled(bottom[0], -12, center[0].Height * 2 - 2 * upperKoef - 10 - 60);
                g.DrawImageUnscaled(RotateImage(bottom[1], 1), bottom[0].Width - 30, center[1].Height * 2 - 2 * upperKoef - 80);
                g.DrawImageUnscaled(RotateImage(bottom[2], 1), bottom[0].Width + bottom[1].Width - 33, center[2].Height * 2 - 2 * upperKoef - 60);
                g.DrawImageUnscaled(RotateImage(bottom[4], 4), bottom[1].Width + bottom[1].Width + bottom[2].Width + bottom[3].Width - 20 - 110, center[4].Height * 2 - 2 * upperKoef - 70);
                g.DrawImageUnscaled(RotateImage(bottom[3], -3), bottom[1].Width + bottom[1].Width + bottom[2].Width - 50, center[3].Height * 2 - 2 * upperKoef - 50);

                var bottomShapes = (List<Dictionary<String, Object>>)bottomDic["drawShapes"];
                bottomShapes.ForEach(s => { shapes.Add(s); });

                //if (center != null)
                //{
                //    using MemoryStream m = new MemoryStream();
                //   // commonBitMap.Save(m, ImageFormat.Png);
                //    //commonBitMap.Save("G:/bitmap/1.png", ImageFormat.Png);
                //    commonBitMap.Save("C:/Cдача 10,11,2021/bitmap/1.png", ImageFormat.Png);

                //    byte[] byteImage = m.ToArray();

                //    var b64 = Convert.ToBase64String(byteImage);
                //    result.Add("b64", b64);

                //    result.Add("shapes", shapes);
                //    digression.DigressionImage = result;

                //    digression.DigImage = b64;
                //}
                if (center != null)
                {
                    using MemoryStream m = new MemoryStream();
                    // commonBitMap.Save(m, ImageFormat.Png);
                    //commonBitMap.Save("G:/bitmap/1.png", ImageFormat.Png);
                    commonBitMap.Save("C:/Cдача 10,11,2021/bitmap/1.png", ImageFormat.Png);
                    using (FileStream fstream = new FileStream($"C:/Cдача 10,11,2021/bitmap/1.png", FileMode.OpenOrCreate))
                    {
                        byte[] byteImage = m.ToArray();

                        var b64 = Convert.ToBase64String(byteImage);
                        result.Add("b64", b64);

                        result.Add("shapes", shapes);
                        digression.DigressionImage = result;

                        digression.DigImage = b64;
                        Console.WriteLine("Текст записан в файл");
                    }
                    //byte[] byteImage = m.ToArray();

                    //var b64 = Convert.ToBase64String(byteImage);
                    //result.Add("b64", b64);

                    //result.Add("shapes", shapes);
                    //digression.DigressionImage = result;

                    //digression.DigImage = b64;
                }
                else
                {
                    digression.DigressionImage = null;
                }
                JSRuntime.InvokeVoidAsync("initCanvas", result);
                JSRuntime.InvokeVoidAsync("loader", false);
                //JSRuntime.InvokeVoidAsync("startZoom");
                //JSRuntime.InvokeVoidAsync("showImage", result);
            }
            catch (Exception)
            {

                digression.DigressionImage = null;

                var result = new Dictionary<String, Object>();

                JSRuntime.InvokeVoidAsync("initCanvas", result);
                JSRuntime.InvokeVoidAsync("loader", false);
            }
        }

        public void GetImageGaps(Gap data, int index, int type)
        {
            try
            {
                digGapCurrentKm = data.Km;
                digGapCurrentIndex = index;
                digType = type;
                JSRuntime.InvokeVoidAsync("loader", true);
                digressionGap = data;
                DigressionImageDialog = true;

                int upperKoef = 55;
                var result = new Dictionary<String, Object>();
                List<Object> shapes = new List<Object>();
                var carPosition = data.Direction;
                List<List<Bitmap>> rows = new List<List<Bitmap>>();
                int N_rows = 3;
                for (int i = 0; i < N_rows; i++)
                {
                    rows.Add(new List<Bitmap>());
                    var dic = AppData.AdditionalParametersRepository.getBitMaps(data.File_Id, data.Ms - 200 * (i - (int)N_rows / 2) * (int)carPosition, data.Fnum + (i - (int)N_rows / 2) * (int)carPosition, RepType.Undefined);
                    rows[i] = (List<Bitmap>)dic["bitMaps"];
                    ((List<Dictionary<String, Object>>)dic["drawShapes"]).ForEach(s => { shapes.Add(s); });
                }
                int W = rows[0][0].Width, H = rows[0][0].Height;
                var commonBitMap = new Bitmap(W * 5 - 87, H * N_rows - 175);
                Graphics g = Graphics.FromImage(commonBitMap);


                for (int i = 0; i < N_rows; i++)
                {
                    g.DrawImageUnscaled(RotateImage(rows[i][0], -1), -12, (H - upperKoef) * i - 46);
                    g.DrawImageUnscaled(RotateImage(rows[i][1], 1), W - 30, (H - upperKoef) * i - 65);
                    g.DrawImageUnscaled(RotateImage(rows[i][2], 1), W * 2 - 33, (H - upperKoef) * i - 35);
                    g.DrawImageUnscaled(RotateImage(rows[i][3], -3), W * 3 - 50, (H - upperKoef) * i - 24);
                    g.DrawImageUnscaled(RotateImage(rows[i][4], 4), W * 4 - 130, (H - upperKoef) * i - 24);
                }


                if (rows[1] != null)
                {
                    using MemoryStream m = new MemoryStream();
                    commonBitMap.Save(m, ImageFormat.Png);
                    // commonBitMap.Save("G:/bitmap/1.png", ImageFormat.Png);
                    // commonBitMap.Save("C:/Cдача 10,11,2021/bitmap/1.png", ImageFormat.Png);
                    byte[] byteImage = m.ToArray();
                    var b64 = Convert.ToBase64String(byteImage);
                    result.Add("b64", b64);
                    result.Add("type", 1);
                    result.Add("shapes", shapes);
                    result.Add("zazor_l", Convert.ToInt32(digressionGap.Zazor));
                    result.Add("zazor_r", Convert.ToInt32(digressionGap.R_zazor));
                    result.Add("zabeg", Convert.ToInt32(digressionGap.Zabeg));
                    digression.DigressionImage = result;

                    digression.DigImage = b64;
                }
                else
                {
                    digression.DigressionImage = null;
                }
                JSRuntime.InvokeVoidAsync("initCanvas", result);
                JSRuntime.InvokeVoidAsync("loader", false);
                //JSRuntime.InvokeVoidAsync("startZoom");
                //JSRuntime.InvokeVoidAsync("showImage", result);
            }
            catch (Exception e)
            {

                digression.DigressionImage = null;

                var result = new Dictionary<String, Object>();

                JSRuntime.InvokeVoidAsync("initCanvas", result);
                JSRuntime.InvokeVoidAsync("loader", false);
            }

        }


        public void GetImageBolts(Digression data, int index, int type)
        {
            try
            {
                Stopwatch stopWatch = new();
                stopWatch.Start();

                digGapCurrentIndex = index;
                digGapCurrentKm = data.Km;
                digType = type;
                JSRuntime.InvokeVoidAsync("loader", true);
                digressionO = data;
                DigressionImageDialog = true;
                int upperKoef = 55;
                var result = new Dictionary<String, Object>();
                List<Object> shapes = new List<Object>();

                var carPosition = data.Direction_num;
                List<List<Bitmap>> rows = new List<List<Bitmap>>();
                int N_rows = 6;
                for (int i = 0; i < N_rows; i++)
                {
                    rows.Add(new List<Bitmap>());
                    var dic = AppData.AdditionalParametersRepository.getBitMaps(data.Fileid, data.Ms - 200 * (i - (int)N_rows / 2) * (int)carPosition, data.Fnum + (i - (int)N_rows / 2) * (int)carPosition, RepType.Undefined);
                    rows[i] = (List<Bitmap>)dic["bitMaps"];
                    ((List<Dictionary<String, Object>>)dic["drawShapes"]).ForEach(s => { shapes.Add(s); });
                }

                int W = rows[0][0].Width, H = rows[0][0].Height;
                var commonBitMap = new Bitmap(W * 5 - 87, H * N_rows - 175);
                Graphics g = Graphics.FromImage(commonBitMap);

                for (int i = 0; i < N_rows; i++)
                {
                    g.DrawImageUnscaled(RotateImage(rows[i][0], -1), -12, (H - upperKoef) * i - 46);
                    g.DrawImageUnscaled(RotateImage(rows[i][1], -1), W - 30, (H - upperKoef) * i - 65);
                    g.DrawImageUnscaled(RotateImage(rows[i][2], 1), W * 2 - 33, (H - upperKoef) * i - 35);
                    g.DrawImageUnscaled(RotateImage(rows[i][3], -2), W * 3 - 50, (H - upperKoef) * i - 24);
                    g.DrawImageUnscaled(RotateImage(rows[i][4], 4), W * 4 - 130, (H - upperKoef) * i - 24);
                }
                if (rows[1] != null)
                {
                    using MemoryStream m = new MemoryStream();
                    commonBitMap.Save(m, ImageFormat.Png);
                    //commonBitMap.Save("G:/bitmap/1.png", ImageFormat.Png);
                    // commonBitMap.Save("C:/Cдача 10,11,2021/bitmap/1.png", ImageFormat.Png);
                    byte[] byteImage = m.ToArray();

                    var b64 = Convert.ToBase64String(byteImage);
                    result.Add("b64", b64);
                    result.Add("type", 2);
                    result.Add("shapes", shapes);
                    digression.DigressionImage = null;

                    //digression.DigImage = b64;
                }
                else
                {
                    digression.DigressionImage = null;
                }

                //digression.DigressionImage = null;

                JSRuntime.InvokeVoidAsync("initCanvas", result);
                JSRuntime.InvokeVoidAsync("loader", false);
                //JSRuntime.InvokeVoidAsync("startZoom");
                //JSRuntime.InvokeVoidAsync("showImage", result);


                stopWatch.Stop();
                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;

                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                Console.WriteLine("Болты  RunTime " + elapsedTime);
            }
            catch (Exception)
            {

                digression.DigressionImage = null;

                var result = new Dictionary<String, Object>();

                JSRuntime.InvokeVoidAsync("initCanvas", result);
                JSRuntime.InvokeVoidAsync("loader", false);
            }

        }

        public void GetImageFasteners(Digression data, int index, int type)
        {
            try
            {
                Stopwatch stopWatch = new();
                stopWatch.Start();
                digGapCurrentIndex = index;
                digGapCurrentKm = data.Km;
                digType = type;
                JSRuntime.InvokeVoidAsync("loader", true);
                digressionO = data;
                DigressionImageDialog = true;
                int upperKoef = 45;
                var result = new Dictionary<String, Object>();
                List<Object> shapes = new List<Object>();

                var carPosition = data.Direction_num;
                List<List<Bitmap>> rows = new List<List<Bitmap>>();
                int N_rows = 3;
                for (int i = 0; i < N_rows; i++)
                {
                    rows.Add(new List<Bitmap>());
                    var dic = AppData.AdditionalParametersRepository.getBitMaps(data.Fileid, data.Ms - 200 * (i - (int)N_rows / 2) * (int)carPosition, data.Fnum + (i - (int)N_rows / 2) * (int)carPosition, RepType.Undefined);
                    rows[i] = (List<Bitmap>)dic["bitMaps"];
                    ((List<Dictionary<String, Object>>)dic["drawShapes"]).ForEach(s => { shapes.Add(s); });
                }

                int W = rows[0][0].Width, H = rows[0][0].Height;
                var commonBitMap = new Bitmap(W * 5 - 87, H * N_rows - 175);
                Graphics g = Graphics.FromImage(commonBitMap);

                for (int i = 0; i < N_rows; i++)
                {
                    g.DrawImageUnscaled(RotateImage(rows[i][0], -1), -12, (H - upperKoef) * i - 46);
                    g.DrawImageUnscaled(RotateImage(rows[i][1], -1), W - 30, (H - upperKoef) * i - 65);
                    g.DrawImageUnscaled(RotateImage(rows[i][2], 1), W * 2 - 33, (H - upperKoef) * i - 35);
                    g.DrawImageUnscaled(RotateImage(rows[i][3], -2), W * 3 - 50, (H - upperKoef) * i - 24);
                    g.DrawImageUnscaled(RotateImage(rows[i][4], 4), W * 4 - 130, (H - upperKoef) * i - 24);
                }
                if (rows[1] != null)
                {
                    using MemoryStream m = new MemoryStream();
                    commonBitMap.Save(m, ImageFormat.Png);
                    //commonBitMap.Save("G:/bitmap/1.png", ImageFormat.Png);
                    byte[] byteImage = m.ToArray();

                    var b64 = Convert.ToBase64String(byteImage);
                    result.Add("b64", b64);
                    result.Add("type", 3);
                    result.Add("shapes", shapes);
                    digression.DigressionImage = result;

                    digression.DigImage = b64;
                }
                else
                {
                    digression.DigressionImage = null;
                }
                JSRuntime.InvokeVoidAsync("initCanvas", result);
                JSRuntime.InvokeVoidAsync("loader", false);
                //JSRuntime.InvokeVoidAsync("startZoom");
                //JSRuntime.InvokeVoidAsync("showImage", result);

                stopWatch.Stop();
                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;

                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                Console.WriteLine("Скрепление RunTime " + elapsedTime);

            }
            catch (Exception)
            {

                digression.DigressionImage = null;

                var result = new Dictionary<String, Object>();

                JSRuntime.InvokeVoidAsync("initCanvas", result);
                JSRuntime.InvokeVoidAsync("loader", false);
            }

        }

        public void GetImagePerShpals(Digression data, int index, int type)
        {
            try
            {
                Stopwatch stopWatch = new();
                stopWatch.Start();
                digGapCurrentIndex = index;
                digGapCurrentKm = data.Km;
                digType = type;
                JSRuntime.InvokeVoidAsync("loader", true);
                digressionO = data;
                DigressionImageDialog = true;
                int upperKoef = 45;
                var result = new Dictionary<String, Object>();
                List<Object> shapes = new List<Object>();

                var carPosition = data.Direction_num;
                List<List<Bitmap>> rows = new List<List<Bitmap>>();
                int N_rows = 5;
                for (int i = 0; i < N_rows; i++)
                {
                    rows.Add(new List<Bitmap>());
                    var dic = AppData.AdditionalParametersRepository.getBitMaps(data.Fileid, data.Ms - 200 * (i - (int)N_rows / 2) * (int)carPosition, data.Fnum + (i - (int)N_rows / 2) * (int)carPosition, RepType.Undefined);
                    rows[i] = (List<Bitmap>)dic["bitMaps"];
                    ((List<Dictionary<String, Object>>)dic["drawShapes"]).ForEach(s => { shapes.Add(s); });
                }

                int W = rows[0][0].Width, H = rows[0][0].Height;
                var commonBitMap = new Bitmap(W * 5 - 87, H * N_rows - 175);
                Graphics g = Graphics.FromImage(commonBitMap);

                for (int i = 0; i < N_rows; i++)
                {
                    g.DrawImageUnscaled(RotateImage(rows[i][0], -1), -12, (H - upperKoef) * i - 46);
                    g.DrawImageUnscaled(RotateImage(rows[i][1], -1), W - 30, (H - upperKoef) * i - 65);
                    g.DrawImageUnscaled(RotateImage(rows[i][2], 1), W * 2 - 33, (H - upperKoef) * i - 35);
                    g.DrawImageUnscaled(RotateImage(rows[i][3], -2), W * 3 - 50, (H - upperKoef) * i - 24);
                    g.DrawImageUnscaled(RotateImage(rows[i][4], 4), W * 4 - 130, (H - upperKoef) * i - 24);
                }
                if (rows[1] != null)
                {
                    using MemoryStream m = new MemoryStream();
                    commonBitMap.Save(m, ImageFormat.Png);
                    //commonBitMap.Save("G:/bitmap/1.png", ImageFormat.Png);
                    byte[] byteImage = m.ToArray();

                    var b64 = Convert.ToBase64String(byteImage);
                    result.Add("b64", b64);
                    result.Add("type", 3);
                    result.Add("shapes", shapes);
                    digression.DigressionImage = result;
                    digression.DigImage = b64;
                }
                else
                {
                    digression.DigressionImage = null;
                }
                JSRuntime.InvokeVoidAsync("initCanvas", result);
                JSRuntime.InvokeVoidAsync("loader", false);
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                Console.WriteLine("Скрепление RunTime " + elapsedTime);

            }
            catch (Exception)
            {
                digression.DigressionImage = null;
                var result = new Dictionary<String, Object>();
                JSRuntime.InvokeVoidAsync("initCanvas", result);
                JSRuntime.InvokeVoidAsync("loader", false);
            }

        }

        public void GetImageDefShpals(Digression data, int index, int type)
        {
            try
            {
                Stopwatch stopWatch = new();
                stopWatch.Start();
                digGapCurrentIndex = index;
                digGapCurrentKm = data.Km;
                digType = type;
                JSRuntime.InvokeVoidAsync("loader", true);
                digressionO = data;
                DigressionImageDialog = true;
                int upperKoef = 45;
                var result = new Dictionary<String, Object>();
                List<Object> shapes = new List<Object>();

                var carPosition = data.Direction_num;
                List<List<Bitmap>> rows = new List<List<Bitmap>>();
                int N_rows = 5;
                for (int i = 0; i < N_rows; i++)
                {
                    rows.Add(new List<Bitmap>());
                    var dic = AppData.AdditionalParametersRepository.getBitMaps(data.Fileid, data.Ms - 200 * (i - (int)N_rows / 2) * (int)carPosition, data.Fnum + (i - (int)N_rows / 2) * (int)carPosition, RepType.Undefined);
                    rows[i] = (List<Bitmap>)dic["bitMaps"];
                    ((List<Dictionary<String, Object>>)dic["drawShapes"]).ForEach(s => { shapes.Add(s); });
                }

                int W = rows[0][0].Width, H = rows[0][0].Height;
                var commonBitMap = new Bitmap(W * 5 - 87, H * N_rows - 175);
                Graphics g = Graphics.FromImage(commonBitMap);

                for (int i = 0; i < N_rows; i++)
                {
                    g.DrawImageUnscaled(RotateImage(rows[i][0], -1), -12, (H - upperKoef) * i - 46);
                    g.DrawImageUnscaled(RotateImage(rows[i][1], -1), W - 30, (H - upperKoef) * i - 65);
                    g.DrawImageUnscaled(RotateImage(rows[i][2], 1), W * 2 - 33, (H - upperKoef) * i - 35);
                    g.DrawImageUnscaled(RotateImage(rows[i][3], -2), W * 3 - 50, (H - upperKoef) * i - 24);
                    g.DrawImageUnscaled(RotateImage(rows[i][4], 4), W * 4 - 130, (H - upperKoef) * i - 24);
                }
                if (rows[1] != null)
                {
                    using MemoryStream m = new MemoryStream();
                    commonBitMap.Save(m, ImageFormat.Png);
                    //commonBitMap.Save("G:/bitmap/1.png", ImageFormat.Png);
                    byte[] byteImage = m.ToArray();

                    var b64 = Convert.ToBase64String(byteImage);
                    result.Add("b64", b64);
                    result.Add("type", 3);
                    result.Add("shapes", shapes);
                    digression.DigressionImage = result;
                    digression.DigImage = b64;
                }
                else
                {
                    digression.DigressionImage = null;
                }
                JSRuntime.InvokeVoidAsync("initCanvas", result);
                JSRuntime.InvokeVoidAsync("loader", false);
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                Console.WriteLine("Скрепление RunTime " + elapsedTime);

            }
            catch (Exception e)
            {
                digression.DigressionImage = null;
                var result = new Dictionary<String, Object>();
                JSRuntime.InvokeVoidAsync("initCanvas", result);
                JSRuntime.InvokeVoidAsync("loader", false);
            }

        }

        public Image RotateImage(Image img, float rotationAngle)
        {
            //create an empty Bitmap image
            Bitmap bmp = new Bitmap(img.Width, img.Height);

            //turn the Bitmap into a Graphics object
            Graphics gfx = Graphics.FromImage(bmp);

            //now we set the rotation point to the row2 of our image
            gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);

            //now rotate the image
            gfx.RotateTransform(rotationAngle);

            gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);

            //set the InterpolationMode to HighQualityBicubic so to ensure a high
            //quality image once it is transformed to the specified size
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;

            //now draw our new image onto the graphics object
            gfx.DrawImage(img, new Point(0, 0));

            //dispose of our Graphics object
            gfx.Dispose();

            //return the image
            return bmp;
        }

        public void NextDigression()
        {
            switch (digType)
            {
                case 1:
                    foreach (var km in Kilometers)
                    {
                        foreach (var gp in km.Gaps.Select((value, i) => new { i, value }))
                            if (gp.value.IsAdditional > 1 && km.Number == digGapCurrentKm && gp.i - 1 == digGapCurrentIndex)
                            {
                                GetImageGaps(gp.value, gp.i, digType);
                                return;
                            }
                    }
                    break;
                case 2:
                    foreach (var km in Kilometers)
                    {
                        foreach (var bolts in km.Bolts.Select((value, i) => new { i, value }))
                            if (km.Number == digGapCurrentKm && bolts.i - 1 == digGapCurrentIndex)
                            {
                                GetImageBolts(bolts.value, bolts.i, digType);
                                return;
                            }
                    }
                    break;
                case 3:
                    foreach (var km in Kilometers)
                    {
                        foreach (var item in km.Fasteners.Select((value, i) => new { i, value }))
                            if (km.Number == digGapCurrentKm && item.i - 1 == digGapCurrentIndex)
                            {
                                GetImageFasteners(item.value, item.i, digType);
                                return;
                            }
                    }
                    break;
                case 4:
                    foreach (var km in Kilometers)
                    {
                        foreach (var item in km.PerShpals.Select((value, i) => new { i, value }))
                            if (km.Number == digGapCurrentKm && item.i - 1 == digGapCurrentIndex)
                            {
                                GetImagePerShpals(item.value, item.i, digType);
                                return;
                            }
                    }
                    break;
                case 5:
                    foreach (var km in Kilometers)
                    {
                        foreach (var item in km.DefShpals.Select((value, i) => new { i, value }))
                            if (km.Number == digGapCurrentKm && item.i - 1 == digGapCurrentIndex)
                            {
                                GetImageDefShpals(item.value, item.i, digType);
                                return;
                            }
                    }
                    break;
            }
        }

        public void PrevDigression()
        {
            switch (digType)
            {
                case 1:
                    foreach (var km in Kilometers)
                    {
                        foreach (var gp in km.Gaps.Select((value, i) => new { i, value }))
                            if (gp.value.IsAdditional > 1 && km.Number == digGapCurrentKm && gp.i + 1 == digGapCurrentIndex)
                            {
                                GetImageGaps(gp.value, gp.i, digType);
                                return;
                            }
                    }
                    break;
                case 2:
                    foreach (var km in Kilometers)
                    {
                        foreach (var bolts in km.Bolts.Select((value, i) => new { i, value }))
                            if (km.Number == digGapCurrentKm && bolts.i + 1 == digGapCurrentIndex)
                            {
                                GetImageBolts(bolts.value, bolts.i, digType);
                                return;
                            }
                    }
                    break;
                case 3:
                    foreach (var km in Kilometers)
                    {
                        foreach (var fastener in km.Fasteners.Select((value, i) => new { i, value }))
                            if (km.Number == digGapCurrentKm && fastener.i + 1 == digGapCurrentIndex)
                            {
                                GetImageFasteners(fastener.value, fastener.i, digType);
                                return;
                            }
                    }
                    break;
                case 4:
                    foreach (var km in Kilometers)
                    {
                        foreach (var shpals in km.PerShpals.Select((value, i) => new { i, value }))
                            if (km.Number == digGapCurrentKm && shpals.i + 1 == digGapCurrentIndex)
                            {
                                GetImagePerShpals(shpals.value, shpals.i, digType);
                                return;
                            }
                    }
                    break;
                case 5:
                    foreach (var km in Kilometers)
                    {
                        foreach (var shpals in km.DefShpals.Select((value, i) => new { i, value }))
                            if (km.Number == digGapCurrentKm && shpals.i + 1 == digGapCurrentIndex)
                            {
                                GetImageDefShpals(shpals.value, shpals.i, digType);
                                return;
                            }
                    }
                    break;
            }
        }

        public void ZoomDigression(int type)
        {
            if (type == 0)
                JSRuntime.InvokeVoidAsync("ZoomIn");
            else
                JSRuntime.InvokeVoidAsync("ZoomOut");
        }

        void ModifyGapClick(Gap gap)
        {
            digressionGap = gap;
            GapEditDialog = true;
        }

        void DeleteGapClick(Gap gap)
        {
            digressionGap = gap;
            GapDeleteDialog = true;
        }

        void ModifyBoltClick(Digression bolt)
        {
            digressionO = bolt;
            BoltEditDialog = true;
        }

        void DeleteBoltClick(Digression bolt)
        {
            digressionO = bolt;
            BoltDeleteDialog = true;
        }

        void ModifyFastenerClick(Digression fastener)
        {
            digressionO = fastener;
            FastenerEditDialog = true;
        }

        void DeleteFastenerClick(Digression fastener)
        {
            digressionO = fastener;
            FastenerDeleteDialog = true;
        }

        void ModifyPerShpalClick(Digression perShpal)
        {
            digressionO = perShpal;
            PerShpalEditDialog = true;
        }

        void DeletePerShpalClick(Digression perShpal)
        {
            digressionO = perShpal;
            PerShpalDeleteDialog = true;
        }

        void ModifyDefShpalClick(Digression perShpal)
        {
            digressionO = perShpal;
            DefShpalEditDialog = true;
        }

        void DeleteDefShpalClick(Digression perShpal)
        {
            digressionO = perShpal;
            DefShpalDeleteDialog = true;
        }

        public void DeleteModal(Digression dig, int type)
        {
            digressionO = dig;
            digType = type;
            DeleteModalState = true;
        }
    }
}
