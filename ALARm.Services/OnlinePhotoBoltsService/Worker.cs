//using ALARm.Core;
//using ALARm.Core.AdditionalParameteres;
//using ALARm.Core.Report;
//using ALARm.DataAccess;
//using ALARm.Services;
//using Microsoft.AspNetCore.Components;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using Microsoft.JSInterop;
//using Newtonsoft.Json.Linq;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Drawing2D;
//using System.Drawing.Imaging;
//using System.IO;
//using System.Linq;
//using System.Net.Http;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace OnlinePhotoBoltsService
//{
//    public class Worker : BackgroundService
//    {

//        private int digGapCurrentKm { get; set; }

//        private int digGapCurrentIndex { get; set; }

//        private int digType { get; set; }

//        [Parameter]
//        public List<Kilometer> Kilometers { get; set; }

//        [Parameter]
//        public int CurrentRow { get; set; } = 0;
//        private string DigressionEditor { get; set; }
//        private string EditReason { get; set; }

//        private DigressionMark digression { get; set; } = new DigressionMark();
//        private bool DigressionDeleteDialog { get; set; } = false;
//        private bool DigressionEditDialog { get; set; } = false;

//        private Gap digressionGap { get; set; } = new Gap();
//        private Digression digressionO { get; set; } = new Digression();
//        public bool DeleteModalState { get; set; } = false;
//        public bool DeleteGapModalState { get; set; } = false;
//        private bool DigressionImageDialog { get; set; } = false;
//        public FrontState State { get; set; } = FrontState.Undefined;
//        public string ModalClass { get; set; } = "image-modal";

      
       
//        void DeleteClick(DigressionMark mark)
//        {
//            digression = mark;
//            DigressionDeleteDialog = true;
//        }
//        void ModifyClick(DigressionMark mark)
//        {
//            digression = mark;
//            DigressionEditDialog = true;
//        }

       




//        public Image RotateImage(Image img, float rotationAngle)
//        {
//            //create an empty Bitmap image
//            Bitmap bmp = new Bitmap(img.Width, img.Height);

//            //turn the Bitmap into a Graphics object
//            Graphics gfx = Graphics.FromImage(bmp);

//            //now we set the rotation point to the center of our image
//            gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);

//            //now rotate the image
//            gfx.RotateTransform(rotationAngle);

//            gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);

//            //set the InterpolationMode to HighQualityBicubic so to ensure a high
//            //quality image once it is transformed to the specified size
//            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;

//            //now draw our new image onto the graphics object
//            gfx.DrawImage(img, new Point(0, 0));

//            //dispose of our Graphics object
//            gfx.Dispose();

//            //return the image
//            return bmp;
//        }




//        private readonly ILogger<Worker> _logger;
//        private ConnectionFactory _connectionFactory;
//        private IConnection _connection;
//        private IModel _channel;
//        private string QueueName = "";
//        public int tryCount = 0;
//        public IMainTrackStructureRepository MainTrackStructureRepository;

//        public Worker(ILogger<Worker> logger, IOptions<RabbitMQConfiguration> options)
//        {
//            _logger = logger;
//            QueueName = options.Value.Queue;

//            _connectionFactory = new ConnectionFactory
//            {
//                HostName = options.Value.Host,

//                UserName = options.Value.Username,
//                Password = options.Value.Password,
//                VirtualHost = "/",
//                Port = options.Value.Port,
//            };
//        }

//        public override Task StartAsync(CancellationToken cancellationToken)
//        {
//            try
//            {
//                _logger.LogInformation($"Connection try [{tryCount++}].");
//                _connection = _connectionFactory.CreateConnection();
//                _channel = _connection.CreateModel();
//                //_channel.QueueDeclarePassive(QueueName);
//                _channel.QueueDeclare(queue: QueueName,
//                                    durable: false,
//                                    exclusive: false,
//                                    autoDelete: false,
//                                    arguments: null);
//                _channel.QueueBind(queue: QueueName,
//                                   exchange: "alarm",
//                                   routingKey: "");
//                _channel.BasicQos(0, 1, false);
//                _logger.LogInformation($"Queue [{QueueName}] is waiting for messages.");



//                var consumer = new EventingBasicConsumer(_channel);
//                consumer.Received += async (model, ea) =>
//                {
//                    var body = ea.Body.ToArray();
//                    var message = Encoding.UTF8.GetString(body);
//                    _logger.LogInformation(" [x] Received {0}", message);

