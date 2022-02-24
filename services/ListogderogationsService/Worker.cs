//using ALARm.Core;
//using ALARm.Core.AdditionalParameteres;
//using ALARm.Core.Report;
//using ALARm.DataAccess;
//using ALARm.Services;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using Newtonsoft.Json.Linq;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace ListogderogationsService
//{
//    public class Worker : BackgroundService
//    {
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

//                    //Âèäåîêîíòðîëü
//                    // todo distanse id
//                    string p = GetListOgDerogations(trip, km, 53); //ñòûêè

//                    _logger.LogInformation(" [x] GetSleepers {0} {1} {2}", fileId, kmIndex, p);

//                };
//                _channel.BasicConsume(queue: QueueName,
//                                      autoAck: true,
//                                      consumer: consumer);

//                return base.StartAsync(cancellationToken);
//            }
//            catch
//            {
//                StartAsync(cancellationToken);
//                return base.StartAsync(cancellationToken);
//            }
//        }

//        private string GetListOgDerogations(Trips trip, Kilometer km, int distId)
//        {
//            try
//            {
//                var mainProcess = new MainParametersProcess { Trip_id = trip.Id };
//                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distId) as AdmUnit;
//                var trackName = AdmStructureService.GetTrackName(km.Track_id);

//                var longrails = RdStructureService.GetTripSections(km.Track_id, km., MainTrackStructureConst.MtoLongRails) as List<LongRails>;


//                gaps = gaps.Where(o => o.Razn > 10 && o.Km > 128).ToList();
//                var speed = new List<Speed>();
//                var pdbSection = new List<PdbSection>();
//                Gap previousGap = null;
//                var sector = "";
//                var temperature = new List<Temperature>();
//                RailFastener prev_fastener = null;

//                var temp = (temperature.Count != 0 ? temperature[0].Kupe.ToString() : " ") + "°";

//                foreach (var gap in gaps)
//                {

//                    gap.R_zazor = gap.R_zazor == -999 ? -999 : gap.R_zazor == -1 ? 0 : (int)Math.Round(gap.R_zazor / 1.5);
//                    gap.Zazor = gap.Zazor == -999 ? -999 : gap.Zazor == -1 ? 0 : (int)Math.Round(gap.Zazor / 1.5);
//                    gap.Zabeg = gap.Zabeg == -999 ? -999 : (int)Math.Round(gap.Zabeg / 1.5);

//                    if ((previousGap == null) || (previousGap.Km != gap.Km))
//                    {
//                        //sector = MainTrackStructureService.GetSector(km.Trip_id, gap.Km, trip.Trip_date);
//                        sector = sector == null ? "" : sector;
//                        speed = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, gap.Km, MainTrackStructureConst.MtoSpeed, trip.Direction, trackName.ToString()) as List<Speed>;

//                        pdbSection = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, gap.Km, MainTrackStructureConst.MtoPdbSection, trip.Direction, trackName.ToString()) as List<PdbSection>;

//                        //temperature = MainTrackStructureService.GetTemp(km.Trip_id, km.Track_id, gap.Km);
//                    }
//                    gap.PassSpeed = speed.Count > 0 ? speed[0].Passenger : -1;
//                    gap.FreightSpeed = speed.Count > 0 ? speed[0].Freight : -1;

//                    var previousKm = -1;

//                    var dig = gap.GetDigressions();
//                    var dig2 = gap.GetDigressions2();
//                    //gap.PCHU = km.PdbSection.Count > 0 ? $"Ï×Ó-{km.PdbSection[0].Pchu}/ÏÄ-{km.PdbSection[0].Pd}/ÏÄÁ-{km.PdbSection[0].Pdb}" : "Ï×Ó-/ÏÄ-/ÏÄÁ-";
//                    gap.Vdop = gap.R_zazor < gap.Zazor ? (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap ? dig.AllowSpeed : "") :
//                                            (dig2.DigName == DigressionName.Gap || dig2.DigName == DigressionName.FusingGap || dig2.DigName == DigressionName.AnomalisticGap ? dig2.AllowSpeed : "");
//                    gap.Otst = gap.R_zazor < gap.Zazor ? (dig.DigName == DigressionName.Gap || dig.DigName == DigressionName.FusingGap || dig.DigName == DigressionName.AnomalisticGap) ? dig.GetName() : "" :
//                                (dig2.DigName == DigressionName.Gap || dig2.DigName == DigressionName.FusingGap || dig2.DigName == DigressionName.AnomalisticGap) ? dig2.GetName() : "";

//                    gap.Pdb_section = km.PdbSection.Count > 0 ? $"Ï×Ó-{km.PdbSection[0].Pchu}/ÏÄ-{km.PdbSection[0].Pd}/ÏÄÁ-{km.PdbSection[0].Pdb}" : "Ï×Ó-/ÏÄ-/ÏÄÁ-";



//                    gap.Fragment = km.StationSection != null && km.StationSection.Count > 0 ?
//                                  "Ñòàíöèÿ: " + km.StationSection[0].Station : (km.Sector != null ? km.Sector.ToString() : "");

//                    //gap.Fragment = sector;

//                    gap.Threat_id = gap.Threat == Threat.Left ? "ëåâàÿ" : "ïðàâàÿ";
//                    gap.Temp = temp;
//                    //   previousGap = gap;
                
//            }
             
//            AdditionalParametersService.Insert_gap(mainProcess.Trip_id, -1, gaps);
//            return "Success";
//        }

//            catch (Exception e)
//            {
//                Console.WriteLine("GetSleepers " + e.Message);

//            }

//            return "Failed";
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



