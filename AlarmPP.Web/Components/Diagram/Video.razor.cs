using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;
using ALARm.DataAccess;
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
    public partial class Video : ComponentBase
    {

        [Parameter]
        public List<Kilometer> Kilometers { get; set; }
        public Kilometer CurrentKm { get; set; }
        public int CurrentVideoFrame = 0;
        public int CurrentMs = 0;
        public int StartMeter { get; set; }
        public int CurrentMeter { get; set; }
        //[Parameter]

        private DigressionTable DigressionTable { get; set; } = new DigressionTable();
        public string Base64 { get; set; }
        public List<long> FileIdList { get; set; }
        public int Number { get; set; }
        public int N_rows { get; set; } 
        public bool ObjectsDialog { get; set; } = false;

        List<Gap> Gaps { get; set; } = new List<Gap>();
        List<Digression> Bolts { get; set; } = new List<Digression>();
        List<Digression> Fasteners { get; set; } = new List<Digression>();
        List<Digression> PerShpals { get; set; } = new List<Digression>();
        List<Digression> DefShpals { get; set; } = new List<Digression>();
        
        public Image RotateImage(Image img, float rotationAngle)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            Graphics gfx = Graphics.FromImage(bmp);
            gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
            gfx.RotateTransform(rotationAngle);
            gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gfx.DrawImage(img, new Point(0, 0));
            gfx.Dispose();
            return bmp;
        }


        public void GetImage2(long fileid)
        {
            try
            {
                int upperKoef = 55;
                int carPosition = (int)AppData.Trip.Car_Position;
                int direction = (int)CurrentKm.Direction;
                List<List<Bitmap>> rows = new List<List<Bitmap>>();
                for (int i = 0; i < N_rows; i++)
                {
                    rows.Add(new List<Bitmap>());
                    rows[i] = (List<Bitmap>)AppData.AdditionalParametersRepository.getBitMaps(fileid, CurrentMs + 200 * i * carPosition * direction, CurrentVideoFrame + i * direction * carPosition, RepType.Undefined)["bitMaps"];
                }
                int W = rows[0][0].Width, H = rows[0][0].Height;
                var commonBitMap = new Bitmap(W * 5 - 87, H * N_rows - 175);
                Graphics g = Graphics.FromImage(commonBitMap);

                //for (int i = 0; i < N_rows; i++)
                //{
                //    g.DrawImageUnscaled(rows[N_rows - i - 1][0], 0, (H - upperKoef) * i);
                //    g.DrawImageUnscaled(rows[N_rows - i - 1][1], W, (H - upperKoef) * i );
                //    g.DrawImageUnscaled(rows[N_rows - i - 1][2], W * 2, (H - upperKoef) * i);
                //    g.DrawImageUnscaled(rows[N_rows - i - 1][3], W * 3, (H - upperKoef) * i);
                //    g.DrawImageUnscaled(rows[N_rows - i - 1][4], W * 4, (H - upperKoef) * i);
                //}

                for (int i = 0; i < N_rows; i++)
                {
                    g.DrawImageUnscaled(RotateImage(rows[N_rows - i - 1][0], -1), 0, (H - upperKoef) * i - 46);
                    g.DrawImageUnscaled(RotateImage(rows[N_rows - i - 1][1], 1), W, (H - upperKoef) * i - 65);
                    g.DrawImageUnscaled(RotateImage(rows[N_rows - i - 1][2], 1), W * 2, (H - upperKoef) * i - 35);
                    g.DrawImageUnscaled(RotateImage(rows[N_rows - i - 1][3], -3), W * 3, (H - upperKoef) * i - 24);
                    g.DrawImageUnscaled(RotateImage(rows[N_rows - i - 1][4], 4), W * 4, (H - upperKoef) * i - 24);
                }

                if (rows[1] != null)
                {
                    using MemoryStream m = new MemoryStream();
                    commonBitMap.Save(m, ImageFormat.Jpeg);
                    byte[] byteImage = m.ToArray();
                    Base64 = Convert.ToBase64String(byteImage);
                }
                else
                {
                    Base64 = null;
                }
            }
            catch (Exception e)
            {
                Base64 = null;
            }

        }

        

        
        public async Task OnTimedEventAsync()
        {
            AppData.VideoProcessing = !AppData.VideoProcessing;
            CurrentKm = Kilometers.Where(km => km.Number == Number).First();
            StartMeter = CurrentKm.Start_m;
            while (AppData.VideoProcessing)
            {
                FileIdList = AppData.RdStructureRepository.GetFileID(242, Number);
                GetImage2(FileIdList[0]);
                StateHasChanged();
                await Task.Delay(AppData.Speed);
                CurrentVideoFrame += 1;
                CurrentMs += 200;
                CurrentMeter = StartMeter + (CurrentVideoFrame / 5);
            }
        }

        void GetObjectsFromFrame()
        {
            try
            {
                Gaps = CurrentKm.Gaps.Where(o => o.Meter == CurrentMeter).ToList();
                Fasteners = CurrentKm.Fasteners.Where(o => o.Meter == CurrentMeter).ToList();
                Bolts = CurrentKm.Bolts.Where(o => o.Meter == CurrentMeter).ToList();
                DefShpals = CurrentKm.DefShpals.Where(o => o.Meter == CurrentMeter).ToList();
                PerShpals = CurrentKm.PerShpals.Where(o => o.Meter == CurrentMeter).ToList();
                ObjectsDialog = true;
            }
            catch(Exception e)
            {
                Toaster.Add($"Отсутствуют данные по указанному километру", MatBlazor.MatToastType.Warning, "Просмотр видео проезда");
            }
            
        }

        public void NextClick()
        {
            CurrentMs += 200;
            CurrentVideoFrame += 1;
            GetImage2(FileIdList[0]);
        }
        public void PrevClick()
        {
            CurrentMs -= 200;
            CurrentVideoFrame -= 1;
            GetImage2(FileIdList[0]);
        }

    }
}