//                    JObject json = JObject.Parse(message);

//                    var kmIndex = (int)json["KmIndex"];
//                    var fileId = (int)json["FileId"];

//                    var trip = RdStructureService.GetTripFromFileId(fileId).Last();
//                    var kilometers = RdStructureService.GetKilometersByTrip(trip);
//                    var km = kilometers.Where(km => km.Number == kmIndex).ToList().First();
//                    this.MainTrackStructureRepository = MainTrackStructureService.GetRepository();

//                    var outData = (List<OutData>)RdStructureService.GetNextOutDatas(km.Start_Index - 1, km.GetLength() - 1, trip.Id);
//                    km.AddDataRange(outData, km);
//                    km.LoadTrackPasport(MainTrackStructureRepository, trip.Trip_date);

//                    //Видеоконтроль
//                    // todo distanse id
//                    string p = GetOnlinePhotoBoltsService(trip, km, 53); //стыки

//                    _logger.LogInformation(" [x] GetPerpen {0} {1} {2}", fileId, kmIndex, p);

//                };
//                _channel.BasicConsume(queue: QueueName,
//                                      autoAck: true,
//                                      consumer: consumer);

//                return base.StartAsync(cancellationToken);
//            }
//            catch(Exception e)
//            {
//                StartAsync(cancellationToken);
//                return base.StartAsync(cancellationToken);
//            }
//        }

//        /// <summary>
//        /// Сервис по стыкам
//        /// </summary>
//        /// <param name="trip"></param>
//        /// <param name="km"></param>
//        /// <param name="distId"></param>
//        private string GetOnlinePhotoBoltsService(Trips trip, Kilometer km, int distId)
//        {
//            try
//            {

//                var mainProcess = new MainParametersProcess { Trip_id = trip.Id };
//                var trackName = AdmStructureService.GetTrackName(km.Track_id);
//                var skreplenie = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, km.Number,
//                    MainTrackStructureConst.MtoRailsBrace, trip.Direction, trackName.ToString()) as List<RailsBrace>;

//                var ViolPerpen = RdStructureService.GetViolPerpen((int)trip.Id, new int[] { 7 }, km.Number);

//                AdditionalParametersService.Insert_ViolPerpen(km, skreplenie, ViolPerpen);
          

//                return "Success";
//            }
//            catch (Exception e)
//            {
//                return "Error " + e.Message;
//            }
//        }



//        public void GetImageBolts(Digression data, int index, int type)
//        {
//            digGapCurrentIndex = index;
//            digGapCurrentKm = data.Km;
//            digType = type;
//            digressionO = data;
//            DigressionImageDialog = true;
//            int upperKoef = 45;
//            var result = new Dictionary<String, Object>();
//            List<Object> shapes = new List<Object>();

//            var carPosition = -1;



//            var topDic = AdditionalParametersRepository.getBitMaps(data.Fileid, data.Ms - 200 * (int)carPosition, data.Fnum + 1 * (int)carPosition, RepType.Undefined);
//            List<Bitmap> top = (List<Bitmap>)topDic["bitMaps"];

//            var FilePath = (List<Dictionary<string, object>>)topDic["filePaths"];
//            var commonBitMap = new Bitmap(top[0].Width * 5 - 87, top[0].Height * 3 - 175);
//            Graphics g = Graphics.FromImage(commonBitMap);

//            int x1 = -7,
//                y1 = -46,
//                x2 = top[0].Width - 20,
//                y2 = -65,
//                x3 = top[1].Width + top[1].Width + top[2].Width - 60,
//                y3 = -24,
//                x4 = top[1].Width + top[1].Width + top[2].Width + top[3].Width - 120,
//                y4 = -24;

//            var coords = new (int, int)[3, 5] {
//            {(x1,y1),(x2,y2) ,(top[0].Width + top[1].Width - 30,-35), (x4 - 20,y4), (x3,y3) },
//            {(1,0),(1,1) ,(1,2), (1,3), (1,4) },
//            {(2,0),(2,1) ,(2,2), (2,3), (2,4) },
//            };



