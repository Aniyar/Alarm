using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ALARm.Core;
using ALARm.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ALARm.WebApi.Controllers{


    [ApiController]
    [Route("[controller]")]
    public class ImageApiController : ControllerBase
    {
        private readonly ILogger<ImageApiController> _logger;
        private readonly IAdditionalParametersRepository _additionalParametersRepository;
        
        public ImageApiController(ILogger<ImageApiController> logger, IAdditionalParametersRepository additionalParametersRepository){
            _logger = logger;
            _additionalParametersRepository = additionalParametersRepository;
        }

        [HttpPost]
        public Dictionary<String, Object> Post([FromBody] ImageData data)
        {
            if (data.Id != -1)
            {
                if (data.State == FrontState.Add)
                {
                    Console.WriteLine("Add");
                }
                else if (data.State == FrontState.Edit)
                {
                    Console.WriteLine("Edit");
                }
                else if (data.State == FrontState.Delete)
                {
                    Console.WriteLine("Delete");
                }

                return null;
            }
            else
            {
                int upperKoef = 45;
                var result = new Dictionary<String, Object>();
                List<Object> shapes = new List<Object>();

                var carPosition = data.CarPosition;

                var topDic = _additionalParametersRepository.getBitMaps(data.FileId, data.Ms - 200 * (int)carPosition, data.Fnum - 1 * (int)carPosition, data.RepType);
                List<Bitmap> top = (List<Bitmap>)topDic["bitMaps"];
                var commonBitMap = new Bitmap(top[0].Width * 5  - 87, top[0].Height * 3  - 175);
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
                g.DrawImageUnscaled(RotateImage(top[2], 0), top[0].Width + top[1].Width-30, -35);
                g.DrawImageUnscaled(RotateImage(top[4], 3), x4-20, y4);
                g.DrawImageUnscaled(RotateImage(top[3], -1), x3, y3);

                var topShapes = (List<Dictionary<String, Object>>)topDic["drawShapes"];
                topShapes.ForEach(s => { shapes.Add(s); });

                var centerDic = _additionalParametersRepository.getBitMaps(data.FileId, data.Ms, data.Fnum, data.RepType);
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
                g.DrawImageUnscaled(RotateImage(center[2], 1), center[0].Width + center[1].Width-28, top[2].Height - upperKoef);
                g.DrawImageUnscaled(RotateImage(center[4], 4), center[1].Width + center[1].Width + center[2].Width + center[3].Width - 135, top[4].Height + y4 - 63);
                g.DrawImageUnscaled(RotateImage(center[3], -3), center[1].Width + center[1].Width + center[2].Width - 57, top[3].Height + y3 - 50);


                var centerShapes = (List<Dictionary<String, Object>>)centerDic["drawShapes"];
                centerShapes.ForEach(s => { shapes.Add(s); });



                var bottomDic = _additionalParametersRepository.getBitMaps(data.FileId, data.Ms + 200 * (int)carPosition, data.Fnum + 1 * (int)carPosition, data.RepType);
                List<Bitmap> bottom = (List<Bitmap>)bottomDic["bitMaps"];
                g.DrawImageUnscaled(bottom[0], -12, center[0].Height * 2 - 2 * upperKoef - 10 - 60);
                g.DrawImageUnscaled(RotateImage(bottom[1], 1), bottom[0].Width - 30, center[1].Height * 2 - 2 * upperKoef - 80);
                g.DrawImageUnscaled(RotateImage(bottom[2], 1), bottom[0].Width + bottom[1].Width-33, center[2].Height * 2 - 2 * upperKoef-60);
                g.DrawImageUnscaled(RotateImage(bottom[4], 4), bottom[1].Width + bottom[1].Width + bottom[2].Width + bottom[3].Width - 20 - 110, center[4].Height * 2 - 2 * upperKoef - 70);
                g.DrawImageUnscaled(RotateImage(bottom[3], -3), bottom[1].Width + bottom[1].Width + bottom[2].Width - 50, center[3].Height * 2 - 2 * upperKoef - 50);



                var bottomShapes = (List<Dictionary<String, Object>>)bottomDic["drawShapes"];
                bottomShapes.ForEach(s => { shapes.Add(s); });




                



                //using (Pen selPen = new Pen(Color.White))
                //{
                //    var objects = _additionalParametersRepository.GetObjectsByFrameNumber(data.FileId, data.Ms, data.Fnum);
                //    foreach (var vo in objects)
                //    {
                //        if (vo.Threat == Threat.Left)
                //        {
                //            g.DrawRectangle(
                //                selPen,
                //                top[0].Width - 95 + vo.X,
                //                top[0].Height - 90 + vo.Y,
                //                vo.W,
                //                vo.H
                //                );

                //            //var d = new Dictionary<string, Object>
                //            //{
                //            //    { "name", "gap" },
                //            //    { "thread", vo.Threat },
                //            //    { "id", vo.Id },
                //            //    { "x7", top[0].Width - 75 + vo.X },
                //            //    { "y7", top[0].Height - 80 + vo.Y },
                //            //    { "w7", vo.W },
                //            //    { "h7", vo.H }
                //            //};
                //            //shapes.Add(d);
                //        }
                //        if (vo.Threat == Threat.Right)
                //        {
                //            g.DrawRectangle(
                //                selPen,
                //                top[0].Width - 95 + vo.X + 1250,
                //                top[0].Height - 220 + vo.Y ,
                //                vo.W,
                //                vo.H
                //                );

                //            //var d = new Dictionary<string, Object>
                //            //{
                //            //    { "name", "gap" },
                //            //    { "thread", vo.Threat },
                //            //    { "id", vo.Id },
                //            //    { "x7", vo.X },
                //            //    { "y7", topy2 + vo.Y + 25 },
                //            //    { "w7", vo.W },
                //            //    { "h7", vo.H }
                //            //};
                //            //shapes.Add(d);
                //        }
                //    }
                //}

                if (center != null)
                {
                    using MemoryStream m = new MemoryStream();
                    commonBitMap.Save(m, ImageFormat.Png);
                    byte[] byteImage = m.ToArray();

                    var b64 = Convert.ToBase64String(byteImage);
                    result.Add("b64", b64);

                    result.Add("shapes", shapes);
                    return result;
                }
                else
                {
                    return null;
                }
            }
        }

        public Image RotateImage(Image img, float rotationAngle)
        {
            //create an empty Bitmap image
            Bitmap bmp = new Bitmap(img.Width, img.Height);

            //turn the Bitmap into a Graphics object
            Graphics gfx = Graphics.FromImage(bmp);

            //now we set the rotation point to the center of our image
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
    }
}
