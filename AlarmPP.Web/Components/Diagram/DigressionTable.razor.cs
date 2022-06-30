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

        private Gap digressionGap { get; set; } = new Gap();
        private bool GapDeleteDialog { get; set; } = false;
        private bool GapEditDialog { get; set; } = false;

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

            if (AppData.Editor == null || AppData.EditReason == null || AppData.Editor.Equals("") || AppData.EditReason.Equals("") || AppData.Editor.Equals(string.Empty) || AppData.EditReason.Equals(string.Empty))
            {
                Toaster.Add($"Заполните все поля диалогового окна", MatBlazor.MatToastType.Warning, "Редактирование отступлений");
                return;
            }
            digression.EditReason = AppData.EditReason;
            digression.Editor = AppData.Editor;
            try
            {
                var kilometer = (from km in Kilometers where km.Number == digression.Km select km).First();
                if (AppData.RdStructureRepository.UpdateDigression(digression, kilometer, action) > 0)
                {

                    if (action == RdAction.Delete)
                    { 
                        DigressionDeleteDialog = false;
                        Toaster.Add($"Удаление успешно завершено", MatBlazor.MatToastType.Success, "Редактирование отступлений");
                    }
                    else 
                    {
                        DigressionEditDialog = false;
                        StateHasChanged();
                        Toaster.Add($"Редактирование успешно завершено", MatBlazor.MatToastType.Success, "Редактирование отступлений"); 
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Не уадлость завершить редактирование из за ошибки: " + e.Message);
            }
        }

        void UpdateGap(RdAction action)
        {

            if (AppData.Editor == null || AppData.EditReason == null || AppData.Editor.Equals("") || AppData.EditReason.Equals("") || AppData.Editor.Equals(string.Empty) || AppData.EditReason.Equals(string.Empty))
            {
                Toaster.Add($"Заполните все поля диалогового окна", MatBlazor.MatToastType.Warning, "Редактирование отступлений");
                return;
            }
            digressionGap.EditReason = AppData.EditReason;
            digressionGap.Editor = AppData.Editor;
            //digressionGap.Zabeg = hidzazor.value;
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

            if (AppData.Editor == null || AppData.EditReason == null || AppData.Editor.Equals("") || AppData.EditReason.Equals("") || AppData.Editor.Equals(string.Empty) || AppData.EditReason.Equals(string.Empty))
            {
                Toaster.Add($"Заполните все поля диалогового окна", MatBlazor.MatToastType.Warning, "Редактирование отступлений");
                return;
            }
            digressionO.EditReason = AppData.EditReason;
            digressionO.Editor = AppData.Editor;
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
                List<List<Bitmap>> rows = new List<List<Bitmap>>();
                int N_rows = 3;
                for (int i = 0; i < N_rows; i++)
                {
                    rows.Add(new List<Bitmap>());
                    var dic = AppData.AdditionalParametersRepository.getBitMaps(data.File_Id, data.Ms - 200 * (i - (int)N_rows / 2) * (int)AppData.Trip.Car_Position, data.Fnum + (i - (int)N_rows / 2) * (int)AppData.Trip.Car_Position, RepType.Undefined);
                    rows[i] = (List<Bitmap>)dic["bitMaps"];
                    ((List<Dictionary<String, Object>>)dic["drawShapes"]).ForEach(s => { shapes.Add(s); });
                }
                int W = rows[0][0].Width, H = rows[0][0].Height;
                var commonBitMap = new Bitmap(W * 5 - 87, H * N_rows - 175);
                Graphics g = Graphics.FromImage(commonBitMap);

                if (AppData.Trip.Car_Position == CarPosition.Base)
                {
                    for (int i = 0; i < N_rows; i++)
                    {
                        g.DrawImageUnscaled(rows[i][0], 0, (H - upperKoef) * i - 46);
                        g.DrawImageUnscaled(rows[i][1], W, (H - upperKoef) * i - 45);
                        g.DrawImageUnscaled(rows[i][2], W * 2, (H - upperKoef) * i - 35);
                        g.DrawImageUnscaled(rows[i][3], W * 3, (H - upperKoef) * i - 24);
                        g.DrawImageUnscaled(rows[i][4], W * 4, (H - upperKoef) * i - 24);
                    }
                }
                else
                {
                    for (int i = 0; i < N_rows; i++)
                    {
                        g.DrawImageUnscaled(rows[N_rows - i - 1][0], 0, (H - upperKoef) * i - 46);
                        g.DrawImageUnscaled(rows[N_rows - i - 1][1], W, (H - upperKoef) * i - 45);
                        g.DrawImageUnscaled(rows[N_rows - i - 1][2], W * 2, (H - upperKoef) * i - 35);
                        g.DrawImageUnscaled(rows[N_rows - i - 1][3], W * 3, (H - upperKoef) * i - 24);
                        g.DrawImageUnscaled(rows[N_rows - i - 1][4], W * 4, (H - upperKoef) * i - 24);
                    }
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
                digGapCurrentIndex = index;
                digGapCurrentKm = data.Km;
                digType = type;
                JSRuntime.InvokeVoidAsync("loader", true);
                digressionO = data;
                DigressionImageDialog = true;
                int upperKoef = 55;
                var result = new Dictionary<String, Object>();
                List<Object> shapes = new List<Object>();
                List<List<Bitmap>> rows = new List<List<Bitmap>>();
                int N_rows = 3;
                if (type == 2)
                    N_rows = 6;
                else if (type == 3 || type == 4)
                    N_rows = 5;
                for (int i = 0; i < N_rows; i++)
                {
                    rows.Add(new List<Bitmap>());
                    var dic = AppData.AdditionalParametersRepository.getBitMaps(data.Fileid, data.Ms - 200 * (i - (int)N_rows / 2) * (int)AppData.Trip.Car_Position, data.Fnum + (i - (int)N_rows / 2) * (int)AppData.Trip.Car_Position, RepType.Undefined);
                    rows[i] = (List<Bitmap>)dic["bitMaps"];
                    ((List<Dictionary<String, Object>>)dic["drawShapes"]).ForEach(s => { shapes.Add(s); });
                }

                int W = rows[0][0].Width, H = rows[0][0].Height;
                var commonBitMap = new Bitmap((W - 17) * N_rows , (H - 30) * N_rows );
                Graphics g = Graphics.FromImage(commonBitMap);

                if (AppData.Trip.Car_Position == CarPosition.Base)
                {
                    for (int i = 0; i < N_rows; i++)
                    {
                        g.DrawImageUnscaled(rows[i][0], 0, (H - upperKoef) * i - 46);
                        g.DrawImageUnscaled(rows[i][1], W, (H - upperKoef) * i - 65);
                        g.DrawImageUnscaled(rows[i][2], W * 2, (H - upperKoef) * i - 35);
                        g.DrawImageUnscaled(rows[i][3], W * 3, (H - upperKoef) * i - 24);
                        g.DrawImageUnscaled(rows[i][4], W * 4, (H - upperKoef) * i - 24);
                    }
                }
                else
                {
                    for (int i = 0; i < N_rows; i++)
                    {
                        g.DrawImageUnscaled(rows[N_rows - i - 1][0], 0, (H - upperKoef) * i - 46);
                        g.DrawImageUnscaled(rows[N_rows - i - 1][1], W, (H - upperKoef) * i - 65);
                        g.DrawImageUnscaled(rows[N_rows - i - 1][2], W * 2, (H - upperKoef) * i - 35);
                        g.DrawImageUnscaled(rows[N_rows - i - 1][3], W * 3, (H - upperKoef) * i - 24);
                        g.DrawImageUnscaled(rows[N_rows - i - 1][4], W * 4, (H - upperKoef) * i - 24);
                    }
                }
                
                if (rows[1] != null)
                {
                    using MemoryStream m = new MemoryStream();
                    commonBitMap.Save(m, ImageFormat.Png);
                    byte[] byteImage = m.ToArray();

                    var b64 = Convert.ToBase64String(byteImage);
                    result.Add("b64", b64);
                    result.Add("type", type);
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
            }
            catch (Exception)
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
                                GetImageBolts(item.value, item.i, digType);
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
                                GetImageBolts(item.value, item.i, digType);
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
                                GetImageBolts(item.value, item.i, digType);
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
                                GetImageBolts(fastener.value, fastener.i, digType);
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
                                GetImageBolts(shpals.value, shpals.i, digType);
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
                                GetImageBolts(shpals.value, shpals.i, digType);
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

        public void ModifyGapClick(Gap gap)
        {
            StateHasChanged();
            digressionGap = gap;
            GapEditDialog = true;
        }

        public void DeleteGapClick(Gap gap)
        {
            StateHasChanged();
            digressionGap = gap;
            GapDeleteDialog = true;
        }

        public void ModifyBoltClick(Digression bolt)
        {
            digressionO = bolt;
            BoltEditDialog = true;
        }

        public void DeleteBoltClick(Digression bolt)
        {
            digressionO = bolt;
            BoltDeleteDialog = true;
        }

        public void ModifyFastenerClick(Digression fastener)
        {
            digressionO = fastener;
            FastenerEditDialog = true;
        }

        public void DeleteFastenerClick(Digression fastener)
        {
            digressionO = fastener;
            FastenerDeleteDialog = true;
        }

        public void ModifyPerShpalClick(Digression perShpal)
        {
            digressionO = perShpal;
            PerShpalEditDialog = true;
        }

        public void DeletePerShpalClick(Digression perShpal)
        {
            digressionO = perShpal;
            PerShpalDeleteDialog = true;
        }

        public void ModifyDefShpalClick(Digression perShpal)
        {
            digressionO = perShpal;
            DefShpalEditDialog = true;
        }

        public void DeleteDefShpalClick(Digression perShpal)
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