//            g.DrawImageUnscaled(RotateImage(top[0], -1), x1, y1);
//            g.DrawImageUnscaled(RotateImage(top[1], 3), x2, y2);
//            g.DrawImageUnscaled(RotateImage(top[2], 0), top[0].Width + top[1].Width - 30, -35);
//            g.DrawImageUnscaled(RotateImage(top[4], 3), x4 - 20, y4);
//            g.DrawImageUnscaled(RotateImage(top[3], -1), x3, y3);

//            var topShapes = (List<Dictionary<String, Object>>)topDic["drawShapes"];
//            topShapes.ForEach(s => { shapes.Add(s); });

//            var centerDic = AdditionalParametersRepository.getBitMaps(data.Fileid, data.Ms, data.Fnum, RepType.Undefined);
//            List<Bitmap> center = (List<Bitmap>)centerDic["bitMaps"];

//            int topx1 = -10,
//                topy1 = top[0].Height + y1 - 55,
//                topx2 = center[0].Width - 25,
//                topy2 = top[1].Height + y2 - 51,
//                topx3 = top[1].Width + top[2].Width - 60,
//                topx4 = top[1].Width + top[2].Width + top[3].Width - 120;


//            //center
//            g.DrawImageUnscaled(center[0], topx1, topy1);
//            g.DrawImageUnscaled(RotateImage(center[1], 1), topx2, topy2);
//            g.DrawImageUnscaled(RotateImage(center[2], 1), center[0].Width + center[1].Width - 28, top[2].Height - upperKoef);
//            g.DrawImageUnscaled(RotateImage(center[4], 4), center[1].Width + center[1].Width + center[2].Width + center[3].Width - 135, top[4].Height + y4 - 63);
//            g.DrawImageUnscaled(RotateImage(center[3], -3), center[1].Width + center[1].Width + center[2].Width - 57, top[3].Height + y3 - 50);

//            var centerShapes = (List<Dictionary<String, Object>>)centerDic["drawShapes"];
//            centerShapes.ForEach(s => { shapes.Add(s); });

//            var bottomDic = AdditionalParametersRepository.getBitMaps(data.Fileid, data.Ms + 200 * (int)carPosition, data.Fnum - 1 * (int)carPosition, RepType.Undefined);
//            List<Bitmap> bottom = (List<Bitmap>)bottomDic["bitMaps"];
//            g.DrawImageUnscaled(bottom[0], -12, center[0].Height * 2 - 2 * upperKoef - 10 - 60);
//            g.DrawImageUnscaled(RotateImage(bottom[1], 1), bottom[0].Width - 30, center[1].Height * 2 - 2 * upperKoef - 80);
//            g.DrawImageUnscaled(RotateImage(bottom[2], 1), bottom[0].Width + bottom[1].Width - 33, center[2].Height * 2 - 2 * upperKoef - 60);
//            g.DrawImageUnscaled(RotateImage(bottom[4], 4), bottom[1].Width + bottom[1].Width + bottom[2].Width + bottom[3].Width - 20 - 110, center[4].Height * 2 - 2 * upperKoef - 70);
//            g.DrawImageUnscaled(RotateImage(bottom[3], -3), bottom[1].Width + bottom[1].Width + bottom[2].Width - 50, center[3].Height * 2 - 2 * upperKoef - 50);

//            var bottomShapes = (List<Dictionary<String, Object>>)bottomDic["drawShapes"];
//            bottomShapes.ForEach(s => { shapes.Add(s); });

//            if (center != null)
//            {
//                using MemoryStream m = new MemoryStream();
//                //commonBitMap.Save(m, ImageFormat.Png);
//                //commonBitMap.Save("G:/bitmap/1.png", ImageFormat.Png);
//                commonBitMap.Save("C:/Cдача 10,11,2021/bitmap/1.png", ImageFormat.Png);
//                byte[] byteImage = m.ToArray();

//                var b64 = Convert.ToBase64String(byteImage);
//                result.Add("b64", b64);
//                result.Add("type", 2);
//                result.Add("shapes", shapes);
//                digression.DigressionImage = result;

//                digression.DigImage = b64;
//            }
//            else
//            {
//                digression.DigressionImage = null;
//            }
          
//        }








//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            // listen to the RabbitMQ messages, and send emails
//        }

//        public override async Task StopAsync(CancellationToken cancellationToken)
//        {
//            await base.StopAsync(cancellationToken);
//            _connection.Close();
//            _logger.LogInformation("RabbitMQ connection is closed.");
//        }
//    }
//}
