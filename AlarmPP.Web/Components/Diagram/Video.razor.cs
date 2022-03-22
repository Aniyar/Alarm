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
        public Bitmap current_img { get; set; }
        public int CurrentVideoFrame = 1000;
        //[Parameter]
        public string Base64 { get; set; }
        public List<long> FileIdList { get; set; }
        public int Number { get; set; }
        public List<Bitmap> Bitmaps {get; set; }
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

        public void GetImage(long fileid)
        {
            int upperKoef = 55;
            var result = new Dictionary<String, Object>();
            List<Object> shapes = new List<Object>();

            int carposition = (Kilometers[0].Direction == Direction.Direct) ? 1 : -1;
            List<List<Bitmap>> rows = new List<List<Bitmap>>();
            int N_rows = 2;
            for (int i = 0; i < N_rows; i++)
            {
                rows.Add(new List<Bitmap>());
                rows[i] = AppData.AdditionalParametersRepository.GetBitMap(fileid, CurrentVideoFrame - 200 * i * carposition);
            }

            int W = rows[0][0].Width, H = rows[0][0].Height;
            var commonBitMap = new Bitmap(W * 5 - 87, H * N_rows - 175);
            Graphics g = Graphics.FromImage(commonBitMap);

            for (int i = 0; i < N_rows; i++)
            {
                g.DrawImageUnscaled(RotateImage(rows[i][0], -1), -12, (H - upperKoef) * i - 46);
                g.DrawImageUnscaled(RotateImage(rows[i][1], 1), W - 12, (H - upperKoef) * i - 65);
                g.DrawImageUnscaled(RotateImage(rows[i][2], 1), W * 2 - 33, (H - upperKoef) * i - 35);
                g.DrawImageUnscaled(RotateImage(rows[i][3], -3), W * 3 - 50, (H - upperKoef) * i - 24);
                g.DrawImageUnscaled(RotateImage(rows[i][4], 4), W * 4 - 130, (H - upperKoef) * i - 24);
            }
            if (rows[1] != null)
            {
                using MemoryStream m = new MemoryStream();
                commonBitMap.Save(m, ImageFormat.Jpeg);
                Base64 = Convert.ToBase64String(m.ToArray());
            }
        }
        public async Task OnTimedEventAsync()
        {
            AppData.VideoProcessing = !AppData.VideoProcessing;
            Kilometer km = Kilometers.Where(km => km.Number == Number).First();
            while (AppData.VideoProcessing)
            {
                FileIdList = AppData.RdStructureRepository.GetFileID(242, Number);
                GetImage(FileIdList[0]);
                Console.WriteLine(AppData.VideoProcessing);
                StateHasChanged();
                await Task.Delay(AppData.Speed);
                CurrentVideoFrame += 200;
                //AppData.CurrentFrameIndex+=10;
            }
        }
       

    }
}
